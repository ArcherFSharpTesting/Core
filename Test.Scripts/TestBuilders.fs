[<AutoOpen>]
module Archer.Arrows.Tests.TestBuilders

open Archer
open Archer.Arrows
open Archer.Arrows.Internals
open Archer.MicroLang

let buildFeatureUnderTestWithSetupAndTeardown setup teardown =
    Arrow.NewFeature (ignoreString (), ignoreString (), setup, teardown)

let buildFeatureUnderTestWithSetup setup =
    Arrow.NewFeature (ignoreString (), ignoreString (), setup, emptyTeardown)

let buildFeatureUnderTestWithTeardown teardown =
    Arrow.NewFeature (ignoreString (), ignoreString (), Setup Ok, teardown)

let buildFeatureUnderTest _ =
    buildFeatureUnderTestWithSetupAndTeardown (Setup Ok) (emptyTeardown)
    
let setupFeatureUnderTest _ =
    buildFeatureUnderTestWithSetupAndTeardown (Setup Ok) (emptyTeardown)
    |> Ok

let setupExecutor _ =
    let feature = buildFeatureUnderTest ()
    let test = feature.Test successfulTest
    test.GetExecutor () |> Ok

let setupBuildExecutorWithSetupAction _ =
    let buildExecutor setupAction =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup setupAction, TestBodyTwoParameters successfulEnvironmentTest, ignoreString (), $"%s{ignoreString ()}.fs", ignoreInt ())
        test.GetExecutor ()
        
    buildExecutor |> Ok

let setupBuildExecutorWithFeatureSetupAction _ =
    let buildExecutor setupAction =
        let feature = buildFeatureUnderTestWithSetup (Setup setupAction)
        let test = feature.Test (Setup Ok, TestBodyTwoParameters successfulEnvironmentTest, ignoreString (), $"%s{ignoreString ()}.fs", ignoreInt ())
        test.GetExecutor ()
        
    buildExecutor |> Ok

let setupBuildExecutorWithFeatureTeardownAction _ =
    let buildExecutor teardownAction =
        let feature = buildFeatureUnderTestWithTeardown (Teardown teardownAction)
        let test = feature.Test (Setup Ok, TestBodyTwoParameters successfulEnvironmentTest, ignoreString (), $"%s{ignoreString ()}.fs", ignoreInt ())
        test.GetExecutor ()
        
    buildExecutor |> Ok
    
let setupBuildExecutorWithTestBody _ =
    let buildExecutor testBody =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup successfulUnitSetup, TestBodyTwoParameters testBody, ignoreString (), $"%s{ignoreString ()}.fs", ignoreInt ())
        test.GetExecutor ()
        
    buildExecutor |> Ok
    
let setupBuildExecutorWithTestBodyAndSetupAction _ =
    let buildExecutor setup testBody =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup setup, TestBodyTwoParameters testBody, ignoreString (), $"%s{ignoreString ()}.fs", ignoreInt ())
        test.GetExecutor ()
        
    buildExecutor |> Ok
    
let setupBuildExecutorWithTeardownAction () =
    let buildExecutor teardownAction =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup successfulUnitSetup, TestBodyTwoParameters successfulEnvironmentTest, Teardown teardownAction)
        test.GetExecutor ()
        
    buildExecutor |> Ok
    
let setupBuildExecutorWithSetupAndTeardownActions _ =
    let buildExecutor setupAction teardownAction =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup setupAction, TestBodyTwoParameters successfulEnvironmentTest, Teardown teardownAction)
        test.GetExecutor ()
        
    buildExecutor |> Ok
    
let setupBuiltExecutorWithTestBodyAndTeardownAction _ =
    let builtExecutor testBody teardownAction =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup successfulUnitSetup, TestBodyTwoParameters testBody, Teardown teardownAction)
        test.GetExecutor ()
        
    builtExecutor |> Ok

type Monitor<'setupInputType, 'setupOutputType> (setupAction: 'setupInputType -> Result<'setupOutputType, SetupTeardownFailure>, testAction: TestFunction<'setupOutputType>, teardownAction: Result<'setupOutputType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>) =
    let mutable setupInput: 'setupInputType option = None
    let mutable setupResult: Result<'setupOutputType, SetupTeardownFailure> option = None
    let mutable testInput: 'setupOutputType option = None
    let mutable testInputEnvironment: TestEnvironment option = None
    let mutable testResultResult: TestResult option = None
    let mutable setupCount = 0
    let mutable teardownCount = 0
    let mutable testCount = 0
    
    new (setupAction: 'setupInputType -> Result<'setupOutputType, SetupTeardownFailure>) =
        Monitor (setupAction, (fun _ -> TestSuccess), (fun _ _ -> Ok ()))
    
    new (setupResult: Result<'setupOutputType, SetupTeardownFailure>, testAction) =
        Monitor ((fun _ -> setupResult), testAction, (fun _ _ -> Ok ()))
    
    new (setupOutput: Result<'setupOutputType, SetupTeardownFailure>, testOutput: TestResult, teardownResult: Result<unit, SetupTeardownFailure>) =
        Monitor ((fun _ -> setupOutput), (fun _ -> testOutput), (fun _ _ -> teardownResult))
        
    new (setupOutput: Result<'setupOutputType, SetupTeardownFailure>) =
        Monitor (setupOutput, TestSuccess, Ok ())
        
    new (setupOutput: Result<'setupOutputType, SetupTeardownFailure>, testResult) =
        Monitor (setupOutput, testResult, Ok ())
    
    member _.CallSetup input =
        setupCount <- setupCount + 1
        setupInput <- (Some input)
        setupAction input
        
    member this.CallTestActionWithEnvironment input env =
        testInputEnvironment <- Some env
        this.CallTestActionWithoutEnvironment input
        
    member _.CallTestActionWithoutEnvironment input =
        testInput <- Some input
        testCount <- testCount + 1
        testAction input
        
    member _.CallTeardown setupValue testValue =
        teardownCount <- teardownCount + 1
        setupResult <- (Some setupValue)
        testResultResult <- testValue
        teardownAction setupValue testValue
        
    member _.SetupWasCalled with get () = 0 < setupCount
    member _.SetupWasCalledWith with get () =
        match setupInput with
        | None -> failwith "Setup was not called"
        | Some value -> value
        
    member _.TeardownWasCalled with get () = 0 < teardownCount
    member _.TeardownWasCalledWith with get () =
        match setupResult, testResultResult with
        | None, _ -> failwith "Teardown was not called"
        | Some setupValue, testValue -> setupValue, testValue
        
    member _.TestWasCalled with get () = 0 < testCount
    member _.TestWasCalledWith with get () =
        match testInput with
        | None -> failwith "Test Action was not called"
        | Some value -> value
        
    member _.TestEnvironmentWas with get () =
        match testInputEnvironment with
        | None -> failwith "Test was not called with environment"
        | Some value -> value
        
    member this.WasCalled with get () = this.SetupWasCalled || this.TeardownWasCalled || this.TestWasCalled
    member _.NumberOfTimesSetupWasCalled with get () = setupCount
    member _.NumberOfTimesTeardownWasCalled with get () = teardownCount
    member _.NumberOfTimesTestWasCalled with get () = testCount

let newMonitorWithTestResult testResult =
    Monitor<unit, unit> (Ok (), testResult, Ok ())
    
let newMonitorWithTestAction (testAction: TestFunction<unit>) =
    Monitor<unit, unit> (Ok (), testAction)
    
let newMonitorWithTeardownAction (teardownAction: Result<unit, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>) =
    Monitor<unit, unit> ((fun _ -> Ok ()), (fun _ -> TestSuccess), teardownAction)
    
let newMonitorWithTeardownResult teardownResult =
    Monitor<unit, unit> (Ok (), TestSuccess, teardownResult)
    
let newMonitorWithTestResultAndTeardownResult testResult teardownResult =
    Monitor<unit, unit>(Ok (), testResult, teardownResult)

let setupBuildExecutorWithMonitorAtTheFeature _ =
    let monitor = Monitor<unit, unit> (Ok ())
    let feature = Arrow.NewFeature (
        ignoreString (),
        ignoreString (),
        Setup monitor.CallSetup,
        Teardown monitor.CallTeardown
    )
    
    let test = feature.Test monitor.CallTestActionWithEnvironment
    Ok (monitor, test.GetExecutor ())

let setupBuildExecutorWithMonitor _ =
    let buildIt (monitor: Monitor<unit, 'b>) =
        let feature = Arrow.NewFeature (
            ignoreString (),
            ignoreString ()
        )
        
        let test = feature.Test (Setup monitor.CallSetup, TestBodyTwoParameters monitor.CallTestActionWithEnvironment, Teardown monitor.CallTeardown)
        test.GetExecutor ()
        
    Ok buildIt