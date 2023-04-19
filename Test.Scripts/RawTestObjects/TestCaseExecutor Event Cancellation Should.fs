module Archer.Arrows.Tests.RawTestObjects.``TestCaseExecutor Event Cancellation Should``

open Archer
open Archer.Arrows
open Archer.Arrows.Tests
open Archer.CoreTypes.InternalTypes
open Archer.MicroLang
open Microsoft.FSharp.Control

let private container = suite.Container ()

let private executeFunction (executor: ITestExecutor) =
    let run () =
        executor.Execute (getFakeEnvironment ())
        
    run

type private Monitor () =
    let mutable setupCalled = false
    let mutable testActionCalled = false
    let mutable teardownCalled = false
    
    member _.CallSetup _ =
        setupCalled <- true
        Ok ()
        
    member _.CallTestAction _ _ =
        testActionCalled <- true
        TestSuccess
        
    member _.CallTeardown _ _ =
        teardownCalled <- true
        Ok ()
        
    member _.SetupWasCalled with get () = setupCalled
    member _.TeardownWasCalled with get () = teardownCalled
    member _.TestWasCalled with get () = testActionCalled
    member _.WasCalled with get () = setupCalled || teardownCalled || testActionCalled

let private setupBuildExecutorWithMonitor _ =
    let feature = Arrow.NewFeature ()
    let monitor = Monitor ()
    
    let test = feature.Test (Setup monitor.CallSetup, TestBody monitor.CallTestAction, Teardown monitor.CallTeardown)
    Ok (monitor, test.GetExecutor ())

let ``Stop all events if done at TestExecutionStart`` =
    container.Test (
        SetupPart setupExecutor,
        
        fun executor _ ->
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestStartExecution cancelEventArgs ->
                    cancelEventArgs.Cancel <- true
                | _ -> ()
            )
            
            executor.TestLifecycleEvent
            |> expects.ToNotBeTriggeredAndIdentifiedBy (fun args ->
                match args with
                | TestStartExecution _ -> false
                | _ -> true
            )
            |> by (executor |> executeFunction)
    )
    
let ``Not call any methods when canceled in TestExecutionStart`` =
    container.Test (
        SetupPart setupBuildExecutorWithMonitor,
        
        fun (monitor, executor) _ ->
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestStartExecution cancelEventArgs ->
                    cancelEventArgs.Cancel <- true
                | _ -> ()
            )
            
            executor.Execute (getFakeEnvironment ())
            |> ignore
            
            monitor.SetupWasCalled
            |> expects.ToBeFalse
            |> withMessage "Setup method was called"
    )
    
let ``Stop all event when canceled at TestStartSetup`` =
    container.Test (
        SetupPart setupExecutor,
        
        fun executor _ ->
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestStartSetup cancelEventArgs ->
                    cancelEventArgs.Cancel <- true
                | _ -> ()
            )
            
            executor.TestLifecycleEvent
            |> expects.ToNotBeTriggeredAndIdentifiedBy (fun args ->
                match args with
                | TestStartExecution _
                | TestStartSetup _ -> false
                | _ -> true
            )
            |> by (executor |> executeFunction)
    )
    
let ``Test Cases`` = container.Tests