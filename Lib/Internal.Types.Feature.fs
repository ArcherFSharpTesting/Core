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
        TestBodyTwoParameters (fun a _b -> testBody a)
    
    let testBodyOneToThree (testBody: 'a -> TestResult) =
        TestBodyThreeParameters (fun a _b _c -> testBody a)
    
    let testBodyTwoToThreeIgnoreSetup (testBody: 'a -> 'b -> TestResult) =
        TestBodyThreeParameters (fun a _b c -> testBody a c)
    
    let testBodyTwoToThreeIgnoreEnv (testBody: 'a -> 'b -> TestResult) =
        TestBodyThreeParameters (fun a b _c -> testBody a b)
        
    let wrapTeardown teardown =
        let (Teardown teardown) = teardown
        Teardown (fun _ -> teardown (Ok ()))
        
    let buildDataTests (ctor: TagsIndicator * SetupIndicator<'featureType,'setupType> * TestBodyIndicatorTwoParameters<'setupType,TestEnvironment> * TeardownIndicator<'setupType> * string * string * int -> ITest) (tags: TagsIndicator) (setup: SetupIndicator<'featureType, 'setupType>) (data: DataIndicator<'dataType>) (testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment>) (teardown: TeardownIndicator<'setupType>) (testName: string) (fileFullName: string) (lineNumber: int) =
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
            
        let (TestBodyThreeParameters testBody) = testBody
        
        let getTestName input =
            let name = sprintf testNameFormat input
            getFixedName name
            
        let (Data data) = data
        
        data
        |> Seq.map (fun datum ->
            ctor (tags, setup, (TestBodyTwoParameters (testBody datum)), teardown, getTestName datum, fileFullName, lineNumber)
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
        
    abstract member Test: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment> * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest 
    default this.Test      (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let location = getLocation fileFullName lineNumber
        
        let test =
            let TestTags tags, Setup setup, TestBodyTwoParameters testBody, Teardown teardown = (tags, setup, testBody, teardown)
            let inner = SetupTeardownExecutor (setup, teardown, fun value env -> env |> testBody value |> TestExecutionResult) :> ISetupTeardownExecutor<'featureType>
            transformer ({ ContainerPath = featurePath; ContainerName = featureName; TestName = testName; Tags = [tags; featureTags] |> List.concat; FilePath = location.FilePath; FileName = location.FileName; LineNumber = lineNumber }, inner)
        
        this.AddTest test

    abstract member Ignore: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment> * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest        
    default this.Ignore (tags: TagsIndicator, _setup: SetupIndicator<'featureType, 'setupType>, _testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, _teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let location = getLocation fileFullName lineNumber
        let failure = TestIgnored (None, location) |> TestFailure
        let tb = TestBodyTwoParameters (fun _ _ -> failure)
        this.Test (tags, Setup (fun _ -> Ok ()), tb, emptyTeardown, testName, fileFullName, lineNumber)
        
    abstract member IsTestedBy: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment> * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    default this.IsTestedBy (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let buildTest (testName: string) =
            this.Test (tags, setup, testBody, teardown, testName, fileFullName, lineNumber)
            |> ignore
            
        buildTest
        
    abstract member IsTestedBy: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment> * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    default this.IsTestedBy (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let buildTest (testName: string) =
            this.Test (tags, setup, data, testBody, teardown, testName, fileFullName, lineNumber)
            |> ignore
            
        buildTest
        
    abstract member IsIgnored:  tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment> * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    default this.IsIgnored (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let buildTest (testName: string) =
            this.Ignore (tags, setup, data, testBody, teardown, testName, fileFullName, lineNumber)
            |> ignore
            
        buildTest

    abstract member IsIgnored: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment> * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    default this.IsIgnored (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let buildTest (testName: string) =
            this.Ignore (tags, setup, testBody, teardown, testName, fileFullName, lineNumber)
            |> ignore
            
        buildTest
        
    abstract member Test:   tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment> * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    default    this.Test   (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        buildDataTests this.Test tags setup data testBody teardown testName fileFullName lineNumber
        
    abstract member Ignore:   tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment> * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    default    this.Ignore   (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        buildDataTests this.Ignore tags setup data testBody teardown testName fileFullName lineNumber
        
    interface IFeature<'featureType> with
        member _.FeatureTags with get () = featureTags
        
        // --------- TEST TAGS (with environment) ---------
        member this.Test      (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test(tags, setup, testBody, teardown, testName, fileFullName, lineNumber)

        member this.Ignore (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, setup, testBody, teardown, testName, fileFullName, lineNumber)
        
        member this.Test   (tags: TagsIndicator, testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
        
        member this.Ignore (tags: TagsIndicator, testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
        
        member this.Test   (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, setup, testBody, emptyTeardown, testName, fileFullName, lineNumber)

        member this.Ignore (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, setup, testBody, emptyTeardown, testName, fileFullName, lineNumber)
                
        member this.Test   (tags: TagsIndicator, testBody: TestFunctionTwoParameters<'featureType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, TestBodyTwoParameters testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test   (tags: TagsIndicator, testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Feature.Test (tags, testBody, testName, fileFullName, lineNumber)
            
        member this.Ignore (tags: TagsIndicator, testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // --------- TEST NAME / Tags (with environment) ---------
        member this.Test   (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, setup, testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test   (testName: string, tags: TagsIndicator, testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test   (testName: string, tags: TagsIndicator, testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test   (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, setup, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test   (testName: string, tags: TagsIndicator, testBody: TestFunctionTwoParameters<'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, TestBodyTwoParameters testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // --------- TEST TAGS (without environment) ---------
        member this.Test  (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
           let (TestBody testBody) = testBody
           this.Test (tags, setup, testBodyOneToTwo testBody, teardown, testName, fileFullName, lineNumber)
           
        member this.Test   (testName: string, tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, testBodyOneToTwo testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test   (testName: string, tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test   (tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, testBodyOneToTwo testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test   (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, setup, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Ignore (tags, setup, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test   (tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test   (tags: TagsIndicator, testBody: TestFunction<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // --------- TEST NAME / TAGS (without environment) ---------
        member this.Test   (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, setup, testBodyOneToTwo testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Ignore (tags, setup, testBodyOneToTwo testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test   (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, setup, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Ignore (tags, setup, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test   (testName: string, tags: TagsIndicator, testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, tags: TagsIndicator, testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test   (testName: string, tags: TagsIndicator, testBody: TestFunction<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, testBodyOneToTwo testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, tags: TagsIndicator, testBody: TestFunction<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, testBodyOneToTwo testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
                
        // --------- SET UP (with environment) ---------
        member this.Test   (setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], setup, testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], setup, testBody, teardown, testName, fileFullName, lineNumber)
        
        member this.Test   (setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], setup, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], setup, testBody, emptyTeardown, testName, fileFullName, lineNumber)
        
        // --------- TEST NAME / SET UP (with environment) ---------
        member this.Test   (testName: string, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], setup, testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test   (testName: string, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], setup, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // --------- SET UP (without environment) ---------
        member this.Test   (setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], setup, testBodyOneToTwo testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test   (setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], setup, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // --------- TEST NAME / SET UP (without environment) ---------
        member this.Test   (testName: string, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], setup, testBodyOneToTwo testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Ignore (TestTags [], setup, testBodyOneToTwo testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test   (testName: string, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], setup, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Ignore (TestTags [], setup, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // --------- TEST BODY (with environment)---------
        member this.Test   (testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
        
        member this.Test   (testBody: TestFunctionTwoParameters<'featureType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, TestBodyTwoParameters testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testBody: TestFunctionTwoParameters<'featureType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, TestBodyTwoParameters testBody, emptyTeardown, testName, fileFullName, lineNumber)
                
        member this.Test   (testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, testBody, emptyTeardown, testName, fileFullName, lineNumber)
                
        // --------- TEST NAME / TEST BODY (with environment)---------
        member this.Test   (testName: string, testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test   (testName: string, testBody: TestFunctionTwoParameters<'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, TestBodyTwoParameters testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test   (testName: string, testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (TestTags [], Setup Ok, TestBodyTwoParameters testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // --------- TEST BODY (without environment)---------
        member this.Test   (testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, testBodyOneToTwo testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Ignore (TestTags [], Setup Ok, testBodyOneToTwo testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test   (testBody: TestFunction<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testBody: TestFunction<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
                
        member this.Test   (testBody: TestBodyIndicator<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testBody: TestBodyIndicator<'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Ignore (TestTags [], Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
                
        // --------- TEST NAME / TEST BODY (without environment)---------
        member this.Test   (testName: string, testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, testBodyOneToTwo testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Ignore (TestTags [], Setup Ok, testBodyOneToTwo testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
                
        member this.Test   (testName: string, testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test   (testName: string, testBody: TestBodyIndicator<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, testBody: TestBodyIndicator<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Ignore (TestTags [], Setup Ok, testBodyOneToTwo testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        // Data Driven tests
        member this.Test (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, setup, data, testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'featureType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'featureType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, data, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, setup, data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, setup, data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, TestBodyThreeParameters testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'featureType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok,data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'featureType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok,data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, setup, data, testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'featureType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, setup, data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, TestBodyThreeParameters testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (tags, setup, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (tags, setup, data, testBodyTwoToThreeIgnoreEnv testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Ignore (tags, setup, data, testBodyTwoToThreeIgnoreEnv testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunctionTwoParameters<'dataType, 'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreSetup testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreSetup testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreSetup testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunctionTwoParameters<'dataType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreSetup testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (tags, setup, data, testBodyTwoToThreeIgnoreEnv testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType,'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Ignore (tags, setup, data, testBodyTwoToThreeIgnoreEnv testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (tags, setup, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Ignore (tags, setup, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunctionTwoParameters<'dataType, 'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunctionTwoParameters<'dataType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreSetup testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreSetup testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunctionTwoParameters<'dataType, 'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunctionTwoParameters<'dataType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBodyTwoToThreeIgnoreSetup testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], setup, data, testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], setup, data, testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test (setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], setup, data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], setup, data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], setup, data, testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], setup, data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (TestTags [], setup, data, testBodyTwoToThreeIgnoreEnv testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test (setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (TestTags [], setup, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Ignore (TestTags [], setup, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<'dataType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyOneToThree testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<'dataType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyOneToThree testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<'dataType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunction<'dataType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<'dataType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (tags, Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunction<'dataType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBodyOneToThree testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunction<'dataType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, data, testBodyOneToThree testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (TestTags [], setup, data, testBodyTwoToThreeIgnoreEnv testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Ignore (TestTags [], setup, data, testBodyTwoToThreeIgnoreEnv testBody, teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (TestTags [], setup, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Ignore (TestTags [], setup, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'featureType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'featureType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, data, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (data: DataIndicator<'dataType>, testBody: TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, TestBodyThreeParameters testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (data: DataIndicator<'dataType>, testBody: TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, data, TestBodyThreeParameters testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'featureType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'featureType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'featureType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, TestBodyThreeParameters testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'featureType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Ignore (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreSetup testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Ignore (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreSetup testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (data: DataIndicator<'dataType>, testBody: TestBodyIndicator<'dataType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyOneToThree testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (data: DataIndicator<'dataType>, testBody: TestBodyIndicator<'dataType>, teardown: TeardownIndicator<unit>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Ignore (TestTags [], Setup Ok, data, testBodyOneToThree testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (data: DataIndicator<'dataType>, testBody: TestFunctionTwoParameters<'dataType, 'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (data: DataIndicator<'dataType>, testBody: TestFunctionTwoParameters<'dataType, 'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (data: DataIndicator<'dataType>, testBody: TestFunctionTwoParameters<'dataType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreSetup testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (data: DataIndicator<'dataType>, testBody: TestFunctionTwoParameters<'dataType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreSetup testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (data: DataIndicator<'dataType>, testBody: TestFunction<'dataType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (data: DataIndicator<'dataType>, testBody: TestFunction<'dataType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'featureType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Ignore (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreSetup testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, TestEnvironment>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Ignore (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreSetup testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (data: DataIndicator<'dataType>, testBody: TestBodyIndicator<'dataType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (data: DataIndicator<'dataType>, testBody: TestBodyIndicator<'dataType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Ignore (TestTags [], Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreSetup testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<'dataType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyOneToThree testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<'dataType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Ignore (TestTags [], Setup Ok, data, testBodyOneToThree testBody, wrapTeardown teardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestFunctionTwoParameters<'dataType, 'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestFunctionTwoParameters<'dataType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreSetup testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestFunction<'dataType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (TestTags [], Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, data: DataIndicator<'dataType>, testBody: TestFunction<'dataType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (TestTags [], Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, 'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreEnv testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorTwoParameters<'dataType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBodyTwoParameters testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyTwoToThreeIgnoreSetup testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, data: DataIndicator<'dataType>, testBody: TestBodyIndicator<'dataType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let (TestBody testBody) = testBody
            this.Test (TestTags [], Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Test (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunction<'dataType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Test (tags, Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.Ignore (testName: string, tags: TagsIndicator, data: DataIndicator<'dataType>, testBody: TestFunction<'dataType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Ignore (tags, Setup Ok, data, testBodyOneToThree testBody, emptyTeardown, testName, fileFullName, lineNumber)
            
        member this.GetTests() = this.GetTests ()
        
    interface IScriptFeature<'featureType> with
        member this.IsTestedBy (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): (string -> unit) =
            this.IsTestedBy (tags, setup, testBody, teardown, fileFullName, lineNumber)

        member this.IsIgnored (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int): (string -> unit) =
            let buildTest (testName: string) =
                this.Ignore (tags, setup, testBody, teardown, testName, fileFullName, lineNumber)
                |> ignore
                
            buildTest

        member this.IsTestedBy (tags: TagsIndicator, setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (tags, setup, testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsIgnored (tags: TagsIndicator, setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (tags, setup, testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (tags: TagsIndicator, testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (tags, Setup Ok, testBody, wrapTeardown teardown, fileFullName, lineNumber)
        
        member this.IsIgnored (tags: TagsIndicator, testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (tags, Setup Ok, testBody, wrapTeardown teardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (tags: TagsIndicator, testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (tags, Setup Ok, testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsIgnored (tags: TagsIndicator, testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (tags, Setup Ok, testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (tags: TagsIndicator, testBody: TestFunctionTwoParameters<'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (tags, Setup Ok, TestBodyTwoParameters testBody, emptyTeardown, fileFullName, lineNumber)
            
        member this.IsIgnored (tags: TagsIndicator, testBody: TestFunctionTwoParameters<'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (tags, Setup Ok, TestBodyTwoParameters testBody, emptyTeardown, fileFullName, lineNumber)
            
        // --------- isTestedBy - Test Tags (without environment) ---------
        member this.IsTestedBy (tags: TagsIndicator, setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let testBody =
               match testBody with
               | TestBody f -> f
               
            this.Script.IsTestedBy (tags, setup, testBodyOneToTwo testBody, teardown, fileFullName, lineNumber)
            
        member this.IsIgnored (tags: TagsIndicator, setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let testBody =
               match testBody with
               | TestBody f -> f
               
            this.Script.IsIgnored (tags, setup, testBodyOneToTwo testBody, teardown, fileFullName, lineNumber)
            
        member this.IsTestedBy (tags: TagsIndicator, setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (tags, setup, testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsIgnored (tags: TagsIndicator, setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (tags, setup, testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (tags, Setup Ok, testBody, wrapTeardown teardown, fileFullName, lineNumber)
        
        member this.IsIgnored (tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (tags, Setup Ok, testBody, wrapTeardown teardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (tags, Setup Ok, testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsIgnored (tags: TagsIndicator, testBody: TestBodyIndicator<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (tags, Setup Ok, testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (tags: TagsIndicator, testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (tags, Setup Ok, TestBody testBody, emptyTeardown, fileFullName, lineNumber)
            
        member this.IsIgnored (tags: TagsIndicator, testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (tags, Setup Ok, TestBody testBody, emptyTeardown, fileFullName, lineNumber)
            
        // --------- isTestedBy - Setup (with environment) ---------
        member this.IsTestedBy (setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (TestTags [], setup, testBody, teardown, fileFullName, lineNumber)
        
        member this.IsIgnored (setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (TestTags [], setup, testBody, teardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (setup, testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsIgnored (setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (setup, testBody, emptyTeardown, fileFullName, lineNumber)
        
        // --------- isTestedBy - Setup (without environment) ---------
        member this.IsTestedBy (setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let testBody =
                match testBody with
                | TestBody f -> f
                
            this.Script.IsTestedBy (setup, testBodyOneToTwo testBody, teardown, fileFullName, lineNumber)
        
        member this.IsIgnored (setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<'setupType>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            let testBody =
                match testBody with
                | TestBody f -> f
                
            this.Script.IsIgnored (setup, testBodyOneToTwo testBody, teardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (setup, testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsIgnored (setup: SetupIndicator<_, 'setupType>, testBody: TestBodyIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (setup, testBody, emptyTeardown, fileFullName, lineNumber)
        
        // --------- isTestedBy - TEST BODY (with environment) ---------
        member this.IsTestedBy (testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (Setup Ok, testBody, wrapTeardown teardown, fileFullName, lineNumber)
        
        member this.IsIgnored (testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (Setup Ok, testBody, wrapTeardown teardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsIgnored (testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (testBody: TestFunctionTwoParameters<'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (TestBodyTwoParameters testBody, fileFullName, lineNumber)
        
        member this.IsIgnored (testBody: TestFunctionTwoParameters<'featureType, TestEnvironment>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (TestBodyTwoParameters testBody, fileFullName, lineNumber)
        
        // --------- isTestedBy - TEST BODY (without environment) ---------
        member this.IsTestedBy (testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (Setup Ok, testBody, wrapTeardown teardown, fileFullName, lineNumber)
        
        member this.IsIgnored (testBody: TestBodyIndicator<'featureType>, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (Setup Ok, testBody, wrapTeardown teardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (testBody: TestBodyIndicator<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsIgnored (testBody: TestBodyIndicator<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (testBody, emptyTeardown, fileFullName, lineNumber)
        
        member this.IsTestedBy (testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsTestedBy (TestBody testBody, fileFullName, lineNumber)

        member this.IsIgnored (testBody: TestFunction<'featureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.Script.IsIgnored (TestBody testBody, fileFullName, lineNumber)
            
        member this.IsTestedBy (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.IsTestedBy (tags, setup, data, testBody, teardown, fileFullName, lineNumber)
            
        member this.IsIgnored (tags: TagsIndicator, setup: SetupIndicator<'featureType, 'setupType>, data: DataIndicator<'dataType>, testBody: TestBodyIndicatorThreeParameters<'dataType, 'setupType, TestEnvironment>, teardown: TeardownIndicator<'setupType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
            this.IsIgnored (tags, setup, data, testBody, teardown, fileFullName, lineNumber)

    member this.GetTests () = tests
    
    override _.ToString () =
        [
            featurePath
            featureName
        ]
        |> List.filter (String.IsNullOrWhiteSpace >> not)
        |> fun items -> String.Join (".", items)