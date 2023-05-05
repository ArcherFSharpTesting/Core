module Archer.Arrows.Tests.RawTestObjects.``TestCaseExecutor Should``

open Archer
open Archer.Arrows
open Archer.MicroLang

let private container = Arrow.NewFeature (
    TestTags [
        Category "TestCaseExecutor"
    ]
)

let ``Have a decent toString`` =
    container.Test (
        fun _ ->
            let feature = Arrow.NewFeature ("TestCase", "Executor")
            let test = feature.Test (successfulTest, "ToString should return a string")
            let executor = test.GetExecutor ()
            
            executor.ToString ()
            |> expects.ToBe "TestCase.Executor.ToString should return a string.IExecutor"
    )

let ``Test Cases`` = container.GetTests ()