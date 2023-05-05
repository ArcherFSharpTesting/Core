module Archer.Arrows.Tests.``Arrow Tests With Setup``

open Archer
open Archer.Arrows
open Archer.MicroLang.Verification

let private feature = Arrow.NewFeature (
    TestTags [
        Category "Arrow"
        Category "Tests"
    ]
)
    
let ``Should execute the setup by every test specified in the test builder when only setup and test builder are specified`` =
    feature.Test (
        fun _ ->
            let monitor = Monitor<unit, unit> (Ok ())
            
            let tests =
                Arrow.Tests (
                    Setup monitor.CallSetup,
                    fun f ->
                        "Test A" |> f.IsTestedBy (fun _ -> TestSuccess)
                        "Test B" |> f.IsTestedBy (fun _ -> TestSuccess)
                        "Test C" |> f.IsTestedBy (fun _ -> TestSuccess)
                )
                |> List.map ((fun tst -> tst.GetExecutor ()) >> executeFunction >> runIt)
            
            monitor.NumberOfTimesSetupWasCalled
            |> Should.BeEqualTo tests.Length
            |> withMessage "Setup was called incorrect number of times"
    )
    
let ``Should execute every test specified in the test builder when only setup and test builder are specified`` =
    feature.Test (
        fun _ ->
            let monitor = Monitor<unit, unit> (Ok ())
            
            let tests =
                Arrow.Tests (
                    Setup monitor.CallSetup,
                    fun f ->
                        "Test A" |> f.IsTestedBy monitor.CallTestActionWithEnvironment
                        "Test B" |> f.IsTestedBy monitor.CallTestActionWithEnvironment
                        "Test C" |> f.IsTestedBy monitor.CallTestActionWithEnvironment
                )
                |> List.map ((fun tst -> tst.GetExecutor ()) >> executeFunction >> runIt)
            
            monitor.NumberOfTimesTestWasCalled
            |> Should.BeEqualTo tests.Length
            |> withMessage "Setup was called incorrect number of times"
    )
    
let ``Should execute the setup by every test specified in the test builder when name, setup, and test builder are specified`` =
    feature.Test (
        fun _ ->
            let monitor = Monitor<unit, unit> (Ok ())
            
            let tests =
                Arrow.Tests (
                    "My Feature Name",
                    Setup monitor.CallSetup,
                    fun f ->
                        "Test A" |> f.IsTestedBy (fun _ -> TestSuccess)
                        "Test B" |> f.IsTestedBy (fun _ -> TestSuccess)
                        "Test C" |> f.IsTestedBy (fun _ -> TestSuccess)
                )
                |> List.map ((fun tst -> tst.GetExecutor ()) >> executeFunction >> runIt)
            
            monitor.NumberOfTimesSetupWasCalled
            |> Should.BeEqualTo tests.Length
            |> withMessage "Setup was called incorrect number of times"
    )
    
let ``Should execute every test specified in the test builder when name, setup, and test builder are specified`` =
    feature.Test (
        fun _ ->
            let monitor = Monitor<unit, unit> (Ok ())
            
            let tests =
                Arrow.Tests (
                    "My Feature Name",
                    Setup monitor.CallSetup,
                    fun f ->
                        "Test A" |> f.IsTestedBy monitor.CallTestActionWithEnvironment
                        "Test B" |> f.IsTestedBy monitor.CallTestActionWithEnvironment
                        "Test C" |> f.IsTestedBy monitor.CallTestActionWithEnvironment
                )
                |> List.map ((fun tst -> tst.GetExecutor ()) >> executeFunction >> runIt)
            
            monitor.NumberOfTimesTestWasCalled
            |> Should.BeEqualTo tests.Length
            |> withMessage "Setup was called incorrect number of times"
    )
    
let ``Should execute the setup by every test specified in the test builder when path, name, setup, and test builder are specified`` =
    feature.Test (
        fun _ ->
            let monitor = Monitor<unit, unit> (Ok ())
            
            let tests =
                Arrow.Tests (
                    "A path to",
                    "My Feature",
                    Setup monitor.CallSetup,
                    fun f ->
                        "Test A" |> f.IsTestedBy (fun _ -> TestSuccess)
                        "Test B" |> f.IsTestedBy (fun _ -> TestSuccess)
                        "Test C" |> f.IsTestedBy (fun _ -> TestSuccess)
                )
                |> List.map ((fun tst -> tst.GetExecutor ()) >> executeFunction >> runIt)
            
            monitor.NumberOfTimesSetupWasCalled
            |> Should.BeEqualTo tests.Length
            |> withMessage "Setup was called incorrect number of times"
    )
    
let ``Should execute every test specified in the test builder when path, name, setup, and test builder are specified`` =
    feature.Test (
        fun _ ->
            let monitor = Monitor<unit, unit> (Ok ())
            
            let tests =
                Arrow.Tests (
                    "A path to",
                    "My Feature",
                    Setup monitor.CallSetup,
                    fun f ->
                        "Test A" |> f.IsTestedBy monitor.CallTestActionWithEnvironment
                        "Test B" |> f.IsTestedBy monitor.CallTestActionWithEnvironment
                        "Test C" |> f.IsTestedBy monitor.CallTestActionWithEnvironment
                )
                |> List.map ((fun tst -> tst.GetExecutor ()) >> executeFunction >> runIt)
            
            monitor.NumberOfTimesTestWasCalled
            |> Should.BeEqualTo tests.Length
            |> withMessage "Setup was called incorrect number of times"
    )

let ``Test Cases`` = feature.GetTests ()