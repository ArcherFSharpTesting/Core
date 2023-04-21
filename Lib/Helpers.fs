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
        
type Sub =
    static member Feature<'a, 'b> (name: string, setup: SetupIndicator<'b>, teardown: TeardownIndicator<'b>) =
        let build (feature: TypedFeature<'a>) =
            if String.IsNullOrWhiteSpace name then
                failwith "Must have a name"
            else
                TypedFeature (feature.ToString (), name, setup, teardown)
                
        build
        
    static member Feature<'a, 'b> (name: string, setup: SetupIndicator<'b>) =
        Sub.Feature (name, setup, Teardown (fun _ _ -> Ok ()))
        
    static member Feature<'a> (name: string) =
        let build (feature: TypedFeature<'a>) =
            if String.IsNullOrWhiteSpace name then
                failwith "Must have a name"
            else
                Feature (feature.ToString (), name)
                
        build
        
    static member Feature (name: string) =
        let build (feature: Feature) =
            if String.IsNullOrWhiteSpace name then
                failwith "Must have a name"
            else
                Feature (feature.ToString (), name)
                
        build

type Arrow =
    static member NewFeature () =
        let featureName, featurePath = getNames ()
        Feature (featurePath, featureName)
        
    static member NewFeature (featurePath, featureName) =
        Feature (featurePath, featureName)
        
    static member NewFeature (feature: Feature) =
        let featureName, _ = getNames ()
        feature
        |> Sub.Feature featureName

    static member NewFeature<'a> (setup: SetupIndicator<'a>) =
        let featureName, featurePath = getNames ()
        TypedFeature<'a> (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()))
        
    static member NewFeature<'a> (setup: SetupIndicator<'a>, teardown: TeardownIndicator<'a>) =
        let featureName, featurePath = getNames ()
        TypedFeature<'a> (featurePath, featureName, setup, teardown)