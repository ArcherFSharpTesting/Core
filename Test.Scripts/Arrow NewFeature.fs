module Archer.Arrows.Tests.RawTestObjects.``Arrow NewFeature`` 

open Archer
open Archer.Arrows
open Archer.Arrows.Internal
open Archer.MicroLang
open Archer.Arrows.Tests

let private feature = Arrow.NewFeature ()

type private Thing = {
    UnitProp: unit
}

let private names =
    let t = typeof<Thing>
    t.Namespace, t.DeclaringType.Name

let ``Should Create a Feature`` =
    feature.Test (
        fun _ ->
            feature
            |> expects.ToBeOfType<Feature<unit>>
    )
    
let ``Should Create a Feature with the name of the containing module`` =
    feature.Test (
        fun _ ->
            let containerPath, containerName = names
            feature.ToString ()
            |> Should.BeEqualTo $"%s{containerPath}.%s{containerName}"
    )
    
let ``Should Create a Feature with the name and path given to it`` =
    feature.Test (
        fun _ ->
            let path = "This feature's path"
            let name = "This feature's name"
            let testFeature = Arrow.NewFeature (path, name)
            
            testFeature.ToString ()
            |> Should.BeEqualTo $"%s{path}.%s{name}"
    )

let ``Should Create a feature with the name given to it`` =
    feature.Test (
        fun _ ->
            let name = "A specified name"
            let testFeature = Arrow.NewFeature name
            
            testFeature.ToString ()
            |> Should.PassTestOf (fun featureName ->
                featureName.EndsWith $".%s{name}"
            )
    )

let ``Test Cases`` = feature.GetTests ()