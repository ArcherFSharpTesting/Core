[<AutoOpen>]
module Archer.Core.SubBaseFeatureTags

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Core.Helpers

type Sub with

    /// <summary>
    /// Creates a sub-feature using the current context's feature name and provided tags, setup, and teardown logic.
    /// </summary>
    /// <param name="featureTags">Tags to associate with the sub-feature.</param>
    /// <param name="setup">Setup logic for the sub-feature.</param>
    /// <param name="teardown">Teardown logic for the sub-feature.</param>
    /// <returns>A function that takes a parent feature and returns the constructed sub-feature.</returns>
    static member Feature (featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, featureTags, setup, teardown)

    /// <summary>
    /// Creates an ignored sub-feature using the current context's feature name and provided tags, setup, and teardown logic.
    /// </summary>
    /// <param name="featureTags">Tags to associate with the sub-feature.</param>
    /// <param name="setup">Setup logic for the sub-feature.</param>
    /// <param name="teardown">Teardown logic for the sub-feature.</param>
    /// <param name="fileFullName">The full file path where the ignore is declared (automatically provided).</param>
    /// <param name="lineNumber">The line number where the ignore is declared (automatically provided).</param>
    /// <returns>A function that takes a parent feature and returns the constructed ignored sub-feature.</returns>
    static member Ignore (featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, featureTags, setup, teardown, fileFullName, lineNumber)
