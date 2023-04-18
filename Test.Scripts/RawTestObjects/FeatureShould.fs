module Archer.Arrows.Tests.RawTestObjects.``Feature Should``

open Archer.Arrows
open Archer.Arrows.Internal
open Archer.MicroLang

let private container = suite.Container ()

let ``have each part of its name dot seperated in the ToString`` =
    container.Test (
        fun _ ->
            let feature = Feature ("Path", "Name")
            feature.ToString ()
            |> expects.ToBe "Path.Name"
    )
    
let ``ignore empty path of the name in the ToString`` =
    container.Test (
        fun _ ->
            let feature = Feature ("", "My Name")
            feature.ToString ()
            |> expects.ToBe "My Name"
    )
    
let ``ignore empty name part of name in the ToString`` =
    container.Test (
        fun _ ->
            let feature = Feature ("A path", "")
            feature.ToString ()
            |> expects.ToBe "A path"
    )
    
let ``create sub feature`` =
    container.Test (
        fun _ ->
            let feature = Feature ("My Really Cool", "Feature")
            let feature = feature.SubFeature ("Sub Feature")
            
            feature.ToString ()
            |> expects.ToBe ("My Really Cool.Feature.Sub Feature")
    )

let ``Test Cases`` = container.Tests