namespace Archer.Arrow.Internal

open Archer
open Archer.CoreTypes.InternalTypes

type InitError =
    | General of string
    | Exception of exn

type TestParts<'a> = {
    Setup: (unit -> Result<'a, InitError>) option
    TestAction: unit -> 'a -> TestResult
    TearDown: (TestResult -> Result<'a, InitError> -> Result<unit, InitError>) option
}

type TestCase (containerPath: string, containerName: string, testName: string, parts: TestParts<'a>, tags: TestTag seq, filePath: string, lineNumber: int) =
    member _.TestName with get () = testName
    member _.ContainerName with get () = containerName