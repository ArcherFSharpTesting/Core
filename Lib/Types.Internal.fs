namespace Archer.Arrows.Internal

open System
open System.ComponentModel
open System.Diagnostics
open System.IO
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer
open Archer.Arrows
open Archer.CoreTypes.InternalTypes

type ExecutionResultsAccumulator<'a> =
    | Empty
    | SetupRun of result: Result<'a, SetupTeardownFailure>
    | TestRun of setupResult: Result<'a, SetupTeardownFailure> * testResult: TestResult
    | TeardownRun of setupResult: Result<'a, SetupTeardownFailure> * testResult: TestResult option * teardownResult: Result<unit, SetupTeardownFailure>
    | FailureAccumulated of GeneralTestingFailure 

type TestCaseExecutor<'a> (parent: ITest, setup: unit -> Result<'a, SetupTeardownFailure>, testBody: 'a -> TestEnvironment -> TestResult, tearDown: Result<'a, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>) =
    let testLifecycleEvent = Event<TestExecutionDelegate, TestEventLifecycle> ()
    
    let getApiEnvironment () =
        let assembly = System.Reflection.Assembly.GetExecutingAssembly ()
        let version = assembly.GetName().Version
        
        {
            ApiName = "Archer.Arrows"
            ApiVersion = version
        }
        
    let executionStarted _ =
        try
            testLifecycleEvent.Trigger (parent, TestStartExecution (CancelEventArgs ()))
            Empty
        with
        | ex -> ex |> GeneralExceptionFailure |> FailureAccumulated
        
    let runSetup acc =
        match acc with
        | Empty ->
            try
                testLifecycleEvent.Trigger (parent, TestStartSetup (CancelEventArgs ()))
                
                let result = () |> setup |> SetupRun
                
                testLifecycleEvent.Trigger (parent, TestEndSetup (SetupSuccess, CancelEventArgs ()))
                
                result
            with
            | ex ->
                ex |> SetupTeardownExceptionFailure |> Error |> SetupRun
        | _ -> acc
        
    let runTestBody environment acc =
        match acc with
        | SetupRun (Ok value as setupState) ->
            try
                testLifecycleEvent.Trigger (parent, TestStart (CancelEventArgs ()))
                
                let result = (setupState, environment |> testBody value) |> TestRun
                
                testLifecycleEvent.Trigger (parent, TestEnd TestSuccess)
                result
            with
            | ex -> (setupState, ex |> TestExceptionFailure |> TestFailure) |> TestRun
        | _ -> acc
        
    let runTeardown setupResult testResult =
        try
            testLifecycleEvent.Trigger (parent, TestStartTeardown)
            
            let result = tearDown setupResult testResult
            let r = TeardownRun (setupResult, testResult, result)
            
            r
        with
        | ex ->
            TeardownRun (setupResult, testResult, ex |> SetupTeardownExceptionFailure |> Error)
        
    let maybeRunTeardown acc =
        match acc with
        | SetupRun setupResult ->
            runTeardown setupResult None
        | TestRun (setupResult, testResult) ->
            runTeardown setupResult (Some testResult)
        | FailureAccumulated _ ->
            acc
        
    member _.Execute environment =
        let env = 
            {
                ApiEnvironment = getApiEnvironment ()
                FrameworkEnvironment = environment
                TestInfo = parent 
            }
            
        
        
        let result =
            executionStarted ()
            |> runSetup
            |> runTestBody env
            |> maybeRunTeardown
        
        let finalValue =
            match result with
            | FailureAccumulated generalTestingFailure ->
                generalTestingFailure |> GeneralExecutionFailure
            | SetupRun (Error error) ->
                error |> SetupExecutionFailure
            | TestRun (_, result) ->
                result |> TestExecutionResult
            | TeardownRun (_setupResult, _testResultOption, Error errorValue) ->
                errorValue
                |> TeardownExecutionFailure
            | TeardownRun (Error errorValue, _testResultOption, _teardownResult) ->
                errorValue
                |> SetupExecutionFailure
            | TeardownRun (Ok _, Some testResult, Ok _) ->
                testResult
                |> TestExecutionResult
            | _ -> failwith "Should never get here"
                
        testLifecycleEvent.Trigger (parent, TestEndExecution (TestSuccess |> TestExecutionResult))
        finalValue
        
    override _.ToString () =
        $"%s{parent.ToString ()}.IExecutor"
    
    interface ITestExecutor with
        member this.Parent = parent
        
        member this.Execute environment = this.Execute environment
        
        [<CLIEvent>]
        member this.TestLifecycleEvent = testLifecycleEvent.Publish


type TestCase<'a> (containerPath: string, containerName: string, testName: string, setup: unit -> Result<'a, SetupTeardownFailure>, testBody: 'a -> TestEnvironment -> TestResult, tearDown: Result<'a, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>, tags: TestTag seq, filePath: string, fileName: string,  lineNumber: int) =
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
        member this.GetExecutor() = TestCaseExecutor (this :> ITest, setup, testBody, tearDown) :> ITestExecutor
        member this.Location = this.Location
        member this.Tags = this.Tags
        member this.TestName = this.TestName
        
type Feature (featurePath, featureName) =
    member _. Test<'a> (tags: TagsIndicator, setup: SetupIndicator<'a>, testBody: TestBodyIndicator<'a>, teardown: TeardownIndicator<'a>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        let fileInfo = FileInfo fileFullName
        let filePath = fileInfo.Directory.FullName
        let fileName = fileInfo.Name
        
        match tags, setup, testBody, teardown with
        | TestTags tags, Setup setup, TestBody testBody, Teardown teardown -> 
            TestCase (featurePath, featureName, testName, setup, testBody, teardown, tags, filePath, fileName, lineNumber) :> ITest
            
    member this.Test (setup: SetupIndicator<'a>, testBody: TestBodyIndicator<'a>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], setup, testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Test (testBody: TestEnvironment -> TestResult, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], Setup (fun _ -> Ok ()), TestBody (fun () -> testBody), Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Test (setup: SetupIndicator<'a>, testBody: TestBodyIndicator<'a>, teardown: TeardownIndicator<'a>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], setup, testBody, teardown, testName, fileFullName, lineNumber)
        
    member this.SubFeature (name: string) =
        if String.IsNullOrWhiteSpace name then
            failwith "Must have a name"
        else
            Feature (this.ToString (), name)
        
    override _.ToString () =
        [
            featurePath
            featureName
        ]
        |> List.filter (String.IsNullOrWhiteSpace >> not)
        |> fun items -> String.Join (".", items)