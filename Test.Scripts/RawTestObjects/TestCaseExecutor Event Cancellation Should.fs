module Archer.Arrows.Tests.RawTestObjects.``TestCaseExecutor Event Cancellation Should``

open Archer.Arrows.Tests
open Archer.CoreTypes.InternalTypes
open Archer.MicroLang
open Microsoft.FSharp.Control

let private container = suite.Container ()

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
            |> Event.filter (fun args ->
                match args with
                | TestStartExecution _ -> false
                | _ -> true
            )
            |> expects.ToNotBeTriggered
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
            |> Event.filter (fun args ->
                match args with
                | TestStartExecution _
                | TestStartSetup _ -> false
                | _ -> true
            )
            |> expects.ToNotBeTriggered
            |> by (executor |> executeFunction)
    )
    
let ``Call Teardown if canceled on TestEndSetup`` =
    container.Test (
        SetupPart setupBuildExecutorWithMonitor,
        
        fun (monitor, executor) _ ->
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestEndSetup (_, cancelEventArgs) ->
                    cancelEventArgs.Cancel <- true
                | _ -> ()
            )
            
            executor
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TeardownWasCalled
            |> expects.ToBeTrue
            |> withMessage "Teardown was not called"
    )
    
let ``Should trigger ending events if canceled at TestEndSetup`` =
    container.Test (
        SetupPart setupExecutor,
        
        fun executor _ ->
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestEndSetup (_, cancelEventArgs) ->
                    cancelEventArgs.Cancel <- true
                | _ -> ()
            )
            
            executor.TestLifecycleEvent
            |> Event.filter (fun args ->
                match args with
                | TestStartTeardown
                | TestEndExecution _ -> true
                | _ -> false
            )
            |> expects.ToBeTriggered
            |> by (executor |> executeFunction)
    )
    
let ``Call Teardown if canceled on TestStart`` =
    container.Test (
        SetupPart setupBuildExecutorWithMonitor,
        
        fun (monitor, executor) _ ->
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestStart cancelEventArgs ->
                    cancelEventArgs.Cancel <- true
                | _ -> ()
            )
            
            executor
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TeardownWasCalled
            |> expects.ToBeTrue
            |> withMessage "Teardown was not called"
    )
    
let ``Should trigger ending events if canceled at TestStart`` =
    container.Test (
        SetupPart setupExecutor,
        
        fun executor _ ->
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestStart cancelEventArgs ->
                    cancelEventArgs.Cancel <- true
                | _ -> ()
            )
            
            executor.TestLifecycleEvent
            |> Event.filter (fun args ->
                match args with
                | TestStartTeardown
                | TestEndExecution _ -> true
                | _ -> false
            )
            |> expects.ToBeTriggered 
            |> by (executor |> executeFunction)
    )
    
let ``Should not trigger TestEnd if canceled at TestStart`` =
    container.Test (
        SetupPart setupExecutor,
        
        fun executor _ ->
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestStart cancelEventArgs ->
                    cancelEventArgs.Cancel <- true
                | _ -> ()
            )
            
            executor.TestLifecycleEvent
            |> Event.filter (fun args ->
                match args with
                | TestEnd _ -> true
                | _ -> false
            )
            |> expects.ToNotBeTriggered
            |> by (executor |> executeFunction)
    )
    
let ``Should not call the test action if canceled at TestStart`` =
    container.Test (
        SetupPart setupBuildExecutorWithMonitor,
        
        fun (monitor, executor) _ ->
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestStart cancelEventArgs ->
                    cancelEventArgs.Cancel <- true
                | _ -> ()
            )
            
            executor
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> expects.ToBeFalse
            |> withMessage "The test action was run"
    )
    
let ``Test Cases`` = container.Tests