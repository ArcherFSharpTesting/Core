namespace Archer.Arrows

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers
open Archer.Arrows.Internal.Types
open Archer.Arrows.Internals
open Archer.CoreTypes.InternalTypes

type Sub =
    static member Feature (subFeatureName, setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>) =
        let buildIt (feature: Feature<'featureType>) =
            let builder = feature :> IBuilder<'featureType, ITest>
            
            let transformer (internals: TestInternals, executor: ISetupTeardownExecutor<'subFeatureType>) =
                let (Setup setup) = setup
                let (Teardown teardown) = teardown
                (internals, (WrappedTeardownExecutor (setup, teardown, executor) :> ISetupTeardownExecutor<'featureType>))
                |> builder.Add
            
            let subFeature = Feature<'subFeatureType> (feature.ToString (), subFeatureName, [], transformer)
            
            subFeature :> IFeature<'subFeatureType>
            
        buildIt
        
    static member Ignore (subFeatureName, setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let buildIt (feature: Feature<'featureType>) =
            let builder = feature :> IBuilder<'featureType, ITest>
            
            let transformer (internals: TestInternals, executor: ISetupTeardownExecutor<'subFeatureType>) =
                let (Setup setup) = setup
                let (Teardown teardown) = teardown
                (internals, (WrappedTeardownExecutor (setup, teardown, executor) :> ISetupTeardownExecutor<'featureType>))
                |> builder.Add
            
            let location = getLocation fileFullName lineNumber
            let subFeature = IgnoreFeature<'subFeatureType> (feature.ToString (), subFeatureName, [], transformer, location)
            
            subFeature :> IFeature<'subFeatureType>
            
        buildIt
        
    static member Feature (subFeatureName, setup: SetupIndicator<'featureType, 'subFeatureType>) =
        Sub.Feature (subFeatureName, setup, Teardown (fun _ _ -> Ok ()))
        
    static member Ignore (subFeatureName, setup: SetupIndicator<'featureType, 'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, setup, Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
        
    static member Feature (subFeatureName, teardown: TeardownIndicator<unit>) =
        Sub.Feature (subFeatureName, Setup (fun _ -> Ok ()), teardown)
        
    static member Ignore (subFeatureName, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, Setup (fun _ -> Ok ()), teardown, fileFullName, lineNumber)
        
    static member Feature subFeatureName =
        Sub.Feature (subFeatureName, Setup (fun _ -> Ok ()), Teardown (fun _ _ -> Ok ()))
        
    static member Ignore (subFeatureName, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, Setup (fun _ -> Ok ()), Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
        
    static member Feature (setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, setup, teardown)
        
    static member Ignore (setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, setup, teardown, fileFullName, lineNumber)
        
    static member Feature (setup: SetupIndicator<'featureType, 'subFeatureType>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, setup, Teardown (fun _ _ -> Ok ()))
        
    static member Ignore (setup: SetupIndicator<'featureType, 'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, setup, Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
        
    static member Feature (teardown: TeardownIndicator<unit>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, Setup (fun _ -> Ok ()), teardown)
        
    static member Ignore (teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, Setup (fun _ -> Ok ()), teardown, fileFullName, lineNumber)
        
    static member Feature () =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, Setup (fun _ -> Ok ()), Teardown (fun _ _ -> Ok ()))
        
    static member Ignore ([<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, Setup (fun _ -> Ok ()), Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
        
    static member Feature (subFeatureName, setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>, testBuilder: IScriptFeature<'subFeatureType> -> unit) =
        let buildIt (feature: IScriptFeature<'featureType>) =
            let builder =
                match feature with
                | :? IBuilder<'featureType, ITest> as builder -> builder
                | _ -> failwith "No Builder found"
            
            let transformer (internals: TestInternals, executor: ISetupTeardownExecutor<'subFeatureType>) =
                let (Setup setup) = setup
                let (Teardown teardown) = teardown
                (internals, (WrappedTeardownExecutor (setup, teardown, executor) :> ISetupTeardownExecutor<'featureType>))
                |> builder.Add
            
            let subFeature = Feature<'subFeatureType> (feature.ToString (), subFeatureName, [], transformer)
            
            subFeature :> IScriptFeature<'subFeatureType> |> testBuilder
            
        buildIt
        
    static member Ignore (subFeatureName, setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>, testBuilder: IScriptFeature<'subFeatureType> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let buildIt (feature: IScriptFeature<'featureType>) =
            let builder =
                match feature with
                | :? IBuilder<'featureType, ITest> as builder -> builder
                | _ -> failwith "No Builder found"
                
            let transformer (internals: TestInternals, executor: ISetupTeardownExecutor<'subFeatureType>) =
                let (Setup setup) = setup
                let (Teardown teardown) = teardown
                (internals, (WrappedTeardownExecutor (setup, teardown, executor) :> ISetupTeardownExecutor<'featureType>))
                |> builder.Add
                
            let location = getLocation fileFullName lineNumber
            let subFeature = IgnoreFeature<'subFeatureType> (feature.ToString (), subFeatureName, [], transformer, location)
            
            subFeature :> IScriptFeature<'subFeatureType> |> testBuilder
            
        buildIt
        
    static member Feature (subFeatureName, setup: SetupIndicator<'featureType, 'subFeatureType>, testBuilder: IScriptFeature<'subFeatureType> -> unit) =
        Sub.Feature (subFeatureName, setup, Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member Ignore (subFeatureName, setup: SetupIndicator<'featureType, 'subFeatureType>, testBuilder: IScriptFeature<'subFeatureType> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, setup, Teardown (fun _ _ -> Ok ()), testBuilder, fileFullName, lineNumber)
        
    static member Feature (subFeatureName, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit) =
        Sub.Feature (subFeatureName, Setup (fun _ -> Ok ()), teardown, testBuilder)
        
    static member Ignore (subFeatureName, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, Setup (fun _ -> Ok ()), teardown, testBuilder, fileFullName, lineNumber)
        
    static member Feature (subFeatureName, testBuilder: IScriptFeature<unit> -> unit) =
        Sub.Feature (subFeatureName, Setup (fun _ -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member Ignore (subFeatureName, testBuilder: IScriptFeature<unit> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, Setup (fun _ -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder, fileFullName, lineNumber)
        