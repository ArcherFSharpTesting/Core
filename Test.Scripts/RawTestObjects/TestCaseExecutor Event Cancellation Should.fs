module Archer.Arrows.Tests.RawTestObjects.``TestCaseExecutor Event Cancellation Should``

open Archer.Arrows
open Archer.Arrows.Tests
open Archer.CoreTypes.InternalTypes
open Archer.MicroLang
open Microsoft.FSharp.Control

let private feature = Arrow.NewFeature ()

let ``Stop all events if done at TestExecutionStart`` =
    feature.Test (
        Setup setupExecutor,
        
        TestBody (fun (executor: ITestExecutor) ->
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
    )
    
let ``Not call any methods when canceled in TestExecutionStart`` =
    feature.Test (
        Setup setupBuildExecutorWithMonitor,
        
        TestBody (fun (monitor: Monitor<unit, unit>, executor: ITestExecutor) ->
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
    )
    
let ``Stop all event when canceled at TestStartSetup`` =
    feature.Test (
        Setup setupExecutor,
        
        TestBody (fun (executor: ITestExecutor) ->
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
                | TestStartSetup _
                | TestEndExecution _ -> false
                | _ -> true
            )
            |> expects.ToNotBeTriggered
            |> by (executor |> executeFunction)
        )
    )
    
let ``Call Teardown if canceled on TestEndSetup`` =
    feature.Test (
        Setup setupBuildExecutorWithMonitor,
        
        TestBody (fun (monitor: Monitor<unit, unit>, executor: ITestExecutor) ->
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
    )
    
let ``Should trigger ending events if canceled at TestEndSetup`` =
    feature.Test (
        Setup setupExecutor,
        
        TestBody (fun (executor: ITestExecutor) ->
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
    )
    
let ``Call Teardown if canceled on TestStart`` =
    feature.Test (
        Setup setupBuildExecutorWithMonitor,
        
        TestBody (fun (monitor: Monitor<unit, unit>, executor: ITestExecutor) ->
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
    )
    
let ``Should trigger ending events if canceled at TestStart`` =
    feature.Test (
        Setup setupExecutor,
        
        TestBody (fun (executor: ITestExecutor) ->
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
    )
    
let ``Should not trigger TestEnd if canceled at TestStart`` =
    feature.Test (
        Setup setupExecutor,
        
        TestBody (fun (executor: ITestExecutor) ->
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
            |> withMessage "TestEnd"
        )
    )
    
let ``Should not call the test action if canceled at TestStart`` =
    feature.Test (
        Setup setupBuildExecutorWithMonitor,
        
        TestBody (fun (monitor: Monitor<unit, unit>, executor: ITestExecutor) ->
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
    )
    
let ``Test Cases`` = feature.GetTests ()