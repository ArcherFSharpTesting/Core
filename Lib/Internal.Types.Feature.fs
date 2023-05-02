namespace Archer.Arrows.Internal.Types

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer
open Archer.Arrows
open Archer.Arrows.Internals
open Archer.CoreTypes.InternalTypes

type Feature<'featureType> (featurePath, featureName, transformer: TestInternals * ISetupTeardownExecutor<'featureType> -> ITest) =
    let mutable tests: ITest list = []
    
    let wrapTestBody (testBody: 'a -> TestResult) =
        TestWithEnvironmentBody (fun setupResult _env -> testBody setupResult)
        
    let wrapTeardown teardown =
        let (Teardown teardown) = teardown
        Teardown (fun _ -> teardown (Ok ()))
        
    interface IBuilder<'featureType, ITest> with
        member _.Add (internals: TestInternals, executor: ISetupTeardownExecutor<'featureType>) =
            let test = transformer (internals, executor)
            tests <- test::tests
            test
    
    member this.Script with get () = this :> IScriptFeature<'featureType>
    member this.Feature with get () = this :> IFeature<'featureType>
    
    member _.AddTest (test: ITest) =
        tests <- test::tests
        test
    
    // ----------------------------------------------------------------
    // -                             Test                             -
    // ----------------------------------------------------------------
    // --------- TEST TAGS (with environment) ---------
    abstract member Test: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyWithEnvironmentIndicator<'setupType> * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest 
    default this.Test      (tags: TagsIndicator,  setup: SetupIndicator<'featureType, 'setupType>,  testBody: TestBodyWithEnvironmentIndicator<'setupType>,  teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let location = getLocation fileFullName lineNumber
        
        let test =
            let TestTags tags, Setup setup, TestWithEnvironmentBody testBody, Teardown teardown = (tags, setup, testBody, teardown)
            let inner = SetupTeardownExecutor (setup, teardown, fun value env -> env |> testBody value |> TestExecutionResult) :> ISetupTeardownExecutor<'featureType>
            transformer ({ ContainerPath = featurePath; ContainerName = featureName; TestName = testName; Tags = tags; FilePath = location.FilePath; FileName = location.FileName; LineNumber = lineNumber }, inner)
        
        this.AddTest test

    member this.Ignore (tags: TagsIndicator, _setup: SetupIndicator<'featureType, 'setupType>, _testBody: TestBodyWithEnvironmentIndicator<'setupType>, _teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let location = getLocation fileFullName lineNumber
        let failure = TestIgnored (None, location) |> TestFailure
        let tb = TestWithEnvironmentBody (fun _ _ -> failure)
        this.Test (tags, Setup (fun _ -> Ok ()), tb, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
    
    member this.Test   (tags: TagsIndicator, testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (tags, Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
    
    member this.Ignore (tags: TagsIndicator, testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (tags, Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
    
    member this.Test   (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (tags, setup, testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)

    member this.Ignore (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (tags, setup, testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
            
    member this.Test   (tags: TagsIndicator, testBody: TestFunctionWithEnvironment<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (tags, Setup Ok, TestWithEnvironmentBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Ignore (tags: TagsIndicator, testBody: TestFunctionWithEnvironment<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (tags, Setup Ok, TestWithEnvironmentBody testBody, testName, fileFullName, lineNumber)
        
    member this.Test   (tags: TagsIndicator, testBody: TestBodyWithEnvironmentIndicator<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let (TestWithEnvironmentBody testBody) = testBody
        this.Test (tags, testBody, testName, fileFullName, lineNumber)
        
    member this.Ignore (tags: TagsIndicator, testBody: TestBodyWithEnvironmentIndicator<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let (TestWithEnvironmentBody testBody) = testBody
        this.Ignore (tags, testBody, testName, fileFullName, lineNumber)
        
    // --------- TEST NAME / Tags (with environment) ---------
    member this.Test   (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (tags, setup, testBody, teardown, testName, fileFullName, lineNumber)
        
    member this.Ignore (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (tags, setup, testBody, teardown, testName, fileFullName, lineNumber)
    
    member this.Test   (testName: string, tags: TagsIndicator, testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (tags, Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
        
    member this.Ignore (testName: string, tags: TagsIndicator, testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (tags, testBody, teardown, testName, fileFullName, lineNumber)
        
    member this.Test   (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (tags, setup, testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Ignore (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (tags, setup, testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Test   (testName: string, tags: TagsIndicator, testBody: TestFunctionWithEnvironment<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (tags, Setup Ok, TestWithEnvironmentBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Ignore (testName: string, tags: TagsIndicator, testBody: TestFunctionWithEnvironment<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (tags, testBody, testName, fileFullName, lineNumber)
    
    // --------- TEST TAGS (without environment) ---------
    member this.Test  (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
       let (TestBody testBody) = testBody
       this.Test (tags, setup, wrapTestBody testBody, teardown, testName, fileFullName, lineNumber)
       
    member this.Ignore (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
       let (TestBody testBody) = testBody
       this.Ignore (tags, setup, wrapTestBody testBody, teardown, testName, fileFullName, lineNumber)
       
    member this.Test   (testName: string, tags: TagsIndicator, TestBody testBody, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (tags, Setup (fun _ -> Ok ()), wrapTestBody testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
        
    member this.Ignore (testName: string, tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let (TestBody testBody) = testBody
        this.Ignore (tags, Setup Ok, wrapTestBody testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
        
    member this.Test   (tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let (TestBody testBody) = testBody
        this.Test (tags, Setup Ok, wrapTestBody testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
        
    member this.Ignore (tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let (TestBody testBody) = testBody
        this.Ignore (tags, Setup Ok, wrapTestBody testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
    
    member this.Test   (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let (TestBody testBody) = testBody
        this.Test (tags, setup, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Ignore (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let (TestBody testBody) = testBody
        this.Ignore (tags, setup, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Test   (tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let (TestBody testBody) = testBody
        this.Test (tags, Setup Ok, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Ignore (tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let (TestBody testBody) = testBody
        this.Ignore (tags, Setup Ok, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Test   (tags: TagsIndicator, testBody: TestFunction<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (tags, Setup Ok, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Ignore (tags: TagsIndicator, testBody: TestFunction<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (tags, Setup Ok, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    // --------- TEST NAME / TAGS (without environment) ---------
    member this.Test   (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let (TestBody testBody) = testBody
        this.Test (tags, setup, wrapTestBody testBody, teardown, testName, fileFullName, lineNumber)
        
    member this.Ignore (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let (TestBody testBody) = testBody
        this.Ignore (tags, setup, wrapTestBody testBody, teardown, testName, fileFullName, lineNumber)
        
    member this.Test   (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let (TestBody testBody) = testBody
        this.Test (tags, setup, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Ignore (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let (TestBody testBody) = testBody
        this.Ignore (tags, setup, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Test   (testName: string, tags: TagsIndicator, testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (tags, Setup Ok, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Ignore (testName: string, tags: TagsIndicator, testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (tags, Setup Ok, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Test   (testName: string, tags: TagsIndicator, testBody: TestFunction<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (tags, Setup Ok, wrapTestBody testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
        
    member this.Ignore (testName: string, tags: TagsIndicator, testBody: TestFunction<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (tags, Setup Ok, wrapTestBody testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
    // --------- SET UP (with environment) ---------
    member this.Test   (setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (TestTags [], setup, testBody, teardown, testName, fileFullName, lineNumber)
        
    member this.Ignore (setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (TestTags [], setup, testBody, teardown, testName, fileFullName, lineNumber)
    
    member this.Test   (setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (TestTags [], setup, testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Ignore (setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (TestTags [], setup, testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
    
    // --------- TEST NAME / SET UP (with environment) ---------
    member this.Test   (testName: string, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (TestTags [], setup, testBody, teardown, testName, fileFullName, lineNumber)
        
    member this.Ignore (testName: string, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (TestTags [], setup, testBody, teardown, testName, fileFullName, lineNumber)
    
    member this.Test   (testName: string, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (TestTags [], setup, testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Ignore (testName: string, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (TestTags [], setup, testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
    
    // --------- SET UP (without environment) ---------
    member this.Test   (setup: SetupIndicator<'featureType, 'setupType>, TestBody testBody, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (TestTags [], setup, wrapTestBody testBody, teardown, testName, fileFullName, lineNumber)
        
    member this.Ignore (setup: SetupIndicator<'featureType, 'setupType>, TestBody testBody, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (TestTags [], setup, wrapTestBody testBody, teardown, testName, fileFullName, lineNumber)
        
    member this.Test   (setup: SetupIndicator<'featureType, 'setupType>, TestBody testBody, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (TestTags [], setup, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Ignore (setup: SetupIndicator<'featureType, 'setupType>, TestBody testBody, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (TestTags [], setup, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    // --------- TEST NAME / SET UP (without environment) ---------
    member this.Test   (testName: string, setup: SetupIndicator<'featureType, 'setupType>, TestBody testBody, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (TestTags [], setup, wrapTestBody testBody, teardown, testName, fileFullName, lineNumber)
        
    member this.Ignore (testName: string, setup: SetupIndicator<'featureType, 'setupType>, TestBody testBody, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (TestTags [], setup, wrapTestBody testBody, teardown, testName, fileFullName, lineNumber)
        
    member this.Test   (testName: string, setup: SetupIndicator<'featureType, 'setupType>, TestBody testBody, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (TestTags [], setup, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Ignore (testName: string, setup: SetupIndicator<'featureType, 'setupType>, TestBody testBody, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (TestTags [], setup, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    // --------- TEST BODY (with environment)---------
    member this.Test   (testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (TestTags [], Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
        
    member this.Ignore (testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (TestTags [], Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
    
    member this.Test   (testBody: TestFunctionWithEnvironment<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (TestTags [], Setup Ok, TestWithEnvironmentBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Ignore (testBody: TestFunctionWithEnvironment<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (TestTags [], Setup Ok, TestWithEnvironmentBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
            
    // --------- TEST NAME / TEST BODY (with environment)---------
    member this.Test   (testName: string, testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (TestTags [], Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
        
    member this.Ignore (testName: string, testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (TestTags [], Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
        
    member this.Test   (testName: string, testBody: TestFunctionWithEnvironment<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (TestTags [], Setup Ok, TestWithEnvironmentBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Ignore (testName: string, testBody: TestFunctionWithEnvironment<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (TestTags [], Setup Ok, TestWithEnvironmentBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
            
    // --------- TEST BODY (without environment)---------
    member this.Test   (testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let (TestBody testBody) = testBody
        this.Test (TestTags [], Setup Ok, wrapTestBody testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
        
    member this.Ignore (testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let (TestBody testBody) = testBody
        this.Ignore (TestTags [], Setup Ok, wrapTestBody testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
        
    member this.Test   (testBody: TestFunction<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (TestTags [], Setup Ok, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Ignore (testBody: TestFunction<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (TestTags [], Setup Ok, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
            
    // --------- TEST NAME / TEST BODY (without environment)---------
    member this.Test   (testName: string, testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let (TestBody testBody) = testBody
        this.Test (TestTags [], Setup Ok, wrapTestBody testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
        
    member this.Ignore (testName: string, testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let (TestBody testBody) = testBody
        this.Ignore (TestTags [], Setup Ok, wrapTestBody testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
    member this.Test   (testName: string, testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Test (TestTags [], Setup Ok, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
    member this.Ignore (testName: string, testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        this.Ignore (TestTags [], Setup Ok, wrapTestBody testBody, Teardown (fun _ _ -> Ok ()), testName, fileFullName, lineNumber)
        
        
    interface IFeature<'featureType> with
        member this.Ignore<'setupType> (tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(tags, setup, testBody, teardown,  testName,  fileFullName,  lineNumber)
            
        member this.Ignore (tags: TagsIndicator, testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(tags, testBody, teardown,  testName,  fileFullName,  lineNumber)
            
        member this.Ignore<'setupType> (tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(tags, setup, testBody,  testName,  fileFullName,  lineNumber)
            
        member this.Ignore (tags: TagsIndicator, testBody: TestFunctionWithEnvironment<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(tags, testBody,  testName,  fileFullName,  lineNumber)
            
        member this.Ignore<'setupType> (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testName, tags, setup, testBody, teardown,  fileFullName,  lineNumber)
            
        member this.Ignore (testName: string, tags: TagsIndicator, testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testName, tags, testBody, teardown,  fileFullName,  lineNumber)
            
        member this.Ignore<'setupType> (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testName, tags, setup, testBody,  fileFullName,  lineNumber)
            
        member this.Ignore (testName: string, tags: TagsIndicator, testBody: TestFunctionWithEnvironment<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testName, tags, testBody,  fileFullName,  lineNumber)
            
        member this.Ignore (testName: string, tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testName, tags, testBody, teardown,  fileFullName,  lineNumber)
            
        member this.Ignore (tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(tags, testBody, teardown,  testName,  fileFullName,  lineNumber)
            
        member this.Ignore<'setupType> (tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(tags, setup, testBody,  testName,  fileFullName,  lineNumber)
            
        member this.Ignore<'setupType> (tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(tags, setup, testBody, teardown,  testName,  fileFullName,  lineNumber)
            
        member this.Ignore (tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(tags, testBody,  testName,  fileFullName,  lineNumber)
            
        member this.Ignore<'setupType> (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testName, tags, setup, testBody, teardown,  fileFullName,  lineNumber)
            
        member this.Ignore<'setupType> (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testName, tags, setup, testBody,  fileFullName,  lineNumber)
            
        member this.Ignore (testName: string, tags: TagsIndicator, testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testName, tags, testBody,  fileFullName,  lineNumber)
            
        member this.Ignore (testName: string, tags: TagsIndicator, testBody: TestFunction<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testName, tags, testBody, teardown,  fileFullName,  lineNumber)
            
        member this.Ignore<'setupType> (setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(setup, testBody, teardown,  testName,  fileFullName,  lineNumber)
            
        member this.Ignore<'setupType> (setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(setup, testBody,  testName,  fileFullName,  lineNumber)
            
        member this.Ignore<'setupType> (testName: string, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testName, setup, testBody, teardown,  fileFullName,  lineNumber)
            
        member this.Ignore<'setupType> (testName: string, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testName, setup, testBody,  fileFullName,  lineNumber)
            
        member this.Ignore<'setupType> (setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(setup, testBody, teardown,  testName,  fileFullName,  lineNumber)
            
        member this.Ignore<'setupType> (setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(setup, testBody,  testName,  fileFullName,  lineNumber)
            
        member this.Ignore<'setupType> (testName: string, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testName, setup, testBody, teardown,  fileFullName,  lineNumber)
            
        member this.Ignore<'setupType> (testName: string, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testName, setup, testBody,  fileFullName,  lineNumber)
            
        member this.Ignore (testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testBody, teardown,  testName,  fileFullName,  lineNumber)
            
        member this.Ignore (testBody: TestFunctionWithEnvironment<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testBody,  testName,  fileFullName,  lineNumber)
            
        member this.Ignore (testName: string, testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testName, testBody, teardown,  fileFullName,  lineNumber)
            
        member this.Ignore (testName: string, testBody: TestFunctionWithEnvironment<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testName, testBody,  fileFullName,  lineNumber)
            
        member this.Ignore (testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testBody, teardown,  testName,  fileFullName,  lineNumber)
            
        member this.Ignore (testBody: TestFunction<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testBody,  testName,  fileFullName,  lineNumber)
            
        member this.Ignore (testName: string, testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testName, testBody, teardown,  fileFullName,  lineNumber)
            
        member this.Ignore (testName: string, testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Ignore(testName, testBody,  fileFullName,  lineNumber)
            
        member this.Test<'setupType> (tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Test(tags, setup, testBody, teardown,  testName,  fileFullName,  lineNumber)
            
        member this.Test (tags: TagsIndicator, testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Test(tags, testBody, teardown,  testName,  fileFullName,  lineNumber)
            
        member this.Test<'setupType> (tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Test(tags, setup, testBody,  testName,  fileFullName,  lineNumber)
            
        member this.Test (tags: TagsIndicator, testBody: TestFunctionWithEnvironment<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Test(tags, testBody,  testName,  fileFullName,  lineNumber)
            
        member this.Test<'setupType> (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Test(testName, tags, setup, testBody, teardown,  fileFullName,  lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Test(testName, tags, testBody, teardown,  fileFullName,  lineNumber)
            
        member this.Test<'setupType> (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Test (testName, tags, setup, testBody,  fileFullName,  lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, testBody: TestFunctionWithEnvironment<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Test(testName, tags, testBody, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest =
            let (TestBody testBody) = testBody
            this.Test(testName, tags, wrapTestBody testBody, teardown, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Test(tags, testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test<'setupType> (tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Test(tags, setup, testBody, testName, fileFullName, lineNumber)
            
        member this.Test<'setupType> (tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Test(tags, setup, testBody, teardown, testName, fileFullName, lineNumber)
        member this.Test (tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Test(tags, testBody, testName, fileFullName, lineNumber)
            
        member this.Test<'setupType> (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int):ITest = 
            this.Test(testName, tags, setup, testBody, teardown, fileFullName, lineNumber)
            
        member this.Test<'setupType>(testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (testName, tags, setup, testBody, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (testName, tags, testBody, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, testBody: TestFunction<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (testName, tags, testBody, teardown, fileFullName, lineNumber)
            
        member this.Test<'setupType>(setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (setup, testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test<'setupType>(setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (setup, testBody, testName, fileFullName, lineNumber)
            
        member this.Test<'setupType>(testName: string, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (testName, setup, testBody, teardown, fileFullName, lineNumber)
            
        member this.Test<'setupType>(testName: string, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (testName, setup, testBody, fileFullName, lineNumber)
            
        member this.Test<'setupType>(setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (setup, testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test<'setupType>(setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (setup, testBody, testName, fileFullName, lineNumber)
            
        member this.Test<'setupType>(testName: string, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (testName, setup, testBody, teardown, fileFullName, lineNumber)
            
        member this.Test<'setupType>(testName: string, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (testName, setup, testBody, fileFullName, lineNumber)
            
        member this.Test (testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testBody: TestFunctionWithEnvironment<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (testBody, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (testName, testBody, teardown, fileFullName, lineNumber)
            
        member this.Test (testName: string, testBody: TestFunctionWithEnvironment<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (testName, testBody, fileFullName, lineNumber)
            
        member this.Test (testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (testBody, teardown, testName, fileFullName, lineNumber)
        member this.Test (testBody: TestFunction<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (testBody, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (testName, testBody, teardown, fileFullName, lineNumber)
            
        member this.Test (testName: string, testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (testName, testBody, fileFullName, lineNumber)
        
        member this.Test    (tags: TagsIndicator, testBody: TestBodyWithEnvironmentIndicator<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Test (tags, testBody, testName, fileFullName, lineNumber)
            
        member this. Ignore (tags: TagsIndicator, testBody: TestBodyWithEnvironmentIndicator<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): ITest =
            this.Ignore (tags, testBody, testName, fileFullName, lineNumber)
    
    
        member this.GetTests() = this.GetTests ()
        
    // ----------------------------------------------------------------
    // -                          isTestedBy                          - 
    // ----------------------------------------------------------------
    interface IScriptFeature<'featureType> with
        member this.isTestedBy (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): (string -> unit) =
            let buildTest (testName: string) =
                this.Test (tags, setup, testBody, teardown, testName, fileFullName, lineNumber)
                |> ignore
                
            buildTest

        member this.isTestedBy (tags: TagsIndicator, setup: SetupIndicator<_, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.isTestedBy (tags, setup, testBody, Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
        
        member this.isTestedBy (tags: TagsIndicator, testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.isTestedBy (tags, Setup Ok, testBody, wrapTeardown teardown, fileFullName, lineNumber)
        
        member this.isTestedBy (tags: TagsIndicator, testBody: TestBodyWithEnvironmentIndicator<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.isTestedBy (tags, Setup Ok, testBody, Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
            
        // --------- isTestedBy - Test Tags (without environment) ---------
        member this.isTestedBy (tags: TagsIndicator, setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let testBody =
               match testBody with
               | TestBody f -> f
               
            this.Script.isTestedBy (tags, setup, wrapTestBody testBody, teardown, fileFullName, lineNumber)
            
        member this.isTestedBy (tags: TagsIndicator, setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.isTestedBy (tags, setup, testBody, Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
        
        member this.isTestedBy (tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.isTestedBy (tags, Setup Ok, testBody, wrapTeardown teardown, fileFullName, lineNumber)
        
        member this.isTestedBy (tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.isTestedBy (tags, Setup Ok, testBody, Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
            
        // --------- isTestedBy - Setup (with environment) ---------
        member this.isTestedBy (setup: SetupIndicator<_, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.isTestedBy (TestTags [], setup, testBody, teardown, fileFullName, lineNumber)
            
        member this.isTestedBy (setup: SetupIndicator<_, 'setupType>, testBody: TestBodyWithEnvironmentIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.isTestedBy (setup, testBody, Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
        
        // --------- isTestedBy - Setup (without environment) ---------
        member this.isTestedBy (setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let testBody =
                match testBody with
                | TestBody f -> f
                
            this.Script.isTestedBy (setup, wrapTestBody testBody, teardown, fileFullName, lineNumber)
        
        member this.isTestedBy (setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.isTestedBy (setup, testBody, Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
        
        // --------- isTestedBy - TEST BODY (with environment) ---------
        member this.isTestedBy (testBody: TestBodyWithEnvironmentIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.isTestedBy (Setup Ok, testBody, wrapTeardown teardown, fileFullName, lineNumber)
            
        member this.isTestedBy (testBody: TestBodyWithEnvironmentIndicator<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.isTestedBy (testBody, Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
            
        member this.isTestedBy (testBody: TestFunctionWithEnvironment<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.isTestedBy (TestWithEnvironmentBody testBody, fileFullName, lineNumber)
        
        // --------- isTestedBy - TEST BODY (without environment) ---------
        member this.isTestedBy (testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.isTestedBy (Setup Ok, testBody, wrapTeardown teardown, fileFullName, lineNumber)
            
        member this.isTestedBy (testBody: TestBodyIndicator<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.isTestedBy (testBody, Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
            
        member this.isTestedBy (testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.isTestedBy (TestBody testBody, fileFullName, lineNumber)

        
    member this.GetTests () = tests
    
    override _.ToString () =
        [
            featurePath
            featureName
        ]
        |> List.filter (String.IsNullOrWhiteSpace >> not)
        |> fun items -> String.Join (".", items)
        
type IgnoreFeature<'featureType> (featurePath, featureName, transformer: TestInternals * ISetupTeardownExecutor<'featureType> -> ITest, featureLocation: CodeLocation) =
    inherit Feature<'featureType> (featurePath, featureName, transformer)
    override this.Test (tags: TagsIndicator,  _setup: SetupIndicator<'featureType, 'setupType>,  _testBody: TestBodyWithEnvironmentIndicator<'setupType>,  _teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let location = getLocation fileFullName lineNumber
        
        let testBody _ _ = TestIgnored (None, featureLocation) |> TestFailure
        
        let test =
            let (TestTags tags) = tags
            let inner = SetupTeardownExecutor (Ok, (fun _ _ -> Ok ()), fun value env -> env |> testBody value |> TestExecutionResult) :> ISetupTeardownExecutor<'featureType>
            transformer ({ ContainerPath = featurePath; ContainerName = featureName; TestName = testName; Tags = tags; FilePath = location.FilePath; FileName = location.FileName; LineNumber = lineNumber }, inner)
        
        this.AddTest test