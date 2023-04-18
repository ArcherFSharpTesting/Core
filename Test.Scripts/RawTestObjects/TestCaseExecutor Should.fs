module Archer.Arrows.Tests.RawTestObjects.``TestCaseExecutor Should``

open Archer.Arrows
open Archer.MicroLang

let private container = suite.Container ()

let ``Have a decent toString`` =
    container.Test (
        fun _ ->
            let feature = Arrow.NewFeature ("TestCase", "Executor")
            let test = feature.Test (successfulTest, "ToString should return a string")
            let executor = test.GetExecutor ()
            
            executor.ToString ()
            |> expects.ToBe "TestCase.Executor.ToString should return a string.IExecutor"
    )

let ``Test Cases`` = container.Tests