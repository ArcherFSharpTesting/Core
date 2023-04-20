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
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup setup, TestBody testBody, ignoreString (), $"%s{ignoreString ()}.fs", ignoreInt ())
        test.GetExecutor ()
        
    buildExecutor |> Ok
    
let setupBuildExecutorWithTeardownAction () =
    let buildExecutor teardownAction =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup successfulUnitSetup, TestBody successfulEnvironmentTest, Teardown teardownAction)
        test.GetExecutor ()
        
    buildExecutor |> Ok
    
let setupBuildExecutorWithSetupAndTeardownActions _ =
    let buildExecutor setupAction teardownAction =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup setupAction, TestBody successfulEnvironmentTest, Teardown teardownAction)
        test.GetExecutor ()
        
    buildExecutor |> Ok
    
let setupBuiltExecutorWithTestBodyAndTeardownAction _ =
    let builtExecutor testBody teardownAction =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup successfulUnitSetup, TestBody testBody, Teardown teardownAction)
        test.GetExecutor ()
        
    builtExecutor |> Ok

type Monitor () =
    let mutable setupCalled = false
    let mutable testActionCalled = false
    let mutable teardownCalled = false
    
    member _.CallSetup _ =
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
    let feature = Arrow.NewFeature ()
    let monitor = Monitor ()
    
    let test = feature.Test (Setup monitor.CallSetup, TestBody monitor.CallTestAction, Teardown monitor.CallTeardown)
    Ok (monitor, test.GetExecutor ())