module Archer.Arrows.Tests.RawTestObjects.``TestCaseExecutor Events Should``

open Archer
open Archer.Arrows
open Archer.Arrows.Tests
open Archer.CoreTypes.InternalTypes
open Archer.MicroLang
open Microsoft.FSharp.Control

let private container = suite.Container ()

let ``Trigger all the events in order`` =
    container.Test (
        SetupPart setupExecutor,
        
        fun executor _ ->
            let mutable cnt = 0
            let mutable result = TestSuccess
            
            let checkResult newResult =
                if result = TestSuccess then
                    result <- newResult
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestStartExecution _ ->
                    cnt
                    |> expects.ToBe 0
                    |> withMessage "TestStartExecution"
                    |> checkResult
                | TestStartSetup _ ->
                    cnt
                    |> expects.ToBe 1
                    |> withMessage "TestStartSetup"
                    |> checkResult
                | TestEndSetup _ ->
                    cnt
                    |> expects.ToBe 2
                    |> withMessage "TestEndSetup"
                    |> checkResult
                | TestStart _ ->
                    cnt
                    |> expects.ToBe 3
                    |> withMessage "TestStart"
                    |> checkResult
                | TestEnd _ ->
                    cnt
                    |> expects.ToBe 4
                    |> withMessage "TestEnd"
                    |> checkResult
                | TestStartTeardown ->
                    cnt
                    |> expects.ToBe 5
                    |> withMessage "TestStartTeardown"
                    |> checkResult
                | TestEndExecution _ ->
                    cnt
                    |> expects.ToBe 6
                    |> withMessage "TestEndExecution"
                    |> checkResult
                
                cnt <- cnt + 1
            )
            
            executor.Execute (getFakeEnvironment ())
            |> ignore
            
            result
            |> andResult (cnt |> expects.ToBe 7 |> withMessage "Did not trigger all events")
    )
    
let ``Trigger events with parent`` =
    container.Test (
        fun _ ->
            let feature = Arrow.NewFeature ("Events", "ThatTrigger")
            let test = feature.Test (successfulTest, "From a test")
            let executor = test.GetExecutor ()
            
            let mutable result = TestSuccess
            let mutable hasRun = false
            
            let checkResult getMessage newResult =
                if result = TestSuccess && newResult = TestSuccess then ()
                elif result = TestSuccess then
                    result <-
                        newResult
                        |> withMessage (getMessage ())
                    
            
            executor.TestLifecycleEvent.AddHandler (fun sender args ->
                hasRun <- true
                sender
                |> expects.ToBe executor.Parent
                |> checkResult (fun () -> $"%A{args}")
            )
            
            executor.Execute (getFakeEnvironment ())
            |> ignore
            
            hasRun
            |> expects.ToBeTrue
            |> withMessage "Events did not trigger"
            |> andResult result
    )
    
let ``Not throw if exception is thrown from TestStartExecution`` =
    container.Test (
        SetupPart setupExecutor,
        
        fun executor _ ->
            let expectedExceptionMessage = "Boom goes the start execution event"
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestStartExecution _ ->
                    failwith expectedExceptionMessage
                | _ -> ()
            )
            
            let result =
                executor
                |> executeFunction
                |> expects.ToNotThrowWithResult
                
            match result with
            | Error errorValue -> errorValue
            | Ok testExecutionResult ->
                match testExecutionResult with
                | GeneralExecutionFailure (GeneralExceptionFailure ex) ->
                    ex.Message
                    |> expects.ToBe expectedExceptionMessage
                | _ ->
                    "Should not get here" |> newFailure.With.TestOtherExpectationFailure |> TestFailure
    )
    
let ``Not throw if exception is thrown from TestStartSetup`` =
    container.Test (
        SetupPart setupExecutor,
        
        fun executor _ ->
            let exceptionMessage = "Boom boom test start setup"
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestStartSetup _ ->
                    failwith exceptionMessage
                | _ -> ()
            )
    
            let result =
                executor
                |> executeFunction
                |> expects.ToNotThrowWithResult
                
            match result with
            | Error errorValue -> errorValue
            | Ok testExecutionResult ->
                match testExecutionResult with
                | SetupExecutionFailure (SetupTeardownExceptionFailure ex) ->
                    ex.Message
                    |> expects.ToBe exceptionMessage
                | _ ->
                    "Should not get here" |> newFailure.With.TestOtherExpectationFailure |> TestFailure
    )
    
let ``Not throw when TestEndSetup throws`` =
    container.Test (
        SetupPart setupExecutor,
        
        fun executor _ ->
            let exceptionMessage = "End Setup goes BOOM!"
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestEndSetup _ ->
                    failwith exceptionMessage
                | _ -> ()
            )
    
            let result =
                executor
                |> executeFunction
                |> expects.ToNotThrowWithResult
                
            match result with
            | Error errorValue -> errorValue
            | Ok testExecutionResult ->
                match testExecutionResult with
                | SetupExecutionFailure (SetupTeardownExceptionFailure ex) ->
                    ex.Message
                    |> expects.ToBe exceptionMessage
                | _ ->
                    "Should not get here" |> newFailure.With.TestOtherExpectationFailure |> TestFailure
    )
    
let ``Trigger TestEndSetup with a successful result if setup function returns Ok`` =
    container.Test (
        SetupPart setupBuildExecutorWithSetupAction,
        
        fun testBuilder _ ->
            let thing = obj ()
            let setup _ =
                thing |> Ok
                
            let executor = testBuilder setup
            let mutable result = newFailure.With.TestExecutionShouldNotRunValidationFailure () |> TestFailure
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestEndSetup (SetupSuccess, _) ->
                    result <- TestSuccess
                | _ -> ()
            )
            
            executor
            |> executeFunction
            |> runIt
            |> ignore
                
            result
    )
    
let ``Trigger TestEndSetup with a failure if setup returns Error`` =
    container.Test (
        SetupPart setupBuildExecutorWithSetupAction,
        
        fun testBuilder _ ->
            let expectedMessage = "this is a bad setup" 
            let setup _ =
                 expectedMessage |> newFailure.With.SetupTeardownGeneralFailure |> Error
                
            let executor = testBuilder setup
            

            let mutable result = newFailure.With.TestExecutionWasNotRunValidationFailure () |> TestFailure
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestEndSetup (SetupFailure (GeneralSetupTeardownFailure (message, _)), _) ->
                    result <-
                        message
                        |> expects.ToBe expectedMessage
                | TestEndSetup (setupResult, _) ->
                    result <- $"%A{setupResult}" |> newFailure.With.TestOtherExpectationFailure |> TestFailure
                | _ -> ()
            )
            
            executor
            |> executeFunction
            |> runIt
            |> ignore
            
            result
    )
    
let ``Not throw when TestStart throws`` =
    container.Test (
        SetupPart setupExecutor,
        
        fun executor _ ->
            let exceptionMessage = "bad test start"
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestStart _ ->
                    failwith exceptionMessage
                | _ -> ()
            )
            
            let result = 
                executor
                |> executeFunction
                |> expects.ToNotThrowWithResult
                
            match result with
            | Error errorValue -> errorValue
            | Ok testExecutionResult ->
                match testExecutionResult with
                | GeneralExecutionFailure (GeneralExceptionFailure ex) ->
                    ex.Message
                    |> expects.ToBe exceptionMessage
                | _ ->
                    "Should not get here" |> newFailure.With.TestOtherExpectationFailure |> TestFailure
    )
    
let ``Not throw when TestEnd throws`` =
    container.Test (
        SetupPart setupExecutor,
        
        fun executor _ ->
            let exceptionMessage = "bad TestEnd event"
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestEnd _ -> failwith exceptionMessage
                | _ -> ()
            )
            
            let result =
                executor
                |> executeFunction
                |> expects.ToNotThrowWithResult
                
            match result with
            | Error errorValue -> errorValue
            | Ok testExecutionResult ->
                match testExecutionResult with
                | GeneralExecutionFailure (GeneralExceptionFailure ex) ->
                    ex.Message
                    |> expects.ToBe exceptionMessage
                | _ ->
                    expects.NotToBeCalled ()
    )
    
let ``Pass a successful result to TestEnd`` =
    container.Test (
        SetupPart setupExecutor,
        
        fun executor _ ->
            let mutable result = newFailure.With.TestExecutionWasNotRunValidationFailure () |> TestFailure
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestEnd testResult ->
                    result <- testResult
                | _ -> ()
            )
            
            executor
            |> executeFunction
            |> runIt
            |> ignore
            
            result
    )
    
let ``Pass a failure result to TestEnd`` =
    container.Test (
        SetupPart setupBuildExecutorWithTestBody,
        
        fun testBuilder _ ->
            let mutable result = newFailure.With.TestExecutionWasNotRunValidationFailure ()  |> TestFailure
            
            let expectedFailure =
                "this was a failing test"
                |> newFailure.With.TestOtherExpectationFailure
                |> TestFailure
                
            let testBody _ _ =
                expectedFailure
                
            let executor = testBuilder testBody
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestEnd testResult ->
                    result <-
                        testResult
                        |> expects.ToBe expectedFailure
                | _ -> ()
            )
            
            executor
            |> executeFunction
            |> runIt
            |> ignore
            
            result
    )
    
let ``Not throw when TestStartTeardown throws`` =
    container.Test (
        SetupPart setupExecutor,
        
        fun executor _ ->
            let exceptionMessage = "Test teardown go down the hole"
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestStartTeardown ->
                    failwith exceptionMessage
                | _ -> ()
            )
            
            let result =
                executor
                |> executeFunction
                |> expects.ToNotThrowWithResult
                
            match result with
            | Error errorValue -> errorValue
            | Ok testExecutionResult ->
                match testExecutionResult with
                | TeardownExecutionFailure (SetupTeardownExceptionFailure ex) ->
                    ex.Message
                    |> expects.ToBe exceptionMessage
                | _ ->
                    expects.NotToBeCalled ()
    )
    
let ``Not throw when TestEndExecution throws`` =
    container.Test (
        SetupPart setupExecutor,
        
        fun executor _ ->
            let exceptionMessage = "Test execution did not end well"
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestEndExecution _ ->
                    failwith exceptionMessage
                | _ -> ()
            )
            
            let result =
                executor
                |> executeFunction
                |> expects.ToNotThrowWithResult
                
            match result with
            | Error errorValue -> errorValue
            | Ok testExecutionResult ->
                match testExecutionResult with
                | GeneralExecutionFailure (GeneralExceptionFailure ex) ->
                    ex.Message
                    |> expects.ToBe exceptionMessage
                | _ ->
                    expects.NotToBeCalled ()
    )
    
let ``Passing results are passed to the TestEndExecution event`` =
    container.Test (
        SetupPart setupExecutor,
        
        fun executor _ ->
            let mutable result = newFailure.With.TestExecutionWasNotRunFailure () |> TestFailure
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestEndExecution (TestExecutionResult TestSuccess) ->
                    result <- TestSuccess
                | _ -> ()
            )
            
            executor
            |> executeFunction
            |> runIt
            |> ignore
            
            result
    )
    
let ``Passes setup failure to TestEndExecution event`` =
    container.Test (
        SetupPart setupBuildExecutorWithSetupAction,
        
        fun testBuilder _ ->
            let expectedFailure =
                "A failed setup"
                |> newFailure.With.SetupTeardownGeneralFailure
                
            let setup _ =
                expectedFailure
                |> Error
            
            let executor = testBuilder setup
                
            let mutable result = newFailure.With.TestExecutionShouldNotRunFailure () |> TestFailure
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestEndExecution (SetupExecutionFailure setupTeardownFailure) ->
                    result <-
                        setupTeardownFailure
                        |> expects.ToBe expectedFailure
                | _ -> ()
            )
            
            executor
            |> executeFunction
            |> runIt
            |> ignore
            
            result
    )

let ``Passes Test failure to TestEndExecution event`` =
    container.Test (
        SetupPart setupBuildExecutorWithTestBody,
        
        fun testBuilder _ ->
            let expectedFailure =
                "A failing test"
                |> newFailure.With.TestOtherExpectationFailure
                |> TestFailure
                
            let testBody _ _ =
                expectedFailure
                
            let executor = testBuilder testBody
                
            let mutable result = newFailure.With.TestExecutionWasNotRunValidationFailure () |> TestFailure
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestEndExecution (TestExecutionResult testResult) ->
                    result <-
                        testResult
                        |> expects.ToBe expectedFailure
                | _ -> ()
            )
            
            executor
            |> executeFunction
            |> runIt
            |> ignore
            
            result
    )

let ``Test Cases`` = container.Tests