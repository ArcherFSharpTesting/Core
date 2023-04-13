namespace Archer.Arrow

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