[<AutoOpen>]
module Archer.Arrows.Helpers

open System
open System.Diagnostics
open Archer.Arrows.Internal
open Archer.CoreTypes.InternalTypes

let private getNamesAt frame = 
    let trace = StackTrace ()
    let method = trace.GetFrame(frame).GetMethod ()
    let containerName = method.ReflectedType.Name
    let containerPath = method.ReflectedType.Namespace |> fun s -> s.Split ([|"$"|], StringSplitOptions.RemoveEmptyEntries) |> Array.last
            
    containerName, containerPath

let private getNames () =
    getNamesAt 3

type Arrow =
    // ------- featurePath -------
    static member NewFeature (featurePath, featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let t = baseTransformer setup teardown
        Feature (featurePath, featureName, t)
        
    static member NewFeature (featurePath, featureName, setup: SetupIndicator<unit, 'a>) =
        Arrow.NewFeature (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()))
        
    static member NewFeature (featurePath, featureName, teardown: TeardownIndicator<unit>) =
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()) , teardown)
        
    static member NewFeature (featurePath, featureName) =
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()))

    // ------- featureName -------
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

    // ------- setup -------
    static member NewFeature (setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, setup, teardown)
        
    static member NewFeature (setup: SetupIndicator<unit, 'a>) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, setup)
        
    // ------- teardown -------
    static member NewFeature (teardown: TeardownIndicator<unit>) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, teardown)
        
    // ------- () -------
    static member NewFeature () =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName)
        
        
        
        
    // ------- featurePath -------
    static member Tests (featurePath, featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, testBuilder: IScriptFeature<'a> -> unit) =
        let feature = Arrow.NewFeature (featurePath, featureName, setup, teardown)
        feature :> IScriptFeature<'a> |> testBuilder 
        feature.GetTests ()
        
    static member Tests (featurePath, featureName, setup: SetupIndicator<unit, 'a>, testBuilder: IScriptFeature<'a> -> unit) =
        Arrow.Tests (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member Tests (featurePath, featureName, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit) =
        Arrow.Tests (featurePath, featureName, Setup (fun () -> Ok ()), teardown, testBuilder)
        
    static member Tests (featurePath, featureName, testBuilder: IScriptFeature<unit> -> unit) =
        Arrow.Tests (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder)
        
    // ------- featureName -------
    static member Tests (featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, testBuilder: IScriptFeature<'a> -> unit) =
        let _, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, setup, teardown, testBuilder)
        
    static member Tests (featureName, setup: SetupIndicator<unit, 'a>, testBuilder: IScriptFeature<'a> -> unit) =
        let _, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member Tests (featureName, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit) =
        let _, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, Setup (fun () -> Ok ()), teardown, testBuilder)
        
    static member Tests (featureName, testBuilder: IScriptFeature<unit> -> unit) =
        let _, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder)
        
    // ------- setup -------
    static member Tests (setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, testBuilder: IScriptFeature<'a> -> unit) =
        let featureName, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, setup, teardown, testBuilder)
        
    static member Tests (setup: SetupIndicator<unit, 'a>, testBuilder: IScriptFeature<'a> -> unit) =
        let featureName, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()), testBuilder)
        
    // ------- teardown -------
    static member Tests (teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit) =
        let featureName, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, Setup (fun () -> Ok ()), teardown, testBuilder)
        
    // ------- testBuilder -------
    static member Tests (testBuilder: IScriptFeature<unit> -> unit) =
        let featureName, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder)
        
type Sub =
    static member Feature (subFeatureName, setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>) =
        let buildIt (feature: Feature<'featureType>) =
            let builder = feature :> IBuilder<'featureType, ITest>
            
            let transformer (internals: TestInternals, executor: ISetupTeardownExecutor<'subFeatureType>) =
                let (Setup setup) = setup
                let (Teardown teardown) = teardown
                (internals, (WrappedTeardownExecutor (setup, teardown, executor) :> ISetupTeardownExecutor<'featureType>))
                |> builder.Add
            
            let subFeature = Feature<'subFeatureType> (feature.ToString (), subFeatureName, transformer)
            
            subFeature
            
        buildIt
        
    static member Feature (subFeatureName, setup: SetupIndicator<'featureType, 'subFeatureType>) =
        Sub.Feature (subFeatureName, setup, Teardown (fun _ _ -> Ok ()))
        
    static member Feature (subFeatureName, teardown: TeardownIndicator<unit>) =
        Sub.Feature (subFeatureName, Setup (fun _ -> Ok ()), teardown)
        
    static member Feature subFeatureName =
        Sub.Feature (subFeatureName, Setup (fun _ -> Ok ()), Teardown (fun _ _ -> Ok ()))
        
    static member Feature (setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, setup, teardown)
        
    static member Feature (setup: SetupIndicator<'featureType, 'subFeatureType>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, setup, Teardown (fun _ _ -> Ok ()))
        
    static member Feature (teardown: TeardownIndicator<unit>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, Setup (fun _ -> Ok ()), teardown)
        
    static member Feature () =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, Setup (fun _ -> Ok ()), Teardown (fun _ _ -> Ok ()))
        
    static member Feature (subFeatureName, setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>, testBuilder: IScriptFeature<'subFeatureType> -> unit) =
        let buildIt (feature: Feature<'featureType>) =
            let builder = feature :> IBuilder<'featureType, ITest>
            
            let transformer (internals: TestInternals, executor: ISetupTeardownExecutor<'subFeatureType>) =
                let (Setup setup) = setup
                let (Teardown teardown) = teardown
                (internals, (WrappedTeardownExecutor (setup, teardown, executor) :> ISetupTeardownExecutor<'featureType>))
                |> builder.Add
            
            let subFeature = Feature<'subFeatureType> (feature.ToString (), subFeatureName, transformer)
            
            subFeature :> IScriptFeature<'subFeatureType> |> testBuilder
            
        buildIt
        
    static member Feature (subFeatureName, setup: SetupIndicator<'featureType, 'subFeatureType>, testBuilder: IScriptFeature<'subFeatureType> -> unit) =
        Sub.Feature (subFeatureName, setup, Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member Feature (subFeatureName, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit) =
        Sub.Feature (subFeatureName, Setup (fun _ -> Ok ()), teardown, testBuilder)
        
    static member Feature (subFeatureName, testBuilder: IScriptFeature<unit> -> unit) =
        Sub.Feature (subFeatureName, Setup (fun _ -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder)