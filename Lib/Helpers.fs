[<AutoOpen>]
module Archer.Arrows.Helpers

open System
open System.Diagnostics
open Archer.Arrows.Internal

let private getNames () =
    let trace = StackTrace ()
    let method = trace.GetFrame(2).GetMethod ()
    let containerName = method.ReflectedType.Name
    let containerPath = method.ReflectedType.Namespace |> fun s -> s.Split ([|"$"|], StringSplitOptions.RemoveEmptyEntries) |> Array.last
            
    containerName, containerPath

type Arrow =
    static member NewFeature () =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName)
        
    static member NewFeature (featurePath, featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        Feature (featurePath, featureName, setup, teardown)

    static member NewFeature (featurePath, featureName, setup: SetupIndicator<unit, 'a>) =
        Arrow.NewFeature (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()))
        
    static member NewFeature (featurePath, featureName, teardown: TeardownIndicator<unit>) =
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()) , teardown)
        
    static member NewFeature (featurePath, featureName) =
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()))

    static member NewFeature (featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, setup, teardown)
        
    static member NewFeature (featureName, setup: SetupIndicator<unit, 'a>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()))
        
    static member NewFeature (featureName, teardown: TeardownIndicator<unit>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()), teardown)
        
    static member NewFeature featureName =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()))
        
    static member NewFeature (setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, setup, teardown)
        
    static member NewFeature (setup: SetupIndicator<unit, 'a>) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, setup)
        
    static member NewFeature (teardown: TeardownIndicator<unit>) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, teardown)