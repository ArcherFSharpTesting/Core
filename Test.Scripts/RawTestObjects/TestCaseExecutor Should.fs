module Archer.Arrow.Tests.RawTestObjects.``TestCaseExecutor Should``

open Archer
open Archer.Arrow.Internal
open Archer.Arrow.Tests
open Archer.MicroLang
open Archer.MicroLang.Types

let private container = suite.Container ()

let ``Execute the test action when run`` =
    container.Test
        (fun _ ->
            let mutable testRun = false
            let testAction _ _ =
                testRun <- true
                TestSuccess
                
            let executor = TestCaseExecutor (getEmptyDummyTest (), successfulUnitSetup, testAction, successfulTeardown)
            
            executor.Execute (getEmptyFrameworkEnvironment ())
            |> ignore
                
            testRun
            |> expects.ToBeTrue
            |> withMessage "Test did not run"
        )

let ``Test Cases`` = container.Tests