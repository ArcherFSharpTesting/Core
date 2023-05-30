[<AutoOpen>]
module Archer.Arrows.Tests.TestBuilders

open System.Runtime.InteropServices
open Archer
open Archer.Arrows
open Archer.Arrows.Internal.Types
open Archer.Arrows.Internals
open Archer.MicroLang
open Microsoft.FSharp.Core

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
    let buildExecutor setupValue (testBody: TestFunctionTwoParameters<_, TestEnvironment>) =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup (fun upstream -> Ok (upstream, setupValue)), TestBody testBody, ignoreString (), $"%s{ignoreString ()}.fs", ignoreInt ())
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
    
type IFeatureMonitor<'featureType> =
    abstract member HasSetupFunctionBeenCalled: bool with get
    abstract member NumberOfTimesSetupHasBeenCalled: int with get
    
    abstract member HasTeardownBeenCalled: bool with get
    abstract member TeardownFunctionCalledWith:  (Result<'featureType, SetupTeardownFailure> * TestResult option) list with get 
    
    abstract member FunctionSetupWith: 'featureType -> (unit -> Result<'featureType, SetupTeardownFailure>)
    abstract member FunctionSetupWith: SetupTeardownFailure -> (unit -> Result<'featureType, SetupTeardownFailure>)
    abstract member FunctionSetupFailsWith: message: string -> (unit -> Result<'featureType, SetupTeardownFailure>)
    
    abstract member FunctionTeardownWith: unit -> (Result<'featureType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>)
    abstract member FunctionTeardownWith: SetupTeardownFailure -> (Result<'featureType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>)
    abstract member FunctionTeardownFailsWith: message: string -> (Result<'featureType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>)
    
type FeatureMonitor<'featureType> () =
    let mutable setupCallCount = 0
    let mutable teardownParams: (Result<'featureType, SetupTeardownFailure> * TestResult option) list = []
    
    member _.HasSetupFunctionBeenCalled with get () = 0 < setupCallCount
    member _.NumberOfTimesSetupHasBeenCalled with get () = setupCallCount
    
    member _.HasTeardownBeenCalled with get () = 0 < teardownParams.Length
    member _.TeardownFunctionCalledWith with get () = teardownParams |> List.rev
    
    member _.FunctionSetupWith (featureValue: 'featureType) =
        let setupFunction () =
            setupCallCount <- setupCallCount + 1
            Ok featureValue
            
        setupFunction
        
    member _.FunctionSetupWith (failure: SetupTeardownFailure) =
        let setupFunction (): Result<'featureType, SetupTeardownFailure> =
            setupCallCount <- setupCallCount + 1
            Error failure
            
        setupFunction
        
    member _.FunctionSetupFailsWith (message: string) =
        let setupFunction (): Result<'featureType, SetupTeardownFailure> =
            setupCallCount <- setupCallCount + 1
            failwith message
            
        setupFunction
    
    member _.FunctionTeardownWith () =
        let teardownFunction (setupResult: Result<'featureType, SetupTeardownFailure>) (testResult: TestResult option) =
            teardownParams <- (setupResult, testResult)::teardownParams
            Ok ()
        
        teardownFunction
        
    member _.FunctionTeardownWith (failure: SetupTeardownFailure) =
        let teardownFunction (setupResult: Result<'featureType, SetupTeardownFailure>) (testResult: TestResult option) =
            teardownParams <- (setupResult, testResult)::teardownParams
            Error failure
            
        teardownFunction
    
    member _.FunctionTeardownFailsWith message =
        let teardownFunction (setupResult: Result<'featureType, SetupTeardownFailure>) (testResult: TestResult option) =
            teardownParams <- (setupResult, testResult)::teardownParams
            failwith message
            
        teardownFunction
        
    interface IFeatureMonitor<'featureType> with
        member this.HasSetupFunctionBeenCalled with get () = this.HasSetupFunctionBeenCalled
        member this.NumberOfTimesSetupHasBeenCalled with get () = this.NumberOfTimesSetupHasBeenCalled
        
        member this.HasTeardownBeenCalled with get () = this.HasTeardownBeenCalled
        member this.TeardownFunctionCalledWith with get () = this.TeardownFunctionCalledWith 
        
        member this.FunctionSetupWith featureValue = this.FunctionSetupWith featureValue
        member this.FunctionSetupWith failure = this.FunctionSetupWith failure
        member this.FunctionSetupFailsWith message = this.FunctionSetupFailsWith message
        
        member this.FunctionTeardownWith () = this.FunctionTeardownWith ()
        member this.FunctionTeardownWith failure = this.FunctionTeardownWith failure
        member this.FunctionTeardownFailsWith message = this.FunctionTeardownFailsWith message

    
    
type ITestMonitor<'dataType, 'featureType, 'setupType> =
    // Verify Calls
    abstract member HasSetupFunctionBeenCalled: bool with get
    abstract member SetupFunctionWasCalledWith: 'featureType list with get
    
    abstract member HasTestFunctionBeenCalled: bool with get
    abstract member TestFunctionWasCalledWith: ('dataType option * ('featureType * 'setupType) option * TestEnvironment option) list with get
    
    abstract member HasTeardownBeenCalled: bool with get
    abstract member TeardownFunctionCalledWith:  (Result<'featureType * 'setupType option, SetupTeardownFailure> * TestResult option) list with get
    
    // Functions
    // -- Setup
    abstract member FunctionFeatureSetupWith: 'setupType -> ('featureType -> Result<'featureType * 'setupType, SetupTeardownFailure>)
    abstract member FunctionSetupWith: 'setupType -> ('featureType -> Result<'setupType, SetupTeardownFailure>)
    abstract member FunctionFeatureSetupFailsWith: message: string -> ('featureType -> Result<'featureType * 'setupType, SetupTeardownFailure>)
    abstract member FunctionSetupFailsWith: message: string -> ('featureType -> Result<'setupType, SetupTeardownFailure>)
    
    // -- Test
    // -- -- Data Three Params
    abstract member FunctionTestFeatureDataThreeParametersWith: testResult: TestResult -> ('dataType -> 'featureType * 'setupType -> TestEnvironment -> TestResult)
    abstract member FunctionTestFeatureDataThreeParametersSuccess: 'dataType -> 'featureType * 'setupType -> TestEnvironment -> TestResult
    abstract member FunctionTestFeatureDataThreeParametersFailsWith: message: string -> ('dataType -> 'featureType * 'setupType -> TestEnvironment -> TestResult)
    
    abstract member FunctionTestDataThreeParametersWith: testResult: TestResult -> ('dataType -> 'setupType -> TestEnvironment -> TestResult)
    abstract member FunctionTestDataThreeParametersSuccess: 'dataType -> 'setupType -> TestEnvironment -> TestResult
    abstract member FunctionTestDataThreeParametersFailsWith: message: string -> ('dataType -> 'setupType -> TestEnvironment -> TestResult)
    
    // -- -- Data Two Params
    abstract member FunctionTestFeatureDataTwoParametersWith: testResult: TestResult -> ('dataType -> 'featureType * 'setupType -> TestResult)
    abstract member FunctionTestFeatureDataTwoParametersSuccess: 'dataType -> 'featureType * 'setupType -> TestResult
    abstract member FunctionTestFeatureDataTwoParametersFailsWith: message: string -> ('dataType -> 'featureType * 'setupType -> TestResult)
    
    abstract member FunctionTestDataTwoParametersWith: testResult: TestResult -> ('dataType -> 'setupType -> TestResult)
    abstract member FunctionTestDataTwoParametersSuccess: 'dataType -> 'setupType -> TestResult
    abstract member FunctionTestDataTwoParametersFailsWith: message: string -> ('dataType -> 'setupType -> TestResult)
    
    // -- -- Data One Param
    abstract member FunctionTestDataOneParameterWith: testResult: TestResult -> 'dataType -> TestResult
    abstract member FunctionTestDataOneParameterSuccess: 'dataType -> TestResult
    abstract member FunctionTestDataOneParameterFailsWith: message: string -> ('dataType -> TestResult)
    
    // -- -- Two Params
    abstract member FunctionTestFeatureTwoParametersWith: testResult: TestResult -> ('featureType * 'setupType) -> TestEnvironment -> TestResult
    abstract member FunctionTestFeatureTwoParametersSuccess: ('featureType * 'setupType) -> TestEnvironment -> TestResult
    abstract member FunctionTestFeatureTwoParametersFailWith: message: string -> (('featureType * 'setupType) -> TestEnvironment -> TestResult)
    
    abstract member FunctionTestFeatureTwoParametersWith: testResult: TestResult -> 'setupType -> TestEnvironment -> TestResult
    abstract member FunctionTestFeatureTwoParametersSuccess: 'setupType -> TestEnvironment -> TestResult
    abstract member FunctionTestFeatureTwoParametersFailWith: message: string -> ('setupType -> TestEnvironment -> TestResult)
    
    // -- -- One Param
    abstract member FunctionTestFeatureOneParameterWith: testResult: TestResult -> ('featureType * 'setupType) -> TestResult
    abstract member FunctionTestFeatureOneParameterSuccess: ('featureType * 'setupType) -> TestResult
    abstract member FunctionTestFeatureOneParameterFailWith: message: string -> (('featureType * 'setupType) -> TestResult)
    
    abstract member FunctionTestOneParameterWith: testResult: TestResult -> ('setupType -> TestResult)
    abstract member FunctionTestOneParameterSuccess: 'setupType -> TestResult
    abstract member FunctionTestOneParameterFailWith: message: string -> ('setupType -> TestResult)
    
    // -- Teardown
    abstract member FunctionTeardownFeatureFromSetup : Result<'featureType * 'setupType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>
    abstract member FunctionTeardownFeatureFromSetupWith : failure: SetupTeardownFailure -> (Result<'featureType * 'setupType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>)
    
    abstract member FunctionTeardownFromFeature: Result<'featureType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>
    abstract member FunctionTeardownFromFeatureWith:  failure: SetupTeardownFailure -> (Result<'featureType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>)
    
    abstract member FunctionTeardownFromSetup : Result<'setupType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>
    abstract member FunctionTeardownFromSetupWith : failure: SetupTeardownFailure -> (Result<'setupType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>)


type TestMonitor<'dataType, 'featureType, 'setupType> () =
    let mutable setupParams : 'featureType list = []
    let mutable testParams: ('dataType option * ('featureType option * 'setupType) option * TestEnvironment option) list = []
    let mutable teardownParams: (Result<'featureType option * 'setupType, SetupTeardownFailure> * TestResult option) list = []
    
    // Verify Calls
    member _.HasSetupFunctionBeenCalled with get () = 0 < setupParams.Length
    member _.SetupFunctionWasCalledWith with get () = setupParams |> List.rev
    
    member _.HasTestFunctionBeenCalled with get () = 0 < testParams.Length 
    member _.TestFunctionWasCalledWith with get () = testParams |> List.rev
    
    member _.HasTeardownBeenCalled with get () = 0 < teardownParams.Length
    member _.TeardownFunctionCalledWith with get () = teardownParams |> List.rev
    
    // Functions
    // -- Setup
    member _.FunctionFeatureSetupWith (setupValue: 'setupType) =
        let setupFunction (featureValue: 'featureType) =
            setupParams <- featureValue:: setupParams
            Ok (featureValue, setupValue)
            
        setupFunction
    
    member _.FunctionSetupWith (setupValue: 'setupType) =
        let setupFunction (featureValue: 'featureType) =
            setupParams <- featureValue:: setupParams
            Ok setupValue
            
        setupFunction
        
    member _.FunctionFeatureSetupFailsWith message =
        let setupFunction (featureValue: 'featureType) =
            setupParams <- featureValue:: setupParams
            failwith message
            
        setupFunction

    member _.FunctionSetupFailsWith message =
        let setupFunction (featureValue: 'featureType): Result<'setupType, SetupTeardownFailure> =
            setupParams <- featureValue:: setupParams
            failwith message
            
        setupFunction
    
    // -- Test
    // -- -- Data Three Params
    member _.FunctionTestFeatureDataThreeParametersWith (testResult: TestResult) =
        let testFunction (data: 'dataType) (featureValue: 'featureType, setupValue: 'setupType) (environment: TestEnvironment) =
            testParams <- (Some data, Some (Some featureValue, setupValue), Some environment)::testParams
            testResult
            
        testFunction
            
    member _.FunctionTestFeatureDataThreeParametersSuccess (data: 'dataType) (featureValue: 'featureType, setupValue: 'setupType) (environment: TestEnvironment) =
        testParams <- (Some data, Some (Some featureValue, setupValue), Some environment)::testParams
        TestSuccess
        
    member _.FunctionTestFeatureDataThreeParametersFailsWith message =
        let testFunction (data: 'dataType) (featureValue: 'featureType, setupValue: 'setupType) (environment: TestEnvironment) =
            testParams <- (Some data, Some (Some featureValue, setupValue), Some environment)::testParams
            failwith message
            
        testFunction
    
    member _.FunctionTestDataThreeParametersWith testResult =
        let testFunction (data: 'dataType) (setupValue: 'setupType) (environment: TestEnvironment) =
            testParams <- (Some data, Some (None, setupValue), Some environment)::testParams
            testResult
            
        testFunction
            
    member _.FunctionTestDataThreeParametersSuccess (data: 'dataType) (setupValue: 'setupType) (environment: TestEnvironment) =
        testParams <- (Some data, Some (None, setupValue), Some environment)::testParams
        TestSuccess
        
    member _.FunctionTestDataThreeParametersFailsWith message = 
        let testFunction (data: 'dataType) (setupValue: 'setupType) (environment: TestEnvironment) =
            testParams <- (Some data, Some (None, setupValue), Some environment)::testParams
            failwith message
            
        testFunction
    
    // -- -- Data Two Params
    member _.FunctionTestFeatureDataTwoParametersWith (testResult: TestResult) =
        let testFunction (data: 'dataType) (featureValue: 'featureType, setupValue: 'setupType) =
            testParams <- (Some data, Some (Some featureValue, setupValue), None)::testParams
            testResult
            
        testFunction
        
    member _.FunctionTestFeatureDataTwoParametersSuccess (data: 'dataType) (featureValue: 'featureType, setupValue: 'setupType) =
        testParams <- (Some data, Some (Some featureValue, setupValue), None)::testParams
        TestSuccess
        
    member _.FunctionTestFeatureDataTwoParametersFailsWith message =
        let testFunction (data: 'dataType) (featureValue: 'featureType, setupValue: 'setupType) =
            testParams <- (Some data, Some (Some featureValue, setupValue), None)::testParams
            failwith message
            
        testFunction
    
    member _.FunctionTestDataTwoParametersWith (testResult: TestResult) =
        let testFunction (data: 'dataType) (setupValue: 'setupType) =
            testParams <- (Some data, Some (None, setupValue), None)::testParams
            testResult
            
        testFunction
        
    member _.FunctionTestDataTwoParametersSuccess (data: 'dataType) (setupValue: 'setupType) =
        testParams <- (Some data, Some (None, setupValue), None)::testParams
        TestSuccess
        
    member _.FunctionTestDataTwoParametersFailsWith message =
        let testFunction (data: 'dataType) (setupValue: 'setupType) =
            testParams <- (Some data, Some (None, setupValue), None)::testParams
            failwith message
            
        testFunction
        
    
    // -- -- Data One Param
    member _.FunctionTestDataOneParameterWith (testResult: TestResult) =
        let testFunction (data: 'dataType) =
            testParams <- (Some data, None, None)::testParams
            testResult
            
        testFunction
        
    member _.FunctionTestDataOneParameterSuccess (data: 'dataType) =
        testParams <- (Some data, None, None)::testParams
        TestSuccess
        
    member _.FunctionTestDataOneParameterFailsWith message =
        let testFunction (data: 'dataType) =
            testParams <- (Some data, None, None)::testParams
            failwith message
            
        testFunction
    
    // -- -- Two Params
    member _.FunctionTestFeatureTwoParametersWith (testResult: TestResult) =
        let testFunction (featureValue: 'featureType, setupValue: 'setupType) (environment: TestEnvironment) =
            testParams <- (None, Some (Some featureValue, setupValue), Some environment)::testParams
            testResult
            
        testFunction
        
    member _.FunctionTestFeatureTwoParametersSuccess (featureValue: 'featureType, setupValue: 'setupType) (environment: TestEnvironment) =
        testParams <- (None, Some (Some featureValue, setupValue), Some environment)::testParams
        TestSuccess
        
    member _.FunctionTestFeatureTwoParametersFailWith message =
        let testFunction (featureValue: 'featureType, setupValue: 'setupType) (environment: TestEnvironment) =
            testParams <- (None, Some (Some featureValue, setupValue), Some environment)::testParams
            failwith message
            
        testFunction
    
    member _.FunctionTestFeatureTwoParametersWith (testResult: TestResult) =
        let testFunction (setupValue: 'setupType) (environment: TestEnvironment) =
            testParams <- (None, Some (None, setupValue), Some environment)::testParams
            testResult
            
        testFunction
        
    member _.FunctionTestFeatureTwoParametersSuccess (setupValue: 'setupType) (environment: TestEnvironment) =
        testParams <- (None, Some (None, setupValue), Some environment)::testParams
        TestSuccess
        
    member _.FunctionTestFeatureTwoParametersFailWith message =
        let testFunction (setupValue: 'setupType) (environment: TestEnvironment) =
            testParams <- (None, Some (None, setupValue), Some environment)::testParams
            failwith message
            
        testFunction
    
    // -- -- One Param
    member _.FunctionTestFeatureOneParameterWith (testResult: TestResult) =
        let testFunction (featureValue: 'featureType, setupValue: 'setupType) =
            testParams <- (None, Some (Some featureValue, setupValue), None)::testParams
            testResult
            
        testFunction
        
    member _.FunctionTestFeatureOneParameterSuccess (featureValue: 'featureType, setupValue: 'setupType) =
        testParams <- (None, Some (Some featureValue, setupValue), None)::testParams
        TestSuccess
        
    member _.FunctionTestFeatureOneParameterFailWith message =
        let testFunction (featureValue: 'featureType, setupValue: 'setupType) =
            testParams <- (None, Some (Some featureValue, setupValue), None)::testParams
            failwith message
            
        testFunction
    
    // abstract member FunctionTestOneParameterWith: testResult: TestResult -> ('setupType -> TestResult)
    // abstract member FunctionTestOneParameterSuccess: 'setupType -> TestResult
    // abstract member FunctionTestOneParameterFailWith: message: string -> ('setupType -> TestResult)
    //
    // // -- Teardown
    // abstract member FunctionTeardownFeatureFromSetup : Result<'featureType * 'setupType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>
    // abstract member FunctionTeardownFeatureFromSetupWith : failure: SetupTeardownFailure -> (Result<'featureType * 'setupType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>)
    //
    // abstract member FunctionTeardownFromFeature: Result<'featureType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>
    // abstract member FunctionTeardownFromFeatureWith:  failure: SetupTeardownFailure -> (Result<'featureType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>)
    //
    // abstract member FunctionTeardownFromSetup : Result<'setupType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>
    // abstract member FunctionTeardownFromSetupWith : failure: SetupTeardownFailure -> (Result<'setupType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>)


    
    

let setupBuildExecutorWithMonitorAtTheFeature _ =
    let monitor = TestMonitor<unit, unit, unit> (Ok (), TestSuccess)
    let monitor = monitor.Interface
    
    let feature = Arrow.NewFeature (
        ignoreString (),
        ignoreString (),
        Setup monitor.FunctionSetup,
        Teardown monitor.FunctionTeardownFromSetup
    )
    
    let test = feature.Test monitor.FunctionTestTwoParameters
    Ok (monitor, test.GetExecutor ())

let setupBuildExecutorWithMonitor _ =
    let buildIt (monitor: ITestMonitor<_, _, 'b>) =
        let feature = Arrow.NewFeature (
            ignoreString (),
            ignoreString ()
        )
        
        let test = feature.Test (Setup monitor.FunctionSetup, TestBody monitor.FunctionTestTwoParameters, Teardown monitor.FunctionTeardownFromSetup)
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
    let monitor = TestMonitor(setupValue, TestSuccess).Interface
    
    monitor, (tags, setupValue), (path, fileName, fullPath, lineNumber)
    
let private getMonitorWithoutSetupBaseTestParts () = 
    let tags, (path, fileName, fullPath, lineNumber) = getBaseTestParts ()
    let monitor = TestMonitor((), TestSuccess).Interface
    
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
    
let private getTestParts () =
    let testName = $"My %s{randomWord 5} Test"
    
    let monitor, (tags, setupValue), (path, fileName, fullPath, lineNumber) = getMonitorWithSetupBaseTestParts ()

    monitor, (testName, tags, setupValue), (path, fileName, fullPath, lineNumber)
    
type TestBuilder =
    static member GetTestNames (f: int -> 'a -> string) data =
        let [a; b; c] =
            data
            |> List.mapi f
            
        (a, b, c)
        
    //test name, tags, setup, data, test body indicator three parameters, teardown
    static member BuildTestWithTestNameTagsSetupDataTestBodyThreeParametersTeardownNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (tags, setupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestPartsNameHints repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup monitor.FunctionSetup,
                Data data,
                TestBody monitor.FunctionTestDataThreeParameters,
                Teardown monitor.FunctionTeardownFromSetup,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, setupValue, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsSetupDataTestBodyThreeParametersTeardown (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup monitor.FunctionSetup,
                Data data,
                TestBody monitor.FunctionTestDataThreeParameters,
                Teardown monitor.FunctionTeardownFromSetup,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, setupValue, data, testName), (path, fileName, lineNumber)
        
    //test name, tags, setup, data, test body indicator three parameters
    static member BuildTestWithTestNameTagsSetupDataTestBodyThreeParametersNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestPartsNameHints  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup monitor.FunctionSetup,
                Data data,
                TestBody monitor.FunctionTestDataThreeParameters,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, setupValue, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsSetupDataTestBodyThreeParameters (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup monitor.FunctionSetup,
                Data data,
                TestBody monitor.FunctionTestDataThreeParameters,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, setupValue, data, testName), (path, fileName, lineNumber)
        
    //test name, tags, setup, data, test body indicator two parameters, teardown
    static member BuildTestWithTestNameTagsSetupDataTestBodyTwoParametersTeardownNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (tags, setupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestPartsNameHints repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup monitor.FunctionSetup,
                Data data,
                TestBody monitor.FunctionTestDataTwoParameters,
                Teardown monitor.FunctionTeardownFromSetup,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, setupValue, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsSetupDataTestBodyTwoParametersTeardown (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup monitor.FunctionSetup,
                Data data,
                TestBody monitor.FunctionTestDataTwoParameters,
                Teardown monitor.FunctionTeardownFromSetup,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, setupValue, data, testName), (path, fileName, lineNumber)
        
    //test name, tags, setup, data, test body indicator two parameters
    static member BuildTestWithTestNameTagsSetupDataTestBodyTwoParametersNameHints (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameBase, testName), (tags, setupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestPartsNameHints repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup monitor.FunctionSetup,
                Data data,
                TestBody monitor.FunctionTestDataTwoParameters,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, setupValue, data, testNameBase), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsSetupDataTestBodyTwoParameters (testFeature: IFeature<unit>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, tags, setupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts  repeatDataValue
    
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup monitor.FunctionSetup,
                Data data,
                TestBody monitor.FunctionTestDataTwoParameters,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, setupValue, data, testName), (path, fileName, lineNumber)
    
    //test name, tags, setup, test body indicator, teardown
    static member BuildTestWithTestNameTagsSetupTestBodyTwoParametersTeardown (testFeature: IFeature<unit>) =
        let monitor, (testName, tags, setupValue), (path, fileName, fullPath, lineNumber) = getTestParts ()
    
        let test =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup monitor.FunctionSetup,
                TestBody monitor.FunctionTestTwoParameters,
                Teardown monitor.FunctionTeardownFromSetup,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (tags, setupValue, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsSetupTestBodyOneParameterTeardown (testFeature: IFeature<unit>) =
        let monitor, (testName, tags, setupValue), (path, fileName, fullPath, lineNumber) = getTestParts ()
    
        let test =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup monitor.FunctionSetup,
                TestBody monitor.FunctionTestOneParameter,
                Teardown monitor.FunctionTeardownFromSetup,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (tags, setupValue, testName), (path, fileName, lineNumber)

    //test name, tags, setup, test body indicator        
    static member BuildTestWithTestNameTagsSetupTestBodyTwoParameters (testFeature: IFeature<unit>) =
        let monitor, (testName, tags, setupValue), (path, fileName, fullPath, lineNumber) = getTestParts ()
    
        let test =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup monitor.FunctionSetup,
                TestBody monitor.FunctionTestTwoParameters,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (tags, setupValue, testName), (path, fileName, lineNumber)

    static member BuildTestWithTestNameTagsSetupTestBodyOneParameter (testFeature: IFeature<unit>) =
        let monitor, (testName, tags, setupValue), (path, fileName, fullPath, lineNumber) = getTestParts ()
    
        let test =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup monitor.FunctionSetup,
                TestBody monitor.FunctionTestOneParameter,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (tags, setupValue, testName), (path, fileName, lineNumber)
