[<AutoOpen>]
module Archer.Arrows.Tests.TestBuilders

open Archer
open Archer.Arrows
open Archer.Arrows.Internals
open Archer.MicroLang

let private rand = System.Random ()

let randomLetter () =
    let letters = seq { seq{'A'..'Z'}; seq{'a'..'z'} } |> Seq.concat |> Seq.map (sprintf "%c") |> Array.ofSeq
    let max = letters.Length - 1
    let i = rand.Next (0, max)
    
    letters[i]
    
let randomDistinctLetters length =
    let catch = System.Collections.Generic.List<string>() 
    let rec getLetters length =
        if length < 1 then catch |> List.ofSeq
        else
            let letter = randomLetter ()
            if catch.Contains letter then getLetters length
            else
                catch.Add letter
                length - 1 |> getLetters
                
    getLetters length
    
    
let randomCapitalLetter () =
    let letter = randomLetter ()
    letter.ToUpper ()
    
let randomWord length =
    seq{ 0..(length - 1) }
    |> Seq.map (fun _ -> randomLetter ())
    |> fun items -> System.String.Join ("", items)

let buildFeatureUnderTestWithSetupAndTeardown setup teardown =
    Arrow.NewFeature (ignoreString (), ignoreString (), setup, teardown)

let buildFeatureUnderTestWithSetup setup =
    Arrow.NewFeature (ignoreString (), ignoreString (), setup, emptyTeardown)

let buildFeatureUnderTestWithTeardown teardown =
    Arrow.NewFeature (ignoreString (), ignoreString (), Setup Ok, teardown)

let buildFeatureUnderTest _ =
    buildFeatureUnderTestWithSetupAndTeardown (Setup Ok) emptyTeardown
    
let setupFeatureUnderTest _ =
    buildFeatureUnderTestWithSetupAndTeardown (Setup Ok) emptyTeardown
    |> Ok

let setupExecutor _ =
    let feature = buildFeatureUnderTest ()
    let test = feature.Test (fun _ _ -> TestSuccess)
    test.GetExecutor () |> Ok

let setupBuildExecutorWithSetupAction _ =
    let buildExecutor setupAction =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup setupAction, TestBody successfulEnvironmentTest, ignoreString (), $"%s{ignoreString ()}.fs", ignoreInt ())
        test.GetExecutor ()
        
    buildExecutor |> Ok

let setupBuildExecutorWithFeatureSetupAction _ =
    let buildExecutor setupAction =
        let feature = buildFeatureUnderTestWithSetup (Setup setupAction)
        let test = feature.Test (Setup Ok, TestBody successfulEnvironmentTest, ignoreString (), $"%s{ignoreString ()}.fs", ignoreInt ())
        test.GetExecutor ()
        
    buildExecutor |> Ok

let setupBuildExecutorWithFeatureTeardownAction _ =
    let buildExecutor teardownAction =
        let feature = buildFeatureUnderTestWithTeardown (Teardown teardownAction)
        let test = feature.Test (Setup Ok, TestBody successfulEnvironmentTest, ignoreString (), $"%s{ignoreString ()}.fs", ignoreInt ())
        test.GetExecutor ()
        
    buildExecutor |> Ok
    
let setupBuildExecutorWithTestBody _ =
    let buildExecutor (testBody: TestFunctionTwoParameters<unit, TestEnvironment>) =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup successfulUnitSetup, TestBody testBody, ignoreString (), $"%s{ignoreString ()}.fs", ignoreInt ())
        test.GetExecutor ()
        
    buildExecutor |> Ok
    
let setupBuildExecutorWithTestBodyAndSetupAction _ =
    let buildExecutor setup (testBody: TestFunction<unit>) =
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
    let builtExecutor (testBody: TestFunction<unit>) teardownAction =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup successfulUnitSetup, TestBody testBody, Teardown teardownAction)
        test.GetExecutor ()
        
    builtExecutor |> Ok

type Monitor<'dataType, 'setupInputType, 'setupOutputType> (setupAction: 'setupInputType -> Result<'setupOutputType, SetupTeardownFailure>, testAction: TestFunction<'setupOutputType>, teardownAction: Result<'setupOutputType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>) =
    let mutable setupInput: 'setupInputType list = []
    let mutable setupResult: Result<'setupOutputType, SetupTeardownFailure> list = []
    let mutable testInput: 'setupOutputType list = []
    let mutable testData: 'dataType list = []
    let mutable testInputEnvironment: TestEnvironment list = []
    let mutable testResultResult: TestResult option list = []
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
        setupInput <- input::setupInput
        setupAction input
        
    member this.CallTestActionWithSetupEnvironment input env =
        testInputEnvironment <- env::testInputEnvironment
        this.CallTestActionWithSetup input
        
    member _.CallTestActionWithSetup input =
        testInput <- input::testInput
        testCount <- testCount + 1
        testAction input
        
    member _.CallTestActionWithData data =
        testData <- data::testData
        testCount <- testCount + 1
        TestSuccess
        
    member this.CallTestActionWithDataSetup data input =
        testData <- data::testData
        this.CallTestActionWithSetup input
        
    member this.CallTestActionWithDataEnvironment data environment =
        testData <- data::testData
        testInputEnvironment <- environment::testInputEnvironment
        testCount <- testCount + 1
        TestSuccess
        
    member this.CallTestActionWithDataSetupEnvironment data input environment =
        testData <- data::testData
        this.CallTestActionWithSetupEnvironment input environment
        
    member _.CallTeardown setupValue testValue =
        teardownCount <- teardownCount + 1
        setupResult <- setupValue::setupResult
        testResultResult <- testValue::testResultResult
        teardownAction setupValue testValue
        
    member _.TestSetupInputWas with get () = setupInput |> List.rev
        
    member _.TeardownWasCalledWith with get () =
        match setupResult, testResultResult with
        | [], _ -> failwith "Teardown was not called"
        | setupValue, testValue -> setupValue |> List.rev, testValue |> List.rev
        
    member _.TestInputSetupWas with get () = testInput |> List.rev
    member _.TestEnvironmentWas with get () = testInputEnvironment |> List.rev
    member _.TestDataWas with get () = testData |> List.rev
    member _.SetupWasCalled with get () = 0 < setupCount
    member _.TestWasCalled with get () = 0 < testCount
    member _.TeardownWasCalled with get () = 0 < teardownCount
    member this.WasCalled with get () = this.SetupWasCalled || this.TeardownWasCalled || this.TestWasCalled
    member _.NumberOfTimesSetupWasCalled with get () = setupCount
    member _.NumberOfTimesTeardownWasCalled with get () = teardownCount
    member _.NumberOfTimesTestWasCalled with get () = testCount

let newMonitorWithTestResult testResult =
    Monitor<unit, unit, unit> (Ok (), testResult, Ok ())
    
let newMonitorWithTestAction (testAction: TestFunction<unit>) =
    Monitor<unit, unit, unit> (Ok (), testAction)
    
let newMonitorWithTeardownAction (teardownAction: Result<unit, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>) =
    Monitor<unit, unit, unit> ((fun _ -> Ok ()), (fun _ -> TestSuccess), teardownAction)
    
let newMonitorWithTeardownResult teardownResult =
    Monitor<unit, unit, unit> (Ok (), TestSuccess, teardownResult)
    
let newMonitorWithTestResultAndTeardownResult testResult teardownResult =
    Monitor<unit, unit, unit>(Ok (), testResult, teardownResult)

let setupBuildExecutorWithMonitorAtTheFeature _ =
    let monitor = Monitor<unit, unit, unit> (Ok ())
    let feature = Arrow.NewFeature (
        ignoreString (),
        ignoreString (),
        Setup monitor.CallSetup,
        Teardown monitor.CallTeardown
    )
    
    let test = feature.Test monitor.CallTestActionWithSetupEnvironment
    Ok (monitor, test.GetExecutor ())

let setupBuildExecutorWithMonitor _ =
    let buildIt (monitor: Monitor<unit, unit, 'b>) =
        let feature = Arrow.NewFeature (
            ignoreString (),
            ignoreString ()
        )
        
        let test = feature.Test (Setup monitor.CallSetup, TestBody monitor.CallTestActionWithSetupEnvironment, Teardown monitor.CallTeardown)
        test.GetExecutor ()
        
    Ok buildIt