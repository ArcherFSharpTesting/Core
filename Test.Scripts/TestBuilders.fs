[<AutoOpen>]
module Archer.Arrows.Tests.TestBuilders

open Archer
open Archer.Arrows
open Archer.MicroLang

let buildFeatureUnderTest _ = Arrow.NewFeature (ignoreString (), ignoreString ())

let setupExecutor _ =
    let feature = buildFeatureUnderTest ()
    let test = feature.Test successfulTest
    test.GetExecutor () |> Ok

let setupBuildExecutorWithSetupAction _ =
    let buildExecutor setupAction =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup setupAction, TestWithEnvironmentBody successfulEnvironmentTest, ignoreString (), $"%s{ignoreString ()}.fs", ignoreInt ())
        test.GetExecutor ()
        
    buildExecutor |> Ok
    
let setupBuildExecutorWithTestBody _ =
    let buildExecutor testBody =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup successfulUnitSetup, TestWithEnvironmentBody testBody, ignoreString (), $"%s{ignoreString ()}.fs", ignoreInt ())
        test.GetExecutor ()
        
    buildExecutor |> Ok
    
let setupBuildExecutorWithTestBodyAndSetupAction _ =
    let buildExecutor setup testBody =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup setup, TestWithEnvironmentBody testBody, ignoreString (), $"%s{ignoreString ()}.fs", ignoreInt ())
        test.GetExecutor ()
        
    buildExecutor |> Ok
    
let setupBuildExecutorWithTeardownAction () =
    let buildExecutor teardownAction =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup successfulUnitSetup, TestWithEnvironmentBody successfulEnvironmentTest, Teardown teardownAction)
        test.GetExecutor ()
        
    buildExecutor |> Ok
    
let setupBuildExecutorWithSetupAndTeardownActions _ =
    let buildExecutor setupAction teardownAction =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup setupAction, TestWithEnvironmentBody successfulEnvironmentTest, Teardown teardownAction)
        test.GetExecutor ()
        
    buildExecutor |> Ok
    
let setupBuiltExecutorWithTestBodyAndTeardownAction _ =
    let builtExecutor testBody teardownAction =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup successfulUnitSetup, TestWithEnvironmentBody testBody, Teardown teardownAction)
        test.GetExecutor ()
        
    builtExecutor |> Ok

type Monitor () =
    let mutable setupCalled = false
    let mutable testActionCalled = false
    let mutable teardownCalled = false
    
    member _.CallSetup () =
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

let setupBuildExecutorWithMonitor _ =
    let monitor = Monitor ()
    let feature = Arrow.NewFeature (
        Setup monitor.CallSetup,
        Teardown monitor.CallTeardown
    )
    
    let test = feature.Test monitor.CallTestAction
    Ok (monitor, test.GetExecutor ())