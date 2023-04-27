module Archer.Arrows.Tests.RawTestObjects.``Arrow Should``

open Archer.Arrows
open Archer.Arrows.Internal
open Archer.CoreTypes.InternalTypes
open Archer.MicroLang

let private container = suite.Container()

let ``Create a Feature`` =
    container.Test (
        fun _ ->
            Arrow.NewFeature ()
            |> expects.ToBeOfType<Feature<unit, ITest>>
    )
    
let ``Create a feature with the correct names`` =
    container.Test (
        fun _ ->
            Arrow.NewFeature("FeaturePath", "FeatureName").ToString ()
            |> expects.ToBe "FeaturePath.FeatureName"
    )
    
let ``Create a feature with the correct but different names`` =
    container.Test (
        fun _ ->
            Arrow.NewFeature("My Path", "My Feature Name").ToString ()
            |> expects.ToBe "My Path.My Feature Name"
    )
    
let ``Test Cases`` = container.Tests