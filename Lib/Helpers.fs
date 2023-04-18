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
        Feature (featurePath, featureName)
        
    static member NewFeature (featurePath, featureName) =
        Feature (featurePath, featureName)
        
    static member NewFeature (feature: Feature) =
        let featureName, _ = getNames ()
        feature.SubFeature featureName