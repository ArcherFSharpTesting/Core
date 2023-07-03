module Archer.Arrows.Internals

open System
open System.ComponentModel
open System.IO
open Archer
open Archer.Arrows
open Archer.CoreTypes.InternalTypes
    
let getLocation (fileFullName: string) (lineNumber: int) =
    let fileInfo = FileInfo fileFullName
    if fileInfo = null then failwith $"'%s{fileFullName}' was null file info"
    if fileInfo.Directory = null then failwith $"'%s{fileFullName}' has no directory"
    let filePath = fileInfo.Directory.FullName
    let fileName = fileInfo.Name
    
    {
        FilePath = filePath
        FileName = fileName
        LineNumber = lineNumber 
    }

type TestInternals = {
    ContainerPath: string
    ContainerName: string
    TestName: string
    Tags: TestTag seq
    FilePath: string
    FileName: string
    LineNumber: int
}

type SetupTeardownExecutorLifecycleEventArgs =
    | ExecuteSetupStart of CancelEventArgs
    | ExecuteSetupEnd of result: SetupResult * cancelEventArgs: CancelEventArgs
    | ExecuteRunner of CancelEventArgs
    | ExecuteRunnerEnd of TestExecutionResult
    | ExecuteStartTeardown 
    
type SetupTeardownExecutionDelegate = delegate of obj * SetupTeardownExecutorLifecycleEventArgs -> unit

type ISetupTeardownExecutor<'inputType> =
    [<CLIEvent>]
    abstract member LifecycleEvent: IEvent<SetupTeardownExecutionDelegate, SetupTeardownExecutorLifecycleEventArgs> with get
    abstract member Execute: value: 'inputType -> TestEnvironment -> TestExecutionResult
    abstract member Clone: unit -> ISetupTeardownExecutor<'inputType>
    
type SetupTeardownExecutor<'inputType, 'outputType>(setup: 'inputType -> Result<'outputType, SetupTeardownFailure>, teardown: Result<'outputType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>, runner: 'outputType -> TestEnvironment -> TestExecutionResult) =
    let lifecycleEvent = Event<SetupTeardownExecutionDelegate, SetupTeardownExecutorLifecycleEventArgs> ()
    
    abstract member Trigger: sender:obj * args:SetupTeardownExecutorLifecycleEventArgs -> unit
    default _.Trigger (sender: obj, args: SetupTeardownExecutorLifecycleEventArgs) =
        lifecycleEvent.Trigger (sender, args)
        
    abstract member Clone: unit -> ISetupTeardownExecutor<'inputType>
    default _.Clone () = SetupTeardownExecutor<'inputType, 'outputType>(setup, teardown, runner) :> ISetupTeardownExecutor<'inputType>
    
    interface ISetupTeardownExecutor<'inputType> with
        [<CLIEvent>]
        member this.LifecycleEvent = lifecycleEvent.Publish
        
        member this.Execute (value: 'inputType) (env: TestEnvironment) =
            try
                let cancelEventArgs = CancelEventArgs ()
                this.Trigger (this, ExecuteSetupStart cancelEventArgs)
                
                if cancelEventArgs.Cancel then
                    SetupTeardownCanceledFailure |> SetupExecutionFailure
                else
                    let setupResult = setup value
                    
                    let setupReport =
                        match setupResult with
                        | Ok _ -> SetupSuccess
                        | Error errorValue -> errorValue |> SetupFailure
                        
                    this.Trigger (this, ExecuteSetupEnd (setupReport, cancelEventArgs))
                    
                    if cancelEventArgs.Cancel then
                        SetupTeardownCanceledFailure |> SetupExecutionFailure
                    else
                        try
                            let runnerResult = 
                                match setupResult with
                                | Ok r ->
                                    this.Trigger (this, ExecuteRunner cancelEventArgs)
                                    if cancelEventArgs.Cancel then
                                        GeneralCancelFailure |> GeneralExecutionFailure
                                    else
                                        try
                                            let runResult = runner r env
                                            try
                                                this.Trigger (this, ExecuteRunnerEnd runResult)
                                                runResult
                                            with
                                            | ex -> ex |> GeneralExceptionFailure |> GeneralExecutionFailure
                                        with
                                        | ex -> ex |> TestExceptionFailure |> TestFailure |> TestExecutionResult
                                | Error setupError -> setupError |> SetupExecutionFailure
                            
                            try
                                this.Trigger (this, ExecuteStartTeardown)
                                try
                                    
                                    let teardownResult =     
                                        match runnerResult with
                                        | TestExecutionResult testResult -> teardown setupResult (Some testResult)
                                        | _ -> teardown setupResult None
                                    
                                    match teardownResult with
                                    | Ok _ -> runnerResult
                                    | Error setupTeardownFailure -> setupTeardownFailure |> TeardownExecutionFailure
                                with
                                | ex -> ex |> SetupTeardownExceptionFailure |> TeardownExecutionFailure
                            with
                            | ex -> ex |> GeneralExceptionFailure |> GeneralExecutionFailure
                        with
                        | ex -> ex |> GeneralExceptionFailure |> GeneralExecutionFailure
            with
            | ex -> ex |> SetupTeardownExceptionFailure |> SetupExecutionFailure

        member this.Clone() = this.Clone ()
        
type TestCaseExecutor(parent: ITest, internals: ISetupTeardownExecutor<unit>) =
    let testLifecycleEvent = Event<TestExecutionDelegate, TestEventLifecycle> ()
    
    let handleEvents (cancelEventArgs: CancelEventArgs) (_sender: obj) args =
        match args with
        | ExecuteSetupStart eventArgs ->
            cancelEventArgs.Cancel <- eventArgs.Cancel
            testLifecycleEvent.Trigger (parent, TestStartSetup cancelEventArgs)
            eventArgs.Cancel <- cancelEventArgs.Cancel
        | ExecuteSetupEnd (result, eventArgs) ->
            cancelEventArgs.Cancel <- eventArgs.Cancel
            testLifecycleEvent.Trigger (parent, TestEndSetup (result, cancelEventArgs))
            eventArgs.Cancel <- cancelEventArgs.Cancel
        | ExecuteStartTeardown ->
            testLifecycleEvent.Trigger (parent, TestStartTeardown)
        | ExecuteRunner eventArgs ->
            cancelEventArgs.Cancel <- eventArgs.Cancel
            testLifecycleEvent.Trigger (parent, TestStart cancelEventArgs)
            eventArgs.Cancel <- cancelEventArgs.Cancel
        | ExecuteRunnerEnd testExecutionResult ->
            match testExecutionResult with
            | TestExecutionResult testResult -> testLifecycleEvent.Trigger (parent, TestEnd testResult)
            | _ -> ()
    
    let getApiEnvironment () =
        let assembly = System.Reflection.Assembly.GetExecutingAssembly ()
        let version = assembly.GetName().Version
        
        {
            ApiName = "Archer.Arrows"
            ApiVersion = version
        }
        
    member _.Execute environment =
        let env = 
            {
                ApiEnvironment = getApiEnvironment ()
                RunEnvironment = environment
                TestInfo = parent 
            }
            
        let cancelEventArgs = CancelEventArgs ()
        let handler = handleEvents cancelEventArgs
        try
            testLifecycleEvent.Trigger (parent, TestStartExecution cancelEventArgs)
            if cancelEventArgs.Cancel then
                GeneralCancelFailure |> GeneralExecutionFailure
            else
                try
                    internals.LifecycleEvent.AddHandler handler
                        
                    try
                        let result = internals.Execute () env
                        
                        testLifecycleEvent.Trigger (parent, TestEndExecution result)
                        
                        result
                    with
                    | ex -> ex |> GeneralExceptionFailure |> GeneralExecutionFailure
                finally
                    internals.LifecycleEvent.RemoveHandler handler
        with
        | ex -> ex |> GeneralExceptionFailure |> GeneralExecutionFailure
        
    override _.ToString () =
        $"%s{parent.ToString ()}.IExecutor"
    
    interface ITestExecutor with
        member this.Parent = parent
        
        member this.Execute environment = this.Execute environment
        
        [<CLIEvent>]
        member this.TestLifecycleEvent = testLifecycleEvent.Publish

type TestCase (containerPath: string, containerName: string, testName: string, workings: ISetupTeardownExecutor<unit>, tags: TestTag seq, filePath: string, fileName: string,  lineNumber: int) =
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
        member this.GetExecutor () =
            
            TestCaseExecutor (this :> ITest, workings.Clone ()) :> ITestExecutor
        member this.Location = this.Location
        member this.Tags = this.Tags
        member this.TestName = this.TestName

let emptyTeardown = Teardown (fun _ _ -> Ok ())