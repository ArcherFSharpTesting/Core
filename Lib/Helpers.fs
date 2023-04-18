[<AutoOpen>]
module Archer.Arrows.Helpers

open System
open System.Diagnostics
open Archer.Arrows.Internal

type Arrow =
    static member NewFeature () =
        let featureName, featurePath =
            let trace = StackTrace ()
            let method = trace.GetFrame(1).GetMethod ()
            let containerName = method.ReflectedType.Name
            let containerPath = method.ReflectedType.Namespace |> fun s -> s.Split ([|"$"|], StringSplitOptions.RemoveEmptyEntries) |> Array.last
                
            containerName, containerPath
            
        Feature (featurePath, featureName)
        
    static member NewFeature (featurePath, featureName) =
        Feature (featurePath, featureName)