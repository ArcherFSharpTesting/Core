module Archer.Arrows.Tests.RawTestObjects.``TestCaseExecutor Execute Should``

open System.Threading
open Archer
open Archer.Arrows
open Archer.Arrows.Tests
open Archer.CoreTypes.InternalTypes
open Archer.MicroLang

let private feature = Arrow.NewFeature (
    TestTags [
        Category "Feature"
        Category "Test"
    ]
)

let ``Run the setup action supplied to feature.Test when called`` =
    feature.Test (
        Setup setupBuildExecutorWithSetupAction,
        
        TestBody (fun testBuilder ->
            let monitor = TestMonitor((), TestSuccess).Interface
                
            let executor: ITestExecutor = testBuilder monitor.FunctionSetup
            
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
            let monitor = buildTestMonitorOfTypeWithSetup<unit, unit> ()
            let testFeature = Arrow.NewFeature (
                ignoreString (),
                ignoreString (),
                Setup monitor.FunctionSetup
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
            let monitor = buildTestMonitorOfTypeWithSetup<unit, unit> ()
            
            let executor: ITestExecutor = testBuilder () monitor.FunctionTestTwoParameters
            
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
            let monitor = buildTestMonitorOfTypeWithSetup<unit, string> expected
                
            let executor: ITestExecutor = testBuilder monitor
            
            executor
            |> silentlyRunExecutor
            
            monitor.TestFunctionWasCalledWith
            |> List.map (fun (a, b, c) -> a, b)
            |> Should.BeEqualTo [None, Some ((), expected)]
        )
    )
    
let ``Pass the result of the setup supplied to the feature to the setup supplied to feature.Test`` =
    feature.Test (fun _ ->
        let expectedValue = "Hello from feature setup"
        let testFeature = Arrow.NewFeature (Setup (fun () -> Ok expectedValue))
        
        let monitor = buildTestMonitorOfTypeWithSetup<string, unit> ()
        
        let t = monitor.FunctionSetup
        
        testFeature.Test(Setup monitor.FunctionSetup, TestBody monitor.FunctionTestOneParameter)
        |> silentlyRunTest
        
        monitor.TestFunctionWasCalledWith
        |> List.map (fun (a, b, _) -> a, b)
        |> Should.BeEqualTo [(None , Some (expectedValue, ()))]
    )
    
let ``Not throw an exception if the setup supplied to feature.Test fails`` =
    feature.Test (
        Setup setupBuildExecutorWithSetupAction,
        
        TestBody (fun testBuilder ->
            let monitor = buildTestMonitorOfTypWithSetupError<unit> ("failed setup" |> newFailure.With.SetupTeardownGeneralFailure)
                
            let executor: ITestExecutor = testBuilder monitor.FunctionSetup
            
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
        let monitor = buildTestMonitorOfTypWithSetupError<unit> ("failed feature setup" |> newFailure.With.SetupTeardownGeneralFailure)
        
        let testFeature = Arrow.NewFeature (Setup monitor.FunctionSetup)

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
            let monitor = buildTestMonitorOfTypWithSetupError<unit> expectedFailure
            
            let executor: ITestExecutor = testBuilder monitor.FunctionSetup
            
            let result =
                executor
                |> runExecutor
            
            result
            |> Should.BeEqualTo (expectedFailure |> SetupExecutionFailure)
        )
    )
    
let ``Should not run the setup provided to feature.Test if the setup provided to the feature fails`` =
    feature.Test (fun _ ->
        let monitor = buildTestMonitorOfTypeWithSetup<obj, unit> ()
        
        let testFeature = Arrow.NewFeature (Setup (fun () -> "failed setup" |> newFailure.With.SetupTeardownGeneralFailure |> Error))
        
        testFeature.Test(Setup monitor.FunctionSetup, TestBody (fun _ -> TestSuccess))
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
                 
            let monitor = buildTestMonitorOfTypeWithTestResult<unit, unit> ((), expectedFailure)
                
            let executor: ITestExecutor = testBuilder () monitor.FunctionTestTwoParameters
            
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
            let monitor = TestMonitor<unit, unit, unit> ((), TestSuccess)
                
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
        let monitor = TestMonitor<unit, unit, unit> (())
        
        let testFeature = Arrow.NewFeature (Setup (monitor.FunctionSetupFailsWith expectedErrorMessage))
        
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
            let monitor = TestMonitor<unit, unit, unit> (())
            let testFunction = monitor.FunctionTestTwoParametersFailWith expectedErrorMessage
                
            let executor: ITestExecutor = testBuilder () testFunction
            
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
            let monitor = TestMonitor<unit, unit, unit> (())
                
            let executor: ITestExecutor = testBuilder monitor.FunctionTeardownFromFeature
            
            executor
            |> silentlyRunExecutor
            
            monitor.HasTeardownBeenCalled
            |> Should.BeTrue
            |> withMessage "Teardown was not called")
    )
   
let ``Run the teardown passed to the feature`` =
    feature.Test (fun _ ->
        let monitor = TestMonitor<unit, unit, unit> (())
        
        let testFeature = Arrow.NewFeature (Teardown monitor.FunctionTeardownFromFeature)
        
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
            let monitor = TestMonitor<unit, string, unit> expectedSetupValue
            
            let executor: ITestExecutor = testBuilder monitor.FunctionSetup monitor.FunctionTeardownFromSetup
            
            executor
            |> silentlyRunExecutor
            
            monitor.TeardownFunctionCalledWith
            |> List.map fst
            |> Should.BeEqualTo [Ok ((), Some expectedSetupValue)]
        )
    )
   
let ``Calls the teardown that was passed to the feature with the successful result of the setup passed to the feature `` =
    feature.Test (fun _ ->
        let setupResult = 1099
        let monitor = TestMonitor<unit, int, unit> setupResult
        
        let testFeature = Arrow.NewFeature (Setup monitor.FunctionSetup, Teardown monitor.FunctionTeardownFromSetup)
        
        testFeature.Test(fun _ -> TestSuccess)
        |> silentlyRunTest
        
        monitor.TeardownFunctionCalledWith
        |> List.map fst
        |> Should.BeEqualTo [Ok ((), Some setupResult)]
    )
    
let ``Each teardown should be called with the corresponding successful setup`` =
    feature.Test (fun _ ->
        let featureSetupResult = "feature setup result"
        let setupResult = 3355
        
        let featureMonitor = TestMonitor<unit, string, unit> featureSetupResult
        let testMonitor = TestMonitor<unit * string, int, unit> setupResult
        
        let testFeature = Arrow.NewFeature (Setup featureMonitor.FunctionSetup, Teardown featureMonitor.FunctionTeardownFromSetup)
        
        let setup = Setup testMonitor.FunctionSetup
        let testBody = TestBody testMonitor.FunctionTestOneParameter
        let teardown = Teardown testMonitor.FunctionTeardownFromSetup
        
        testFeature.Test(setup, testBody, teardown)
        |> silentlyRunTest
        
        let featureResult =
            featureMonitor.TeardownFunctionCalledWith
            |> List.map fst
            |> Should.BeEqualTo [Ok ((), Some featureSetupResult)]
            
        featureResult
        |> andResult (
            testMonitor.TeardownFunctionCalledWith
            |> List.map fst
            |> Should.BeEqualTo [Ok (((), featureSetupResult), Some setupResult)]
        )
    )
    
let ``Calls the teardown that was passed to feature.Test with the unsuccessful result of the setup passed to feature.Test`` =
    feature.Test (
        Setup setupBuildExecutorWithSetupAndTeardownActions,
        
        TestBody (fun testBuilder ->
            let expectedSetupValue =
                "Bad setup, bad"
                |> newFailure.With.SetupTeardownGeneralFailure
                
            let monitor = TestMonitor<unit, unit, unit> expectedSetupValue
                
            let executor: ITestExecutor = testBuilder monitor.FunctionSetup monitor.FunctionTeardownFromSetup
            
            executor
            |> silentlyRunExecutor
            
            monitor.TeardownFunctionCalledWith
            |> List.map fst
            |> Should.BeEqualTo [Error expectedSetupValue]
        )
    )
    
let ``Calls the teardown that was passed to the feature with the unsuccessful result of the setup passed to the feature`` =
    feature.Test (fun _ ->
        let expectedResult =
            "Bad feature setup"
            |> newFailure.With.SetupTeardownGeneralFailure
            
        let monitor = TestMonitor<unit, unit, unit> expectedResult
        
        let testFeature = Arrow.NewFeature(Setup monitor.FunctionSetup, Teardown monitor.FunctionTeardownFromSetup)
            
        testFeature.Test(fun _ -> TestSuccess)
        |> silentlyRunTest
        
        monitor.TeardownFunctionCalledWith
        |> List.map fst
        |> Should.BeEqualTo [Error expectedResult]
    )

let ``Calls the teardown that was passed to feature.Test with the TestSuccess if test is successful`` =
    feature.Test (
        Setup setupBuildExecutorWithMonitor,
        
        TestBody (fun testBuilder ->
            let monitor = newMonitorWithTestResult TestSuccess
            
            let executor : ITestExecutor = testBuilder monitor
            
            executor
            |> silentlyRunExecutor
            
            monitor.TeardownFunctionCalledWith
            |> List.map snd
            |> Should.BeEqualTo [Some TestSuccess]
        )
    )

let ``Calls the teardown that was passed to the feature with the TestSuccess if test is successful`` =
    feature.Test (fun _ ->
        let monitor = newMonitorWithTestResult TestSuccess
        
        let testFeature = Arrow.NewFeature (Teardown monitor.FunctionTeardownFromFeature)
        
        testFeature.Test(fun () -> TestSuccess)
        |> silentlyRunTest
        
        monitor.TeardownFunctionCalledWith
        |> List.map snd
        |> Should.BeEqualTo [Some TestSuccess]
    )

let ``Calls the teardown that was passed to feature.Test with the TestFailure if test fails`` =
    feature.Test (
        Setup setupBuildExecutorWithMonitor,
        
        TestBody (fun testBuilder ->
            let expectedFailure =
                newFailure.With.TestOtherExpectationFailure "a failed test"
                |> TestFailure
            
            let monitor = newMonitorWithTestResult expectedFailure 
            
            let executor : ITestExecutor = testBuilder monitor
            
            executor
            |> silentlyRunExecutor
            
            monitor.TeardownFunctionCalledWith
            |> List.map snd
            |> Should.BeEqualTo [Some expectedFailure]
        )
    )
    
let ``Calls the teardown that was passed to the feature with the TestFailure if test fails`` =
    feature.Test (fun _ ->
        let expectedFailure =
            newFailure.With.TestOtherExpectationFailure "a failed test"
            |> TestFailure
        
        let monitor = newMonitorWithTestResult expectedFailure
        
        let testFeature = Arrow.NewFeature (Teardown monitor.FunctionTeardownFromFeature)
        
        testFeature.Test(Setup (fun a -> Ok (a, ())), TestBody monitor.FunctionTestOneParameter)
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
                
            let monitor = newMonitorWithTeardownResult (Error teardownFailure)
            
            let executor: ITestExecutor = testBuilder monitor.FunctionTeardownFromFeature
            
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
            
        let monitor = TestMonitor<unit, unit, unit> ((), TestSuccess, expectedError)
        
        let testFeature = Arrow.NewFeature(Teardown monitor.FunctionTeardownFromFeature)
        
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
            let monitor = newMonitorWithTeardownAction (fun _ _ -> failwith expectedErrorMessage)
            
            let executor: ITestExecutor = testBuilder monitor.CallTeardown
            
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
        let monitor = newMonitorWithTeardownAction (fun _ _ -> failwith expectedErrorMessage)
        
        let testFeature = Arrow.NewFeature (Teardown monitor.CallTeardown)
        
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
        let monitor = TestMonitor<unit, unit, unit> (Ok ())
        
        let expectedFailure =
            "Bad test teardown"
            |> newFailure.With.SetupTeardownGeneralFailure
        
        let testFeature = Arrow.NewFeature(Teardown monitor.CallTeardown)
        
        testFeature.Test(TestBody (fun () -> TestSuccess), Teardown (fun _ _ -> Error expectedFailure))
        |> silentlyRunTest
        
        monitor.TeardownWasCalledWith
        |> snd
        |> Should.BeEqualTo [None]
    )

let ``Test Cases`` = feature.GetTests ()