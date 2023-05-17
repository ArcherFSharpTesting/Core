namespace Archer.Arrows.Internal.Types

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows

type IScriptFeature<'featureType> =
    abstract member IsTestedBy: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment> * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    
    abstract member IsTestedBy: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    
    abstract member IsTestedBy: tags: TagsIndicator * testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment> * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    
    abstract member IsTestedBy: tags: TagsIndicator * testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
        
    abstract member IsTestedBy: tags: TagsIndicator * testBody: TestFunctionTwoParameters<'featureType, TestEnvironment> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
        
    // --------- isTestedBy - Test Tags (without environment) ---------
    abstract member IsTestedBy: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<'setupType> * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
        
    abstract member IsTestedBy: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    
    abstract member IsTestedBy: tags: TagsIndicator * testBody: TestBodyIndicator<'featureType> * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    
    abstract member IsTestedBy: tags: TagsIndicator * testBody: TestBodyIndicator<'featureType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    
    abstract member IsTestedBy: tags: TagsIndicator * testBody: TestFunction<'featureType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
        
    // --------- isTestedBy - Setup (with environment) ---------
    abstract member IsTestedBy: setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment> * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
        
    abstract member IsTestedBy: setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicatorTwoParameters<'setupType, TestEnvironment> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    
    // --------- isTestedBy - Setup (without environment) ---------
    abstract member IsTestedBy: setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<'setupType> * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    
    abstract member IsTestedBy: setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    
    // --------- isTestedBy - TEST BODY (with environment) ---------
    abstract member IsTestedBy: testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment> * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    
    abstract member IsTestedBy: testBody: TestBodyIndicatorTwoParameters<'featureType, TestEnvironment> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    
    abstract member IsTestedBy: testBody: TestFunctionTwoParameters<'featureType, TestEnvironment> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    
    // --------- isTestedBy - TEST BODY (without environment) ---------
    abstract member IsTestedBy: testBody: TestBodyIndicator<'featureType> * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)

    abstract member IsTestedBy: testBody: TestBodyIndicator<'featureType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)

    abstract member IsTestedBy: testBody: TestFunction<'featureType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    
    abstract member IsIgnored: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: 'testBodyType * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    abstract member IsIgnored: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: 'testBodyType * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    abstract member IsIgnored: tags: TagsIndicator * testBody: 'testBodyType * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    abstract member IsIgnored: tags: TagsIndicator * testBody: 'testBodyType * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    abstract member IsIgnored: setup: SetupIndicator<'featureType, 'setupType> * 'testBodyType * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    abstract member IsIgnored: setup: SetupIndicator<'featureType, 'setupType> * testBody: 'testBodyType * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    abstract member IsIgnored: testBody: 'testBodyType * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    abstract member IsIgnored: testBody: 'testBodyType * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> (string -> unit)
    