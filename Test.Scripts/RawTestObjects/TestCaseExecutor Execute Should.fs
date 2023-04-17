module Archer.Arrow.Tests.RawTestObjects.``TestCaseExecutor Execute Should``

open Archer
open Archer.Arrow.Tests
open Archer.CoreTypes.InternalTypes
open Archer.MicroLang

let private container = suite.Container ()

let ``Run the setup action when called`` =
    container.Test (
        SetupPart setupBuildExecutorWithSetupAction,
        
        fun testBuilder _ ->
            let mutable wasRun = false
            let setupAction _ =
                wasRun <- true
                Ok ()
                
            let executor = testBuilder setupAction
            
            executor.Execute (getFakeEnvironment ())
            |> ignore
                
            wasRun
            |> expects.ToBeTrue
            |> withMessage "Setup did not run"
    )
    
let ``Run the test body when called`` =
    container.Test (
        SetupPart setupBuildExecutorWithTestBody,
        
        fun testBuilder _ ->
            let mutable wasRun = false
            let testBody _ _ =
                wasRun <- true
                TestSuccess
                
            let executor = testBuilder testBody
            
            executor.Execute (getFakeEnvironment ())
            |> ignore
            
            wasRun
            |> expects.ToBeTrue
            |> withMessage "Test did not run"
    )
    
let ``Pass the result of setup to the test`` =
    container.Test (
        SetupPart setupBuildExecutorWithTestBodyAndSetupAction,
        
        fun testBuilder _ ->
            let expected = "a value to pass to the test"
            let mutable actual = System.String.Empty
            let setup _ =
                expected |> Ok
                
            let testAction setupValue _ =
                actual <- setupValue
                TestSuccess
                
            let executor = testBuilder setup testAction
            
            executor.Execute (getFakeEnvironment ())
            |> ignore
            
            actual
            |> expects.ToBe expected
    )
    
let ``Not throw except if setup fails`` =
    container.Test (
        SetupPart setupBuildExecutorWithSetupAction,
        
        fun testBuilder _ ->
            let setup _ =
                newFailure.With.SetupTeardownGeneralFailure "failed setup" |> Error
                
            let executor = testBuilder setup
            
            try
                executor.Execute (getFakeEnvironment ())
                |> ignore
                
                TestSuccess
            with
            | ex ->
                ex |> TestExceptionFailure |> TestFailure
    )
    
let ``Return the setup error if setup fails`` =
    container.Test (
        SetupPart setupBuildExecutorWithSetupAction,
        
        fun testBuilder _ ->
            let expectedFailure = newFailure.With.SetupTeardownGeneralFailure "failed setup"
            
            let setup _ =
                expectedFailure |> Error
                
            let executor = testBuilder setup
            
            let result = executor.Execute (getFakeEnvironment ())
            
            result
            |> expects.ToBe (expectedFailure |> SetupExecutionFailure)
    )
    
let ``Return the result of a failing test body when executed`` =
    container.Test (
        SetupPart setupBuildExecutorWithTestBody,
        
        fun testBuilder _ ->
            let expectedFailure = 
                "A failing test"
                |> newFailure.As.TestExecutionResultOf.OtherFailure
                
            let testAction _ _ = expectedFailure
            
            let executor = testBuilder testAction
            
            let result = executor.Execute (getFakeEnvironment ())
            
            result
            |> expects.ToBe (
                expectedFailure
                |> TestExecutionResult
            )
    )
    
let ``Not throw when setup throws`` =
    container.Test (
        SetupPart setupBuildExecutorWithSetupAction,
        
        fun buildTest _ ->
            let expectedErrorMessage = "A really bad setup"
            let setupAction _ =
                failwith expectedErrorMessage
                
            let executor = buildTest setupAction
            
            try
                let result = executor.Execute (getFakeEnvironment ())
                
                match result with
                | SetupExecutionFailure (SetupTeardownExceptionFailure ex) ->
                    ex.Message
                    |> expects.ToBe expectedErrorMessage
                | _ -> "Should not get here" |> newFailure.As.TestExecutionResultOf.OtherFailure
                 
            with
            | ex ->
                ex |> newFailure.As.TestExecutionResultOf.ExceptionFailure
    )
    
let ``Not throw when test action throws`` =
    container.Test (
        SetupPart setupBuildExecutorWithTestBody,
        
        fun testBuilder _ ->
            let expectedErrorMessage = "Really bad test body"
            let testBody _ _ =
                failwith expectedErrorMessage
                
            let executor = testBuilder testBody
            
            try
                let result = executor.Execute (getFakeEnvironment ())
                match result with
                | TestExecutionResult (TestFailure (TestExceptionFailure ex)) ->
                    ex.Message
                    |> expects.ToBe expectedErrorMessage
                | _ -> "Should not get here" |> newFailure.As.TestExecutionResultOf.OtherFailure
            with
                ex -> ex |> newFailure.As.TestExecutionResultOf.ExceptionFailure
    )
    
let ``Run the teardown action when called`` =
    container.Test (
        SetupPart setupBuildExecutorWithTeardownAction,
        
        fun testBuilder _ ->
            let mutable called = false
            
            let teardownAction _ _ =
                called <- true
                Ok ()
                
            let executor = testBuilder teardownAction
            
            executor.Execute (getFakeEnvironment ())
            |> ignore
            
            called
            |> expects.ToBeTrue
            |> withMessage "Teardown was not called"
    )
    
let ``Calls the teardown action with the successful setup result`` =
    container.Test (
        SetupPart setupBuildExecutorWithSetupAndTeardownActions,
        
        fun testBuilder _ ->
            let expectedSetupValue = "Hello from setup"
            let setupAction _ =
                expectedSetupValue |> Ok
                
            let mutable result = newFailure.With.TestExecutionNotRunFailure () |> TestFailure
            
            let teardownAction setupValue _ =
                match setupValue with
                | Ok value ->
                    result <-
                        value
                        |> expects.ToBe expectedSetupValue
                | _ ->
                    result <-
                        "Should not be here" |> newFailure.As.TestExecutionResultOf.OtherFailure
                        
                Ok ()
                
            let executor = testBuilder setupAction teardownAction
            
            executor.Execute (getFakeEnvironment ())
            |> ignore
            
            result
    )
    
let ``Calls the teardown action with the unsuccessful setup result`` =
    container.Test (
        SetupPart setupBuildExecutorWithSetupAndTeardownActions,
        
        fun testBuilder _ ->
            let expectedSetupValue =
                "Bad setup, bad"
                |> newFailure.With.SetupTeardownGeneralFailure
                
            let setupAction _ =
                expectedSetupValue
                |> Error
                
            let mutable result = newFailure.With.TestExecutionNotRunFailure () |> TestFailure
            
            let teardownAction setupValue _ =
                match setupValue with
                | Error value ->
                    result <-
                        value
                        |> expects.ToBe expectedSetupValue
                | _ ->
                    result <-
                        "Should not be here" |> newFailure.As.TestExecutionResultOf.OtherFailure
                        
                Ok ()
                
            let executor = testBuilder setupAction teardownAction
            
            executor.Execute (getFakeEnvironment ())
            |> ignore
            
            result
    )

let ``Calls the teardown with the TestSuccess if test is successful`` =
    container.Test (
        SetupPart setupBuildExecutorWithTeardownAction,
        
        fun testBuilder _ ->
            let mutable result = newFailure.With.TestExecutionNotRunFailure () |> TestFailure
            
            let teardownAction _ testResult =
                result <-
                    testResult
                    |> expects.ToBe (Some TestSuccess)
                    
                Ok ()
                
            let executor : ITestExecutor = testBuilder teardownAction
            
            executor.Execute (getFakeEnvironment ())
            |> ignore
            
            result
    )

let ``Calls the teardown with the TestFailure if test fails`` =
    container.Test (
        SetupPart setupBuiltExecutorWithTestBodyAndTeardownAction,
        
        fun testBuilder _ ->
            let mutable result = newFailure.With.TestExecutionNotRunFailure () |> TestFailure
            
            let expectedFailure =
                newFailure.With.TestExecutionOtherFailure "a failed test"
                |> TestFailure
            
            let testAction _ _ =
                expectedFailure
            
            let teardownAction _ testResult =
                result <-
                    testResult
                    |> expects.ToBe (Some expectedFailure)
                    
                Ok ()
                
            let executor : ITestExecutor = testBuilder testAction teardownAction
            
            executor.Execute (getFakeEnvironment ())
            |> ignore
            
            result
    )

let ``Return the failure if the teardown action fails`` =
    container.Test (
        SetupPart setupBuildExecutorWithTeardownAction,
        
        fun testBuilder _ ->
            let teardownFailure = 
                "failed teardown"
                |> newFailure.With.SetupTeardownGeneralFailure
            
            let teardownAction _ _ =
                teardownFailure
                |> Error
                
            let executor = testBuilder teardownAction
            
            executor.Execute (getFakeEnvironment ())
            |> expects.ToBe (
                teardownFailure
                |> TeardownExecutionFailure
            )
    )

let ``Return failure if teardown throws exception`` =
    container.Test (
        SetupPart setupBuildExecutorWithTeardownAction,
        
        fun testBuilder _ ->
            let expectedErrorMessage = "Boom goes the teardown"
            let teardownAction _ _ =
                failwith expectedErrorMessage
                
            let executor = testBuilder teardownAction
            
            try
                let result = executor.Execute (getFakeEnvironment ())
                match result with
                | TeardownExecutionFailure (SetupTeardownExceptionFailure ex) ->
                    ex.Message
                    |> expects.ToBe expectedErrorMessage
                    
                | _ -> "Should not be here" |> newFailure.With.TestExecutionOtherFailure |> TestFailure
            with
            | ex -> ex |> newFailure.With.TestExecutionExceptionFailure |> TestFailure
    )

let ``Test Cases`` = container.Tests