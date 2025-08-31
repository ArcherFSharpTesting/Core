[<AutoOpen>]
module Archer.Core.SubBaseFeature

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Core.Helpers
open Archer.Core.Internal.Types
open Archer.Core.Internals
open Archer.Types.InternalTypes

/// <summary>
/// Provides methods for creating sub-features within a test feature.
/// </summary>
type Sub =
    /// <summary>
    /// Creates a sub-feature for a given feature, allowing custom setup and teardown logic.
    /// </summary>
    /// <param name="subFeatureName">The name of the sub-feature.</param>
    /// <param name="featureTags">Tags to associate with the sub-feature.</param>
    /// <param name="setup">Setup logic for the sub-feature.</param>
    /// <param name="teardown">Teardown logic for the sub-feature.</param>
    /// <returns>A function that takes a parent feature and returns the constructed sub-feature.</returns>
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

    /// <summary>
    /// Creates an ignored sub-feature for a given feature, allowing custom setup and teardown logic.
    /// </summary>
    /// <param name="subFeatureName">The name of the sub-feature.</param>
    /// <param name="featureTags">Tags to associate with the sub-feature.</param>
    /// <param name="setup">Setup logic for the sub-feature.</param>
    /// <param name="teardown">Teardown logic for the sub-feature.</param>
    /// <param name="fileFullName">The full file path where the ignore is declared (automatically provided).</param>
    /// <param name="lineNumber">The line number where the ignore is declared (automatically provided).</param>
    /// <returns>A function that takes a parent feature and returns the constructed ignored sub-feature.</returns>
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
