module Archer.Arrows.Tests.RawTestObjects.``Arrow NewFeature`` 

open Archer
open Archer.Arrows
open Archer.Arrows.Internals
open Archer.Arrows.Internal.Types
open Archer.MicroLang

let private feature = Arrow.NewFeature (
    TestTags [
        Category "Arrow"
        Category "NewFeature"
    ]
)

let private exampleWithSetup = Arrow.NewFeature (
    Setup (fun () -> Ok ())
)

let private exampleWithTeardown = Arrow.NewFeature (
    emptyTeardown
)

let private exampleWithBoth = Arrow.NewFeature (
    Setup (fun () -> Ok ()),
    emptyTeardown
)

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
    
let ``Should Create a Feature with the name of the containing module even if setup added`` =
    feature.Test (
        fun _ ->
            let containerPath, containerName = names
            exampleWithSetup.ToString ()
            |> Should.BeEqualTo $"%s{containerPath}.%s{containerName}"
    )
    
let ``Should Create a Feature with the name of the containing module even if teardown added`` =
    feature.Test (
        fun _ ->
            let containerPath, containerName = names
            exampleWithTeardown.ToString ()
            |> Should.BeEqualTo $"%s{containerPath}.%s{containerName}"
    )
    
let ``Should Create a Feature with the name of the containing module even if both setup and teardown added`` =
    feature.Test (
        fun _ ->
            let containerPath, containerName = names
            exampleWithTeardown.ToString ()
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
    
let ``Should Create a Feature with the name, path, and setup given to it`` =
    feature.Test (
        fun _ ->
            let path = "This feature's path"
            let name = "This feature's name"
            let testFeature = Arrow.NewFeature (
                path,
                name,
                Setup (fun () -> Ok ())
            )
            
            testFeature.ToString ()
            |> Should.BeEqualTo $"%s{path}.%s{name}"
    )
    
let ``Should Create a Feature with the name, path, and teardown given to it`` =
    feature.Test (
        fun _ ->
            let path = "This feature's path"
            let name = "This feature's name"
            let testFeature = Arrow.NewFeature (
                path,
                name,
                emptyTeardown
            )
            
            testFeature.ToString ()
            |> Should.BeEqualTo $"%s{path}.%s{name}"
    )
    
let ``Should Create a Feature with the name, path, setup and teardown given to it`` =
    feature.Test (
        fun _ ->
            let path = "This feature's path"
            let name = "This feature's name"
            let testFeature = Arrow.NewFeature (
                path,
                name,
                Setup (fun () -> Ok ()),
                emptyTeardown
            )
            
            testFeature.ToString ()
            |> Should.BeEqualTo $"%s{path}.%s{name}"
    )

let ``Should Create a feature with the name given to it`` =
    feature.Test (
        fun _ ->
            let name = "A specified name"
            let testFeature = Arrow.NewFeature name
            
            testFeature.ToString ()
            |> Should.PassTestOf <@fun featureName ->
                featureName.EndsWith $".%s{name}"
            @>
    )

let ``Should Create a feature with the name and setup given to it`` =
    feature.Test (
        fun _ ->
            let name = "A specified name"
            let testFeature = Arrow.NewFeature (
                name,
                Setup (fun () -> Ok ())
            )
            
            testFeature.ToString ()
            |> Should.PassTestOf <@fun featureName ->
                featureName.EndsWith $".%s{name}"
            @>
    )

let ``Should Create a feature with the name and teardown given to it`` =
    feature.Test (
        fun _ ->
            let name = "A specified name"
            let testFeature = Arrow.NewFeature (
                name,
                emptyTeardown
            )
            
            testFeature.ToString ()
            |> Should.PassTestOf <@fun featureName ->
                featureName.EndsWith $".%s{name}"
            @>
    )

let ``Should Create a feature with the name, setup and teardown given to it`` =
    feature.Test (
        fun _ ->
            let name = "A specified name"
            let testFeature = Arrow.NewFeature (
                name,
                Setup (fun () -> Ok ()),
                emptyTeardown
            )
            
            testFeature.ToString ()
            |> Should.PassTestOf <@fun featureName ->
                featureName.EndsWith $".%s{name}"
            @>
    )

let ``Test Cases`` = feature.GetTests ()