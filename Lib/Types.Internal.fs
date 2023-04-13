namespace Archer.Arrow.Internal

open System.ComponentModel
open Archer
open Archer.Arrow
open Archer.CoreTypes.InternalTypes
open WhatsYourVersion

type TestParts<'a> = {
    Setup: unit -> Result<'a, SetupTearDownFailure>
    TestAction: TestEnvironment -> 'a -> TestResult
    TearDown: TestResult -> Result<'a, SetupTearDownFailure> -> Result<unit, SetupTearDownFailure>
}

type TestCaseExecutor<'a> (parent: ITest, parts: TestParts<'a>) =
    let testLifecycleEvent = Event<TestExecutionDelegate, TestEventLifecycle> ()
    
    let getApiEnvironment () =
        let assembly = System.Reflection.Assembly.GetExecutingAssembly ()
        let version = assembly.GetName().Version
        
        {
            ApiName = "Archer.Arrow"
            ApiVersion = version
        }
    
    interface ITestExecutor with
        member _.Execute environment =
            let cancelEventArgs = CancelEventArgs ()
            testLifecycleEvent.Trigger (parent, TestStartExecution cancelEventArgs)
            
            if cancelEventArgs.Cancel then
                CancelFailure |> TestFailure
            else
                testLifecycleEvent.Trigger (parent, TestStartSetup cancelEventArgs)
                if cancelEventArgs.Cancel then
                    testLifecycleEvent.Trigger (parent, TestEndExecution (CancelFailure |> TestFailure))
                    CancelFailure |> TestFailure
                else
                    let setupResult =
                        try
                            parts.Setup ()
                        with
                        | e -> e |> SetupTearDownExceptionFailure |> Error
                        
                    let testResult =
                        match setupResult with
                        | Error setupTearDownFailure -> setupTearDownFailure |> SetupFailure |> TestFailure
                        | Ok v ->
                            if cancelEventArgs.Cancel then
                                CancelFailure |> TestFailure
                            else
                                testLifecycleEvent.Trigger (parent, TestStart cancelEventArgs)
                                if cancelEventArgs.Cancel then
                                    CancelFailure |> TestFailure
                                else
                                    try
                                        let env = {
                                            ApiEnvironment = getApiEnvironment ()
                                            FrameworkEnvironment = environment
                                            TestInfo = parent :> ITestInfo 
                                        }
                                        
                                        let r = parts.TestAction env v
                                        testLifecycleEvent.Trigger (parent, TestEnd r)
                                        r
                                    with
                                    | e ->
                                        e |> TestExceptionFailure |> TestExecutionFailure |> TestFailure
                    
                    let endResult =
                        testLifecycleEvent.Trigger (parent, TestStartTearDown)
                        try
                            let teardownResult = parts.TearDown testResult setupResult
                            match teardownResult with
                            | Error setupTearDownFailure -> setupTearDownFailure |> TearDownFailure |> TestFailure
                            | Ok _ -> testResult
                        with
                        | e -> e |> SetupTearDownExceptionFailure |> TearDownFailure |> TestFailure
                        
                    testLifecycleEvent.Trigger (parent, TestEndExecution endResult)
                            
                    endResult
                
        member this.Parent = parent
        
        [<CLIEvent>]
        member this.TestLifecycleEvent = testLifecycleEvent.Publish

type TestCase<'a> (containerPath: string, containerName: string, testName: string, parts: TestParts<'a>, tags: TestTag seq, filePath: string, fileName: string,  lineNumber: int) =
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
        member this.GetExecutor() = TestCaseExecutor (this :> ITest, parts) :> ITestExecutor
        member this.Location = this.Location
        member this.Tags = this.Tags
        member this.TestName = this.TestName