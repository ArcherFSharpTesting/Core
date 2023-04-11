module Archer.Arrow.Tests.TestCase.Should

open Archer
open Archer.MicroLang
open Archer.Arrow.Internal

let container = suite.Container("TestCase", "Should")

let ``Test Cases`` = [
    container.Test ("have the test name", fun _ ->
        let expectedTestName = "My awesome test"
        let test = TestCase (ignoreString (), ignoreString (), expectedTestName, { Setup = None; TestAction = (fun _ _ -> TestSuccess); TearDown = None }, [], ignoreString (), ignoreInt ())
        
        test.TestName
        |> expectsToBe expectedTestName
    )
    
    container.Test ("have the container name", fun _ ->
        let expectedContainerName = "My Awesome Test Container name"
        let test = TestCase (ignoreString (), expectedContainerName, ignoreString (), { Setup = None; TestAction = (fun _ _ -> TestSuccess); TearDown = None }, [], ignoreString (), ignoreInt ())
        
        test.ContainerName
        |> expectsToBe expectedContainerName
    )
]