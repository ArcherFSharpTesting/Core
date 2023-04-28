module Archer.Arrows.Tests.``Arrow Tests``

open Archer.Arrows
open Archer.ShouldTypes

type private Thing = {
    UnitProp: unit
}

let private names =
    let t = typeof<Thing>
    t.Namespace, t.DeclaringType.Name
    
let path, name = names

let ``Test Cases`` =
    [
        Arrow.Tests (fun feature ->
            "Should create a feature with the module namespace and name"
            |> feature.AsTest (fun _ ->
                feature.ToString ()
                |> Should.BeEqualTo $"%s{path}.%s{name}"
            )
        )
        
        Arrow.Tests (
            "With a given feature name",
            fun feature ->
                "Should have a name passed to it"
                |> feature.AsTest (
                    fun _ ->
                        feature.ToString ()
                        |> Should.BeEqualTo $"%s{path}.With a given feature name"
                )
        )
        
        Arrow.Tests (
            $"%s{path}.{name}.Given Path",
            "With given path and name",
            fun feature ->
                "Should have the path and name passed to it"
                |> feature.AsTest (
                    fun _ ->
                        feature.ToString ()
                        |> Should.BeEqualTo $"%s{path}.{name}.Given Path.With given path and name"
                )
        )
    ]
    |> List.concat