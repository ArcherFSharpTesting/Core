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

type Monitor<'setupInputType, 'setupOutputType> (setupOutput: Result<'setupOutputType, SetupTeardownFailure>, teardownResult: Result<unit, SetupTeardownFailure>) =
    let mutable setupInput: 'setupInputType option = None
    let mutable setupResult: Result<'setupOutputType, SetupTeardownFailure> option = None
    let mutable testResultResult: TestResult option = None
    let mutable setupCount = 0
    let mutable teardownCount = 0
    let mutable testCount = 0
    
    new (setupOutput: Result<'setupOutputType, SetupTeardownFailure>) =
        Monitor (setupOutput, Ok ())
    
    member _.CallSetup input =
        setupCount <- setupCount + 1
        setupInput <- (Some input)
        setupOutput
        
    member _.CallTestActionWithEnvironment _ _ =
        testCount <- testCount + 1
        TestSuccess
        
    member this.CallTestActionWithoutEnvironment _ =
        testCount <- testCount + 1
        TestSuccess
        
    member _.CallTeardown setupValue testValue =
        teardownCount <- teardownCount + 1
        setupResult <- (Some setupValue)
        testResultResult <- testValue
        teardownResult
        
    member _.SetupWasCalled with get () = 0 < setupCount
    member _.TeardownWasCalled with get () = 0 < teardownCount
    member _.TestWasCalled with get () = 0 < testCount
    member this.WasCalled with get () = this.SetupWasCalled || this.TeardownWasCalled || this.TestWasCalled
    member _.NumberOfTimesSetupWasCalled with get () = setupCount
    member _.NumberOfTimesTeardownWasCalled with get () = teardownCount
    member _.NumberOfTimesTestWasCalled with get () = testCount

let setupBuildExecutorWithMonitor _ =
    let monitor = Monitor<unit, unit> (Ok ())
    let feature = Arrow.NewFeature (
        Setup monitor.CallSetup,
        Teardown monitor.CallTeardown
    )
    
    let test = feature.Test monitor.CallTestActionWithEnvironment
    Ok (monitor, test.GetExecutor ())