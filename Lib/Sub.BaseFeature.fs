[<AutoOpen>]
module Archer.Arrows.SubBaseFeature

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
