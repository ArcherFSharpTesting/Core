module Archer.Arrow.Tests.RawTestObjects.``TestCaseExecutor Should``

open Archer
open Archer.Arrow.Internal
open Archer.Arrow.Tests
open Archer.MicroLang
open Archer.MicroLang.Types

let private container = suite.Container ()

let ``Run the setup action when execute is called`` =
    container.Test
        (fun _ ->
            let mutable wasRun = false
            let setupAction _ =
                wasRun <- true
                Ok ()
                
            let executor = TestCaseExecutor (getEmptyDummyTest (), setupAction, successfulEnvironmentTest, successfulTeardown)
            
            executor.Execute (getEmptyFrameworkEnvironment ())
            |> ignore
                
            wasRun
            |> expects.ToBeTrue
            |> withMessage "Test did not run"
        )

let ``Test Cases`` = container.Tests