module Archer.Arrows.Tests.``Arrow Tests``

open Archer
open Archer.Arrows
open Archer.MicroLang.Verification

type private Thing = {
    UnitProp: unit
}

let private names =
    let t = typeof<Thing>
    t.Namespace, t.DeclaringType.Name
    
let path, name = names

let ``Test Cases`` =
    [
        Arrow.Tests (
            TestTags [
                Category "Arrow"
                Category "Tests"
            ],
            fun feature ->
                "Should create a feature with the module namespace and name"
                |> feature.IsTestedBy (fun _ ->
                    feature.ToString ()
                    |> Should.BeEqualTo $"%s{path}.%s{name}"
                )
                
                "Should run every test specified in the builder"
                |> feature.IsTestedBy (fun _ ->
                    let monitor = Monitor<unit, unit, unit> (Ok ())
                    let tests =
                        Arrow.Tests (
                            fun f ->
                                "A" |> f.IsTestedBy monitor.CallTestActionWithSetupEnvironment
                                "B" |> f.IsTestedBy monitor.CallTestActionWithSetupEnvironment
                                "3" |> f.IsTestedBy monitor.CallTestActionWithSetupEnvironment
                                "30" |> f.IsTestedBy monitor.CallTestActionWithSetupEnvironment
                        )
                        |> List.map ((fun tst -> tst.GetExecutor ()) >> executeFunction >> runIt)
                    
                    
                    monitor.NumberOfTimesTestWasCalled
                    |> Should.BeEqualTo tests.Length
                    |> withMessage "Incorrect number of tests were run."
                )
        )
        
        Arrow.Tests (
            "With a given feature name",
            TestTags [
                Category "Arrow"
                Category "Tests"
            ],
            fun feature ->
                "Should have a name passed to it"
                |> feature.IsTestedBy (
                    fun _ ->
                        feature.ToString ()
                        |> Should.BeEqualTo $"%s{path}.With a given feature name"
                )
        )
        
        Arrow.Tests (
            $"%s{path}.{name}.Given Path",
            "With given path and name",
            TestTags [
                Category "Arrow"
                Category "Tests"
            ],
            fun feature ->
                "Should have the path and name passed to it"
                |> feature.IsTestedBy (
                    fun _ ->
                        feature.ToString ()
                        |> Should.BeEqualTo $"%s{path}.{name}.Given Path.With given path and name"
                )
        )
    ]
    |> List.concat