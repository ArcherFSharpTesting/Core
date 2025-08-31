namespace Archer.Arrows.Internal.Types

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer
open Archer.Arrows
open Archer.CoreTypes.InternalTypes
        
type IFeature<'featureType> =
    /// <summary>
    /// Gets the list of tags associated with this feature for categorization and filtering.
    /// </summary>
    /// <returns>A list of TestTag instances representing the feature's tags</returns>
    abstract member FeatureTags: TestTag list with get
    /// <summary>
    /// Retrieves all tests defined within this feature.
    /// </summary>
    /// <returns>A list of ITest instances representing all tests in the feature</returns>
    abstract member GetTests: unit -> ITest list
    
    //-----------------------------------------------------//
    //                    Test Builders                    //
    //-----------------------------------------------------//
        
    // -- test name, tags, setup, data, test body, teardown
    /// <summary>
    /// Creates a list of tests with the specified name, tags, setup, data, test body, and teardown.
    /// This overload supports test functions that take three parameters: data, setup result, and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data, setup result, and test environment</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*001*) abstract member Test: testName: string * tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'setupType, TestEnvironment>> * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified name, tags, setup, data, test body, and teardown.
    /// This overload supports test functions that take two parameters: data and setup result.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data and setup result</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*002*) abstract member Test: testName: string * tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'setupType>> * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    
    // -- test name, tags, setup, data, test body
    /// <summary>
    /// Creates a list of tests with the specified name, tags, setup, data, and test body without teardown.
    /// This overload supports test functions that take three parameters: data, setup result, and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data, setup result, and test environment</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*003*) abstract member Test: testName: string * tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'setupType, TestEnvironment>> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified name, tags, setup, data, and test body without teardown.
    /// This overload supports test functions that take two parameters: data and setup result.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data and setup result</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*004*) abstract member Test: testName: string * tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'setupType>> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    
    // -- test name, tags, setup, test body, teardown
    /// <summary>
    /// Creates a test with the specified name, tags, setup, test body, and teardown.
    /// This overload supports test functions that take two parameters: setup result and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts setup result and test environment</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*005*) abstract member Test: testName: string * tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>> * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified name, tags, setup, test body, and teardown.
    /// This overload supports test functions that take one parameter: setup result.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts setup result</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*006*) abstract member Test: testName: string * tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<TestFunction<'setupType>> * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    
    // -- test name, tags, setup, test body
    /// <summary>
    /// Creates a test with the specified name, tags, setup, and test body without teardown.
    /// This overload supports test functions that take two parameters: setup result and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts setup result and test environment</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*007*) abstract member Test: testName: string * tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified name, tags, setup, and test body without teardown.
    /// This overload supports test functions that take one parameter: setup result.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts setup result</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*008*) abstract member Test: testName: string * tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<TestFunction<'setupType>> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- test name, tags, data, test body, teardown
    /// <summary>
    /// Creates a list of tests with the specified name, tags, data, test body, and teardown.
    /// This overload supports test functions that take three parameters: data, feature type, and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data, feature type, and test environment</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*009*) abstract member Test: testName: string * tags: TagsIndicator * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>> * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified name, tags, data, test body, and teardown.
    /// This overload supports test functions that take two parameters: data and feature type.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data and feature type</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*010*) abstract member Test: testName: string * tags: TagsIndicator * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'featureType>> * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified name, tags, data, test body, and teardown.
    /// This overload supports test functions that take one parameter: data.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*011*) abstract member Test: testName: string * tags: TagsIndicator * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunction<'dataType>> * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    
    // -- test name, tags, data, test body
    /// <summary>
    /// Creates a list of tests with the specified name, tags, data, and test body without teardown.
    /// This overload supports test functions that take three parameters: data, feature type, and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data, feature type, and test environment</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*012*) abstract member Test: testName: string * tags: TagsIndicator * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified name, tags, data, and test body without teardown.
    /// This overload supports test functions that take two parameters: data and feature type.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data and feature type</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*013*) abstract member Test: testName: string * tags: TagsIndicator * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'featureType>> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified name, tags, data, and test body without teardown.
    /// This overload supports test functions that take one parameter: data.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*014*) abstract member Test: testName: string * tags: TagsIndicator * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunction<'dataType>> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    
    // -- test name, tags, data, test function
    /// <summary>
    /// Creates a list of tests with the specified name, tags, data, and test function.
    /// This overload supports test functions that take three parameters: data, feature type, and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test function that accepts data, feature type, and test environment</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*015*) abstract member Test: testName: string * tags: TagsIndicator * data: DataIndicator<'dataType> * testBody: TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified name, tags, data, and test function.
    /// This overload supports test functions that take two parameters: data and feature type.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test function that accepts data and feature type</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*016*) abstract member Test: testName: string * tags: TagsIndicator * data: DataIndicator<'dataType> * testBody: TestFunctionTwoParameters<'dataType, 'featureType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified name, tags, data, and test function.
    /// This overload supports test functions that take one parameter: data.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test function that accepts data</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*017*) abstract member Test: testName: string * tags: TagsIndicator * data: DataIndicator<'dataType> * testBody: TestFunction<'dataType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    
    // -- test name, tags, test body, teardown
    /// <summary>
    /// Creates a test with the specified name, tags, test body, and teardown.
    /// This overload supports test functions that take two parameters: feature type and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts feature type and test environment</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*018*) abstract member Test: testName: string * tags: TagsIndicator * testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>> * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified name, tags, test body, and teardown.
    /// This overload supports test functions that take one parameter: feature type.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts feature type</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*019*) abstract member Test: testName: string * tags: TagsIndicator * testBody: TestBodyIndicator<TestFunction<'featureType>> * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    
    // -- test name, tags, test body
    /// <summary>
    /// Creates a test with the specified name, tags, and test body without teardown.
    /// This overload supports test functions that take two parameters: feature type and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts feature type and test environment</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*020*) abstract member Test: testName: string * tags: TagsIndicator * testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified name, tags, and test body without teardown.
    /// This overload supports test functions that take one parameter: feature type.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts feature type</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*021*) abstract member Test: testName: string * tags: TagsIndicator * testBody: TestBodyIndicator<TestFunction<'featureType>> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    
    // -- test name, tags, test function
    /// <summary>
    /// Creates a test with the specified name, tags, and test function.
    /// This overload supports test functions that take two parameters: feature type and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="testBody">The test function that accepts feature type and test environment</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*022*) abstract member Test: testName: string * tags: TagsIndicator * testBody: TestFunctionTwoParameters<'featureType, TestEnvironment> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified name, tags, and test function.
    /// This overload supports test functions that take one parameter: feature type.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="testBody">The test function that accepts feature type</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*023*) abstract member Test: testName: string * tags: TagsIndicator * testBody: TestFunction<'featureType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    
    // -- test name, setup, data, test body, teardown
    /// <summary>
    /// Creates a list of tests with the specified name, setup, data, test body, and teardown.
    /// This overload supports test functions that take three parameters: data, setup result, and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data, setup result, and test environment</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*024*) abstract member Test: testName: string * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'setupType, TestEnvironment>> * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified name, setup, data, test body, and teardown.
    /// This overload supports test functions that take two parameters: data and setup result.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data and setup result</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*025*) abstract member Test: testName: string * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'setupType>> * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- test name, setup, data, test body
    /// <summary>
    /// Creates a list of tests with the specified name, setup, data, and test body without teardown.
    /// This overload supports test functions that take three parameters: data, setup result, and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data, setup result, and test environment</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*026*) abstract member Test: testName: string * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'setupType, TestEnvironment>> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified name, setup, data, and test body without teardown.
    /// This overload supports test functions that take two parameters: data and setup result.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data and setup result</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*027*) abstract member Test: testName: string * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'setupType>> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- test name, setup, test body, teardown
    /// <summary>
    /// Creates a test with the specified name, setup, test body, and teardown.
    /// This overload supports test functions that take two parameters: setup result and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts setup result and test environment</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*028*) abstract member Test: testName: string * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>> * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified name, setup, test body, and teardown.
    /// This overload supports test functions that take one parameter: setup result.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts setup result</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*029*) abstract member Test: testName: string * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<TestFunction<'setupType>> * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- test name, setup, test body
    /// <summary>
    /// Creates a test with the specified name, setup, and test body without teardown.
    /// This overload supports test functions that take two parameters: setup result and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts setup result and test environment</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*030*) abstract member Test: testName: string * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified name, setup, and test body without teardown.
    /// This overload supports test functions that take one parameter: setup result.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts setup result</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*031*) abstract member Test: testName: string * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<TestFunction<'setupType>> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- test name, data, test body, teardown    
    /// <summary>
    /// Creates a list of tests with the specified name, data, test body, and teardown.
    /// This overload supports test functions that take three parameters: data, feature type, and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data, feature type, and test environment</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*032*) abstract member Test: testName: string * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>> * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified name, data, test body, and teardown.
    /// This overload supports test functions that take two parameters: data and feature type.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data and feature type</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*033*) abstract member Test: testName: string * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'featureType>> * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified name, data, test body, and teardown.
    /// This overload supports test functions that take one parameter: data.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*034*) abstract member Test: testName: string * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunction<'dataType>> * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- test name, data, test body
    /// <summary>
    /// Creates a list of tests with the specified name, data, and test body without teardown.
    /// This overload supports test functions that take three parameters: data, feature type, and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data, feature type, and test environment</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*035*) abstract member Test: testName: string * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified name, data, and test body without teardown.
    /// This overload supports test functions that take two parameters: data and feature type.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data and feature type</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*036*) abstract member Test: testName: string * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'featureType>> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified name, data, and test body without teardown.
    /// This overload supports test functions that take one parameter: data.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*037*) abstract member Test: testName: string * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunction<'dataType>> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- test name, data, test function    
    /// <summary>
    /// Creates a list of tests with the specified name, data, and test function.
    /// This overload supports test functions that take three parameters: data, feature type, and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test function that accepts data, feature type, and test environment</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*038*) abstract member Test: testName: string * data: DataIndicator<'dataType> * testBody: TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified name, data, and test function.
    /// This overload supports test functions that take two parameters: data and feature type.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test function that accepts data and feature type</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*039*) abstract member Test: testName: string * data: DataIndicator<'dataType> * testBody: TestFunctionTwoParameters<'dataType, 'featureType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified name, data, and test function.
    /// This overload supports test functions that take one parameter: data.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test function that accepts data</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*040*) abstract member Test: testName: string * data: DataIndicator<'dataType> * testBody: TestFunction<'dataType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- test name, test body, teardown
    /// <summary>
    /// Creates a test with the specified name, test body, and teardown.
    /// This overload supports test functions that take two parameters: feature type and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts feature type and test environment</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*041*) abstract member Test: testName: string * testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>> * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified name, test body, and teardown.
    /// This overload supports test functions that take one parameter: feature type.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts feature type</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*042*) abstract member Test: testName: string * testBody: TestBodyIndicator<TestFunction<'featureType>> * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- test name, test body
    /// <summary>
    /// Creates a test with the specified name and test body without teardown.
    /// This overload supports test functions that take two parameters: feature type and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts feature type and test environment</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*043*) abstract member Test: testName: string * testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified name and test body without teardown.
    /// This overload supports test functions that take one parameter: feature type.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts feature type</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*044*) abstract member Test: testName: string * testBody: TestBodyIndicator<TestFunction<'featureType>> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- test name, test function
    /// <summary>
    /// Creates a test with the specified name and test function.
    /// This overload supports test functions that take two parameters: feature type and test environment.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="testBody">The test function that accepts feature type and test environment</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*045*) abstract member Test: testName: string * testBody: TestFunctionTwoParameters<'featureType, TestEnvironment> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified name and test function.
    /// This overload supports test functions that take one parameter: feature type.
    /// </summary>
    /// <param name="testName">The name of the test</param>
    /// <param name="testBody">The test function that accepts feature type</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*046*) abstract member Test: testName: string * testBody: TestFunction<'featureType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- tags, setup, data, test body, teardown
    /// <summary>
    /// Creates a list of tests with the specified tags, setup, data, test body, and teardown.
    /// This overload supports test functions that take three parameters: data, setup result, and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data, setup result, and test environment</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*047*) abstract member Test: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'setupType, TestEnvironment>> * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified tags, setup, data, test body, and teardown.
    /// This overload supports test functions that take two parameters: data and setup result.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data and setup result</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*048*) abstract member Test: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'setupType>> * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- tags, setup, data, test body
    /// <summary>
    /// Creates a list of tests with the specified tags, setup, data, and test body without teardown.
    /// This overload supports test functions that take three parameters: data, setup result, and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data, setup result, and test environment</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*049*) abstract member Test: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'setupType, TestEnvironment>> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified tags, setup, data, and test body without teardown.
    /// This overload supports test functions that take two parameters: data and setup result.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data and setup result</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*050*) abstract member Test: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'setupType>> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- tags, setup, test body, teardown
    /// <summary>
    /// Creates a test with the specified tags, setup, test body, and teardown.
    /// This overload supports test functions that take two parameters: setup result and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts setup result and test environment</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*051*) abstract member Test: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>> * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified tags, setup, test body, and teardown.
    /// This overload supports test functions that take one parameter: setup result.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts setup result</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*052*) abstract member Test: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<TestFunction<'setupType>> * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- tags, setup, test body
    /// <summary>
    /// Creates a test with the specified tags, setup, and test body without teardown.
    /// This overload supports test functions that take two parameters: setup result and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts setup result and test environment</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*053*) abstract member Test: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified tags, setup, and test body without teardown.
    /// This overload supports test functions that take one parameter: setup result.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts setup result</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*054*) abstract member Test: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<TestFunction<'setupType>> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- tags, data, test body, teardown
    /// <summary>
    /// Creates a list of tests with the specified tags, data, test body, and teardown.
    /// This overload supports test functions that take three parameters: data, feature type, and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data, feature type, and test environment</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*055*) abstract member Test: tags: TagsIndicator * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>> * teardown: TeardownIndicator<unit> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified tags, data, test body, and teardown.
    /// This overload supports test functions that take two parameters: data and feature type.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data and feature type</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*056*) abstract member Test: tags: TagsIndicator * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'featureType>> * teardown: TeardownIndicator<unit> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified tags, data, test body, and teardown.
    /// This overload supports test functions that take one parameter: data.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*057*) abstract member Test: tags: TagsIndicator * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunction<'dataType>> * teardown: TeardownIndicator<unit> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- tags, data, test body
    /// <summary>
    /// Creates a list of tests with the specified tags, data, and test body without teardown.
    /// This overload supports test functions that take three parameters: data, feature type, and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data, feature type, and test environment</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*058*) abstract member Test: tags: TagsIndicator * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified tags, data, and test body without teardown.
    /// This overload supports test functions that take two parameters: data and feature type.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data and feature type</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*059*) abstract member Test: tags: TagsIndicator * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'featureType>> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified tags, data, and test body without teardown.
    /// This overload supports test functions that take one parameter: data.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*060*) abstract member Test: tags: TagsIndicator * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunction<'dataType>> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    
    // -- tags, data, test function
    /// <summary>
    /// Creates a list of tests with the specified tags, data, and test function.
    /// This overload supports test functions that take three parameters: data, feature type, and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test function that accepts data, feature type, and test environment</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*061*) abstract member Test: tags: TagsIndicator * data: DataIndicator<'dataType> * testBody: TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified tags, data, and test function.
    /// This overload supports test functions that take two parameters: data and feature type.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test function that accepts data and feature type</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*062*) abstract member Test: tags: TagsIndicator * data: DataIndicator<'dataType> * testBody: TestFunctionTwoParameters<'dataType, 'featureType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified tags, data, and test function.
    /// This overload supports test functions that take one parameter: data.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test function that accepts data</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*063*) abstract member Test: tags: TagsIndicator * data: DataIndicator<'dataType> * testBody: TestFunction<'dataType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- tags, test body, teardown
    /// <summary>
    /// Creates a test with the specified tags, test body, and teardown.
    /// This overload supports test functions that take two parameters: feature type and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts feature type and test environment</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*064*) abstract member Test: tags: TagsIndicator * testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>> * teardown: TeardownIndicator<unit> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified tags, test body, and teardown.
    /// This overload supports test functions that take one parameter: feature type.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts feature type</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*065*) abstract member Test: tags: TagsIndicator * testBody: TestBodyIndicator<TestFunction<'featureType>> * teardown: TeardownIndicator<unit> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- tags, test body
    /// <summary>
    /// Creates a test with the specified tags and test body without teardown.
    /// This overload supports test functions that take two parameters: feature type and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts feature type and test environment</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*066*) abstract member Test: tags: TagsIndicator * testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified tags and test body without teardown.
    /// This overload supports test functions that take one parameter: feature type.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts feature type</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*067*) abstract member Test: tags: TagsIndicator * testBody: TestBodyIndicator<TestFunction<'featureType>> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- tags, test function
    /// <summary>
    /// Creates a test with the specified tags and test function.
    /// This overload supports test functions that take two parameters: feature type and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="testBody">The test function that accepts feature type and test environment</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*068*) abstract member Test: tags: TagsIndicator * testBody: TestFunctionTwoParameters<'featureType, TestEnvironment> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified tags and test function.
    /// This overload supports test functions that take one parameter: feature type.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="testBody">The test function that accepts feature type</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*069*) abstract member Test: tags: TagsIndicator * testBody: TestFunction<'featureType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- setup, data, test body, teardown
    /// <summary>
    /// Creates a list of tests with the specified setup, data, test body, and teardown.
    /// This overload supports test functions that take three parameters: data, setup result, and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data, setup result, and test environment</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*070*) abstract member Test: setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'setupType, TestEnvironment>> * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified setup, data, test body, and teardown.
    /// This overload supports test functions that take two parameters: data and setup result.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data and setup result</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*071*) abstract member Test: setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'setupType>> * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- setup, data, test body
    /// <summary>
    /// Creates a list of tests with the specified setup, data, and test body without teardown.
    /// This overload supports test functions that take three parameters: data, setup result, and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data, setup result, and test environment</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*072*) abstract member Test: setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'setupType, TestEnvironment>> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified setup, data, and test body without teardown.
    /// This overload supports test functions that take two parameters: data and setup result.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data and setup result</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*073*) abstract member Test: setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'setupType>> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- setup, test body, teardown
    /// <summary>
    /// Creates a test with the specified setup, test body, and teardown.
    /// This overload supports test functions that take two parameters: setup result and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts setup result and test environment</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*074*) abstract member Test: setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>> * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified setup, test body, and teardown.
    /// This overload supports test functions that take one parameter: setup result.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts setup result</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*075*) abstract member Test: setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<TestFunction<'setupType>> * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- setup, test body
    /// <summary>
    /// Creates a test with the specified setup and test body without teardown.
    /// This overload supports test functions that take two parameters: setup result and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts setup result and test environment</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*076*) abstract member Test: setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified setup and test body without teardown.
    /// This overload supports test functions that take one parameter: setup result.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts setup result</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*077*) abstract member Test: setup: SetupIndicator<'featureType, 'setupType> * testBody: TestBodyIndicator<TestFunction<'setupType>> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- data, test body, teardown
    /// <summary>
    /// Creates a list of tests with the specified data, test body, and teardown.
    /// This overload supports test functions that take three parameters: data, feature type, and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data, feature type, and test environment</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*078*) abstract member Test: data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>> * teardown: TeardownIndicator<unit> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified data, test body, and teardown.
    /// This overload supports test functions that take two parameters: data and feature type.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data and feature type</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*079*) abstract member Test: data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'featureType>> * teardown: TeardownIndicator<unit> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified data, test body, and teardown.
    /// This overload supports test functions that take one parameter: data.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*080*) abstract member Test: data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunction<'dataType>> * teardown: TeardownIndicator<unit> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- data, test body
    /// <summary>
    /// Creates a list of tests with the specified data and test body without teardown.
    /// This overload supports test functions that take three parameters: data, feature type, and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data, feature type, and test environment</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*081*) abstract member Test: data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment>> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified data and test body without teardown.
    /// This overload supports test functions that take two parameters: data and feature type.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data and feature type</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*082*) abstract member Test: data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunctionTwoParameters<'dataType, 'featureType>> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified data and test body without teardown.
    /// This overload supports test functions that take one parameter: data.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test body indicator containing a test function that accepts data</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*083*) abstract member Test: data: DataIndicator<'dataType> * testBody: TestBodyIndicator<TestFunction<'dataType>> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- data, test function
    /// <summary>
    /// Creates a list of tests with the specified data and test function.
    /// This overload supports test functions that take three parameters: data, feature type, and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test function that accepts data, feature type, and test environment</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*084*) abstract member Test: data: DataIndicator<'dataType> * testBody: TestFunctionThreeParameters<'dataType, 'featureType, TestEnvironment> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified data and test function.
    /// This overload supports test functions that take two parameters: data and feature type.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test function that accepts data and feature type</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*085*) abstract member Test: data: DataIndicator<'dataType> * testBody: TestFunctionTwoParameters<'dataType, 'featureType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of tests with the specified data and test function.
    /// This overload supports test functions that take one parameter: data.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="testBody">The test function that accepts data</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the created tests</returns>
    (*086*) abstract member Test: data: DataIndicator<'dataType> * testBody: TestFunction<'dataType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- test body, teardown
    /// <summary>
    /// Creates a test with the specified test body and teardown.
    /// This overload supports test functions that take two parameters: feature type and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="testBody">The test body indicator containing a test function that accepts feature type and test environment</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*087*) abstract member Test: testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>> * teardown: TeardownIndicator<unit> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified test body and teardown.
    /// This overload supports test functions that take one parameter: feature type.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="testBody">The test body indicator containing a test function that accepts feature type</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*088*) abstract member Test: testBody: TestBodyIndicator<TestFunction<'featureType>> * teardown: TeardownIndicator<unit> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- test body
    /// <summary>
    /// Creates a test with the specified test body without teardown.
    /// This overload supports test functions that take two parameters: feature type and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="testBody">The test body indicator containing a test function that accepts feature type and test environment</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*089*) abstract member Test: testBody: TestBodyIndicator<TestFunctionTwoParameters<'featureType, TestEnvironment>> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified test body without teardown.
    /// This overload supports test functions that take one parameter: feature type.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="testBody">The test body indicator containing a test function that accepts feature type</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*090*) abstract member Test: testBody: TestBodyIndicator<TestFunction<'featureType>> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- test function
    /// <summary>
    /// Creates a test with the specified test function.
    /// This overload supports test functions that take two parameters: feature type and test environment.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="testBody">The test function that accepts feature type and test environment</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*091*) abstract member Test: testBody: TestFunctionTwoParameters<'featureType, TestEnvironment> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates a test with the specified test function.
    /// This overload supports test functions that take one parameter: feature type.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="testBody">The test function that accepts feature type</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the created test</returns>
    (*092*) abstract member Test: testBody: TestFunction<'featureType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    
    
    //-------------------------------------------------------//
    //                    Ignore Builders                    //
    //-------------------------------------------------------//
    
    // -- test name, tags, setup, data, test, (teardown)
    /// <summary>
    /// Creates a list of ignored tests with the specified name, tags, setup, data, test body, and teardown.
    /// The tests will be marked as ignored and will not be executed during test runs.
    /// </summary>
    /// <param name="testName">The name of the ignored test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the ignored tests</returns>
    (*001*)abstract member Ignore: testName: string * tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * test: 'testBodyType * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of ignored tests with the specified name, tags, setup, data, and test body without teardown.
    /// The tests will be marked as ignored and will not be executed during test runs.
    /// </summary>
    /// <param name="testName">The name of the ignored test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the ignored tests</returns>
    (*002*)abstract member Ignore: testName: string * tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * test: 'testBodyType * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- test name, tags, setup, test, (teardown)
    /// <summary>
    /// Creates an ignored test with the specified name, tags, setup, test body, and teardown.
    /// The test will be marked as ignored and will not be executed during test runs.
    /// </summary>
    /// <param name="testName">The name of the ignored test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the ignored test</returns>
    (*003*)abstract member Ignore: testName: string * tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * test: 'testBodyType * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates an ignored test with the specified name, tags, setup, and test body without teardown.
    /// The test will be marked as ignored and will not be executed during test runs.
    /// </summary>
    /// <param name="testName">The name of the ignored test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the ignored test</returns>
    (*004*)abstract member Ignore: testName: string * tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * test: 'testBodyType * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- test name, tags, data, test, (teardown)
    /// <summary>
    /// Creates a list of ignored tests with the specified name, tags, data, test body, and teardown.
    /// The tests will be marked as ignored and will not be executed during test runs.
    /// </summary>
    /// <param name="testName">The name of the ignored test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the ignored tests</returns>
    (*005*)abstract member Ignore: testName: string * tags: TagsIndicator * data: DataIndicator<'dataType> * test: 'testBodyType * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of ignored tests with the specified name, tags, data, and test body without teardown.
    /// The tests will be marked as ignored and will not be executed during test runs.
    /// </summary>
    /// <param name="testName">The name of the ignored test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the ignored tests</returns>
    (*006*)abstract member Ignore: testName: string * tags: TagsIndicator * data: DataIndicator<'dataType> * test: 'testBodyType * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- test name, tags, test, (teardown)
    /// <summary>
    /// Creates an ignored test with the specified name, tags, test body, and teardown.
    /// The test will be marked as ignored and will not be executed during test runs.
    /// </summary>
    /// <param name="testName">The name of the ignored test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the ignored test</returns>
    (*007*)abstract member Ignore: testName: string * tags: TagsIndicator * test: 'testBodyType * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates an ignored test with the specified name, tags, and test body without teardown.
    /// The test will be marked as ignored and will not be executed during test runs.
    /// </summary>
    /// <param name="testName">The name of the ignored test</param>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the ignored test</returns>
    (*008*)abstract member Ignore: testName: string * tags: TagsIndicator * test: 'testBodyType * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- test name, setup, data, test, (teardown)
    /// <summary>
    /// Creates a list of ignored tests with the specified name, setup, data, test body, and teardown.
    /// The tests will be marked as ignored and will not be executed during test runs.
    /// </summary>
    /// <param name="testName">The name of the ignored test</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the ignored tests</returns>
    (*009*)abstract member Ignore: testName: string * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * test: 'testBodyType * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of ignored tests with the specified name, setup, data, and test body without teardown.
    /// The tests will be marked as ignored and will not be executed during test runs.
    /// </summary>
    /// <param name="testName">The name of the ignored test</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the ignored tests</returns>
    (*010*)abstract member Ignore: testName: string * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * test: 'testBodyType * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- test name, setup, test, (teardown)
    /// <summary>
    /// Creates an ignored test with the specified name, setup, test body, and teardown.
    /// The test will be marked as ignored and will not be executed during test runs.
    /// </summary>
    /// <param name="testName">The name of the ignored test</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the ignored test</returns>
    (*011*)abstract member Ignore: testName: string * setup: SetupIndicator<'featureType, 'setupType> * test: 'testBodyType * teardown: TeardownIndicator<'setupType> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates an ignored test with the specified name, setup, and test body without teardown.
    /// The test will be marked as ignored and will not be executed during test runs.
    /// </summary>
    /// <param name="testName">The name of the ignored test</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the ignored test</returns>
    (*012*)abstract member Ignore: testName: string * setup: SetupIndicator<'featureType, 'setupType> * test: 'testBodyType * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- test name, data, test, (teardown)
    /// <summary>
    /// Creates a list of ignored tests with the specified name, data, test body, and teardown.
    /// The tests will be marked as ignored and will not be executed during test runs.
    /// </summary>
    /// <param name="testName">The name of the ignored test</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the ignored tests</returns>
    (*013*)abstract member Ignore: testName: string * data: DataIndicator<'dataType> * test: 'testBodyType * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of ignored tests with the specified name, data, and test body without teardown.
    /// The tests will be marked as ignored and will not be executed during test runs.
    /// </summary>
    /// <param name="testName">The name of the ignored test</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the ignored tests</returns>
    (*014*)abstract member Ignore: testName: string * data: DataIndicator<'dataType> * test: 'testBodyType * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- test name, test, (teardown)
    /// <summary>
    /// Creates an ignored test with the specified name, test body, and teardown.
    /// The test will be marked as ignored and will not be executed during test runs.
    /// </summary>
    /// <param name="testName">The name of the ignored test</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the ignored test</returns>
    (*015*)abstract member Ignore: testName: string * test: 'testBodyType * teardown: TeardownIndicator<unit> * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates an ignored test with the specified name and test body without teardown.
    /// The test will be marked as ignored and will not be executed during test runs.
    /// </summary>
    /// <param name="testName">The name of the ignored test</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the ignored test</returns>
    (*016*)abstract member Ignore: testName: string * test: 'testBodyType * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    
    // -- tags, setup, data, test, (teardown)
    /// <summary>
    /// Creates a list of ignored tests with the specified tags, setup, data, test body, and teardown.
    /// The tests will be marked as ignored and will not be executed during test runs.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the ignored tests</returns>
    (*017*)abstract member Ignore: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * test: 'testBodyType * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of ignored tests with the specified tags, setup, data, and test body without teardown.
    /// The tests will be marked as ignored and will not be executed during test runs.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the ignored tests</returns>
    (*018*)abstract member Ignore: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * test: 'testBodyType * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- tags, setup, test, (teardown)
    /// <summary>
    /// Creates an ignored test with the specified tags, setup, test body, and teardown.
    /// The test will be marked as ignored and will not be executed during test runs.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the ignored test</returns>
    (*019*)abstract member Ignore: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * test: 'testBodyType * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates an ignored test with the specified tags, setup, and test body without teardown.
    /// The test will be marked as ignored and will not be executed during test runs.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the ignored test</returns>
    (*020*)abstract member Ignore: tags: TagsIndicator * setup: SetupIndicator<'featureType, 'setupType> * test: 'testBodyType * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- tags, data, test, (teardown)
    /// <summary>
    /// Creates a list of ignored tests with the specified tags, data, test body, and teardown.
    /// The tests will be marked as ignored and will not be executed during test runs.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the ignored tests</returns>
    (*021*)abstract member Ignore: tags: TagsIndicator * data: DataIndicator<'dataType> * test: 'testBodyType * teardown: TeardownIndicator<unit> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of ignored tests with the specified tags, data, and test body without teardown.
    /// The tests will be marked as ignored and will not be executed during test runs.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the ignored tests</returns>
    (*022*)abstract member Ignore: tags: TagsIndicator * data: DataIndicator<'dataType> * test: 'testBodyType * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- tags, test, (teardown)
    /// <summary>
    /// Creates an ignored test with the specified tags, test body, and teardown.
    /// The test will be marked as ignored and will not be executed during test runs.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the ignored test</returns>
    (*023*)abstract member Ignore: tags: TagsIndicator * test: 'testBodyType * teardown: TeardownIndicator<unit> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates an ignored test with the specified tags and test body without teardown.
    /// The test will be marked as ignored and will not be executed during test runs.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="tags">Tags to be applied to the test for categorization and filtering</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the ignored test</returns>
    (*024*)abstract member Ignore: tags: TagsIndicator * test: 'testBodyType * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- setup, data, test, (teardown)
    /// <summary>
    /// Creates a list of ignored tests with the specified setup, data, test body, and teardown.
    /// The tests will be marked as ignored and will not be executed during test runs.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the ignored tests</returns>
    (*025*)abstract member Ignore: setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * test: 'testBodyType * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of ignored tests with the specified setup, data, and test body without teardown.
    /// The tests will be marked as ignored and will not be executed during test runs.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the ignored tests</returns>
    (*026*)abstract member Ignore: setup: SetupIndicator<'featureType, 'setupType> * data: DataIndicator<'dataType> * test: 'testBodyType * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- setup, test, (teardown)
    /// <summary>
    /// Creates an ignored test with the specified setup, test body, and teardown.
    /// The test will be marked as ignored and will not be executed during test runs.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="teardown">Teardown configuration that cleans up the setup result</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the ignored test</returns>
    (*027*)abstract member Ignore: setup: SetupIndicator<'featureType, 'setupType> * test: 'testBodyType * teardown: TeardownIndicator<'setupType> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates an ignored test with the specified setup and test body without teardown.
    /// The test will be marked as ignored and will not be executed during test runs.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="setup">Setup configuration that produces a setup result of type 'setupType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the ignored test</returns>
    (*028*)abstract member Ignore: setup: SetupIndicator<'featureType, 'setupType> * test: 'testBodyType * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest

    // -- data, test, (teardown)
    /// <summary>
    /// Creates a list of ignored tests with the specified data, test body, and teardown.
    /// The tests will be marked as ignored and will not be executed during test runs.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the ignored tests</returns>
    (*029*)abstract member Ignore: data: DataIndicator<'dataType> * test: 'testBodyType * teardown: TeardownIndicator<unit> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list
    /// <summary>
    /// Creates a list of ignored tests with the specified data and test body without teardown.
    /// The tests will be marked as ignored and will not be executed during test runs.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="data">Data configuration that provides test data of type 'dataType</param>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>A list of ITest instances representing the ignored tests</returns>
    (*030*)abstract member Ignore: data: DataIndicator<'dataType> * test: 'testBodyType * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest list

    // -- test, (teardown)
    /// <summary>
    /// Creates an ignored test with the specified test body and teardown.
    /// The test will be marked as ignored and will not be executed during test runs.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="teardown">Teardown configuration that cleans up after test execution</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the ignored test</returns>
    (*031*)abstract member Ignore: test: 'testBodyType * teardown: TeardownIndicator<unit> * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    /// <summary>
    /// Creates an ignored test with the specified test body without teardown.
    /// The test will be marked as ignored and will not be executed during test runs.
    /// The test name is automatically derived from the calling method name.
    /// </summary>
    /// <param name="test">The test body that would be executed if the test were not ignored</param>
    /// <param name="testName">The name of the test (automatically provided by CallerMemberName)</param>
    /// <param name="fileFullName">The full path of the source file (automatically provided by CallerFilePath)</param>
    /// <param name="lineNumber">The line number in the source file (automatically provided by CallerLineNumber)</param>
    /// <returns>An ITest instance representing the ignored test</returns>
    (*032*)abstract member Ignore: test: 'testBodyType * [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string * [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string * [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int -> ITest
    