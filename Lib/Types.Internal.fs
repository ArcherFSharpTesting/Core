namespace Archer.Arrow.Internal

open Archer.CoreTypes
open Archer.CoreTypes.InternalTypes

type InitError =
    | General of string
    | Exception of exn

type TestParts<'a> = {
    Setup: (unit -> Result<'a, InitError>) option
    TestAction: unit -> 'a -> TestResult
    TearDown: (TestResult -> Result<'a, InitError> -> Result<unit, InitError>) option
}

type TestCase (filePath: string, containerPath: string, containerName: string, testName: string, lineNumber: int, tags: TestTag seq, parts: TestParts<'a>) =
    member _.TestName with get () = testName
    member _.ContainerName with get () = containerName