module Archer.Arrows.Tests.RawTestObjects.``TestCaseExecutor Execute Should``

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
            let monitor = Monitor (Ok ())
                
            let executor: ITestExecutor = testBuilder monitor.CallSetup
            
            executor
            |> silentlyRunExecutor
                
            monitor.SetupWasCalled
            |> Should.BeTrue
            |> withMessage "Setup did not run"
        )
    )

let ``Run the setup action supplied to feature with called`` =
    feature.Test (
        fun _ ->
            let monitor = Monitor<unit, unit, unit> (Ok ())
            let testFeature = Arrow.NewFeature (
                ignoreString (),
                ignoreString (),
                Setup monitor.CallSetup
            )
            
            testFeature.Test(fun _ -> TestSuccess)
            |> silentlyRunTest
            
            monitor.SetupWasCalled
            |> Should.BeTrue
            |> withMessage "Setup was not called"
    )
    
let ``Run the test body when called`` =
    feature.Test (
        Setup setupBuildExecutorWithTestBody,
        
        TestBody (fun testBuilder ->
            let monitor = Monitor (Ok ())
            
            let executor: ITestExecutor = testBuilder monitor.CallTestActionWithSetupEnvironment
            
            executor
            |> silentlyRunExecutor
            
            monitor.TestWasCalled
            |> Should.BeTrue
            |> withMessage "Test did not run"
        )
    )
    
let ``Pass the result of the setup supplied to feature.Test to the test action`` =
    feature.Test (
        Setup setupBuildExecutorWithMonitor,
        
        TestBody (fun testBuilder ->
            let expected = "a value to pass to the test"
            let monitor = Monitor<unit, unit, string> (Ok expected)
                
            let executor: ITestExecutor = testBuilder monitor
            
            executor
            |> silentlyRunExecutor
            
            monitor.TestInputSetupWas
            |> Should.BeEqualTo [expected]
        )
    )
    
let ``Pass the result of the setup supplied to the feature to the setup supplied to feature.Test`` =
    feature.Test (fun _ ->
        let expectedValue = "Hello from feature setup"
        let testFeature = Arrow.NewFeature (Setup (fun () -> Ok expectedValue))
        
        let monitor = Monitor<unit, string, unit> (Ok ())
        
        testFeature.Test(Setup monitor.CallSetup, TestBody monitor.CallTestActionWithSetup)
        |> silentlyRunTest
        
        monitor.TestSetupInputWas
        |> Should.BeEqualTo [expectedValue]
    )
    
let ``Not throw an exception if the setup supplied to feature.Test fails`` =
    feature.Test (
        Setup setupBuildExecutorWithSetupAction,
        
        TestBody (fun testBuilder ->
            let monitor = Monitor<unit, unit, unit> ("failed setup" |> newFailure.With.SetupTeardownGeneralFailure |> Error)
                
            let executor: ITestExecutor = testBuilder monitor.CallSetup
            
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
        let monitor = Monitor<unit, unit, unit> ("failed feature setup" |> newFailure.With.SetupTeardownGeneralFailure |> Error)
        
        let testFeature = Arrow.NewFeature (Setup monitor.CallSetup)

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
            let monitor = Monitor<unit, unit, string> (Error expectedFailure)
            
            let executor: ITestExecutor = testBuilder monitor.CallSetup
            
            let result =
                executor
                |> runExecutor
            
            result
            |> Should.BeEqualTo (expectedFailure |> SetupExecutionFailure)
        )
    )
    
let ``Should not run the setup provided to feature.Test if the setup provided to the feature fails`` =
    feature.Test (fun _ ->
        let monitor = Monitor<unit, unit, unit> (Ok ())
        
        let testFeature = Arrow.NewFeature (Setup (fun () -> "failed setup" |> newFailure.With.SetupTeardownGeneralFailure |> Error))
        
        testFeature.Test(Setup monitor.CallSetup, TestBody (fun _ -> TestSuccess))
        |> silentlyRunTest
        
        monitor.SetupWasCalled
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
                
            let monitor = newMonitorWithTestResult expectedFailure
                
            let executor: ITestExecutor = testBuilder monitor.CallTestActionWithSetupEnvironment
            
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
            let monitor = Monitor<unit, unit, unit> (fun _ -> failwith expectedErrorMessage)
                
            let executor: ITestExecutor = buildTest monitor.CallSetup
            
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
        let monitor = Monitor<unit, unit, unit> (fun _ -> failwith expectedErrorMessage)
        
        let testFeature = Arrow.NewFeature (Setup monitor.CallSetup)
        
        try
            testFeature.Test(fun _ -> TestSuccess)
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
            let monitor = newMonitorWithTestAction (fun _ -> failwith expectedErrorMessage)
                
            let executor: ITestExecutor = testBuilder monitor.CallTestActionWithSetupEnvironment
            
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
            let monitor = Monitor<unit, unit, unit> (Ok ())
                
            let executor: ITestExecutor = testBuilder monitor.CallTeardown
            
            executor
            |> silentlyRunExecutor
            
            monitor.TeardownWasCalled
            |> Should.BeTrue
            |> withMessage "Teardown was not called")
    )
   
let ``Run the teardown passed to the feature`` =
    feature.Test (fun _ ->
        let monitor = Monitor<unit, uint, unit> (Ok ())
        
        let testFeature = Arrow.NewFeature (Teardown monitor.CallTeardown)
        
        testFeature.Test(fun _ -> TestSuccess)
        |> silentlyRunTest
        
        monitor.TeardownWasCalled
        |> Should.BeTrue
        |> withMessage "Teardown was not called"
    )
    
let ``Calls the teardown that was passed to feature.Test with the successful result of the setup passed to feature.Test`` =
    feature.Test (
        Setup setupBuildExecutorWithSetupAndTeardownActions,
        
        TestBody (fun testBuilder ->
            let expectedSetupValue = Ok "Hello from setup"
            let monitor = Monitor<unit, unit, string> expectedSetupValue
            
            let executor: ITestExecutor = testBuilder monitor.CallSetup monitor.CallTeardown
            
            executor
            |> silentlyRunExecutor
            
            monitor.TeardownWasCalledWith
            |> fst
            |> Should.BeEqualTo [expectedSetupValue]
        )
    )
   
let ``Calls the teardown that was passed to the feature with the successful result of the setup passed to the feature `` =
    feature.Test (fun _ ->
        let setupResult = Ok 1099
        let monitor = Monitor<unit, unit, int> setupResult
        
        let testFeature = Arrow.NewFeature (Setup monitor.CallSetup, Teardown monitor.CallTeardown)
        
        testFeature.Test(fun _ -> TestSuccess)
        |> silentlyRunTest
        
        monitor.TeardownWasCalledWith
        |> fst
        |> Should.BeEqualTo [setupResult]
    )
    
let ``Each teardown should be called with the corresponding successful setup`` =
    feature.Test (fun _ ->
        let featureResult = Ok "feature setup result"
        let testResult = Ok 3355
        
        let featureMonitor = Monitor<unit, unit, string> featureResult
        let testMonitor = Monitor<unit, string, int> testResult
        
        let testFeature = Arrow.NewFeature (Setup featureMonitor.CallSetup, Teardown featureMonitor.CallTeardown)
        
        testFeature.Test(Setup testMonitor.CallSetup, TestBody (fun _ -> TestSuccess), Teardown testMonitor.CallTeardown)
        |> silentlyRunTest
        
        let featureResult =
            featureMonitor.TeardownWasCalledWith
            |> fst
            |> Should.BeEqualTo [featureResult]
            
        featureResult
        |> andResult (
            testMonitor.TeardownWasCalledWith
            |> fst
            |> Should.BeEqualTo [testResult]
        )
    )
    
let ``Calls the teardown that was passed to feature.Test with the unsuccessful result of the setup passed to feature.Test`` =
    feature.Test (
        Setup setupBuildExecutorWithSetupAndTeardownActions,
        
        TestBody (fun testBuilder ->
            let expectedSetupValue =
                "Bad setup, bad"
                |> newFailure.With.SetupTeardownGeneralFailure
                |> Error
                
            let monitor = Monitor<unit, unit, unit> expectedSetupValue
                
            let executor: ITestExecutor = testBuilder monitor.CallSetup monitor.CallTeardown
            
            executor
            |> silentlyRunExecutor
            
            monitor.TeardownWasCalledWith
            |> fst
            |> Should.BeEqualTo [expectedSetupValue]
        )
    )
    
let ``Calls the teardown that was passed to the feature with the unsuccessful result of the setup passed to the feature`` =
    feature.Test (fun _ ->
        let expectedResult =
            "Bad feature setup"
            |> newFailure.With.SetupTeardownGeneralFailure
            |> Error
            
        let monitor = Monitor<unit, unit, unit> expectedResult
        
        let testFeature = Arrow.NewFeature(Setup monitor.CallSetup, Teardown monitor.CallTeardown)
            
        testFeature.Test(fun _ -> TestSuccess)
        |> silentlyRunTest
        
        monitor.TeardownWasCalledWith
        |> fst
        |> Should.BeEqualTo [expectedResult]
    )

let ``Calls the teardown that was passed to feature.Test with the TestSuccess if test is successful`` =
    feature.Test (
        Setup setupBuildExecutorWithMonitor,
        
        TestBody (fun testBuilder ->
            let monitor = newMonitorWithTestResult TestSuccess
            
            let executor : ITestExecutor = testBuilder monitor
            
            executor
            |> silentlyRunExecutor
            
            monitor.TeardownWasCalledWith
            |> snd
            |> Should.BeEqualTo [Some TestSuccess]
        )
    )

let ``Calls the teardown that was passed to the feature with the TestSuccess if test is successful`` =
    feature.Test (fun _ ->
        let monitor = newMonitorWithTestResult TestSuccess
        
        let testFeature = Arrow.NewFeature (Teardown monitor.CallTeardown)
        
        testFeature.Test(fun _ -> TestSuccess)
        |> silentlyRunTest
        
        monitor.TeardownWasCalledWith
        |> snd
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
            
            monitor.TeardownWasCalledWith
            |> snd
            |> Should.BeEqualTo [Some expectedFailure]
        )
    )
    
let ``Calls the teardown that was passed to the feature with the TestFailure if test fails`` =
    feature.Test (fun _ ->
        let expectedFailure =
            newFailure.With.TestOtherExpectationFailure "a failed test"
            |> TestFailure
        
        let monitor = newMonitorWithTestResult expectedFailure
        
        let testFeature = Arrow.NewFeature (Teardown monitor.CallTeardown)
        
        testFeature.Test(monitor.CallTestActionWithSetup)
        |> silentlyRunTest
        
        monitor.TeardownWasCalledWith
        |> snd
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
            
            let executor: ITestExecutor = testBuilder monitor.CallTeardown
            
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
            
        let monitor = newMonitorWithTeardownAction (fun _ _ -> Error expectedError)
        
        let testFeature = Arrow.NewFeature(Teardown monitor.CallTeardown)
        
        let result =
            testFeature.Test(fun _ -> TestSuccess)
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
                testFeature.Test(fun _ -> TestSuccess)
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
        let monitor = Monitor<unit, unit, unit> (Ok ())
        
        let expectedFailure =
            "Bad test teardown"
            |> newFailure.With.SetupTeardownGeneralFailure
        
        let testFeature = Arrow.NewFeature(Teardown monitor.CallTeardown)
        
        testFeature.Test(TestBody (fun _ -> TestSuccess), Teardown (fun _ _ -> Error expectedFailure))
        |> silentlyRunTest
        
        monitor.TeardownWasCalledWith
        |> snd
        |> Should.BeEqualTo [None]
    )

let ``Test Cases`` = feature.GetTests ()