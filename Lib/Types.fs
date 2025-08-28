namespace Archer.Arrows

open System
open Archer

/// <summary>
/// Represents the API environment for a test, including the API name and version.
/// </summary>
type ApiEnvironment = {
    /// <summary>The name of the API under test.</summary>
    ApiName: string
    /// <summary>The version of the API under test.</summary>
    ApiVersion: Version
}

/// <summary>
/// Represents the environment in which a test is run, including runner, API, and test info.
/// </summary>
type TestEnvironment = {
    /// <summary>The environment in which the test runner is executing.</summary>
    RunEnvironment: RunnerEnvironment
    /// <summary>The API environment for the test.</summary>
    ApiEnvironment: ApiEnvironment
    /// <summary>Information about the test being executed.</summary>
    TestInfo: ITestInfo
}

/// <summary>
/// Discriminated union representing a set of tags for a test.
/// </summary>
type TagsIndicator =
    /// <summary>Wraps a list of test tags.</summary>
    | TestTags of TestTag list

/// <summary>
/// Discriminated union representing setup logic for a test.
/// </summary>
/// <typeparam name="'a">The input type for the setup function.</typeparam>
/// <typeparam name="'b">The output type for the setup function.</typeparam>
type SetupIndicator<'a, 'b> =
    /// <summary>Wraps a setup function that returns a result.</summary>
    | Setup of ('a -> Result<'b, SetupTeardownFailure>)

/// <summary>
/// Discriminated union representing a sequence of data for parameterized tests.
/// </summary>
/// <typeparam name="'a">The type of the data elements.</typeparam>
type DataIndicator<'a> =
    /// <summary>Wraps a sequence of data.</summary>
    | Data of 'a seq

/// <summary>
/// Represents a test function that takes one parameter and returns a <see cref="TestResult"/>.
/// </summary>
type TestFunction<'a> = 'a -> TestResult

/// <summary>
/// Represents a test function that takes two parameters and returns a <see cref="TestResult"/>.
/// </summary>
type TestFunctionTwoParameters<'a, 'b> = 'a -> 'b -> TestResult

/// <summary>
/// Represents a test function that takes three parameters and returns a <see cref="TestResult"/>.
/// </summary>
type TestFunctionThreeParameters<'a, 'b, 'c> = 'a -> 'b -> 'c -> TestResult

/// <summary>
/// Discriminated union representing the body of a test.
/// </summary>
/// <typeparam name="'a">The type of the test body.</typeparam>
type TestBodyIndicator<'a> =
    /// <summary>Wraps the test body.</summary>
    | TestBody of 'a

/// <summary>
/// Discriminated union representing teardown logic for a test.
/// </summary>
/// <typeparam name="'a">The type of the teardown input.</typeparam>
type TeardownIndicator<'a> =
    /// <summary>Wraps a teardown function that takes the setup result and test result, and returns a result.</summary>
    | Teardown of (Result<'a, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>)
