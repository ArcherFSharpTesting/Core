namespace Archer.Arrows

open System
open Archer

type ApiEnvironment = {
    ApiName: string
    ApiVersion: Version
}

type TestEnvironment = {
    FrameworkEnvironment: FrameworkEnvironment
    ApiEnvironment: ApiEnvironment
    TestInfo: ITestInfo
}

type TestBodyWithEnvironment<'a> = 'a -> TestEnvironment -> TestResult
type TestFunction<'a> = 'a -> TestResult
type SetupIndicator<'a, 'b> = | Setup of ('a -> Result<'b, SetupTeardownFailure>)
type TestBodyWithEnvironmentIndicator<'a> = | TestWithEnvironmentBody of TestBodyWithEnvironment<'a> 
type TestBodyIndicator<'a> = | TestBody of TestFunction<'a> 
type TeardownIndicator<'a> = | Teardown of (Result<'a, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>)
type TagsIndicator = | TestTags of TestTag list