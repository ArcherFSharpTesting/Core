module Archer.Core.Tests.``FeatureFactory NewFeature With Teardown``

open Archer
open Archer.Core
open Archer.MicroLang.Verification

let private feature = FeatureFactory.NewFeature (
    TestTags [
        Category "FeatureFactory"
        Category "NewFeature"
    ]
)

let ``Should call teardown when it is the only thing specified`` =
    feature.Test (fun _ ->
        let monitor = getFeatureMonitor<unit> ()

        let testFeature = FeatureFactory.NewFeature (
            Teardown (monitor.FunctionTeardownWith ())
        )
        
        let test = testFeature.Test (fun _ -> TestSuccess)
        
        test
        |> silentlyRunTest
        
        monitor.HasTeardownBeenCalled
        |> Should.BeTrue
        |> withMessage "Setup didn't run"
    )

let ``Should call name, and teardown when are specified`` =
    feature.Test (fun _ ->
        let monitor = getFeatureMonitor<unit> ()

        let testFeature = FeatureFactory.NewFeature (
            "A feature",
            Teardown (monitor.FunctionTeardownWith ())
        )
        
        let test = testFeature.Test (fun _ -> TestSuccess)
        
        test
        |> silentlyRunTest
        
        monitor.HasTeardownBeenCalled
        |> Should.BeTrue
        |> withMessage "Setup didn't run"
    )

let ``Should call path, name, and teardown when are specified`` =
    feature.Test (fun _ ->
        let monitor = getFeatureMonitor<unit> ()

        let testFeature = FeatureFactory.NewFeature (
            "Path out",
            "A feature",
            Teardown (monitor.FunctionTeardownWith ())
        )
        
        let test = testFeature.Test (fun _ -> TestSuccess)
        
        test
        |> silentlyRunTest
        
        monitor.HasTeardownBeenCalled
        |> Should.BeTrue
        |> withMessage "Setup didn't run"
    )

let ``Should call teardown when setup, and teardown are specified`` =
    feature.Test (fun _ ->
        let monitor = getFeatureMonitor<unit> ()

        let testFeature = FeatureFactory.NewFeature (
            Setup (fun () -> Ok ()),
            Teardown (monitor.FunctionTeardownWith ())
        )
        
        let test = testFeature.Test (fun _ -> TestSuccess)
        
        test
        |> silentlyRunTest
        
        monitor.HasTeardownBeenCalled
        |> Should.BeTrue
        |> withMessage "Setup didn't run"
    )

let ``Should call teardown when name, setup, and teardown are specified`` =
    feature.Test (fun _ ->
        let monitor = getFeatureMonitor<unit> ()

        let testFeature = FeatureFactory.NewFeature (
            "My Feat Ure",
            Setup (fun () -> Ok ()),
            Teardown (monitor.FunctionTeardownWith ())
        )
        
        let test = testFeature.Test (fun _ -> TestSuccess)
        
        test
        |> silentlyRunTest
        
        monitor.HasTeardownBeenCalled
        |> Should.BeTrue
        |> withMessage "Setup didn't run"
    )

let ``Should call teardown when path, name, setup, and teardown are specified`` =
    feature.Test (fun _ ->
        let monitor = getFeatureMonitor<unit> ()

        let testFeature = FeatureFactory.NewFeature (
            "The root of",
            "Feature Creature",
            Setup (fun () -> Ok ()),
            Teardown (monitor.FunctionTeardownWith ())
        )
        
        let test = testFeature.Test (fun _ -> TestSuccess)
        
        test
        |> silentlyRunTest
        
        monitor.HasTeardownBeenCalled
        |> Should.BeTrue
        |> withMessage "Setup didn't run"
    )

let ``Test Cases`` = feature.GetTests ()

