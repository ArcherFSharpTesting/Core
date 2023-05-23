namespace Archer.Arrows

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers
open Archer.Arrows.Internal.Types
open Archer.Arrows.Internals
open Archer.CoreTypes.InternalTypes

type Sub =
    static member Feature (subFeatureName, TestTags featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>) =
        let buildIt (feature: IFeature<'featureType>) =
            let builder =
                match feature with
                | :? IBuilder<'featureType, ITest> as builder -> builder
                | _ -> failwith "No Builder found"
                
            let transformer (internals: TestInternals, executor: ISetupTeardownExecutor<'subFeatureType>) =
                let (Setup setup) = setup
                let (Teardown teardown) = teardown
                (internals, (WrappedTeardownExecutor (setup, teardown, executor) :> ISetupTeardownExecutor<'featureType>))
                |> builder.Add
            
            let subFeature = Feature<'subFeatureType> (feature.ToString (), subFeatureName, [feature.FeatureTags; featureTags] |> List.concat, transformer)
            
            subFeature :> IFeature<'subFeatureType>
            
        buildIt
        
    static member Ignore (subFeatureName, TestTags featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let buildIt (feature: IFeature<'featureType>) =
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
            let subFeature = IgnoreFeature<'subFeatureType> (feature.ToString (), subFeatureName, [feature.FeatureTags; featureTags] |> List.concat, transformer, location)
            
            subFeature :> IFeature<'subFeatureType>
            
        buildIt
        
    static member Feature (subFeatureName, setup: SetupIndicator<'featureType, 'subFeatureType>) =
        Sub.Feature (subFeatureName, TestTags [], setup, emptyTeardown)
        
    static member Feature (subFeatureName, featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>) =
        Sub.Feature (subFeatureName, featureTags, setup, emptyTeardown)
        
    static member Ignore (subFeatureName, setup: SetupIndicator<'featureType, 'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, TestTags [], setup, emptyTeardown, fileFullName, lineNumber)
        
    static member Ignore (subFeatureName, featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, featureTags, setup, emptyTeardown, fileFullName, lineNumber)
        
    static member Feature (subFeatureName, teardown: TeardownIndicator<unit>) =
        Sub.Feature (subFeatureName, TestTags [], Setup Ok, teardown)
        
    static member Feature (subFeatureName, featureTags, teardown: TeardownIndicator<unit>) =
        Sub.Feature (subFeatureName, featureTags, Setup Ok, teardown)
        
    static member Ignore (subFeatureName, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, TestTags [], Setup Ok, teardown, fileFullName, lineNumber)
        
    static member Ignore (subFeatureName, featureTags, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, featureTags, Setup Ok, teardown, fileFullName, lineNumber)
        
    static member Feature subFeatureName =
        Sub.Feature (subFeatureName, TestTags [], Setup Ok, emptyTeardown)
        
    static member Feature (subFeatureName, featureTags) =
        Sub.Feature (subFeatureName, featureTags, Setup Ok, emptyTeardown)
        
    static member Ignore (subFeatureName, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, TestTags [], Setup Ok, emptyTeardown, fileFullName, lineNumber)
        
    static member Ignore (subFeatureName, featureTags, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, featureTags, Setup Ok, emptyTeardown, fileFullName, lineNumber)
        
    static member Feature (setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, TestTags [], setup, teardown)
        
    static member Feature (featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, featureTags, setup, teardown)
        
    static member Ignore (setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, TestTags [], setup, teardown, fileFullName, lineNumber)
        
    static member Ignore (featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, featureTags, setup, teardown, fileFullName, lineNumber)
        
    static member Feature (setup: SetupIndicator<'featureType, 'subFeatureType>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, TestTags [], setup, emptyTeardown)
        
    static member Feature (featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, featureTags, setup, emptyTeardown)
        
    static member Ignore (setup: SetupIndicator<'featureType, 'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, TestTags [], setup, emptyTeardown, fileFullName, lineNumber)
        
    static member Ignore (featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, featureTags, setup, emptyTeardown, fileFullName, lineNumber)
        
    static member Feature (teardown: TeardownIndicator<unit>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, TestTags [], Setup Ok, teardown)
        
    static member Feature (featureTags, teardown: TeardownIndicator<unit>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, featureTags, Setup Ok, teardown)
        
    static member Ignore (teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, TestTags [], Setup Ok, teardown, fileFullName, lineNumber)
        
    static member Ignore (featureTags, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, featureTags, Setup Ok, teardown, fileFullName, lineNumber)
        
    static member Feature () =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, TestTags [], Setup Ok, emptyTeardown)
        
    static member Feature featureTags =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, featureTags, Setup Ok, emptyTeardown)
        
    static member Ignore ([<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, TestTags [], Setup Ok, emptyTeardown, fileFullName, lineNumber)
        
    static member Ignore (featureTags, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, featureTags, Setup Ok, emptyTeardown, fileFullName, lineNumber)
        
    static member Feature (subFeatureName, TestTags featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>, testBuilder: IScriptFeature<'subFeatureType> -> unit) =
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
            
            let subFeature = Feature<'subFeatureType> (feature.ToString (), subFeatureName, [feature.FeatureTags; featureTags] |> List.concat, transformer)
            
            subFeature :> IScriptFeature<'subFeatureType> |> testBuilder
            
        buildIt
        
    static member Ignore (subFeatureName, TestTags featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>, testBuilder: IScriptFeature<'subFeatureType> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
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
            let subFeature = IgnoreFeature<'subFeatureType> (feature.ToString (), subFeatureName, [feature.FeatureTags; featureTags] |> List.concat, transformer, location)
            
            subFeature :> IScriptFeature<'subFeatureType> |> testBuilder
            
        buildIt
        
    static member Feature (subFeatureName, setup: SetupIndicator<'featureType, 'subFeatureType>, testBuilder: IScriptFeature<'subFeatureType> -> unit) =
        Sub.Feature (subFeatureName, TestTags [], setup, emptyTeardown, testBuilder)
        
    static member Feature (subFeatureName, featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>, testBuilder: IScriptFeature<'subFeatureType> -> unit) =
        Sub.Feature (subFeatureName, featureTags, setup, emptyTeardown, testBuilder)
        
    static member Ignore (subFeatureName, setup: SetupIndicator<'featureType, 'subFeatureType>, testBuilder: IScriptFeature<'subFeatureType> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, TestTags [], setup, emptyTeardown, testBuilder, fileFullName, lineNumber)
        
    static member Ignore (subFeatureName, featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>, testBuilder: IScriptFeature<'subFeatureType> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, featureTags, setup, emptyTeardown, testBuilder, fileFullName, lineNumber)
        
    static member Feature (subFeatureName, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit) =
        Sub.Feature (subFeatureName, TestTags [], Setup Ok, teardown, testBuilder)
        
    static member Feature (subFeatureName, featureTags, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit) =
        Sub.Feature (subFeatureName, featureTags, Setup Ok, teardown, testBuilder)
        
    static member Ignore (subFeatureName, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, TestTags [], Setup Ok, teardown, testBuilder, fileFullName, lineNumber)
        
    static member Ignore (subFeatureName, featureTags, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, featureTags, Setup Ok, teardown, testBuilder, fileFullName, lineNumber)
        
    static member Feature (subFeatureName, testBuilder: IScriptFeature<unit> -> unit) =
        Sub.Feature (subFeatureName, TestTags [], Setup Ok, emptyTeardown, testBuilder)
        
    static member Feature (subFeatureName, featureTags, testBuilder: IScriptFeature<unit> -> unit) =
        Sub.Feature (subFeatureName, featureTags, Setup Ok, emptyTeardown, testBuilder)
        
    static member Ignore (subFeatureName, testBuilder: IScriptFeature<unit> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, TestTags [], Setup Ok, emptyTeardown, testBuilder, fileFullName, lineNumber)
        
    static member Ignore (subFeatureName, featureTags, testBuilder: IScriptFeature<unit> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, featureTags, Setup Ok, emptyTeardown, testBuilder, fileFullName, lineNumber)
        