module Archer.Core.Tests.RawTestObjects.``TestCaseExecutor Execute Should``

open System.Threading
open Archer
open Archer.Core
open Archer.Core.Tests
open Archer.Types.InternalTypes
open Archer.MicroLang

let private feature = FeatureFactory.NewFeature (
    TestTags [
        Category "Feature"
        Category "Test"
    ]
)

let ``Run the setup action supplied to feature.Test when called`` =
    feature.Test (
        Setup setupBuildExecutorWithSetupAction,
        
        TestBody (fun testBuilder ->
            let monitor = getTestMonitor<unit, unit, unit> ()
            let setupValue = ()
                
            let executor: ITestExecutor = testBuilder (monitor.FunctionSetupWith setupValue)
            
            executor
            |> silentlyRunExecutor
                
            monitor.HasSetupFunctionBeenCalled
            |> Should.BeTrue
            |> withMessage "Setup did not run"
        )
    )

let ``Run the setup action supplied to feature with called`` =
    feature.Test (
        fun _ ->
            let monitor = getFeatureMonitor<unit> ()
            let setupValue = ()
            
            let testFeature = FeatureFactory.NewFeature (
                ignoreString (),
                ignoreString (),
                Setup (monitor.FunctionSetupWith setupValue)
            )
            
            testFeature.Test(fun _ -> TestSuccess)
            |> silentlyRunTest
            
            monitor.HasSetupFunctionBeenCalled
            |> Should.BeTrue
            |> withMessage "Setup was not called"
    )
    
let ``Run the test body when called`` =
    feature.Test (
        Setup setupBuildExecutorWithTestBody,
        
        TestBody (fun testBuilder ->
            let monitor = getUnitTestMonitor ()
            
            let executor: ITestExecutor = testBuilder monitor.FunctionTestTwoParametersSuccess
            
            executor
            |> silentlyRunExecutor
            
            monitor.HasTestFunctionBeenCalled
            |> Should.BeTrue
            |> withMessage "Test did not run"
        )
    )
    
let ``Pass the result of the setup supplied to feature.Test to the test action`` =
    feature.Test (
        Setup setupBuildExecutorWithMonitor,
        
        TestBody (fun testBuilder ->
            let expected = "a value to pass to the test"
            let monitor = getTestMonitor<unit, unit, string> ()
                
            let executor = testBuilder monitor (Ok expected)
            
            executor
            |> silentlyRunExecutor
            
            monitor
            |> verifyAllTestFunctionShouldHaveBeenCalledWithTestSetupValueOf expected
        )
    )
    
let ``Pass the result of the setup supplied to the feature to the setup supplied to feature.Test`` =
    feature.Test (fun _ ->
        let expectedValue = "Hello from feature setup"
        let testFeature = FeatureFactory.NewFeature (Setup (fun () -> Ok expectedValue))
        
        let monitor = getTestMonitor<unit, string, unit> ()
        let setupValue = ()
        
        let setup = monitor.FunctionSetupFeatureWith setupValue
        let testBody = monitor.FunctionTestFeatureOneParameterSuccess
        
        testFeature.Test(Setup setup, TestBody testBody)
        |> silentlyRunTest
        
        monitor
        |> Should.PassAllOf [
            verifyAllTestFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf expectedValue
        ]
    )
    
let ``Not throw an exception if the setup supplied to feature.Test fails`` =
    feature.Test (
        Setup setupBuildExecutorWithSetupAction,
        
        TestBody (fun testBuilder ->
            let failure = "failed setup" |> newFailure.With.SetupTeardownGeneralFailure
            let monitor = getUnitTestMonitor ()
                
            let executor: ITestExecutor = testBuilder (monitor.FunctionSetupWith failure)
            
            try
                executor
                |> silentlyRunExecutor
                
                TestSuccess
            with
            | ex ->
                ex |> TestExceptionFailure |> TestFailure
        )
    )
    
let ``Not throw an exception if the setup supplied to the feature fails`` =
    feature.Test (fun _ ->
        let failure = "failed feature setup" |> newFailure.With.SetupTeardownGeneralFailure
        let monitor = getUnitTestMonitor ()
        
        let testFeature = FeatureFactory.NewFeature (Setup (monitor.FunctionSetupWith failure))

        try        
            testFeature.Test(fun _ -> TestSuccess)
            |> silentlyRunTest
            
            TestSuccess
        with
        | ex ->
            ex |> TestExceptionFailure |> TestFailure
    )
    
let ``Return the setup error if the setup passed to feature.Test fails`` =
    feature.Test (
        Setup setupBuildExecutorWithSetupAction,
        
        TestBody (fun testBuilder ->
            let expectedFailure = newFailure.With.SetupTeardownGeneralFailure "failed setup"
            let monitor = getUnitTestMonitor ()
            
            let executor: ITestExecutor = testBuilder (monitor.FunctionSetupWith expectedFailure)
            
            let result =
                executor
                |> runExecutor
            
            result
            |> Should.BeEqualTo (expectedFailure |> SetupExecutionFailure)
        )
    )
    
let ``Should not run the setup provided to feature.Test if the setup provided to the feature fails`` =
    feature.Test (fun _ ->
        let monitor = getUnitTestMonitor ()
        
        let testFeature = FeatureFactory.NewFeature<unit> (Setup (fun () -> "failed setup" |> newFailure.With.SetupTeardownGeneralFailure |> Error))
        
        let setupValue = ()
        testFeature.Test(Setup (monitor.FunctionSetupWith setupValue), TestBody (fun _ -> TestSuccess))
        |> silentlyRunTest
        
        monitor.HasSetupFunctionBeenCalled
        |> Should.BeFalse
        |> withMessage "Setup should not have be called"
    )
    
let ``Return the result of a failing test body when executed`` =
    feature.Test (
        Setup setupBuildExecutorWithTestBody,
        
        TestBody (fun testBuilder ->
            let expectedFailure = 
                "A failing test"
                |> newFailure.As.TestExecutionResultOf.OtherExpectationFailure
                 
            let monitor = getUnitTestMonitor ()
                
            let executor: ITestExecutor = testBuilder (monitor.FunctionTestTwoParametersWith expectedFailure)
            
            let result =
                executor
                |> runExecutor
                
            result
            |> Should.BeEqualTo (
                expectedFailure
                |> TestExecutionResult
            )
        )
    )
    
let ``Not throw when the setup passed to feature.Test throws`` =
    feature.Test (
        Setup setupBuildExecutorWithSetupAction,
        
        TestBody (fun buildTest ->
            let expectedErrorMessage = "A really bad setup"
            let monitor = getUnitTestMonitor ()
                
            let executor: ITestExecutor = buildTest (monitor.FunctionSetupFailsWith expectedErrorMessage)
            
            try
                let result =
                    executor
                    |> runExecutor
                
                match result with
                | SetupExecutionFailure (SetupTeardownExceptionFailure ex) ->
                    ex.Message
                    |> Should.BeEqualTo expectedErrorMessage
                | _ -> expects.NotToBeCalled ()
                 
            with
            | ex ->
                ex |> newFailure.As.TestExecutionResultOf.TestExceptionFailure
        )
    )

let ``Not throw when the setup passed to the feature throws`` =
    feature.Test (fun _ ->
        let expectedErrorMessage = "Boom goes the feature"
        let monitor = getUnitTestMonitor ()
        
        let testFeature = FeatureFactory.NewFeature (Setup (monitor.FunctionSetupFailsWith expectedErrorMessage))
        
        try
            testFeature.Test(fun () -> TestSuccess)
            |> silentlyRunTest
            
            TestSuccess
        with
        | ex -> ex |> TestExceptionFailure |> TestFailure
    )

let ``Not throw when test action throws`` =
    feature.Test (
        Setup setupBuildExecutorWithTestBody,
        
        TestBody (fun testBuilder ->
            let expectedErrorMessage = "Really bad test body"
            let monitor = getUnitTestMonitor ()
            let testFunction = monitor.FunctionTestTwoParametersFailWith expectedErrorMessage
                
            let executor: ITestExecutor = testBuilder testFunction
            
            try
                let result =
                    executor
                    |> runExecutor
                
                match result with
                | TestExecutionResult (TestFailure (TestExceptionFailure ex)) ->
                    ex.Message
                    |> Should.BeEqualTo expectedErrorMessage
                | _ -> expects.NotToBeCalled ()
            with
                ex -> ex |> newFailure.As.TestExecutionResultOf.TestExceptionFailure
        )
    )
    
let ``Run the teardown passed to feature.Test`` =
    feature.Test (
        Setup setupBuildExecutorWithTeardownAction,
        
        TestBody (fun testBuilder ->
            let monitor = getUnitTestMonitor ()
                
            let executor: ITestExecutor = testBuilder monitor.FunctionTeardownPassThrough
            
            executor
            |> silentlyRunExecutor
            
            monitor.HasTeardownBeenCalled
            |> Should.BeTrue
            |> withMessage "Teardown was not called")
    )
   
let ``Run the teardown passed to the feature`` =
    feature.Test (fun _ ->
        let monitor = getUnitTestMonitor ()
        
        let testFeature = FeatureFactory.NewFeature (Teardown monitor.FunctionTeardownPassThrough)
        
        testFeature.Test(fun () -> TestSuccess)
        |> silentlyRunTest
        
        monitor.HasTeardownBeenCalled
        |> Should.BeTrue
        |> withMessage "Teardown was not called"
    )
    
let ``Calls the teardown that was passed to feature.Test with the successful result of the setup passed to feature.Test`` =
    feature.Test (
        Setup setupBuildExecutorWithSetupAndTeardownActions,
        
        TestBody (fun testBuilder ->
            let expectedSetupValue = "Hello from setup"
            let monitor = getTestMonitor<unit, unit, string> ()
            
            let executor: ITestExecutor = testBuilder (monitor.FunctionSetupWith expectedSetupValue) monitor.FunctionTeardownFromSetup
            
            executor
            |> silentlyRunExecutor
            
            monitor.TeardownFunctionParameterValues
            |> List.map fst
            |> Should.BeEqualTo [Ok (None, Some expectedSetupValue)]
        )
    )
   
let ``Calls the teardown that was passed to the feature with the successful result of the setup passed to the feature `` =
    feature.Test (fun _ ->
        let setupResult = 1099
        let monitor = getTestMonitor<unit, unit, int> ()
        
        let testFeature = FeatureFactory.NewFeature (Setup (monitor.FunctionSetupWith setupResult), Teardown monitor.FunctionTeardownFromSetup)
        
        testFeature.Test(fun _ -> TestSuccess)
        |> silentlyRunTest
        
        monitor.TeardownFunctionParameterValues
        |> List.map fst
        |> Should.BeEqualTo [Ok (None, Some setupResult)]
    )
    
let ``Each teardown should be called with the corresponding successful setup`` =
    feature.Test (fun _ ->
        let featureSetupResult = "feature setup result"
        let testSetupResult = 3355
        
        let featureMonitor = getFeatureMonitor<string> () 
        let testMonitor = getTestMonitor<unit, string, int> ()
        
        let setup = featureMonitor.FunctionSetupWith featureSetupResult
        let teardown = featureMonitor.FunctionTeardownWith ()
        
        let testFeature = FeatureFactory.NewFeature (Setup setup, Teardown teardown)
        
        let setup = Setup (testMonitor.FunctionSetupFeatureWith testSetupResult)
        let testBody = TestBody testMonitor.FunctionTestFeatureOneParameterSuccess
        let teardown = Teardown testMonitor.FunctionTeardownFeatureFromSetup
        
        testFeature.Test(setup, testBody, teardown)
        |> silentlyRunTest
        
        let featureResult =
            featureMonitor.TeardownFunctionCalledWith
            |> List.map fst
            |> Should.BeEqualTo [Ok featureSetupResult]
            
        featureResult
        |> andResult (
            testMonitor.TeardownFunctionParameterValues
            |> List.map fst
            |> Should.BeEqualTo [Ok (Some featureSetupResult, Some testSetupResult)]
        )
    )
    
let ``Calls the teardown that was passed to feature.Test with the unsuccessful result of the setup passed to feature.Test`` =
    feature.Test (
        Setup setupBuildExecutorWithSetupAndTeardownActions,
        
        TestBody (fun testBuilder ->
            let expectedSetupValue =
                "Bad setup, bad"
                |> newFailure.With.SetupTeardownGeneralFailure
                
            let monitor = getUnitTestMonitor ()
                
            let executor: ITestExecutor = testBuilder (monitor.FunctionSetupWith expectedSetupValue) monitor.FunctionTeardownFromSetup
            
            executor
            |> silentlyRunExecutor
            
            monitor.TeardownFunctionParameterValues
            |> List.map fst
            |> Should.BeEqualTo [Error expectedSetupValue]
        )
    )
    
let ``Calls the teardown that was passed to the feature with the unsuccessful result of the setup passed to the feature`` =
    feature.Test (fun _ ->
        let expectedResult =
            "Bad feature setup"
            |> newFailure.With.SetupTeardownGeneralFailure
            
        let monitor = getUnitTestMonitor ()
        
        let testFeature = FeatureFactory.NewFeature(Setup (monitor.FunctionSetupWith expectedResult), Teardown monitor.FunctionTeardownFromSetup)
            
        testFeature.Test(fun _ -> TestSuccess)
        |> silentlyRunTest
        
        monitor.TeardownFunctionParameterValues
        |> List.map fst
        |> Should.BeEqualTo [Error expectedResult]
    )

let ``Calls the teardown that was passed to feature.Test with the TestSuccess if test is successful`` =
    feature.Test (
        Setup setupBuildExecutorWithMonitor,
        
        TestBody (fun testBuilder ->
            let monitor = getUnitTestMonitor ()
            
            let executor : ITestExecutor = testBuilder monitor (Ok ())
            
            executor
            |> silentlyRunExecutor
            
            monitor.TeardownFunctionParameterValues
            |> List.map snd
            |> Should.BeEqualTo [Some TestSuccess]
        )
    )

let ``Calls the teardown that was passed to the feature with the TestSuccess if test is successful`` =
    feature.Test (fun _ ->
        let monitor = getFeatureMonitor<unit> ()
        
        let testFeature = FeatureFactory.NewFeature (Teardown (monitor.FunctionTeardownWith ()))
        
        testFeature.Test(fun () -> TestSuccess)
        |> silentlyRunTest
        
        monitor.TeardownFunctionCalledWith
        |> List.map snd
        |> Should.BeEqualTo [Some TestSuccess]
    )

let ``Calls the teardown that was passed to feature.Test with the TestFailure if test fails`` =
    feature.Test (
        Setup setupBuildExecutorWithMonitorAndTestResult,
        
        TestBody (fun testBuilder ->
            let expectedFailure =
                newFailure.With.TestOtherExpectationFailure "a failed test"
                |> TestFailure
            
            let monitor = getUnitTestMonitor () 
            
            let executor : ITestExecutor = testBuilder monitor expectedFailure
            
            executor
            |> silentlyRunExecutor
            
            monitor.TeardownFunctionParameterValues
            |> List.map snd
            |> Should.BeEqualTo [Some expectedFailure]
        )
    )
    
let ``Calls the teardown that was passed to the feature with the TestFailure if test fails`` =
    feature.Test (fun _ ->
        let expectedFailure =
            newFailure.With.TestOtherExpectationFailure "a failed test"
            |> TestFailure
        
        let monitor = getFeatureMonitor<unit> ()
        
        let testFeature = FeatureFactory.NewFeature (Teardown (monitor.FunctionTeardownWith ()))
        
        testFeature.Test(Setup (fun a -> Ok (a, ())), TestBody (fun _ -> expectedFailure))
        |> silentlyRunTest
        
        monitor.TeardownFunctionCalledWith
        |> List.map snd
        |> Should.BeEqualTo [Some expectedFailure]
    )

let ``Return the failure if the teardown that was passed to feature.Test fails`` =
    feature.Test (
        Setup setupBuildExecutorWithTeardownAction,
        
        TestBody (fun testBuilder ->
            let teardownFailure = 
                "failed teardown"
                |> newFailure.With.SetupTeardownGeneralFailure
                
            let monitor = getUnitTestMonitor ()
            
            let executor: ITestExecutor = testBuilder (monitor.FunctionTeardownPassThroughWith teardownFailure)
            
            executor
            |> runExecutor
            |> Should.BeEqualTo (
                teardownFailure
                |> TeardownExecutionFailure
            )
        )
    )
    
let ``Returns the failure if the teardown given to the feature fails`` =
    feature.Test (fun _ ->
        let expectedError =
            "failed feature teardown"
            |> newFailure.With.SetupTeardownGeneralFailure
            
        let monitor = getFeatureMonitor<unit> ()
        
        let testFeature = FeatureFactory.NewFeature(Teardown (monitor.FunctionTeardownWith expectedError))
        
        let result =
            testFeature.Test(fun () -> TestSuccess)
            |> runTest
        
        result
        |> Should.BeEqualTo (TeardownExecutionFailure expectedError)
    )

let ``Return failure if teardown that was passed to feature.Test throws exception`` =
    feature.Test (
        Setup setupBuildExecutorWithTeardownAction,
        
        TestBody (fun testBuilder ->
            let expectedErrorMessage = "Boom goes the teardown"
            let monitor = getUnitTestMonitor ()
            
            let executor: ITestExecutor = testBuilder (monitor.FunctionTeardownPassThroughFailsWith expectedErrorMessage)
            
            try
                let result =
                    executor
                    |> runExecutor
                
                match result with
                | TeardownExecutionFailure (SetupTeardownExceptionFailure ex) ->
                    ex.Message
                    |> Should.BeEqualTo expectedErrorMessage
                    
                | _ -> "Should not be here" |> newFailure.With.TestOtherExpectationFailure |> TestFailure
            with
            | ex -> ex |> newFailure.With.TestExecutionExceptionFailure |> TestFailure
        )
    )
    
let ``Return failure if teardown that was passed to the feature throws exception`` =
    feature.Test (fun _ ->
        let expectedErrorMessage = "Boom goes the feature teardown"
        let monitor = getFeatureMonitor<unit> ()
        
        let testFeature = FeatureFactory.NewFeature (Teardown (monitor.FunctionTeardownFailsWith expectedErrorMessage))
        
        try
            let result =
                testFeature.Test(fun () -> TestSuccess)
                |> runTest
            
            match result with
            | TeardownExecutionFailure (SetupTeardownExceptionFailure ex) ->
                ex.Message
                |> Should.BeEqualTo expectedErrorMessage
                
            | _ -> "Should not be here" |> newFailure.With.TestOtherExpectationFailure |> TestFailure
        with
        | ex -> ex |> newFailure.With.TestExecutionExceptionFailure |> TestFailure
    )
    
let ``Not have a test result passed to the teardown given to the feature if the teardown given to feature.Test fails`` =
    feature.Test (fun _ ->
        let monitor = getFeatureMonitor<unit> ()
        
        let expectedFailure =
            "Bad test teardown"
            |> newFailure.With.SetupTeardownGeneralFailure
        
        let testFeature = FeatureFactory.NewFeature(Teardown (monitor.FunctionTeardownWith ()))
        
        testFeature.Test(TestBody (fun () -> TestSuccess), Teardown (fun _ _ -> Error expectedFailure))
        |> silentlyRunTest
        
        monitor.TeardownFunctionCalledWith
        |> List.map snd
        |> Should.BeEqualTo [None]
    )

let ``Test Cases`` = feature.GetTests ()