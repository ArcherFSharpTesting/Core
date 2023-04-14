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
    
    member _.Execute environment =  failwith "Not Implemented"
    
    interface ITestExecutor with
        member this.Parent = parent
        
        member this.Execute environment = this.Execute environment
        
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