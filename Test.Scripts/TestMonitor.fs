[<AutoOpen>]
module Archer.Arrows.Tests.TestMonitors

open System.Runtime.InteropServices
open Archer
open Archer.Arrows
open Archer.Arrows.Internal.Types
open Archer.Arrows.Internals
open Archer.MicroLang
open Microsoft.FSharp.Core

let private rand = System.Random ()

let hasValue item =
    match item with
    | Some _ -> true
    | _ -> false
    
let getValue item =
    match item with
    | Some v -> v
    | _ -> failwith "No Value"

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
    let length =
        let mutable l = length
        while l <= 0 do
            l <- rand.Next (1, 5)
            
        l
        
    seq{ 0..length }
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
    let testSetupValue = randomWord (rand.Next (3, 10))
    let setup () =
        testSetupValue |> Ok
    (testSetupValue, buildFeatureUnderTestWithSetupAndTeardown (Setup setup) emptyTeardown)
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
        let test = feature.Test (Setup (fun _ -> Ok ()), TestBody testBody, ignoreString (), $"%s{ignoreString ()}.fs", ignoreInt ())
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
    
    member _.FunctionSetupWith (featureValue: 'featureType): (unit -> Result<'featureType, SetupTeardownFailure>) =
        let setupFunction () =
            setupCallCount <- setupCallCount + 1
            Ok featureValue
            
        setupFunction
        
    member _.FunctionSetupWith (failure: SetupTeardownFailure): (unit -> Result<'featureType, SetupTeardownFailure>) =
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
        
        member this.FunctionSetupWith (featureValue: 'featureType) = this.FunctionSetupWith featureValue
        member this.FunctionSetupWith (failure: SetupTeardownFailure) = this.FunctionSetupWith failure
        member this.FunctionSetupFailsWith message = this.FunctionSetupFailsWith message
        
        member this.FunctionTeardownWith () = this.FunctionTeardownWith ()
        member this.FunctionTeardownWith failure = this.FunctionTeardownWith failure
        member this.FunctionTeardownFailsWith message = this.FunctionTeardownFailsWith message
    
type ITestMonitor<'dataType, 'featureType, 'setupType> =
    // Verify Calls
    abstract member HasSetupFunctionBeenCalled: bool with get
    abstract member SetupFunctionParameterValues: 'featureType list with get
    abstract member NumberOfTimesSetupFunctionWasCalled: int
    
    abstract member HasTestFunctionBeenCalled: bool with get
    abstract member NumberOfTimesTestFunctionWasCalled: int with get
    
    abstract member HasTestFunctionBeenCalledWithDataParameter: bool
    abstract member TestFunctionDataParameterValues: 'dataType option list with get
    
    abstract member HasTestFunctionBeenCalledWithFeatureSetupParameter: bool
    abstract member TestFunctionFeatureSetupParameterValues: 'featureType option list with get
    
    abstract member HasTestFunctionBeenCalledWithTestSetupParameter: bool with get
    abstract member TestFunctionTestSetupParameterValues: 'setupType option list with get
    
    abstract member HasTestFunctionBeenCalledWithEnvironmentParameter: bool with get
    abstract member TestFunctionEnvironmentParameterValues: TestEnvironment option list with get
    
    abstract member HasTeardownBeenCalled: bool with get
    abstract member TeardownFunctionParameterValues:  (Result<'featureType option * 'setupType option, SetupTeardownFailure> * TestResult option) list with get
    abstract member NumberOfTimesTeardownFunctionWasCalled: int with get
    
    // Functions
    // -- Setup
    abstract member FunctionSetupFeatureWith: 'setupType -> ('featureType -> Result<'featureType * 'setupType, SetupTeardownFailure>)
    abstract member FunctionSetupFeatureWith: SetupTeardownFailure -> ('featureType -> Result<'featureType * 'setupType, SetupTeardownFailure>)
    abstract member FunctionSetupWith: 'setupType -> ('featureType -> Result<'setupType, SetupTeardownFailure>)
    abstract member FunctionSetupWith: SetupTeardownFailure -> ('featureType -> Result<'setupType, SetupTeardownFailure>)
    abstract member FunctionSetupFeatureFailsWith: message: string -> ('featureType -> Result<'featureType * 'setupType, SetupTeardownFailure>)
    abstract member FunctionSetupFailsWith: message: string -> ('featureType -> Result<'setupType, SetupTeardownFailure>)
    
    // -- Test
    // -- -- Data Three Params
    abstract member FunctionTestFeatureDataThreeParametersWith: testResult: TestResult -> ('dataType -> 'featureType * 'setupType -> TestEnvironment -> TestResult)
    abstract member FunctionTestFeatureDataThreeParametersSuccess: 'dataType -> 'featureType * 'setupType -> TestEnvironment -> TestResult
    abstract member FunctionTestFeatureDataThreeParametersFailsWith: message: string -> ('dataType -> 'featureType * 'setupType -> TestEnvironment -> TestResult)
    
    abstract member FunctionTestDataThreeParametersWith: testResult: TestResult -> ('dataType -> 'setupType -> TestEnvironment -> TestResult)
    abstract member FunctionTestDataThreeParametersSuccess: 'dataType -> 'setupType -> TestEnvironment -> TestResult
    abstract member FunctionTestDataThreeParametersFailsWith: message: string -> ('dataType -> 'setupType -> TestEnvironment -> TestResult)
    
    abstract member FunctionTestPassThroughDataThreeParametersWith: testResult: TestResult -> ('dataType -> 'featureType -> TestEnvironment -> TestResult)
    abstract member FunctionTestPassThroughDataThreeParametersSuccess: 'dataType -> 'featureType -> TestEnvironment -> TestResult
    abstract member FunctionTestPassThroughDataThreeParametersFailsWith: message: string -> ('dataType -> 'featureType -> TestEnvironment -> TestResult)

    
    // -- -- Data Two Params
    abstract member FunctionTestFeatureDataTwoParametersWith: testResult: TestResult -> ('dataType -> 'featureType * 'setupType -> TestResult)
    abstract member FunctionTestFeatureDataTwoParametersSuccess: 'dataType -> 'featureType * 'setupType -> TestResult
    abstract member FunctionTestFeatureDataTwoParametersFailsWith: message: string -> ('dataType -> 'featureType * 'setupType -> TestResult)
    
    abstract member FunctionTestDataTwoParametersWith: testResult: TestResult -> ('dataType -> 'setupType -> TestResult)
    abstract member FunctionTestDataTwoParametersSuccess: 'dataType -> 'setupType -> TestResult
    abstract member FunctionTestDataTwoParametersFailsWith: message: string -> ('dataType -> 'setupType -> TestResult)

    abstract member FunctionTestPassThroughDataTwoParametersWith: testResult: TestResult -> ('dataType -> 'featureType -> TestResult)
    abstract member FunctionTestPassThroughDataTwoParametersSuccess: 'dataType -> 'featureType -> TestResult
    abstract member FunctionTestPassThroughDataTwoParametersFailsWith: message: string -> ('dataType -> 'featureType -> TestResult)
    
    // -- -- Data One Param
    abstract member FunctionTestDataOneParameterWith: testResult: TestResult -> ('dataType -> TestResult)
    abstract member FunctionTestDataOneParameterSuccess: 'dataType -> TestResult
    abstract member FunctionTestDataOneParameterFailsWith: message: string -> ('dataType -> TestResult)
    
    // -- -- Two Params
    abstract member FunctionTestFeatureTwoParametersWith: testResult: TestResult -> (('featureType * 'setupType) -> TestEnvironment -> TestResult)
    abstract member FunctionTestFeatureTwoParametersSuccess: ('featureType * 'setupType) -> TestEnvironment -> TestResult
    abstract member FunctionTestFeatureTwoParametersFailWith: message: string -> (('featureType * 'setupType) -> TestEnvironment -> TestResult)
    
    abstract member FunctionTestTwoParametersWith: testResult: TestResult -> ('setupType -> TestEnvironment -> TestResult)
    abstract member FunctionTestTwoParametersSuccess: 'setupType -> TestEnvironment -> TestResult
    abstract member FunctionTestTwoParametersFailWith: message: string -> ('setupType -> TestEnvironment -> TestResult)
    
    abstract member FunctionTestPassThroughTwoParametersWith: testResult: TestResult -> ('featureType -> TestEnvironment -> TestResult)
    abstract member FunctionTestPassThroughTwoParametersSuccess: 'featureType -> TestEnvironment -> TestResult
    abstract member FunctionTestPassThroughTwoParametersFailsWith: message: string -> ('featureType -> TestEnvironment -> TestResult)
    
    // -- -- One Param
    abstract member FunctionTestFeatureOneParameterWith: testResult: TestResult -> (('featureType * 'setupType) -> TestResult)
    abstract member FunctionTestFeatureOneParameterSuccess: ('featureType * 'setupType) -> TestResult
    abstract member FunctionTestFeatureOneParameterFailWith: message: string -> (('featureType * 'setupType) -> TestResult)
    
    abstract member FunctionTestOneParameterWith: testResult: TestResult -> ('setupType -> TestResult)
    abstract member FunctionTestOneParameterSuccess: 'setupType -> TestResult
    abstract member FunctionTestOneParameterFailWith: message: string -> ('setupType -> TestResult)
    
    abstract member FunctionTestPassThroughOneParameterWith: testResult: TestResult -> ('featureType -> TestResult)
    abstract member FunctionTestPassThroughOneParameterSuccess: 'featureType -> TestResult
    abstract member FunctionTestPassThroughOneParameterFailsWith: message: string -> ('featureType -> TestResult)
    
    // -- Teardown
    abstract member FunctionTeardownFeatureFromSetup: Result<'featureType * 'setupType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>
    abstract member FunctionTeardownFeatureFromSetupWith: failure: SetupTeardownFailure -> (Result<'featureType * 'setupType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>)
    abstract member FunctionTeardownFeatureFromSetupFailsWith: message: string -> (Result<'featureType * 'setupType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>)
    
    abstract member FunctionTeardownPassThrough: Result<unit, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>
    abstract member FunctionTeardownPassThroughWith: failure: SetupTeardownFailure -> (Result<unit, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>)
    abstract member FunctionTeardownPassThroughFailsWith: message: string -> (Result<unit, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>)
    
    abstract member FunctionTeardownFromSetup: Result<'setupType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>
    abstract member FunctionTeardownFromSetupWith: failure: SetupTeardownFailure -> (Result<'setupType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>)
    abstract member FunctionTeardownFromSetupFailsWith: message: string -> (Result<'setupType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>)
    
let hasSetupFunctionBeenCalled (monitor: ITestMonitor<_, _, _>) =
    monitor.HasSetupFunctionBeenCalled
    
let setupFunctionParameterValues (monitor: ITestMonitor<_, _, _>) =
    monitor.SetupFunctionParameterValues
    
let numberOfTimesSetupFunctionWasCalled (monitor: ITestMonitor<_, _, _>) =
    monitor.NumberOfTimesSetupFunctionWasCalled
    
let hasTestFunctionBeenCalled (monitor: ITestMonitor<_, _, _>) =
    monitor.HasTestFunctionBeenCalled
    
let numberOfTimesTestFunctionWasCalled (monitor: ITestMonitor<_, _, _>) =
    monitor.NumberOfTimesTestFunctionWasCalled
    
let hasTestFunctionBeenCalledWithDataParameter (monitor: ITestMonitor<_, _, _>) =
    monitor.HasTestFunctionBeenCalledWithDataParameter
    
let hasTestFunctionBeenCalledWithFeatureSetupParameter (monitor: ITestMonitor<_, _, _>) =
    monitor.HasTestFunctionBeenCalledWithFeatureSetupParameter
    
let hasTestFunctionBeenCalledWithTestSetupParameter (monitor: ITestMonitor<_, _, _>) =
    monitor.HasTestFunctionBeenCalledWithTestSetupParameter
    
let testFunctionDataParameterValues (monitor: ITestMonitor<_, _, _>) =
    monitor.TestFunctionDataParameterValues
    
let testFunctionFeatureSetupParameterValues (monitor: ITestMonitor<_, _, _>) =
    monitor.TestFunctionFeatureSetupParameterValues
    
let testFunctionTestSetupParameterValues (monitor: ITestMonitor<_, _, _>) =
    monitor.TestFunctionTestSetupParameterValues
    
let testFunctionEnvironmentParameterValues (monitor: ITestMonitor<_, _, _>) =
    monitor.TestFunctionEnvironmentParameterValues
    
let hasTestFunctionBeenCalledWithEnvironmentParameter (monitor: ITestMonitor<_, _, _>) =
    monitor.HasTestFunctionBeenCalledWithEnvironmentParameter
    
let hasTeardownBeenCalled (monitor: ITestMonitor<_, _, _>) =
    monitor.HasTeardownBeenCalled
    
let teardownFunctionParameterValues (monitor: ITestMonitor<_, _, _>) =
    monitor.TeardownFunctionParameterValues
    
let numberOfTimesTeardownFunctionWasCalled (monitor: ITestMonitor<_, _, _>) =
    monitor.NumberOfTimesTeardownFunctionWasCalled
    
let verifyAllTestFunctionShouldHaveBeenCalledWithDataOf (data: 'dataType list) (monitor: ITestMonitor<'dataType, _, _>) =
    let data = data |> List.map Some
    
    monitor.TestFunctionDataParameterValues
    |> Should.BeEqualTo data
    |> withFailureComment "Incorrect test function data parameters"
    
let verifyAllSetupFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf (featureSetupValue: 'featureType) (monitor: ITestMonitor<_, 'featureType, _>) =
    let values = List.init monitor.NumberOfTimesTestFunctionWasCalled (fun _ -> featureSetupValue)
    
    monitor.SetupFunctionParameterValues
    |> Should.BeEqualTo values
    |> withFailureComment "Incorrect setup parameters"
    
let verifyAllTestFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf (featureSetupValue: 'featureType) (monitor: ITestMonitor<_, 'featureType, _>) =
    monitor
    |> testFunctionFeatureSetupParameterValues
    |> ListShould.HaveAllValuesBe (Some featureSetupValue)
    |> withFailureComment "Incorrect test function feature setup parameters"
    
let verifyAllTestFunctionShouldHaveBeenCalledWithTestSetupValueOf (testSetupValue: 'setupType) (monitor: ITestMonitor<_, _, 'setupType>) =
    monitor
    |> testFunctionTestSetupParameterValues
    |> ListShould.HaveAllValuesBe (Some testSetupValue)
    |> withFailureComment "Incorrect test function test setup parameters"
    
let verifyNoTestWasCalledWithData (monitor: ITestMonitor<_, _, _>) =
    monitor
    |> testFunctionDataParameterValues
    |> List.filter hasValue
    |> ListShould.HaveLengthOf 0
    |> withFailureComment "Test was called with data"
    
let verifyNoTestWasCalledWithAFeatureSetupValue (monitor: ITestMonitor<_, _, _>) =
    monitor
    |> testFunctionFeatureSetupParameterValues
    |> List.filter hasValue
    |> ListShould.HaveLengthOf 0
    |> withFailureComment "Test was called with feature setup value"
    
let verifyNoTestWasCalledWithATestSetupValue (monitor: ITestMonitor<_, _, _>) =
    monitor
    |> testFunctionTestSetupParameterValues
    |> List.filter hasValue
    |> ListShould.HaveLengthOf 0
    |> withFailureComment "test was called with test setup value"
    
let verifyNoTestWasCalledWithTestEnvironment (monitor: ITestMonitor<_, _, _>) =
    monitor
    |> hasTestFunctionBeenCalledWithEnvironmentParameter
    |> Should.BeFalse
    |> withFailureComment "test was called with environment variable"
    
let verifyNoSetupFunctionsShouldHaveBeenCalled (monitor: ITestMonitor<_, _, _>) =
    monitor
    |> numberOfTimesSetupFunctionWasCalled
    |> Should.BeEqualTo 0
    |> withFailureComment "Setup was called"
    
let verifyNoTeardownFunctionsShouldHaveBeenCalled (monitor: ITestMonitor<_, _, _>) =
    monitor
    |> numberOfTimesTeardownFunctionWasCalled
    |> Should.BeEqualTo 0
    |> withFailureComment "Teardown was called"
    
let verifyTeardownShouldHaveBeenCalled (monitor: ITestMonitor<_, _, _>) =
    monitor
    |> hasTeardownBeenCalled
    |> Should.BeTrue
    |> withFailureComment "Teardown was not called"

type TestMonitor<'dataType, 'featureType, 'setupType> () =
    let mutable setupParams: 'featureType list = []
    let mutable testParams: ('dataType option * ('featureType option * 'setupType option) option * TestEnvironment option) list = []
    let mutable teardownParams: (Result<'featureType option * 'setupType option, SetupTeardownFailure> * TestResult option) list = []
    
    // Verify Calls
    member _.HasSetupFunctionBeenCalled with get () = 0 < setupParams.Length
    member _.SetupFunctionWasCalledWith with get () = setupParams |> List.rev
    member _.NumberOfTimesSetupFunctionWasCalled with get () = setupParams.Length
    
    member _.HasTestFunctionBeenCalled with get () = 0 < testParams.Length
    member _.NumberOfTimesTestFunctionWasCalled with get () = testParams.Length
    
    member this.HasTestFunctionBeenCalledWithDataParameter with get () =
        let length =
            this.TestFunctionDataParameterValues
            |> List.filter hasValue
            |> List.length
            
        0 < length
        
    member this.HasTestFunctionBeenCalledWithFeatureSetupParameter with get () =
        let length =
            this.TestFunctionFeatureSetupParameterValues
            |> List.filter hasValue
            |> List.length
            
        0 < length
        
    member this.HasTestFunctionBeenCalledWithTestSetupParameter with get () =
        let length =
            this.TestFunctionTestSetupParameterValues
            |> List.filter hasValue
            |> List.length
        
        0 < length
            
    member _.TestFunctionDataParameterValues with get () =
        testParams
        |> List.map (fun (a, _, _) -> a)
        |> List.rev
        
    member _.TestFunctionFeatureSetupParameterValues with get () =
         testParams
        |> List.map (fun (_, b, _) -> b)
        |> List.filter hasValue
        |> List.map (getValue >> fst)
        |> List.rev
        
    member _.TestFunctionTestSetupParameterValues with get () =
        testParams
        |> List.map (fun (_, b, _) -> b)
        |> List.filter hasValue
        |> List.map (getValue >> snd)
        |> List.rev
        
    member _.TestFunctionEnvironmentParameterValues with get () =
        testParams
        |> List.map (fun (_, _, c) -> c)
        |> List.rev
        
    member this.HasTestFunctionBeenCalledWithEnvironmentParameter with get () =
        let length = 
            this.TestFunctionEnvironmentParameterValues
            |> List.filter hasValue
            |> List.length
            
        0 < length
        
    member _.HasTeardownBeenCalled with get () = 0 < teardownParams.Length
    member _.TeardownFunctionParameterValues with get () = teardownParams |> List.rev
    
    member _.NumberOfTimesTeardownFunctionWasCalled with get () = teardownParams.Length
    
    // Functions
    // -- Setup
    member _.FunctionSetupFeatureWith (testSetupValue: 'setupType): ('featureType -> Result<'featureType * 'setupType, SetupTeardownFailure>) =
        let setupFunction (featureValue: 'featureType) =
            setupParams <- featureValue:: setupParams
            Ok (featureValue, testSetupValue)
            
        setupFunction
    
    member _.FunctionSetupFeatureWith (failure: SetupTeardownFailure): ('featureType -> Result<'featureType * 'setupType, SetupTeardownFailure>) =
        let setupFunction (featureValue: 'featureType) =
            setupParams <- featureValue:: setupParams
            Error failure
            
        setupFunction
    
    member _.FunctionSetupWith (testSetupValue: 'setupType): ('featureType -> Result<'setupType, SetupTeardownFailure>) =
        let setupFunction (featureValue: 'featureType) =
            setupParams <- featureValue:: setupParams
            Ok testSetupValue
            
        setupFunction
        
    member _.FunctionSetupWith (failure: SetupTeardownFailure): ('featureType -> Result<'setupType, SetupTeardownFailure>) =
        let setupFunction (featureValue: 'featureType) =
            setupParams <- featureValue:: setupParams
            Error failure
            
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
        let testFunction (data: 'dataType) (featureValue: 'featureType, testSetupValue: 'setupType) (environment: TestEnvironment) =
            testParams <- (Some data, Some (Some featureValue, Some testSetupValue), Some environment)::testParams
            testResult
            
        testFunction
            
    member _.FunctionTestFeatureDataThreeParametersSuccess (data: 'dataType) (featureValue: 'featureType, testSetupValue: 'setupType) (environment: TestEnvironment) =
        testParams <- (Some data, Some (Some featureValue, Some testSetupValue), Some environment)::testParams
        TestSuccess
        
    member _.FunctionTestFeatureDataThreeParametersFailsWith message =
        let testFunction (data: 'dataType) (featureValue: 'featureType, testSetupValue: 'setupType) (environment: TestEnvironment) =
            testParams <- (Some data, Some (Some featureValue, Some testSetupValue), Some environment)::testParams
            failwith message
            
        testFunction
    
    member _.FunctionTestDataThreeParametersWith testResult =
        let testFunction (data: 'dataType) (testSetupValue: 'setupType) (environment: TestEnvironment) =
            testParams <- (Some data, Some (None, Some testSetupValue), Some environment)::testParams
            testResult
            
        testFunction
            
    member _.FunctionTestDataThreeParametersSuccess (data: 'dataType) (testSetupValue: 'setupType) (environment: TestEnvironment) =
        testParams <- (Some data, Some (None, Some testSetupValue), Some environment)::testParams
        TestSuccess
        
    member _.FunctionTestDataThreeParametersFailsWith message = 
        let testFunction (data: 'dataType) (testSetupValue: 'setupType) (environment: TestEnvironment) =
            testParams <- (Some data, Some (None, Some testSetupValue), Some environment)::testParams
            failwith message
            
        testFunction
        
    member _.FunctionTestPassThroughDataThreeParametersWith (testResult: TestResult): ('dataType -> 'featureType -> TestEnvironment -> TestResult) =
        let testFunction (data: 'dataType) (featureValue: 'featureType) (environment: TestEnvironment) =
            testParams <- (Some data, Some (Some featureValue, None), Some environment)::testParams
            testResult
            
        testFunction
        
    member _.FunctionTestPassThroughDataThreeParametersSuccess (data: 'dataType) (featureValue: 'featureType) (environment: TestEnvironment) =
        testParams <- (Some data, Some (Some featureValue, None), Some environment)::testParams
        TestSuccess
        
    member _.FunctionTestPassThroughDataThreeParametersFailsWith message =
        let testFunction (data: 'dataType) (featureValue: 'featureType) (environment: TestEnvironment) =
            testParams <- (Some data, Some (Some featureValue, None), Some environment)::testParams
            failwith message
            
        testFunction
    
    // -- -- Data Two Params
    member _.FunctionTestFeatureDataTwoParametersWith (testResult: TestResult) =
        let testFunction (data: 'dataType) (featureValue: 'featureType, testSetupValue: 'setupType) =
            testParams <- (Some data, Some (Some featureValue, Some testSetupValue), None)::testParams
            testResult
            
        testFunction
        
    member _.FunctionTestFeatureDataTwoParametersSuccess (data: 'dataType) (featureValue: 'featureType, testSetupValue: 'setupType) =
        testParams <- (Some data, Some (Some featureValue, Some testSetupValue), None)::testParams
        TestSuccess
        
    member _.FunctionTestFeatureDataTwoParametersFailsWith message =
        let testFunction (data: 'dataType) (featureValue: 'featureType, testSetupValue: 'setupType) =
            testParams <- (Some data, Some (Some featureValue, Some testSetupValue), None)::testParams
            failwith message
            
        testFunction
    
    member _.FunctionTestDataTwoParametersWith (testResult: TestResult) =
        let testFunction (data: 'dataType) (testSetupValue: 'setupType) =
            testParams <- (Some data, Some (None, Some testSetupValue), None)::testParams
            testResult
            
        testFunction
        
    member _.FunctionTestDataTwoParametersSuccess (data: 'dataType) (testSetupValue: 'setupType) =
        testParams <- (Some data, Some (None, Some testSetupValue), None)::testParams
        TestSuccess
        
    member _.FunctionTestDataTwoParametersFailsWith message =
        let testFunction (data: 'dataType) (testSetupValue: 'setupType) =
            testParams <- (Some data, Some (None, Some testSetupValue), None)::testParams
            failwith message
            
        testFunction
        
    member _.FunctionTestPassThroughDataTwoParametersWith (testResult: TestResult) =
        let testFunction (data: 'dataType) (featureValue: 'featureType) =
            testParams <- (Some data, Some (Some featureValue, None), None)::testParams
            testResult
            
        testFunction
        
    member _.FunctionTestPassThroughDataTwoParametersSuccess (data: 'dataType) (featureValue: 'featureType) =
        testParams <- (Some data, Some (Some featureValue, None), None)::testParams
        TestSuccess
        
    member _.FunctionTestPassThroughDataTwoParametersFailsWith (message: string) =
        let testFunction (data: 'dataType) (featureValue: 'featureType) =
            testParams <- (Some data, Some (Some featureValue, None), None)::testParams
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
        let testFunction (featureValue: 'featureType, testSetupValue: 'setupType) (environment: TestEnvironment) =
            testParams <- (None, Some (Some featureValue, Some testSetupValue), Some environment)::testParams
            testResult
            
        testFunction
        
    member _.FunctionTestFeatureTwoParametersSuccess (featureValue: 'featureType, testSetupValue: 'setupType) (environment: TestEnvironment) =
        testParams <- (None, Some (Some featureValue, Some testSetupValue), Some environment)::testParams
        TestSuccess
        
    member _.FunctionTestFeatureTwoParametersFailWith message =
        let testFunction (featureValue: 'featureType, testSetupValue: 'setupType) (environment: TestEnvironment) =
            testParams <- (None, Some (Some featureValue, Some testSetupValue), Some environment)::testParams
            failwith message
            
        testFunction
    
    member _.FunctionTestTwoParametersWith (testResult: TestResult) =
        let testFunction (testSetupValue: 'setupType) (environment: TestEnvironment) =
            testParams <- (None, Some (None, Some testSetupValue), Some environment)::testParams
            testResult
            
        testFunction
        
    member _.FunctionTestTwoParametersSuccess (testSetupValue: 'setupType) (environment: TestEnvironment) =
        testParams <- (None, Some (None, Some testSetupValue), Some environment)::testParams
        TestSuccess
        
    member _.FunctionTestTwoParametersFailWith message =
        let testFunction (testSetupValue: 'setupType) (environment: TestEnvironment) =
            testParams <- (None, Some (None, Some testSetupValue), Some environment)::testParams
            failwith message
            
        testFunction
        
    member _.FunctionTestPassThroughTwoParametersWith (testResult: TestResult) =
        let testFunction (featureValue: 'featureType) (environment: TestEnvironment) =
            testParams <- (None, Some (Some featureValue, None), Some environment)::testParams
            testResult
            
        testFunction
        
    member _.FunctionTestPassThroughTwoParametersSuccess (featureValue: 'featureType) (environment: TestEnvironment) =
        testParams <- (None, Some (Some featureValue, None), Some environment)::testParams
        TestSuccess
        
    member _.FunctionTestPassThroughTwoParametersFailsWith (message: string) =
        let testFunction (featureValue: 'featureType) (environment: TestEnvironment) =
            testParams <- (None, Some (Some featureValue, None), Some environment)::testParams
            failwith message
            
        testFunction
    
    // -- -- One Param
    member _.FunctionTestFeatureOneParameterWith (testResult: TestResult) =
        let testFunction (featureValue: 'featureType, testSetupValue: 'setupType) =
            testParams <- (None, Some (Some featureValue, Some testSetupValue), None)::testParams
            testResult
            
        testFunction
        
    member _.FunctionTestFeatureOneParameterSuccess (featureValue: 'featureType, testSetupValue: 'setupType) =
        testParams <- (None, Some (Some featureValue, Some testSetupValue), None)::testParams
        TestSuccess
        
    member _.FunctionTestFeatureOneParameterFailWith message =
        let testFunction (featureValue: 'featureType, testSetupValue: 'setupType) =
            testParams <- (None, Some (Some featureValue, Some testSetupValue), None)::testParams
            failwith message
            
        testFunction
    
    member _.FunctionTestOneParameterWith (testResult: TestResult) =
        let testFunction (testSetupValue: 'setupType) =
            testParams <- (None, Some (None, Some testSetupValue), None)::testParams
            testResult
            
        testFunction
            
    member _.FunctionTestOneParameterSuccess (testSetupValue: 'setupType) =
        testParams <- (None, Some (None, Some testSetupValue), None)::testParams
        TestSuccess
        
    member _.FunctionTestOneParameterFailWith message =
        let testFunction (testSetupValue: 'setupType) =
            testParams <- (None, Some (None, Some testSetupValue), None)::testParams
            failwith message
            
        testFunction
        
    member _.FunctionTestPassThroughOneParameterWith (testResult: TestResult) =
        let testFunction (featureValue: 'featureType) =
            testParams <- (None, Some (Some featureValue, None), None)::testParams
            testResult
            
        testFunction
        
    member _.FunctionTestPassThroughOneParameterSuccess (featureValue: 'featureType) =
        testParams <- (None, Some (Some featureValue, None), None)::testParams
        TestSuccess
        
    member _.FunctionTestPassThroughOneParameterFailsWith (message: string) =
        let testFunction (featureValue: 'featureType) =
            testParams <- (None, Some (Some featureValue, None), None)::testParams
            failwith message
            
        testFunction
        
    
    // -- Teardown
    member _.FunctionTeardownFeatureFromSetup (setupResult: Result<'featureType * 'setupType, SetupTeardownFailure>) (testResult: TestResult option): Result<unit, SetupTeardownFailure> =
        match setupResult with
        | Ok (featureValue, testSetupValue) ->
            teardownParams <- (Ok (Some featureValue, Some testSetupValue), testResult)::teardownParams
        | Error errorValue ->
            teardownParams <- (Error errorValue, testResult)::teardownParams
            
        Ok ()
            
    member _.FunctionTeardownFeatureFromSetupWith (failure: SetupTeardownFailure) =
        let teardownFunction (setupResult: Result<'featureType * 'setupType, SetupTeardownFailure>) (testResult: TestResult option): Result<unit, SetupTeardownFailure> =
            match setupResult with
            | Ok (featureValue, testSetupValue) ->
                teardownParams <- (Ok (Some featureValue, Some testSetupValue), testResult)::teardownParams
            | Error errorValue ->
                teardownParams <- (Error errorValue, testResult)::teardownParams
                
            Error failure
            
        teardownFunction
        
    member _.FunctionTeardownFeatureFromSetupFailWith message =
        let teardownFunction (setupResult: Result<'featureType * 'setupType, SetupTeardownFailure>) (testResult: TestResult option): Result<unit, SetupTeardownFailure> =
            match setupResult with
            | Ok (featureValue, testSetupValue) ->
                teardownParams <- (Ok (Some featureValue, Some testSetupValue), testResult)::teardownParams
            | Error errorValue ->
                teardownParams <- (Error errorValue, testResult)::teardownParams
                
            failwith message
            
        teardownFunction
    
    member _.FunctionTeardownPassThrough (setupResult: Result<unit, SetupTeardownFailure>) (testResult: TestResult option): Result<unit, SetupTeardownFailure> =
        match setupResult with
        | Ok _ ->
            teardownParams <- (Ok (None, None), testResult)::teardownParams
        | Error errorValue ->
            teardownParams <- (Error errorValue, testResult)::teardownParams
        
        Ok ()
        
    member _.FunctionTeardownPassThroughWith  (failure: SetupTeardownFailure) =
        let teardownFunction (setupResult: Result<unit, SetupTeardownFailure>) (testResult: TestResult option): Result<unit, SetupTeardownFailure> =
            match setupResult with
            | Ok _ ->
                teardownParams <- (Ok (None, None), testResult)::teardownParams
            | Error errorValue ->
                teardownParams <- (Error errorValue, testResult)::teardownParams
            
            Error failure
        
        teardownFunction
        
    member _.FunctionTeardownPassThroughFailsWith (message: string) =
        let teardownFunction (setupResult: Result<unit, SetupTeardownFailure>) (testResult: TestResult option): Result<unit, SetupTeardownFailure> =
            match setupResult with
            | Ok _ ->
                teardownParams <- (Ok (None, None), testResult)::teardownParams
            | Error errorValue ->
                teardownParams <- (Error errorValue, testResult)::teardownParams
            
            failwith message
        
        teardownFunction
    
    member _.FunctionTeardownFromSetup (setupResult: Result<'setupType, SetupTeardownFailure>) (testResult: TestResult option): Result<unit, SetupTeardownFailure> =
        match setupResult with
        | Ok testSetupValue ->
            teardownParams <- (Ok (None, Some testSetupValue), testResult)::teardownParams
        | Error errorValue ->
            teardownParams <- (Error errorValue, testResult)::teardownParams
        
        Ok ()
        
    member _.FunctionTeardownFromSetupWith (failure: SetupTeardownFailure) =
        let teardownFunction (setupResult: Result<'setupType, SetupTeardownFailure>) (testResult: TestResult option): Result<unit, SetupTeardownFailure> =
            match setupResult with
            | Ok testSetupValue ->
                teardownParams <- (Ok (None, Some testSetupValue), testResult)::teardownParams
            | Error errorValue ->
                teardownParams <- (Error errorValue, testResult)::teardownParams
            
            Error failure
            
        teardownFunction
        
    member _.FunctionTeardownFromSetupFailWith (message: string) = 
        let teardownFunction (setupResult: Result<'setupType, SetupTeardownFailure>) (testResult: TestResult option): Result<unit, SetupTeardownFailure> =
            match setupResult with
            | Ok testSetupValue ->
                teardownParams <- (Ok (None, Some testSetupValue), testResult)::teardownParams
            | Error errorValue ->
                teardownParams <- (Error errorValue, testResult)::teardownParams
            
            failwith message
            
        teardownFunction
        
    interface ITestMonitor<'dataType, 'featureType, 'setupType> with
        // Verify Calls
        member this.HasSetupFunctionBeenCalled with get () = this.HasSetupFunctionBeenCalled
        member this.SetupFunctionParameterValues with get () = this.SetupFunctionWasCalledWith
        member this.NumberOfTimesSetupFunctionWasCalled with get () = this.NumberOfTimesSetupFunctionWasCalled
        
        member this.HasTestFunctionBeenCalled with get () = this.HasTestFunctionBeenCalled
        member this.NumberOfTimesTestFunctionWasCalled with get () = this.NumberOfTimesTestFunctionWasCalled
        member this.HasTestFunctionBeenCalledWithDataParameter with get () = this.HasTestFunctionBeenCalledWithDataParameter
        member this.HasTestFunctionBeenCalledWithFeatureSetupParameter with get () = this.HasTestFunctionBeenCalledWithFeatureSetupParameter
        member this.HasTestFunctionBeenCalledWithTestSetupParameter with get () = this.HasTestFunctionBeenCalledWithTestSetupParameter
        member this.TestFunctionDataParameterValues with get () = this.TestFunctionDataParameterValues
        member this.TestFunctionFeatureSetupParameterValues with get () = this.TestFunctionFeatureSetupParameterValues
        member this.TestFunctionTestSetupParameterValues with get () = this.TestFunctionTestSetupParameterValues
        member this.TestFunctionEnvironmentParameterValues with get () = this.TestFunctionEnvironmentParameterValues
        member this.HasTestFunctionBeenCalledWithEnvironmentParameter with get () = this.HasTestFunctionBeenCalledWithEnvironmentParameter
        
        member this.HasTeardownBeenCalled with get () = this.HasTeardownBeenCalled
        member this.TeardownFunctionParameterValues with get ()  = this.TeardownFunctionParameterValues
        member this.NumberOfTimesTeardownFunctionWasCalled with get () = this.NumberOfTimesTeardownFunctionWasCalled
        
        // Functions
        // -- Setup
        member this.FunctionSetupFeatureWith (testSetupValue: 'setupType) = this.FunctionSetupFeatureWith testSetupValue
        member this.FunctionSetupFeatureWith (failure: SetupTeardownFailure) = this.FunctionSetupFeatureWith failure
        member this.FunctionSetupWith (testSetupValue: 'setupType) = this.FunctionSetupWith testSetupValue
        member this.FunctionSetupWith (failure: SetupTeardownFailure) = this.FunctionSetupWith failure
        member this.FunctionSetupFeatureFailsWith message = this.FunctionFeatureSetupFailsWith message
        member this.FunctionSetupFailsWith message = this.FunctionSetupFailsWith message
        
        // -- Test
        // -- -- Data Three Params
        member this.FunctionTestFeatureDataThreeParametersWith testResult = this.FunctionTestFeatureDataThreeParametersWith testResult
        member this.FunctionTestFeatureDataThreeParametersSuccess (dataValue: 'dataType) (featureValue: 'featureType, testSetupValue: 'setupType) (environment: TestEnvironment) = this.FunctionTestFeatureDataThreeParametersSuccess dataValue (featureValue, testSetupValue) environment
        member this.FunctionTestFeatureDataThreeParametersFailsWith message = this.FunctionTestFeatureDataThreeParametersFailsWith message
        
        member this.FunctionTestDataThreeParametersWith testResult = this.FunctionTestDataThreeParametersWith testResult
        member this.FunctionTestDataThreeParametersSuccess (data: 'dataType) (testSetupValue: 'setupType) (environment: TestEnvironment) = this.FunctionTestDataThreeParametersSuccess data testSetupValue environment
        member this.FunctionTestDataThreeParametersFailsWith message = this.FunctionTestDataThreeParametersFailsWith message
        
        member this.FunctionTestPassThroughDataThreeParametersWith testResult = this.FunctionTestPassThroughDataThreeParametersWith testResult
        member this.FunctionTestPassThroughDataThreeParametersSuccess (data: 'dataType) (featureValue: 'featureType) (environment: TestEnvironment) = this.FunctionTestPassThroughDataThreeParametersSuccess data featureValue environment
        member this.FunctionTestPassThroughDataThreeParametersFailsWith message = this.FunctionTestPassThroughDataThreeParametersFailsWith message
        
        // // -- -- Data Two Params
        member this.FunctionTestFeatureDataTwoParametersWith testResult = this.FunctionTestFeatureDataTwoParametersWith testResult
        member this.FunctionTestFeatureDataTwoParametersSuccess (dataValue: 'dataType) (featureValue: 'featureType, testSetupValue: 'setupType) = this.FunctionTestFeatureDataTwoParametersSuccess dataValue (featureValue, testSetupValue)
        member this.FunctionTestFeatureDataTwoParametersFailsWith message = this.FunctionTestFeatureDataTwoParametersFailsWith message
        
        member this.FunctionTestDataTwoParametersWith (testResult: TestResult) = this.FunctionTestDataTwoParametersWith testResult
        member this.FunctionTestDataTwoParametersSuccess (dataValue: 'dataType) (testSetupValue: 'setupType) = this.FunctionTestDataTwoParametersSuccess dataValue testSetupValue
        member this.FunctionTestDataTwoParametersFailsWith message = this.FunctionTestDataTwoParametersFailsWith message
        
        member this.FunctionTestPassThroughDataTwoParametersWith testResult = this.FunctionTestPassThroughDataTwoParametersWith testResult
        member this.FunctionTestPassThroughDataTwoParametersSuccess data featureValue = this.FunctionTestPassThroughDataTwoParametersSuccess data featureValue
        member this.FunctionTestPassThroughDataTwoParametersFailsWith message = this.FunctionTestPassThroughDataTwoParametersFailsWith message
        
        // -- -- Data One Param
        member this.FunctionTestDataOneParameterWith testResult = this.FunctionTestDataOneParameterWith testResult
        member this.FunctionTestDataOneParameterSuccess (dataValue: 'dataType)  = this.FunctionTestDataOneParameterSuccess dataValue
        member this.FunctionTestDataOneParameterFailsWith message = this.FunctionTestDataOneParameterFailsWith message
        
        // -- -- Two Params
        member this.FunctionTestFeatureTwoParametersWith testResult = this.FunctionTestFeatureTwoParametersWith testResult
        member this.FunctionTestFeatureTwoParametersSuccess (setupResult: 'featureType * 'setupType) (environment: TestEnvironment) = this.FunctionTestFeatureTwoParametersSuccess setupResult environment
        member this.FunctionTestFeatureTwoParametersFailWith message = this.FunctionTestFeatureTwoParametersFailWith message
        
        member this.FunctionTestTwoParametersWith testResult = this.FunctionTestTwoParametersWith testResult
        member this.FunctionTestTwoParametersSuccess (testSetupValue: 'setupType) (environment: TestEnvironment)  = this.FunctionTestTwoParametersSuccess testSetupValue environment
        member this.FunctionTestTwoParametersFailWith message = this.FunctionTestTwoParametersFailWith message
        
        member this.FunctionTestPassThroughTwoParametersWith testResult = this.FunctionTestPassThroughTwoParametersWith testResult
        member this.FunctionTestPassThroughTwoParametersSuccess featureValue environment = this.FunctionTestPassThroughTwoParametersSuccess featureValue environment
        member this.FunctionTestPassThroughTwoParametersFailsWith message = this.FunctionTestPassThroughTwoParametersFailsWith message
        
         // -- -- One Param
        member this.FunctionTestFeatureOneParameterWith testResult = this.FunctionTestFeatureOneParameterWith testResult
        member this.FunctionTestFeatureOneParameterSuccess (setupResult: 'featureType * 'setupType) = this.FunctionTestFeatureOneParameterSuccess setupResult
        member this.FunctionTestFeatureOneParameterFailWith message = this.FunctionTestFeatureOneParameterFailWith message
        
        member this.FunctionTestOneParameterWith testResult = this.FunctionTestOneParameterWith testResult
        member this.FunctionTestOneParameterSuccess (testSetupValue: 'setupType) = this.FunctionTestOneParameterSuccess testSetupValue
        member this.FunctionTestOneParameterFailWith message = this.FunctionTestOneParameterFailWith message
        
        member this.FunctionTestPassThroughOneParameterWith testResult = this.FunctionTestPassThroughOneParameterWith testResult
        member this.FunctionTestPassThroughOneParameterSuccess featureValue = this.FunctionTestPassThroughOneParameterSuccess featureValue
        member this.FunctionTestPassThroughOneParameterFailsWith message = this.FunctionTestPassThroughOneParameterFailsWith message
        
         // -- Teardown
        member this.FunctionTeardownFeatureFromSetup (setupResult: Result<'featureType * 'setupType, SetupTeardownFailure>)  (testResult: TestResult option) = this.FunctionTeardownFeatureFromSetup setupResult testResult
        member this.FunctionTeardownFeatureFromSetupWith (failure: SetupTeardownFailure) = this.FunctionTeardownFeatureFromSetupWith failure
        member this.FunctionTeardownFeatureFromSetupFailsWith message = this.FunctionTeardownFeatureFromSetupFailWith message
        
        member this.FunctionTeardownPassThrough (setupResult: Result<unit, SetupTeardownFailure>)  (testResult: TestResult option) = this.FunctionTeardownPassThrough setupResult testResult
        member this.FunctionTeardownPassThroughWith failure = this.FunctionTeardownPassThroughWith failure
        member this.FunctionTeardownPassThroughFailsWith message = this.FunctionTeardownPassThroughFailsWith message
        
        member this.FunctionTeardownFromSetup (setupResult: Result<'setupType, SetupTeardownFailure>) (testResult: TestResult option) = this.FunctionTeardownFromSetup setupResult testResult
        member this.FunctionTeardownFromSetupWith failure = this.FunctionTeardownFromSetupWith failure
        member this.FunctionTeardownFromSetupFailsWith message = this.FunctionTeardownFromSetupFailWith message
    
let getTestMonitor<'dataType, 'featureType, 'setupType> () =
    TestMonitor<'dataType, 'featureType, 'setupType> ()
    :> ITestMonitor<'dataType, 'featureType, 'setupType>
    
let getUnitTestMonitor () =
    getTestMonitor<unit, unit, unit> ()
    
let getFeatureMonitor<'featureType> () =
    FeatureMonitor<'featureType> ()
    :> IFeatureMonitor<'featureType>

let setupBuildExecutorWithMonitorAtTheFeature _ =
    let featureMonitor = getFeatureMonitor<unit>()
    let testMonitor = getTestMonitor<unit, unit, unit> ()
    let testSetupValue = ()
    
    let feature = Arrow.NewFeature (
        ignoreString (),
        ignoreString (),
        Setup (featureMonitor.FunctionSetupWith testSetupValue),
        Teardown (featureMonitor.FunctionTeardownWith ())
    )
    
    let test = feature.Test testMonitor.FunctionTestTwoParametersSuccess
    Ok (featureMonitor, testMonitor, test.GetExecutor ())

let setupBuildExecutorWithMonitor _ =
    let buildIt (monitor: ITestMonitor<unit, unit, 'setupType>) (setupResult: Result<'setupType, SetupTeardownFailure>) =
        let feature = Arrow.NewFeature (
            ignoreString (),
            ignoreString ()
        )
        
        
        let test =
            match setupResult with
            | Ok testSetupValue ->
                let setup = monitor.FunctionSetupWith testSetupValue
                let testBody = monitor.FunctionTestTwoParametersSuccess
                let teardown = monitor.FunctionTeardownFromSetup
                feature.Test (Setup setup, TestBody testBody, Teardown teardown)
            | Error errorValue ->
                let setup = monitor.FunctionSetupWith errorValue
                let testBody = monitor.FunctionTestTwoParametersSuccess
                let teardown = monitor.FunctionTeardownFromSetup
                feature.Test (Setup setup, TestBody testBody, Teardown teardown)
            
        test.GetExecutor ()
        
    Ok buildIt

let setupBuildExecutorWithMonitorAndTestResult _ =
    let buildIt (monitor: ITestMonitor<unit, unit, unit>) (testResult: TestResult) =
        let feature = Arrow.NewFeature (
            ignoreString (),
            ignoreString ()
        )
        
        
        let test =
            let testSetupValue = ()
            let setup = monitor.FunctionSetupWith testSetupValue
            let testBody = monitor.FunctionTestTwoParametersWith testResult
            let teardown = monitor.FunctionTeardownFromSetup
            feature.Test (Setup setup, TestBody testBody, Teardown teardown)
            
        test.GetExecutor ()
        
    Ok buildIt
   