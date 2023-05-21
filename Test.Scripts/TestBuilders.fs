[<AutoOpen>]
module Archer.Arrows.Tests.TestBuilders

open System.Runtime.InteropServices
open Archer
open Archer.Arrows
open Archer.Arrows.Internal.Types
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

type Monitor<'dataType, 'featureType, 'setupOutputType> (setupAction: 'featureType -> Result<'setupOutputType, SetupTeardownFailure>, testAction: TestFunction<'setupOutputType>, teardownAction: Result<'setupOutputType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>) =
    let mutable setupInput: 'featureType list = []
    let mutable setupResult: Result<'setupOutputType, SetupTeardownFailure> list = []
    let mutable testInput: 'setupOutputType list = []
    let mutable testData: 'dataType list = []
    let mutable testInputEnvironment: TestEnvironment list = []
    let mutable testResultResult: TestResult option list = []
    let mutable setupCount = 0
    let mutable teardownCount = 0
    let mutable testCount = 0
    
    new (setupAction: 'featureType -> Result<'setupOutputType, SetupTeardownFailure>) =
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
        
    member this.CallTestActionWithDataSetupEnvironment data input environment =
        testData <- data::testData
        this.CallTestActionWithSetupEnvironment input environment
        
    member _.CallTeardown setupValue testValue =
        teardownCount <- teardownCount + 1
        setupResult <- setupValue::setupResult
        testResultResult <- testValue::testResultResult
        teardownAction setupValue testValue
        
    member _.CallUnitTeardown (_: Result<unit, SetupTeardownFailure>) testValue : Result<unit, SetupTeardownFailure> =
        teardownCount <- teardownCount + 1
        testResultResult <- testValue::testResultResult
        Ok ()
        
    member _.TestSetupInputWas with get () = setupInput |> List.rev
        
    member _.TeardownWasCalledWith with get () =
        setupResult |> List.rev, testResultResult |> List.rev
        
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
    
let private getBaseTestParts () =
    let path = $"%s{randomCapitalLetter ()}:\\"
    let fileName = $"%s{randomWord (rand.Next (1, 5))}.%s{randomLetter ()}"
    let fullPath = $"%s{path}%s{fileName}"
    let lineNumber = rand.Next ()

    let tags = [
        Category $"%s{randomWord (rand.Next (3, 8))}"
        if rand.Next () % 2 = 0 then Only else (Category $"%s{randomWord (rand.Next (3, 8))}")
        if rand.Next () % 2 = 0 then Serial else (Category $"%s{randomWord (rand.Next (3, 8))}")
    ]

    tags, (path, fileName, fullPath, lineNumber)
    
let private getMonitorWithSetupBaseTestParts () =
    let tags, (path, fileName, fullPath, lineNumber) = getBaseTestParts ()
    let setupValue = rand.Next ()
    let monitor = Monitor (Ok setupValue)
    
    monitor, (tags, setupValue), (path, fileName, fullPath, lineNumber)
    
let private getMonitorWithoutSetupBaseTestParts () = 
    let tags, (path, fileName, fullPath, lineNumber) = getBaseTestParts ()
    let monitor = Monitor (Ok ())
    
    monitor, tags, (path, fileName, fullPath, lineNumber)
    
let private getDataTestPartsNameHints repeat =
    let testNameBase = $"My %s{randomWord 5} Test"
    let testName = $"%s{testNameBase} %%s"
    
    let monitor, (tags, setupValue), (path, fileName, fullPath, lineNumber) = getMonitorWithSetupBaseTestParts ()

    let data =
        if repeat then
            let l = randomLetter ()
            [l; l; l]
        else
            randomDistinctLetters 3

    let nameInfo = testNameBase, testName
    let testParts = tags, setupValue, data
    let locationParts = path, fileName, fullPath, lineNumber
    monitor, nameInfo, testParts, locationParts
    
let private getDataTestPartsNoSetupNameHints repeat =
    let testNameBase = $"My %s{randomWord 5} Test"
    let testName = $"%s{testNameBase} %%s"
    
    let monitor, tags, (path, fileName, fullPath, lineNumber) = getMonitorWithoutSetupBaseTestParts ()

    let data =
        if repeat then
            let l = randomLetter ()
            [l; l; l]
        else
            randomDistinctLetters 3

    monitor, (testNameBase, testName), (tags, data), (path, fileName, fullPath, lineNumber)
    
let private getDataTestParts repeat =
    let testName = $"My %s{randomWord 5} Test"
    
    let monitor, (tags, setupValue), (path, fileName, fullPath, lineNumber) = getMonitorWithSetupBaseTestParts ()

    let data =
        if repeat then
            let l = randomLetter ()
            [l; l; l]
        else
            randomDistinctLetters 3

    monitor, (testName, tags, setupValue, data), (path, fileName, fullPath, lineNumber)
    
let private getDataTestPartsNoSetup repeat =
    let testName = $"My %s{randomWord 5} Test"
    
    let monitor, tags, (path, fileName, fullPath, lineNumber) = getMonitorWithoutSetupBaseTestParts ()

    let data =
        if repeat then
            let l = randomLetter ()
            [l; l; l]
        else
            randomDistinctLetters 3

    monitor, (testName, tags, data), (path, fileName, fullPath, lineNumber)
    
type TestBuilder =
    static member GetTestNames (f: int -> 'a -> string) data =
        let [a; b; c] =
            data
            |> List.mapi f
            
        (a, b, c)
        
    static member BuildTestWithTestNameTagsSetupDataTestBodyThreeParametersTeardownNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, nameInfo, testParts, locationInfo = getDataTestPartsNameHints repeatDataValue
        let testNameBase, testName = nameInfo
        let tags, setupValue, data = testParts
        let path, fileName, fullPath, lineNumber = locationInfo

        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup monitor.CallSetup,
                Data data,
                TestBody monitor.CallTestActionWithDataSetupEnvironment,
                Teardown monitor.CallTeardown,
                fullPath,
                lineNumber
            )

        let testingParts = (monitor, tests)
        let testParts = (tags, setupValue, data, testNameBase)
        let locationParts = (path, fileName, lineNumber)
        testingParts, testParts, locationParts
        
    static member BuildTestWithTestNameTagsSetupDataTestBodyThreeParametersNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup monitor.CallSetup,
                Data data,
                TestBody monitor.CallTestActionWithDataSetupEnvironment,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, setupValue, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestBodyTwoParametersTeardownNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody monitor.CallTestActionWithDataSetup,
                Teardown monitor.CallUnitTeardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestBodyTeardownNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody monitor.CallTestActionWithData,
                Teardown monitor.CallUnitTeardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestBodyTwoParametersNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody monitor.CallTestActionWithDataSetup,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestFunctionTwoParametersNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                monitor.CallTestActionWithDataSetup,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestBodyNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (tags, setupValue, data), (path, fileName, fullPath, lineNumber)= getDataTestPartsNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody monitor.CallTestActionWithData,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, setupValue, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameSetupDataTestBodyThreeParametersTeardownNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (_tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                Setup monitor.CallSetup,
                Data data,
                TestBody monitor.CallTestActionWithDataSetupEnvironment,
                Teardown monitor.CallTeardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (setupValue, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameSetupDataTestBodyThreeParametersNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (_tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                Setup monitor.CallSetup,
                Data data,
                TestBody monitor.CallTestActionWithDataSetupEnvironment,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (setupValue, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameDataTestBodyTwoParametersTeardownNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (_tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody monitor.CallTestActionWithDataSetup,
                Teardown monitor.CallTeardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameDataTestBodyTeardownNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (_tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody monitor.CallTestActionWithData,
                Teardown monitor.CallTeardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameDataTestBodyTwoParametersNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (_tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody monitor.CallTestActionWithDataSetup,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameDataTestFunctionTwoParametersNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (_tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                Data data,
                monitor.CallTestActionWithDataSetup,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameDataTestBodyNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (_tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody monitor.CallTestActionWithData,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (setupValue, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsSetupDataTestBodyThreeParametersTeardownNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                TestTags tags,
                Setup monitor.CallSetup,
                Data data,
                TestBody monitor.CallTestActionWithDataSetupEnvironment,
                Teardown monitor.CallTeardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, setupValue, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsSetupDataTestActionThreeParametersNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                TestTags tags,
                Setup monitor.CallSetup,
                Data data,
                TestBody monitor.CallTestActionWithDataSetupEnvironment,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, setupValue, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestBodyTwoParametersTeardownNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody monitor.CallTestActionWithDataSetup,
                Teardown monitor.CallTeardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestBodyTeardownNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody monitor.CallTestActionWithData,
                Teardown monitor.CallTeardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestBodyTwoParametersNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody monitor.CallTestActionWithDataSetup,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestBodyNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody monitor.CallTestActionWithData,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestFunctionTwoParametersNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                monitor.CallTestActionWithDataSetup,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestFunctionNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                monitor.CallTestActionWithData,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithSetupDataTestBodyThreeParametersTeardownNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (_tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                Setup monitor.CallSetup,
                Data data,
                TestBody monitor.CallTestActionWithDataSetupEnvironment,
                Teardown monitor.CallTeardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (setupValue, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithSetupDataTestBodyThreeParametersNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (_tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                Setup monitor.CallSetup,
                Data data,
                TestBody monitor.CallTestActionWithDataSetupEnvironment,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (setupValue, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithDataTestBodyTwoParametersTeardownNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (_tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                Data data,
                TestBody monitor.CallTestActionWithDataSetup,
                Teardown monitor.CallTeardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithDataTestBodyTeardownNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (_tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                Data data,
                TestBody monitor.CallTestActionWithData,
                Teardown monitor.CallTeardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithDataTestBodyTwoParametersNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (_tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                Data data,
                TestBody monitor.CallTestActionWithDataSetup,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithDataTestBodyNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (_tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                Data data,
                TestBody monitor.CallTestActionWithData,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithDataTestFunctionTwoParametersNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (_tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                Data data,
                monitor.CallTestActionWithDataSetup,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithDataTestFunctionNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (_tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetupNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                Data data,
                monitor.CallTestActionWithData,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsSetupDataTestBodyThreeParametersTeardown (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup monitor.CallSetup,
                Data data,
                TestBody monitor.CallTestActionWithDataSetupEnvironment,
                Teardown monitor.CallTeardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, setupValue, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsSetupDataTestBodyThreeParameters (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup monitor.CallSetup,
                Data data,
                TestBody monitor.CallTestActionWithDataSetupEnvironment,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, setupValue, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestBodyTwoParametersTeardown (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody monitor.CallTestActionWithDataSetup,
                Teardown monitor.CallUnitTeardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestBodyTeardown (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody monitor.CallTestActionWithData,
                Teardown monitor.CallUnitTeardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestBodyTwoParameters (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody monitor.CallTestActionWithDataSetup,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestFunctionTwoParameters (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                monitor.CallTestActionWithDataSetup,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestBody (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody monitor.CallTestActionWithData,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, setupValue, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameSetupDataTestBodyThreeParametersTeardown (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                Setup monitor.CallSetup,
                Data data,
                TestBody monitor.CallTestActionWithDataSetupEnvironment,
                Teardown monitor.CallTeardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (setupValue, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameSetupDataTestBodyThreeParameters (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                Setup monitor.CallSetup,
                Data data,
                TestBody monitor.CallTestActionWithDataSetupEnvironment,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (setupValue, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameDataTestBodyTwoParametersTeardown (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody monitor.CallTestActionWithDataSetup,
                Teardown monitor.CallTeardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameDataTestBodyTeardown (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody monitor.CallTestActionWithData,
                Teardown monitor.CallTeardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameDataTestBodyTwoParameters (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody monitor.CallTestActionWithDataSetup,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameDataTestFunctionTwoParameters (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                Data data,
                monitor.CallTestActionWithDataSetup,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameDataTestBody (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody monitor.CallTestActionWithData,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (setupValue, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsSetupDataTestBodyThreeParametersTeardown (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts  repeatDataValue
    
        let tests =
            testFeature.Test (
                TestTags tags,
                Setup monitor.CallSetup,
                Data data,
                TestBody monitor.CallTestActionWithDataSetupEnvironment,
                Teardown monitor.CallTeardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, setupValue, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsSetupDataTestActionThreeParameters (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts  repeatDataValue
    
        let tests =
            testFeature.Test (
                TestTags tags,
                Setup monitor.CallSetup,
                Data data,
                TestBody monitor.CallTestActionWithDataSetupEnvironment,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, setupValue, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestBodyTwoParametersTeardown (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody monitor.CallTestActionWithDataSetup,
                Teardown monitor.CallTeardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestBodyTeardown (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody monitor.CallTestActionWithData,
                Teardown monitor.CallTeardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestBodyTwoParameters (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody monitor.CallTestActionWithDataSetup,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestBody (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody monitor.CallTestActionWithData,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestFunctionTwoParameters (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                monitor.CallTestActionWithDataSetup,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestFunction (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                monitor.CallTestActionWithData,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithSetupDataTestBodyThreeParametersTeardown (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts  repeatDataValue
    
        let tests =
            testFeature.Test (
                Setup monitor.CallSetup,
                Data data,
                TestBody monitor.CallTestActionWithDataSetupEnvironment,
                Teardown monitor.CallTeardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (setupValue, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithSetupDataTestBodyThreeParameters (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts  repeatDataValue
    
        let tests =
            testFeature.Test (
                Setup monitor.CallSetup,
                Data data,
                TestBody monitor.CallTestActionWithDataSetupEnvironment,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (setupValue, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithDataTestBodyTwoParametersTeardown (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                Data data,
                TestBody monitor.CallTestActionWithDataSetup,
                Teardown monitor.CallTeardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithDataTestBodyTeardown (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                Data data,
                TestBody monitor.CallTestActionWithData,
                Teardown monitor.CallTeardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithDataTestBodyTwoParameters (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                Data data,
                TestBody monitor.CallTestActionWithDataSetup,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithDataTestBody (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                Data data,
                TestBody monitor.CallTestActionWithData,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithDataTestFunctionTwoParameters (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                Data data,
                monitor.CallTestActionWithDataSetup,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithDataTestFunction (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _tags, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNoSetup  repeatDataValue
    
        let tests =
            testFeature.Test (
                Data data,
                monitor.CallTestActionWithData,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)
        