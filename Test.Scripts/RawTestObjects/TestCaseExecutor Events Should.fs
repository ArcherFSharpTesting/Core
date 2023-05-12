module Archer.Arrows.Tests.RawTestObjects.``TestCaseExecutor Events Should``

open Archer
open Archer.Arrows
open Archer.Arrows.Tests
open Archer.CoreTypes.InternalTypes
open Archer.MicroLang
open Microsoft.FSharp.Control

let private feature = Arrow.NewFeature (
    TestTags [
        Category "TestCaseExecutor"
        Category "TestLifecycleEvent"
    ]
)

let ``Trigger all the events in order`` =
    feature.Test (
        Setup setupExecutor,
        
        TestBody (fun (executor: ITestExecutor) ->
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
            
            executor
            |> silentlyRunExecutor
            
            result
            |> andResult (cnt |> expects.ToBe 7 |> withMessage "Did not trigger all events")
        )
    )
    
let ``Trigger events with parent`` =
    feature.Test (
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
            
            executor
            |> silentlyRunExecutor
            
            hasRun
            |> expects.ToBeTrue
            |> withMessage "Events did not trigger"
            |> andResult result
    )
    
let ``Not throw if exception is thrown from TestStartExecution`` =
    feature.Test (
        Setup setupExecutor,
        
        TestBody (fun (executor: ITestExecutor) ->
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
    )
    
let ``Not throw if exception is thrown from TestStartSetup`` =
    feature.Test (
        Setup setupExecutor,
        
        TestBody (fun (executor: ITestExecutor) ->
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
    )
    
let ``Not throw when TestEndSetup throws`` =
    feature.Test (
        Setup setupExecutor,
        
        TestBody (fun (executor: ITestExecutor) ->
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
    )
    
let ``Trigger TestEndSetup with a successful result if setup function returns Ok`` =
    feature.Test (
        Setup setupBuildExecutorWithSetupAction,
        
        TestBody (fun testBuilder ->
            let thing = obj ()
            let setup _ =
                thing |> Ok
                
            let executor: ITestExecutor = testBuilder setup
            let mutable result = newFailure.With.TestExecutionShouldNotRunValidationFailure () |> TestFailure
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestEndSetup (SetupSuccess, _) ->
                    result <- TestSuccess
                | _ -> ()
            )
            
            executor
            |> silentlyRunExecutor
                
            result
        )
    )
    
let ``Trigger TestEndSetup with a failure if setup returns Error`` =
    feature.Test (
        Setup setupBuildExecutorWithSetupAction,
        
        TestBody (fun testBuilder ->
            let expectedMessage = "this is a bad setup" 
            let setup _ =
                 expectedMessage |> newFailure.With.SetupTeardownGeneralFailure |> Error
                
            let executor: ITestExecutor = testBuilder setup
            
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
            |> silentlyRunExecutor
            
            result
        )
    )
    
let ``Not throw when TestStart throws`` =
    feature.Test (
        Setup setupExecutor,
        
        TestBody (fun (executor: ITestExecutor) ->
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
    )
    
let ``Not throw when TestEnd throws`` =
    feature.Test (
        Setup setupExecutor,
        
        TestBody (fun (executor: ITestExecutor) ->
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
    )
    
let ``Pass a successful result to TestEnd`` =
    feature.Test (
        Setup setupExecutor,
        
        TestBody (fun (executor: ITestExecutor) ->
            let mutable result = newFailure.With.TestExecutionWasNotRunValidationFailure () |> TestFailure
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestEnd testResult ->
                    result <- testResult
                | _ -> ()
            )
            
            executor
            |> silentlyRunExecutor
            
            result
        )
    )
    
let ``Pass a failure result to TestEnd`` =
    feature.Test (
        Setup setupBuildExecutorWithTestBody,
        
        TestBody (fun testBuilder ->
            let mutable result = newFailure.With.TestExecutionWasNotRunValidationFailure ()  |> TestFailure
            
            let expectedFailure =
                "this was a failing test"
                |> newFailure.With.TestOtherExpectationFailure
                |> TestFailure
                
            let testBody _ _ =
                expectedFailure
                
            let executor: ITestExecutor = testBuilder testBody
            
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
            |> silentlyRunExecutor
            
            result
        )
    )
    
let ``Not throw when TestStartTeardown throws`` =
    feature.Test (
        Setup setupExecutor,
        
        TestBody (fun (executor: ITestExecutor) ->
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
                | GeneralExecutionFailure (GeneralExceptionFailure ex) ->
                    ex.Message
                    |> expects.ToBe exceptionMessage
                | _ ->
                    expects.NotToBeCalled ()
        )
    )
    
let ``Not throw when TestEndExecution throws`` =
    feature.Test (
        Setup setupExecutor,
        
        TestBody (fun (executor: ITestExecutor) ->
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
    )
    
let ``Passing results are passed to the TestEndExecution event`` =
    feature.Test (
        Setup setupExecutor,
        
        TestBody (fun (executor: ITestExecutor) ->
            let mutable result = newFailure.With.TestExecutionWasNotRunFailure () |> TestFailure
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestEndExecution (TestExecutionResult TestSuccess) ->
                    result <- TestSuccess
                | _ -> ()
            )
            
            executor
            |> silentlyRunExecutor
            
            result
        )
    )
    
let ``Passes setup failure to TestEndExecution event`` =
    feature.Test (
        Setup setupBuildExecutorWithSetupAction,
        
        TestBody (fun testBuilder ->
            let expectedFailure =
                "A failed setup"
                |> newFailure.With.SetupTeardownGeneralFailure
                
            let setup _ =
                expectedFailure
                |> Error
            
            let executor: ITestExecutor = testBuilder setup
                
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
            |> silentlyRunExecutor
            
            result
        )
    )

let ``Passes Test failure to TestEndExecution event`` =
    feature.Test (
        Setup setupBuildExecutorWithTestBody,
        
        TestBody (fun testBuilder ->
            let expectedFailure =
                "A failing test"
                |> newFailure.With.TestOtherExpectationFailure
                |> TestFailure
                
            let testBody _ _ =
                expectedFailure
                
            let executor: ITestExecutor = testBuilder testBody
                
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
            |> silentlyRunExecutor
            
            result
        )
    )

let ``Test Cases`` = feature.GetTests ()