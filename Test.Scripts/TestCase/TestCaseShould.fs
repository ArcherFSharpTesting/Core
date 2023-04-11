module Archer.Arrow.Tests.TestCase.Should

open Archer
open Archer.MicroLang
open Archer.Arrow.Internal

let container = suite.Container("TestCase", "Should")

let ``Test Cases`` = [
    container.Test ("have the test name", fun _ ->
        let expectedTestName = "My awesome test"
        let test = TestCase (ignoreString (), ignoreString (), ignoreString (), expectedTestName, ignoreInt (), [], { Setup = None; TestAction = (fun _ _ -> TestSuccess); TearDown = None })
        
        test.TestName
        |> expectsToBe expectedTestName
    )
    
    container.Test ("have the container name", fun _ ->
        let expectedContainerName = "My Awesome Test Container name"
        let test = TestCase (ignoreString (), ignoreString (), expectedContainerName, ignoreString (), ignoreInt (), [], { Setup = None; TestAction = (fun _ _ -> TestSuccess); TearDown = None })
        
        test.ContainerName
        |> expectsToBe expectedContainerName
    )
]