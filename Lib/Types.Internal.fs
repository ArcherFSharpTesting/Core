namespace Archer.Arrow.Internal

open System
open System.Diagnostics
open System.IO
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer
open Archer.Arrow
open Archer.CoreTypes.InternalTypes

type ExecutionResultsAccumulator<'a> =
    | SetupRun of result: Result<'a, SetupTeardownFailure>
    | TestRun of setupResult: Result<'a, SetupTeardownFailure> * testResult: TestResult
    | TeardownRun of setupResult: Result<'a, SetupTeardownFailure> * testResult: TestResult option * teardownResult: Result<unit, SetupTeardownFailure> 

type TestCaseExecutor<'a> (parent: ITest, setup: unit -> Result<'a, SetupTeardownFailure>, testBody: 'a -> TestEnvironment -> TestResult, tearDown: Result<'a, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>) =
    let testLifecycleEvent = Event<TestExecutionDelegate, TestEventLifecycle> ()
    
    let getApiEnvironment () =
        let assembly = System.Reflection.Assembly.GetExecutingAssembly ()
        let version = assembly.GetName().Version
        
        {
            ApiName = "Archer.Arrow"
            ApiVersion = version
        }
        
    let runSetup _ =
        try
            () |> setup |> SetupRun
        with
        | ex ->
            ex |> SetupTeardownExceptionFailure |> Error |> SetupRun
        
    let runTestBody environment acc =
        match acc with
        | SetupRun (Ok value as setupState) ->
            try
                (setupState, environment |> testBody value) |> TestRun
            with
            | ex -> (setupState, ex |> TestExceptionFailure |> TestFailure) |> TestRun
        | _ -> acc
        
    let runTeardown acc =
        let setupResult, testResult = 
            match acc with
            | SetupRun setupResult -> setupResult, None
            | TestRun (setupResult, testResult) ->
                setupResult, (Some testResult)
            
        try
            let result = tearDown setupResult testResult
            TeardownRun (setupResult, testResult, result)
        with
        | ex ->
            TeardownRun (setupResult, testResult, ex |> SetupTeardownExceptionFailure |> Error)
        
    member _.Execute environment =
        let env = 
            {
                ApiEnvironment = getApiEnvironment ()
                FrameworkEnvironment = environment
                TestInfo = parent 
            }
        
        let result =
            runSetup ()
            |> runTestBody env
            |> runTeardown
        
        match result with
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
        
    member this.Test (setup: SetupIndicator<'a>, testBody: TestBodyIndicator<'a>, teardown: TeardownIndicator<'a>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>]lineNumber: int) =
        this.Test (TestTags [], setup, testBody, teardown, testName, fileFullName, lineNumber)

type ArrowBuilder () =
    member _.NewFeature () =
        let featureName, featurePath =
            let trace = StackTrace ()
            let method = trace.GetFrame(1).GetMethod ()
            let containerName = method.ReflectedType.Name
            let containerPath = method.ReflectedType.Namespace |> fun s -> s.Split ([|"$"|], StringSplitOptions.RemoveEmptyEntries) |> Array.last
                
            containerName, containerPath
            
        Feature (featurePath, featureName)
        
    member _.NewFeature (featurePath, featureName) =
        Feature (featurePath, featureName)