module Archer.Arrow.Tests.TestCase.Should

open Archer.CoreTypes
open Archer.MicroLang
open Archer.Arrow.Internal

let container = suite.Container("TestCase", "Should")

let ``Test Cases`` = [
    container.Test ("have the test name", fun () ->
        let expectedTestName = "My awesome test"
        let test = TestCase (ignoreString (), ignoreString (), ignoreString (), expectedTestName, ignoreInt (), [], { Setup = None; TestAction = (fun _ _ -> TestSuccess); TearDown = None })
        
        test.TestName
        |> expectsToBe expectedTestName
    )
]