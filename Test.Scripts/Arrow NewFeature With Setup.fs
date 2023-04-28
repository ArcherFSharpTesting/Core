module Archer.Arrows.Tests.``Arrow NewFeature With Setup``

open Archer
open Archer.Arrows
open Archer.MicroLang.Verification
open Archer.ShouldTypes

let private feature = Arrow.NewFeature ()

let ``Should call setup when it is the only thing passed to it`` =
    feature.Test (
        fun _ ->
            let monitor = Monitor (Ok ())
                
            let testFeature = Arrow.NewFeature (
                Setup monitor.CallSetup
            )
            
            let test = testFeature.Test (fun _ -> TestSuccess)
            
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.SetupWasCalled
            |> Should.BeEqualTo true
            |> withMessage "Setup didn't run"
    )
    
let ``Should call setup when it is passed with a teardown`` =
    feature.Test (
        fun _ ->
            let monitor = Monitor (Ok ())
            
            let testFeature = Arrow.NewFeature (
                Setup monitor.CallSetup,
                Teardown (fun _ _ -> Ok ())
            )
            
            let test = testFeature.Test (fun _ -> TestSuccess)
            
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.SetupWasCalled
            |> Should.BeEqualTo true
            |> withMessage "Setup didn't run"
    )

let ``Test Cases`` = feature.GetTests ()