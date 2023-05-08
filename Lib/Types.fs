namespace Archer.Arrows

open System
open Archer

type ApiEnvironment = {
    ApiName: string
    ApiVersion: Version
}

type TestEnvironment = {
    RunEnvironment: RunnerEnvironment
    ApiEnvironment: ApiEnvironment
    TestInfo: ITestInfo
}

type TagsIndicator = | TestTags of TestTag list

type SetupIndicator<'a, 'b> = | Setup of ('a -> Result<'b, SetupTeardownFailure>)

type TestFunction<'a> = 'a -> TestResult
type TestFunctionTwoParameters<'a, 'b> = 'a -> 'b -> TestResult

type TestBodyIndicator<'a> = | TestBody of TestFunction<'a> 
type TestBodyIndicatorWithTwoParameters<'a, 'b> = | TestWithEnvironmentBody of TestFunctionTwoParameters<'a, 'b> 

type TeardownIndicator<'a> = | Teardown of (Result<'a, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>)
