module Archer.Core.Tests.RawTestObjects.``Feature Should``

open Archer.Core
open Archer.Core.Internals
open Archer.Core.Internal.Types
open Archer.Core.Helpers
open Archer.MicroLang

let private container = suite.Container ()

let ``have each part of its name dot seperated in the ToString`` =
    container.Test (
        fun _ ->
            let t = baseTransformer (Setup Ok) (emptyTeardown)
            let feature = Feature ("Path", "Name", [], t)
            feature.ToString ()
            |> expects.ToBe "Path.Name"
    )
    
let ``ignore empty path of the name in the ToString`` =
    container.Test (
        fun _ ->
            let t = baseTransformer (Setup Ok) (emptyTeardown)
            let feature = Feature ("", "My Name", [], t)
            feature.ToString ()
            |> expects.ToBe "My Name"
    )
    
let ``ignore empty name part of name in the ToString`` =
    container.Test (
        fun _ ->
            let t = baseTransformer (Setup Ok) (emptyTeardown)
            let feature = Feature ("A path", "", [], t)
            feature.ToString ()
            |> expects.ToBe "A path"
    )
    
let ``Test Cases`` = container.Tests