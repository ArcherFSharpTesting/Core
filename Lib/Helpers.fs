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
        
    static member NewFeature (featurePath, featureName) =
        Feature (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()))

    static member NewFeature<'a> (setup: SetupIndicator<'a>) =
        let featureName, featurePath = getNames ()
        Feature<'a> (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()))
        
    static member NewFeature<'a> (setup: SetupIndicator<'a>, teardown: TeardownIndicator<'a>) =
        let featureName, featurePath = getNames ()
        Feature<'a> (featurePath, featureName, setup, teardown)