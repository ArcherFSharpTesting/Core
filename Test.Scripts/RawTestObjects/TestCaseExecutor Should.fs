module Archer.Arrow.Tests.RawTestObjects.``TestCaseExecutor Should``

open Archer
open Archer.Arrow
open Archer.Arrow.Tests
open Archer.MicroLang

let private container = suite.Container ()

let buildFeatureUnderTest _ = arrow.NewFeature (ignoreString (), ignoreString ()) 

let setupBuildExecutorWithSetupAction _ =
    let buildExecutor setupAction =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup setupAction, TestBody successfulEnvironmentTest, ignoreString (), $"%s{ignoreString ()}.fs", ignoreInt ())
        test.GetExecutor ()
        
    buildExecutor |> Ok
    
let setupBuildExecutorWithTestBody _ =
    let buildExecutor testBody =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup successfulUnitSetup, TestBody testBody, ignoreString (), $"%s{ignoreString ()}.fs", ignoreInt ())
        test.GetExecutor ()
        
    buildExecutor |> Ok
    
let setupBuildExecutorWithTestBodyAndSetupAction _ =
    let buildExecutor setup testBody =
        let featuer = buildFeatureUnderTest ()
        let test = featuer.Test (Setup setup, TestBody testBody, ignoreString (), $"%s{ignoreString ()}.fs", ignoreInt ())
        test.GetExecutor ()
        
    buildExecutor |> Ok

let ``Run the setup action when execute is called`` =
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
    
let ``Run the test body when execute is called`` =
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
                newFailure.With.GeneralSetupTeardownFailure "failed setup" |> Error
                
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
            let expectedFailure = newFailure.With.GeneralSetupTeardownFailure "failed setup"
            
            let setup _ =
                expectedFailure |> Error
                
            let executor = testBuilder setup
            
            let result = executor.Execute (getFakeEnvironment ())
            
            result
            |> expects.ToBe (expectedFailure |> SetupExecutionFailure)
    )

let ``Test Cases`` = container.Tests