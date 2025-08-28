[<AutoOpen>]
module Archer.Arrows.SubFeatureNameTags

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers
open Archer.Arrows.Internals

type Sub with

    /// <summary>
    /// Creates a sub-feature with the specified name, tags, and setup logic, using an empty teardown.
    /// </summary>
    /// <param name="subFeatureName">The name of the sub-feature.</param>
    /// <param name="featureTags">Tags to associate with the sub-feature.</param>
    /// <param name="setup">Setup logic for the sub-feature.</param>
    /// <returns>A function that takes a parent feature and returns the constructed sub-feature.</returns>
    static member Feature (subFeatureName, featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>) =
        Sub.Feature (subFeatureName, featureTags, setup, emptyTeardown)

    /// <summary>
    /// Creates an ignored sub-feature with the specified name, tags, and setup logic, using an empty teardown.
    /// </summary>
    /// <param name="subFeatureName">The name of the sub-feature.</param>
    /// <param name="featureTags">Tags to associate with the sub-feature.</param>
    /// <param name="setup">Setup logic for the sub-feature.</param>
    /// <param name="fileFullName">The full file path where the ignore is declared (automatically provided).</param>
    /// <param name="lineNumber">The line number where the ignore is declared (automatically provided).</param>
    /// <returns>A function that takes a parent feature and returns the constructed ignored sub-feature.</returns>
    static member Ignore (subFeatureName, featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, featureTags, setup, emptyTeardown, fileFullName, lineNumber)

    /// <summary>
    /// Creates a sub-feature with the specified name, tags, and teardown logic, using a default setup.
    /// </summary>
    /// <param name="subFeatureName">The name of the sub-feature.</param>
    /// <param name="featureTags">Tags to associate with the sub-feature.</param>
    /// <param name="teardown">Teardown logic for the sub-feature.</param>
    /// <returns>A function that takes a parent feature and returns the constructed sub-feature.</returns>
    static member Feature (subFeatureName, featureTags, teardown: TeardownIndicator<unit>) =
        Sub.Feature (subFeatureName, featureTags, Setup Ok, teardown)

    /// <summary>
    /// Creates an ignored sub-feature with the specified name, tags, and teardown logic, using a default setup.
    /// </summary>
    /// <param name="subFeatureName">The name of the sub-feature.</param>
    /// <param name="featureTags">Tags to associate with the sub-feature.</param>
    /// <param name="teardown">Teardown logic for the sub-feature.</param>
    /// <param name="fileFullName">The full file path where the ignore is declared (automatically provided).</param>
    /// <param name="lineNumber">The line number where the ignore is declared (automatically provided).</param>
    /// <returns>A function that takes a parent feature and returns the constructed ignored sub-feature.</returns>
    static member Ignore (subFeatureName, featureTags, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, featureTags, Setup Ok, teardown, fileFullName, lineNumber)

    /// <summary>
    /// Creates a sub-feature with the specified name and tags, using a default setup and an empty teardown.
    /// </summary>
    /// <param name="subFeatureName">The name of the sub-feature.</param>
    /// <param name="featureTags">Tags to associate with the sub-feature.</param>
    /// <returns>A function that takes a parent feature and returns the constructed sub-feature.</returns>
    static member Feature (subFeatureName, featureTags) =
        Sub.Feature (subFeatureName, featureTags, Setup Ok, emptyTeardown)

    /// <summary>
    /// Creates an ignored sub-feature with the specified name and tags, using a default setup and an empty teardown.
    /// </summary>
    /// <param name="subFeatureName">The name of the sub-feature.</param>
    /// <param name="featureTags">Tags to associate with the sub-feature.</param>
    /// <param name="fileFullName">The full file path where the ignore is declared (automatically provided).</param>
    /// <param name="lineNumber">The line number where the ignore is declared (automatically provided).</param>
    /// <returns>A function that takes a parent feature and returns the constructed ignored sub-feature.</returns>
    static member Ignore (subFeatureName, featureTags, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, featureTags, Setup Ok, emptyTeardown, fileFullName, lineNumber)