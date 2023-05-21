namespace Archer.Arrows.Internal.Types

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer
open Archer.Arrows
open Archer.Arrows.Internals
open Archer.CoreTypes.InternalTypes

type Feature<'featureType> (featurePath, featureName, featureTags: TestTag list, transformer: TestInternals * ISetupTeardownExecutor<'featureType> -> ITest) =
    let mutable tests: ITest list = []
    
    let testBodyOneToTwo (testBody: 'a -> TestResult) =
        TestBody (fun a _b -> testBody a)
        
    let testBodyOneToTwoEnvironment (testBody: 'a -> TestResult) =
        TestBody (fun _a b -> testBody b)
    
    let testBodyOneToThree (testBody: 'a -> TestResult) =
        TestBody (fun a _b _c -> testBody a)
    
    let testBodyTwoToThreeIgnoreSetup (testBody: 'a -> 'b -> TestResult) =
        TestBody (fun a _b c -> testBody a c)
    
    let testBodyTwoToThreeIgnoreEnv (testBody: 'a -> 'b -> TestResult) =
        TestBody (fun a b _c -> testBody a b)
        
    let wrapTeardown teardown =
        let (Teardown teardown) = teardown
        Teardown (fun _ -> teardown (Ok ()))
        
    let buildDataTests (converter: 'dataType -> 'testBodyTypeA -> 'testBodyTypeB) (ctor: TagsIndicator * SetupIndicator<'featureType,'setupType> * 'testBodyTypeB * TeardownIndicator<'setupType> * string * string * int -> ITest) (tags: TagsIndicator) (setup: SetupIndicator<'featureType, 'setupType>) (data: DataIndicator<'dataType>) (testBody: 'testBodyTypeA) (teardown: TeardownIndicator<'setupType>) (testName: string) (fileFullName: string) (lineNumber: int) =
        let names = System.Collections.Generic.Dictionary<string, int>()
        let getFixedName name =
            if names.ContainsKey name |> not then
                names.Add (name, 1)
                name
            else
                let c = names[name]
                names[name] <- c + 1
                $"%s{name}^%i{c}"
                
        let testNameFormat =
            let tn = 
                let regexPattern = @"(^|[^%])%(\d+-)?\d*[A-z]"
                let regex = System.Text.RegularExpressions.Regex regexPattern
                if regex.IsMatch testName then testName
                else $"%s{testName} (%%A)"
            Printf.StringFormat<'dataType -> string> tn
            
        let getTestName input =
            let name = sprintf testNameFormat input
            getFixedName name
            
        let (Data data) = data
        
        data
        |> Seq.map (fun datum ->
            ctor (tags, setup, (converter datum testBody), teardown, getTestName datum, fileFullName, lineNumber)
        )
        |> List.ofSeq
        
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
        
    abstract member Test: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>> * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest 
    default this.Test (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let location = getLocation fileFullName lineNumber
        
        let test =
            let TestTags tags, Setup setup, TestBody testBody, Teardown teardown = (tags, setup, testBody, teardown)
            let inner = SetupTeardownExecutor (setup, teardown, fun value env -> env |> testBody value |> TestExecutionResult) :> ISetupTeardownExecutor<'featureType>
            transformer ({ ContainerPath = featurePath; ContainerName = featureName; TestName = testName; Tags = [tags; featureTags] |> List.concat; FilePath = location.FilePath; FileName = location.FileName; LineNumber = lineNumber }, inner)
        
        this.AddTest test

    abstract member Ignore: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: 'testBodyType * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest 
    default this.Ignore (tags: TagsIndicator, _setup: SetupIndicator<'featureType, 'setupType>, _testBody: 'testBodyType, _teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let location = getLocation fileFullName lineNumber
        let failure = TestIgnored (None, location) |> TestFailure
        let tb = TestBody (fun _ _ -> failure)
        this.Test (tags, Setup (fun _ -> Ok ()), tb, emptyTeardown, testName, fileFullName, lineNumber)
        
    abstract member IsTestedBy: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>> * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    default this.IsTestedBy (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let buildTest (testName: string) =
            this.Test (tags, setup, testBody, teardown, testName, fileFullName, lineNumber)
            |> ignore
            
        buildTest
        
    abstract member IsIgnored: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: 'testBodyType * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    default this.IsIgnored (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, _testBody: 'testBodyType, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let buildTest (testName: string) =
            this.Ignore (tags, setup, (), teardown, testName, fileFullName, lineNumber)
            |> ignore
            
        buildTest
        
    abstract member Test:   tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'setupType, TestEnvironment>> * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    default this.Test (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'setupType, TestEnvironment>>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let converter (datum: 'dataType) (TestBody testBody) =
            TestBody (testBody datum)
            
        buildDataTests converter this.Test tags setup data testBody teardown testName fileFullName lineNumber
        
    abstract member Ignore:   tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: 'testBodyType * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    default this.Ignore (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: 'testBodyType, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let converter _ = ignore

        buildDataTests converter this.Ignore tags setup data testBody teardown testName fileFullName lineNumber
        
    interface IFeature<'featureType> with
        member _.FeatureTags with get () = featureTags
        
        //-----------------------------------------------------//
        //                    Test Builders                    //
        //-----------------------------------------------------//
        
        // -- TestName, tags, setup, data, test body indicators, teardown
        member this.Test (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'setupType, TestEnvironment>>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, setup, data, testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'setupType>>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, setup, data, testBodyTwoToThreeIgnoreEnv testBody, teardown, testName, fileFullName, lineNumber)
            
        // -- test name, tags, setup, data, test body indicator
        member this.Test (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'setupType, TestEnvironment>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, setup, data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'setupType>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, setup, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- test name, tags, setup test body indicator, teardown
        member this.Test (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, setup, testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<TestFunction<'setupType>>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, setup, testBodyOneToTwo testBody, teardown, testName, fileFullName, lineNumber)
            
        // -- test name, tags, setup test body indicator
        member this.Test (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, setup, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<TestFunction<'setupType>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, setup, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- test name, tags, data, test body indicator, tear down
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'featureType>>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunction<'dataType>>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyOneToThree testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)

        // -- test name, tags, data, test body indicator
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'featureType>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunction<'dataType>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)

        // -- test name, tags, data, test function
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, TestBody testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunctionTwoParameters<'dataType, 'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunction<'dataType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)
        
        // -- test name, tags, test body indicator, teardown
        member this.Test (testName: string, tags: TagsIndicator, testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
           
        member this.Test (testName: string, tags: TagsIndicator, testBody: TestBodyIndicator<TestFunction<'featureType>>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, testBodyOneToTwo testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)

        // -- test name, tags, test body indicator
        member this.Test (testName: string, tags: TagsIndicator, testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, testBody: TestBodyIndicator<TestFunction<'featureType>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- test name, tags, test function
        member this.Test (testName: string, tags: TagsIndicator, testBody: TestFunctionTwoParameters<'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, TestBody testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)

        // -- test name, setup, data, test body indicator, teardown
        member this.Test (testName: string, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'setupType, TestEnvironment>>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], setup, data, testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'setupType>>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], setup, data, testBodyTwoToThreeIgnoreEnv testBody, teardown, testName, fileFullName, lineNumber)

        // -- test name, setup, data, test body indicator
        member this.Test (testName: string, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'setupType, TestEnvironment>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], setup, data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'setupType>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], setup, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)

        // -- test name, setup, test body indicator, teardown
        member this.Test (testName: string, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], setup, testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<TestFunction<'setupType>>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], setup, testBodyOneToTwo testBody, teardown, testName, fileFullName, lineNumber)
            
        // -- test name, setup, test body indicator
        member this.Test (testName: string, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], setup, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<TestFunction<'setupType>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], setup, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)

        // -- test name, data, test body indicator, teardown
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'featureType>>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunction<'dataType>>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyOneToThree testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        // -- test name, data, test body indicator
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'featureType>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunction<'dataType>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- test name, data, test function
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, TestBody testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestFunctionTwoParameters<'dataType, 'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestFunction<'dataType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)

        // -- test name, test body indicator, teardown
        member this.Test (testName: string, testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)

        member this.Test (testName: string, testBody: TestBodyIndicator<TestFunction<'featureType>>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, testBodyOneToTwo testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        // -- test name, test body indicator
        member this.Test (testName: string, testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, TestBody testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, testBody: TestBodyIndicator<TestFunction<'featureType>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- test name, test function
        member this.Test (testName: string, testBody: TestFunctionTwoParameters<'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, TestBody testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
        
        // -- tags, setup, data, test body indicator, teardown
        member this.Test (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'setupType, TestEnvironment>>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, setup, data, testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'setupType>>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, setup, data, testBodyTwoToThreeIgnoreEnv testBody, teardown, testName, fileFullName, lineNumber)

        // -- tags, setup, data, test body indicator
        member this.Test (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'setupType, TestEnvironment>>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, setup, data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'setupType>>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, setup, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)

        // -- tags, setup, test body indicator teardown
        member this.Test (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test(tags, setup, testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<TestFunction<'setupType>>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
           let (TestBody testBody) = testBody
           this.Test (tags, setup, testBodyOneToTwo testBody, teardown, testName, fileFullName, lineNumber)
        
        // -- tags, setup, test body indicator
        member this.Test (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, setup, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<TestFunction<'setupType>>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, setup, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
                
        // -- tags, data, test body indicator, teardown
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'featureType>>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunction<'dataType>>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyOneToThree testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        // -- tags, data, test body indicator
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok,data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'featureType>>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunction<'dataType>>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- tags, data, test function
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, TestBody testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunctionTwoParameters<'dataType, 'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunction<'dataType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)
        
        // -- tags, test body indicator, teardown
        member this.Test (tags: TagsIndicator, testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, testBody: TestBodyIndicator<TestFunction<'featureType>>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, testBodyOneToTwo testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        // -- tags, test body indicator
        member this.Test (tags: TagsIndicator, testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Feature.Test (tags, testBody, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, testBody: TestBodyIndicator<TestFunction<'featureType>>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
        
        // -- tags, test function
        member this.Test (tags: TagsIndicator, testBody: TestFunctionTwoParameters<'featureType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, TestBody testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, testBody: TestFunction<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- setup, data, test body indicator, teardown
        member this.Test (setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'setupType, TestEnvironment>>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], setup, data, testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test (setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'setupType>>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], setup, data, testBodyTwoToThreeIgnoreEnv testBody, teardown, testName, fileFullName, lineNumber)
            
        // -- setup, data, test body indicator
        member this.Test (setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'setupType, TestEnvironment>>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], setup, data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'setupType>>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], setup, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- setup, test body indicator, teardown
        member this.Test (setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], setup, testBody, teardown, testName, fileFullName, lineNumber)
        
        member this.Test (setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<TestFunction<'setupType>>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], setup, testBodyOneToTwo testBody, teardown, testName, fileFullName, lineNumber)
        
        // -- setup, test body indicator
        member this.Test (setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], setup, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<TestFunction<'setupType>>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], setup, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- data, test body indicator teardown
        member this.Test (data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'featureType>>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunction<'dataType>>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyOneToThree testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        // -- data, test body indicator
        member this.Test (data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'featureType>>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (data: DataIndicator<'dataType>, testBody: TestBodyIndicator<TestFunction<'dataType>>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- data, test function
        member this.Test (data: DataIndicator<'dataType>, testBody: TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, TestBody testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (data: DataIndicator<'dataType>, testBody: TestFunctionTwoParameters<'dataType, 'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (data: DataIndicator<'dataType>, testBody: TestFunction<'dataType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)

        // -- test body indicator, teardown
        member this.Test (testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testBody: TestBodyIndicator<TestFunction<'featureType>>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, testBodyOneToTwo testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        // -- test body indicator
        member this.Test (testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testBody: TestBodyIndicator<TestFunction<'featureType>>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- test function
        member this.Test (testBody: TestFunctionTwoParameters<'featureType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, TestBody testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testBody: TestFunction<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        //-------------------------------------------------------//
        //                    Ignore Builders                    //
        //-------------------------------------------------------//
        
        // -- test name, tags, setup, data, test body, (teardown)
        member this.Ignore (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, data: DataIndicator<'dataType>, _testBody: 'testBodyType, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, setup, data, (), teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, _testBody: 'testBodyType, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, setup, data, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- test name, tags, setup, data, (teardown)
        member this.Ignore (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, setup, data, (), teardown, testName, fileFullName, lineNumber)
        
        member this.Ignore (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, setup, data, (), emptyTeardown, testName, fileFullName, lineNumber)
    
        // -- test name, tags, setup, test body, (teardown)
        member this.Ignore (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, _testBody: 'testBodyType, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, setup, (), teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, _testBody: 'testBodyType, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, setup, (), emptyTeardown, testName, fileFullName, lineNumber)
            
            // -- test name, tags, setup, (teardown)
        member this.Ignore (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, _teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, setup, (), emptyTeardown, testName, fileFullName, lineNumber)
    
        member this.Ignore (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, setup, (), emptyTeardown, testName, fileFullName, lineNumber)
        
        // -- test name, tags, data, test body, (teardown)
        member this.Ignore (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, _testBody: 'testBodyType, _teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, data, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, _testBody: 'testBodyType, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, data, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- test name, tags, data, (teardown)
        member this.Ignore (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, _teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, data, (), emptyTeardown, testName, fileFullName, lineNumber)
        
        member this.Ignore (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, data, (), emptyTeardown, testName, fileFullName, lineNumber)
        
        // -- test name, tags, test body, (teardown)
        member this.Ignore (testName: string, tags: TagsIndicator, _testBody: 'testBodyType, _teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, tags: TagsIndicator, _testBody: 'testBodyType, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, (), emptyTeardown, testName, fileFullName, lineNumber)
            
    
        // -- test name, tags, (teardown)
        member this.Ignore (testName: string, tags: TagsIndicator, _teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, tags: TagsIndicator, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, (), emptyTeardown, testName, fileFullName, lineNumber)

        // -- test name, setup, data, test body, (teardown)
        member this.Ignore (testName: string, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, _testBody: 'testBodyType, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], setup, data, (), teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, _testBody: 'testBodyType, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], setup, data, (), emptyTeardown, testName, fileFullName, lineNumber)

        // -- test name, setup, data, (teardown)
        member this.Ignore (testName: string, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], setup, data, (), teardown, testName, fileFullName, lineNumber)
        
        member this.Ignore (testName: string, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], setup, data, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- test name, setup, test body, (teardown)
        member this.Ignore (testName: string, setup: SetupIndicator<'featureType, 'setupType>, _testBody: 'testBodyType, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], setup, (), teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, setup: SetupIndicator<'featureType, 'setupType>, _testBody: 'testBodyType, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], setup, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- test name, setup, (teardown)
        member this.Ignore (testName: string, setup: SetupIndicator<'featureType, 'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], setup, (), teardown, testName, fileFullName, lineNumber)

        member this.Ignore (testName: string, setup: SetupIndicator<'featureType, 'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], setup, (), emptyTeardown, testName, fileFullName, lineNumber)
        
        // -- test name, data, test body, (teardown)
        member this.Ignore (testName: string, data: DataIndicator<'dataType>, _testBody: 'testBodyType, _teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, data, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, data: DataIndicator<'dataType>, _testBody: 'testBodyType, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, data, (), emptyTeardown, testName, fileFullName, lineNumber)
    
        // -- test name, data, (teardown)
        member this.Ignore (testName: string, data: DataIndicator<'dataType>, _teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, data, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, data: DataIndicator<'dataType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, data, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- test name, test body, (teardown)
        member this.Ignore (testName: string, _testBody: 'testBodyType, _teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok,(), emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, _testBody: 'testBodyType, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, (), emptyTeardown, testName, fileFullName, lineNumber)
    
        // -- test name, (teardown)
        member this.Ignore (testName: string, _teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok,(), emptyTeardown, testName, fileFullName, lineNumber)
        
        member this.Ignore (testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) = 
            this.Ignore (TestTags [], Setup Ok, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- tags, setup, data, test body, (teardown)
        member this.Ignore (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, _testBody: 'testBodyType, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, setup, data, (), teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, _testBody: 'testBodyType, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, setup, data, (), emptyTeardown, testName, fileFullName, lineNumber)
        
        // -- tags, setup, test body, (teardown)
        member this.Ignore (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, _testBody: 'testBodyType, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, setup, (), teardown, testName, fileFullName, lineNumber)

        member this.Ignore (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, _testBody: 'testBodyType, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, setup, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- tags data, test body, teardown
        member this.Ignore (tags: TagsIndicator, data: DataIndicator<'dataType>, _testBody: 'testBodyType, _teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, data, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (tags: TagsIndicator, data: DataIndicator<'dataType>, _testBody: 'testBodyType, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, data, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- tags, test body, (teardown)
        member this.Ignore (tags: TagsIndicator, _testBody: 'testBodyType, _teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (tags: TagsIndicator, _testBody: 'testBodyType, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- setup, data, test body, (teardown)
        member this.Ignore (setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, _testBody: 'testBodyType, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], setup, data, (), teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, _testBody: 'testBodyType, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], setup, data, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- setup, test body, (teardown)
        member this.Ignore (setup: SetupIndicator<'featureType, 'setupType>, _testBody: 'testBodyType, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], setup, (), teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (setup: SetupIndicator<'featureType, 'setupType>, _testBody: 'testBodyType, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], setup, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- data, test body, (teardown)
        member this.Ignore (data: DataIndicator<'dataType>, _testBody: 'testBodyType, _teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, data, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (data: DataIndicator<'dataType>, _testBody: 'testBodyType, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, data, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        // -- test body, (teardown)
        member this.Ignore (_testBody: 'testBodyType, _teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (_testBody: 'testBodyType, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, (), emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.GetTests() = this.GetTests ()
        
    interface IScriptFeature<'featureType> with
        member this.IsTestedBy (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): (string -> unit) =
            this.IsTestedBy (tags, setup, testBody, teardown, fileFullName, lineNumber)

        member this.IsIgnored (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, _testBody: 'testBodyType, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): (string -> unit) =
            let buildTest (testName: string) =
                this.Ignore (tags, setup, (), teardown, testName, fileFullName, lineNumber)
                |> ignore
                
            buildTest

        member this.IsTestedBy (tags: TagsIndicator, setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (tags, setup, testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsIgnored (tags: TagsIndicator, setup: SetupIndicator<_, 'setupType>, _testBody: 'testBodyType, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (tags, setup, (), emptyTeardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (tags: TagsIndicator, testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (tags, Setup Ok, testBody, wrapTeardown teardown, fileFullName, lineNumber)
        
        member this.IsIgnored (tags: TagsIndicator, testBody: 'testBodyType, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (tags, Setup Ok, testBody, wrapTeardown teardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (tags: TagsIndicator, testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (tags, Setup Ok, testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsIgnored (tags: TagsIndicator, testBody: 'testBodyType, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (tags, Setup Ok, testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (tags: TagsIndicator, testBody: TestFunctionTwoParameters<'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (tags, Setup Ok, TestBody testBody, emptyTeardown, fileFullName, lineNumber)
            
        // --------- isTestedBy - Test Tags (without environment) ---------
        member this.IsTestedBy (tags: TagsIndicator, setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<TestFunction<'setupType>>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let testBody =
               match testBody with
               | TestBody f -> f
               
            this.Script.IsTestedBy (tags, setup, testBodyOneToTwo testBody, teardown, fileFullName, lineNumber)
            
        member this.IsTestedBy (tags: TagsIndicator, setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<TestFunction<'setupType>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (tags, setup, testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (tags: TagsIndicator, testBody: TestBodyIndicator<TestFunction<'featureType>>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (tags, Setup Ok, testBody, wrapTeardown teardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (tags: TagsIndicator, testBody: TestBodyIndicator<TestFunction<'featureType>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (tags, Setup Ok, testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (tags: TagsIndicator, testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (tags, Setup Ok, TestBody testBody, emptyTeardown, fileFullName, lineNumber)
            
        // --------- isTestedBy - Setup (with environment) ---------
        member this.IsTestedBy (setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (TestTags [], setup, testBody, teardown, fileFullName, lineNumber)
        
        member this.IsIgnored (setup: SetupIndicator<_, 'setupType>, testBody: 'testBodyType, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (TestTags [], setup, testBody, teardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (setup, testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsIgnored (setup: SetupIndicator<_, 'setupType>, testBody: 'testBodyType, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (setup, testBody, emptyTeardown, fileFullName, lineNumber)
        
        // --------- isTestedBy - Setup (without environment) ---------
        member this.IsTestedBy (setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<TestFunction<'setupType>>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let testBody =
                match testBody with
                | TestBody f -> f
                
            this.Script.IsTestedBy (setup, testBodyOneToTwo testBody, teardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<TestFunction<'setupType>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (setup, testBody, emptyTeardown, fileFullName, lineNumber)
        
        // --------- isTestedBy - TEST BODY (with environment) ---------
        member this.IsTestedBy (testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (Setup Ok, testBody, wrapTeardown teardown, fileFullName, lineNumber)
        
        member this.IsIgnored (_testBody: 'testBodyType, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (Setup Ok, (), wrapTeardown teardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsIgnored (testBody: 'testBodyType, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (testBody: TestFunctionTwoParameters<'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (TestBody testBody, fileFullName, lineNumber)
        
        // --------- isTestedBy - TEST BODY (without environment) ---------
        member this.IsTestedBy (testBody: TestBodyIndicator<TestFunction<'featureType>>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (Setup Ok, testBody, wrapTeardown teardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (testBody: TestBodyIndicator<TestFunction<'featureType>>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (TestBody testBody, fileFullName, lineNumber)

    member this.GetTests () = tests
    
    override _.ToString () =
        [
            featurePath
            featureName
        ]
        |> List.filter (String.IsNullOrWhiteSpace >> not)
        |> fun items -> String.Join (".", items)