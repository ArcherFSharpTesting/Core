module Archer.Arrows.Internal

open System
open System.ComponentModel
open System.IO
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer
open Archer.Arrows
open Archer.CoreTypes.InternalTypes

type TestInternals<'a, 'b> = {
    FeatureSetup: unit -> Result<'a, SetupTeardownFailure>
    TestSetup: 'a -> Result<'b, SetupTeardownFailure>
    TestBody: 'b -> TestEnvironment -> TestResult
    TestTeardown: Result<'b, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>
    FeatureTeardown: Result<'a, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>
}


type ExecutionResultsAccumulator<'featureSetupResult, 'setupResult> =
    | Empty
    | FeatureSetupRun of result: Result<'featureSetupResult, SetupTeardownFailure>
    | SetupRun of result: Result<'featureSetupResult, SetupTeardownFailure> * Result<'setupResult, SetupTeardownFailure>
    | TestRun of setupResult: Result<'featureSetupResult, SetupTeardownFailure> * Result<'setupResult, SetupTeardownFailure> * testResult: TestResult
    | TeardownRun of setupResult: Result<'featureSetupResult, SetupTeardownFailure> * Result<'setupResult, SetupTeardownFailure> * testResult: TestResult option * teardownResult: Result<unit, SetupTeardownFailure>
    | FeatureTeardownRun of setupResult: Result<'featureSetupResult, SetupTeardownFailure> * Result<'setupResult, SetupTeardownFailure> option * testResult: TestResult option * teardownResult: Result<unit, SetupTeardownFailure>
    | FailureAccumulated of setupResult: Result<'featureSetupResult, SetupTeardownFailure> option * Result<'setupResult, SetupTeardownFailure> option * GeneralTestingFailure 

type SetupTeardownExecutorLifecycleEventArgs =
    | ExecuteSetupStart of CancelEventArgs
    | ExecuteSetupEnd of result: SetupResult * cancelEventArgs: CancelEventArgs
    | ExecuteStartTeardown 
    
type SetupTeardownExecutor<'inputType, 'outputType>(parent: ITest, setup: 'inputType -> Result<'outputType, SetupTeardownFailure>, teardown: Result<'outputType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>) =
    member _.Execute (runner: 'outputType -> TestExecutionResult option) (value: 'inputType) =
        let setupResult = setup value
        let runnerResult = 
            match setupResult with
            | Ok r -> runner r
            | Error _ -> None
        
        let teardownResult =     
            match runnerResult with
            | Some (TestExecutionResult testResult) -> teardown setupResult (Some testResult)
            | _ -> teardown setupResult None
            
        match teardownResult with
        | Ok _ -> runnerResult
        | Error setupTeardownFailure -> setupTeardownFailure |> TeardownExecutionFailure |> Some
        

type TestCaseExecutor<'featureType, 'setupType> (parent: ITest, internals: TestInternals<'featureType, 'setupType>) =
    let testLifecycleEvent = Event<TestExecutionDelegate, TestEventLifecycle> ()
    
    let getApiEnvironment () =
        let assembly = System.Reflection.Assembly.GetExecutingAssembly ()
        let version = assembly.GetName().Version
        
        {
            ApiName = "Archer.Arrows"
            ApiVersion = version
        }
        
    let executionStarted (cancelEventArgs: CancelEventArgs) =
        try
            testLifecycleEvent.Trigger (parent, TestStartExecution cancelEventArgs)
            cancelEventArgs, Empty
        with
        | ex -> cancelEventArgs, (None, None, ex |> GeneralExceptionFailure) |> FailureAccumulated
        
    let runFeatureSetup (cancelEventArgs: CancelEventArgs, acc) =
        if cancelEventArgs.Cancel then
            cancelEventArgs, acc
        else
            match acc with
            | Empty ->
                try
                    testLifecycleEvent.Trigger (parent, TestStartSetup cancelEventArgs)
                    
                    if cancelEventArgs.Cancel then
                        cancelEventArgs, acc
                    else
                        let result = () |> internals.FeatureSetup
                        
                        cancelEventArgs, (result |> FeatureSetupRun)
                with
                | ex ->
                    cancelEventArgs, ex |> SetupTeardownExceptionFailure |> Error |> FeatureSetupRun
            | _ -> cancelEventArgs, acc
            
    let runSetup (cancelEventArgs: CancelEventArgs, acc) =
        if cancelEventArgs.Cancel then
            cancelEventArgs, acc
        else
            match acc with
            | FeatureSetupRun (Ok result) ->
                try
                    let sResult = internals.TestSetup result
                    
                    let setupResult =
                        match sResult with
                        | Ok _ -> SetupSuccess
                        | Error errorValue ->
                            errorValue |> SetupFailure
                        
                    testLifecycleEvent.Trigger (parent, TestEndSetup (setupResult, cancelEventArgs))
                    cancelEventArgs, SetupRun (Ok result, sResult)
                with
                | ex -> cancelEventArgs, ((Ok result, ex |> SetupTeardownExceptionFailure |> Error) |> SetupRun)
            | _ -> cancelEventArgs, acc
        
    let runTestBody environment (cancelEventArgs: CancelEventArgs, acc) =
        if cancelEventArgs.Cancel then
            cancelEventArgs, acc
        else
            match acc with
            | SetupRun(featureResult, Ok value) ->
                try
                    testLifecycleEvent.Trigger (parent, TestStart cancelEventArgs)
                    try
                        if cancelEventArgs.Cancel then
                            cancelEventArgs, acc
                        else
                            let testResult = environment |> internals.TestBody value
                            let result = (featureResult, Ok value, testResult) |> TestRun
                            
                            try
                                testLifecycleEvent.Trigger (parent, TestEnd testResult)
                                cancelEventArgs, result
                            with
                            | ex -> cancelEventArgs, (featureResult |> Some ,value |> Ok |> Some, ex |> GeneralExceptionFailure) |> FailureAccumulated
                    with
                    | ex -> cancelEventArgs, (featureResult ,value |> Ok, ex |> TestExceptionFailure |> TestFailure) |> TestRun
                with
                | ex -> cancelEventArgs, (featureResult |> Some ,value |> Ok |> Some, ex |> GeneralExceptionFailure) |> FailureAccumulated
            | _ -> cancelEventArgs, acc
    
    let mutable teardownTriggered = false    
    let triggerTearTeardown () =
        try
            if teardownTriggered |> not then
                testLifecycleEvent.Trigger (parent, TestStartTeardown)
                teardownTriggered <- true
            
            Ok ()
        with
            | ex -> ex |> SetupTeardownExceptionFailure |> Error
            
    let runTeardown featureResult setupResult testResult =
        try
            let result = internals.TestTeardown setupResult testResult
            
            TeardownRun (featureResult, setupResult, testResult, result)
        with
        | ex ->
            TeardownRun (featureResult, setupResult, testResult, ex |> SetupTeardownExceptionFailure |> Error)
            
    let runFeatureTeardown featureResult setupResult testResult =
        try
            let result = internals.FeatureTeardown featureResult testResult
            
            FeatureTeardownRun (featureResult, setupResult, testResult, result)
        with
        | ex -> FeatureTeardownRun (featureResult, setupResult, testResult, ex |> SetupTeardownExceptionFailure |> Error)
        
        
    let maybeRunTeardown (cancelEventArgs: CancelEventArgs, acc) =
        match acc with
        | FeatureSetupRun result ->
            let triggered = triggerTearTeardown ()
            match triggered with
            | Ok _ ->
                cancelEventArgs, runFeatureTeardown result None None
            | Error err ->
                cancelEventArgs, FeatureTeardownRun (result, None, None, Error err)  
        | SetupRun (featureResult, setupResult) ->
            let triggered = triggerTearTeardown ()
            match triggered with
            | Ok _ ->
                cancelEventArgs, runTeardown featureResult setupResult None
            | Error err ->
                cancelEventArgs, TeardownRun (featureResult, setupResult, None, Error err)
        | TestRun (featureResult, setupResult, testResult) ->
            let triggered = triggerTearTeardown ()
            match triggered with
            | Ok _ ->
                cancelEventArgs, runTeardown featureResult setupResult (Some testResult)
            | Error err ->
                cancelEventArgs, TeardownRun (featureResult, setupResult, Some testResult, Error err)
        | FailureAccumulated (Some featureResult, Some setupResult, _) ->
            let triggered = triggerTearTeardown ()
           
            match triggered with
            | Ok _ ->
                let r = runTeardown featureResult setupResult None
                match r with
                | TeardownRun (_, _, _, Ok ()) ->
                    let r = runFeatureTeardown featureResult (Some setupResult) None
                    match r with
                    | FeatureTeardownRun (_, _, _, Ok ()) ->
                        cancelEventArgs, acc
                    | FeatureTeardownRun (_, _, _, Error _) ->
                        cancelEventArgs, r    
                | TeardownRun (_, _, _, Error _) ->
                    cancelEventArgs, r
                | _ -> failwith "should not get here"
            | Error err ->
                cancelEventArgs, FeatureTeardownRun (featureResult, Some setupResult, None, Error err)
        | FailureAccumulated (None, _, _) -> cancelEventArgs, acc
        | FailureAccumulated (Some featureResult, None, _) ->
            let triggered = triggerTearTeardown ()
            match triggered with
            | Ok _ ->
                cancelEventArgs, acc
            | Error errorValue ->
                cancelEventArgs, FeatureTeardownRun (featureResult, None, None, Error errorValue)
        | _ -> cancelEventArgs, acc
        
    let maybeRunFeatureTeardown (cancelEventArgs: CancelEventArgs, acc) = 
        match acc with
        | FeatureSetupRun setupResult ->
            cancelEventArgs, runFeatureTeardown setupResult None None
        | SetupRun (featureResult, setupResult) ->
            let r = runFeatureTeardown featureResult (Some setupResult) None 
            cancelEventArgs, r
        | TestRun (featureResult, setupResult, testResult) ->
            cancelEventArgs, runFeatureTeardown featureResult (Some setupResult) (Some testResult)
        | TeardownRun (featureTypeResult, setupResult, testResult, Ok _) ->
            let r = runFeatureTeardown featureTypeResult (Some setupResult) testResult
            cancelEventArgs, r
        | TeardownRun (setupFeatureResult, setupResult, testResult, Error _) ->
            let r = runFeatureTeardown setupFeatureResult (Some setupResult) testResult
            match r with
            | FeatureTeardownRun (_, _, _, Ok _) ->
                cancelEventArgs, acc
            | FeatureTeardownRun (_, _, _, Error _) ->
                cancelEventArgs, r
            | _ -> failwith "should not get here"
        | FailureAccumulated (Some setupFeatureResult, setupResult, _) ->
            let r = runFeatureTeardown setupFeatureResult setupResult None
            match r with
            | FeatureTeardownRun (_, _, _, Ok _) ->
                cancelEventArgs, acc
            | FeatureTeardownRun (_, _, _, Error _) ->
                cancelEventArgs, r
            | _ -> failwith "should not get here"
        | FailureAccumulated (None, _, _) ->
            cancelEventArgs, acc
        | _ -> cancelEventArgs, acc
        
    member _.Execute environment =
        let env = 
            {
                ApiEnvironment = getApiEnvironment ()
                FrameworkEnvironment = environment
                TestInfo = parent 
            }
            
        let cancelEventArgs, result =
            CancelEventArgs ()
            |> executionStarted
            |> runFeatureSetup
            |> runSetup
            |> runTestBody env
            |> maybeRunTeardown
            |> maybeRunFeatureTeardown
        
        let finalValue =
            match cancelEventArgs.Cancel, result with
            | true, _ -> GeneralCancelFailure |> GeneralExecutionFailure
            | _, FailureAccumulated (_, _, generalTestingFailure) ->
                generalTestingFailure |> GeneralExecutionFailure
            | _, SetupRun (Error error, _) ->
                error |> SetupExecutionFailure
            | _, SetupRun (_, Error error) ->
                error |> SetupExecutionFailure
            | _, TestRun (_, _, result) ->
                result |> TestExecutionResult
            | _, TeardownRun (_, _, _, Error errorValue) ->
                errorValue |> TeardownExecutionFailure
            | _, FeatureTeardownRun (_featureResult, _setupResult, _testResultOption, Error errorValue) ->
                errorValue |> TeardownExecutionFailure
            | _, FeatureTeardownRun (_featureResult, Some (Error errorValue), _testResultOption, _teardownResult) ->
                errorValue |> SetupExecutionFailure
            | _, FeatureTeardownRun (Error errorValue, _setupResult, _testResultOption, _teardownResult) ->
                errorValue |> SetupExecutionFailure
            | _, FeatureTeardownRun (Ok _, Some (Ok _), Some testResult, Ok _) ->
                testResult |> TestExecutionResult
            | _ -> failwith "Should never get here"
            
        let isEmpty value =
            match value with
            | Empty -> true
            | _ -> false

        try
            if cancelEventArgs.Cancel && result |> isEmpty then
                finalValue
            else
                testLifecycleEvent.Trigger (parent, TestEndExecution finalValue)
                finalValue
        with
        | ex -> ex |> GeneralExceptionFailure |> GeneralExecutionFailure
        
    override _.ToString () =
        $"%s{parent.ToString ()}.IExecutor"
    
    interface ITestExecutor with
        member this.Parent = parent
        
        member this.Execute environment = this.Execute environment
        
        [<CLIEvent>]
        member this.TestLifecycleEvent = testLifecycleEvent.Publish

type TestCase<'featureType, 'setupType> (containerPath: string, containerName: string, testName: string, workings: TestInternals<'featureType, 'setupType>, tags: TestTag seq, filePath: string, fileName: string,  lineNumber: int) =
    let location = {
        FilePath = filePath
        FileName = fileName
        LineNumber = lineNumber 
    }
    
    member _.ContainerPath with get () = containerPath
    member _.ContainerName with get () = containerName
    member _.TestName with get () = testName
    member _.Location with get () = location
    member _.Tags with get () = tags
        
    override _.ToString () =
        [
            containerPath
            containerName
            testName
        ]
        |> List.filter (String.IsNullOrWhiteSpace >> not)
        |> fun items -> String.Join (".", items)
    
    interface ITest with
        member this.ContainerName = this.ContainerName
        member this.ContainerPath = this.ContainerPath
        member this.GetExecutor() =
            
            TestCaseExecutor (this :> ITest, workings) :> ITestExecutor
        member this.Location = this.Location
        member this.Tags = this.Tags
        member this.TestName = this.TestName
        
type Feature<'featureType> (featurePath, featureName, featureSetup: SetupIndicator<unit, 'featureType>, featureTeardown: TeardownIndicator<'featureType>) =
    let mutable tests: ITest list = []
    
    let wrapTestBody (testBody: 'a -> TestResult) =
        TestWithEnvironmentBody (fun setupResult _env -> testBody setupResult)
        
    let wrapTeardown teardown =
        let (Teardown teardown) = teardown
        Teardown (fun _ -> teardown (Ok ()))
    
    let (Setup featureSetup) = featureSetup
    let (Teardown featureTeardown) = featureTeardown
    
    member _.FeatureSetup with get () = featureSetup
    member _.FeatureTeardown with get () = featureTeardown
    
    // --------- TEST TAGS ---------
    member _.Test (tags: TagsIndicator, setup: SetupIndicator<_, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        let fileInfo = FileInfo fileFullName
        let filePath = fileInfo.Directory.FullName
        let fileName = fileInfo.Name
        
        let test =
            match tags, setup, testBody, teardown with
            | TestTags tags, Setup setup, TestWithEnvironmentBody testBody, Teardown teardown -> 
                let internals: TestInternals<'featureType, 'setupType> = { FeatureSetup = featureSetup; TestSetup = setup; TestBody = testBody; TestTeardown = teardown; FeatureTeardown = featureTeardown }
                TestCase (featurePath, featureName, testName, internals, tags, filePath, fileName, lineNumber) :> ITest
        
        tests <- test::tests
        test
            
    member this.Test (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'a>, testBody: TestBodyWithEnvironmentIndicator<'a>, teardown: TeardownIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (tags, setup, testBody, teardown, testName, fileFullName, lineNumber)
             
    member this.Test (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'a>, TestBody testBody, teardown: TeardownIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (tags, setup, wrapTestBody testBody, teardown, testName, fileFullName, lineNumber)
    
    member this.Test (tags: TagsIndicator, testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (tags, Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
    
    member this.Test (tags: TagsIndicator, TestBody testBody, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (tags, Setup Ok, wrapTestBody testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
    
    member this.Test (testName: string, tags: TagsIndicator, testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (tags, Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
    
    member this.Test (testName: string, tags: TagsIndicator, TestBody testBody, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (tags, Setup Ok, wrapTestBody testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
        
    member this.Test (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'a>, testBody: TestBodyWithEnvironmentIndicator<'a>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (tags, setup, testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Test (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'a>, TestBody testBody, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (tags, setup, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Test (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'a>, testBody: TestBodyWithEnvironmentIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (tags, setup, testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Test (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'a>, TestBody testBody, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (tags, setup, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Test (tags: TagsIndicator, testBody: 'featureType -> TestEnvironment -> TestResult, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (tags, Setup Ok, TestWithEnvironmentBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Test (tags: TagsIndicator, testBody: 'featureType -> TestResult, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (tags, Setup Ok, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Test (testName: string, tags: TagsIndicator, testBody: 'featureType -> TestEnvironment -> TestResult, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (tags, Setup Ok, TestWithEnvironmentBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Test (testName: string, tags: TagsIndicator, testBody: 'featureType -> TestResult, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (tags, Setup Ok, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
            
    // --------- SET UP ---------
    member this.Test (setup: SetupIndicator<'featureType, 'a>, testBody: TestBodyWithEnvironmentIndicator<'a>, teardown: TeardownIndicator<'a>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], setup, testBody, teardown, testName, fileFullName, lineNumber)
        
    member this.Test (setup: SetupIndicator<'featureType, 'a>, TestBody testBody, teardown: TeardownIndicator<'a>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], setup, wrapTestBody testBody, teardown, testName, fileFullName, lineNumber)
        
    member this.Test (testName: string, setup: SetupIndicator<'featureType, 'a>, testBody: TestBodyWithEnvironmentIndicator<'a>, teardown: TeardownIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], setup, testBody, teardown, testName, fileFullName, lineNumber)
        
    member this.Test (testName: string, setup: SetupIndicator<'featureType, 'a>, TestBody testBody, teardown: TeardownIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], setup, wrapTestBody testBody, teardown, testName, fileFullName, lineNumber)
        
    member this.Test (setup: SetupIndicator<'featureType, 'a>, testBody: TestBodyWithEnvironmentIndicator<'a>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], setup, testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Test (setup: SetupIndicator<'featureType, 'a>, TestBody testBody, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], setup, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Test (testName: string, setup: SetupIndicator<'featureType, 'a>, testBody: TestBodyWithEnvironmentIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], setup, testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Test (testName: string, setup: SetupIndicator<'featureType, 'a>, TestBody testBody, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], setup, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    // --------- TEST BODY ---------
    member this.Test (testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
    
    member this.Test (TestBody testBody, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], Setup Ok, wrapTestBody testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
        
    member this.Test (testName: string, testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
        
    member this.Test (testName: string, TestBody testBody, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], Setup Ok, wrapTestBody testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
    member this.Test (testBody: 'featureType -> TestEnvironment -> TestResult, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], Setup Ok, TestWithEnvironmentBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
            
    member this.Test (testBody: 'featureType -> TestResult, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], Setup Ok, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
            
    member this.Test (testName: string, testBody: 'featureType -> TestEnvironment -> TestResult, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], Setup Ok, TestWithEnvironmentBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
            
    member this.Test (testName: string, testBody: 'featureType -> TestResult, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], Setup Ok, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
            
    member this.GetTests () = tests
        
    override _.ToString () =
        [
            featurePath
            featureName
        ]
        |> List.filter (String.IsNullOrWhiteSpace >> not)
        |> fun items -> String.Join (".", items)