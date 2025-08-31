[<AutoOpen>]
module Archer.Core.SubFeatureName

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Core.Helpers
open Archer.Core.Internals

type Sub with

    /// <summary>
    /// Creates a sub-feature with the specified name and setup logic, using no tags and an empty teardown.
    /// </summary>
    /// <param name="subFeatureName">The name of the sub-feature.</param>
    /// <param name="setup">Setup logic for the sub-feature.</param>
    /// <returns>A function that takes a parent feature and returns the constructed sub-feature.</returns>
    static member Feature (subFeatureName, setup: SetupIndicator<'featureType, 'subFeatureType>) =
        Sub.Feature (subFeatureName, TestTags [], setup, emptyTeardown)

    /// <summary>
    /// Creates an ignored sub-feature with the specified name and setup logic, using no tags and an empty teardown.
    /// </summary>
    /// <param name="subFeatureName">The name of the sub-feature.</param>
    /// <param name="setup">Setup logic for the sub-feature.</param>
    /// <param name="fileFullName">The full file path where the ignore is declared (automatically provided).</param>
    /// <param name="lineNumber">The line number where the ignore is declared (automatically provided).</param>
    /// <returns>A function that takes a parent feature and returns the constructed ignored sub-feature.</returns>
    static member Ignore (subFeatureName, setup: SetupIndicator<'featureType, 'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, TestTags [], setup, emptyTeardown, fileFullName, lineNumber)

    /// <summary>
    /// Creates a sub-feature with the specified name and teardown logic, using no tags and a default setup.
    /// </summary>
    /// <param name="subFeatureName">The name of the sub-feature.</param>
    /// <param name="teardown">Teardown logic for the sub-feature.</param>
    /// <returns>A function that takes a parent feature and returns the constructed sub-feature.</returns>
    static member Feature (subFeatureName, teardown: TeardownIndicator<unit>) =
        Sub.Feature (subFeatureName, TestTags [], Setup Ok, teardown)

    /// <summary>
    /// Creates an ignored sub-feature with the specified name and teardown logic, using no tags and a default setup.
    /// </summary>
    /// <param name="subFeatureName">The name of the sub-feature.</param>
    /// <param name="teardown">Teardown logic for the sub-feature.</param>
    /// <param name="fileFullName">The full file path where the ignore is declared (automatically provided).</param>
    /// <param name="lineNumber">The line number where the ignore is declared (automatically provided).</param>
    /// <returns>A function that takes a parent feature and returns the constructed ignored sub-feature.</returns>
    static member Ignore (subFeatureName, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, TestTags [], Setup Ok, teardown, fileFullName, lineNumber)

    /// <summary>
    /// Creates a sub-feature with the specified name, using no tags, a default setup, and an empty teardown.
    /// </summary>
    /// <param name="subFeatureName">The name of the sub-feature.</param>
    /// <returns>A function that takes a parent feature and returns the constructed sub-feature.</returns>
    static member Feature subFeatureName =
        Sub.Feature (subFeatureName, TestTags [], Setup Ok, emptyTeardown)

    /// <summary>
    /// Creates an ignored sub-feature with the specified name, using no tags, a default setup, and an empty teardown.
    /// </summary>
    /// <param name="subFeatureName">The name of the sub-feature.</param>
    /// <param name="fileFullName">The full file path where the ignore is declared (automatically provided).</param>
    /// <param name="lineNumber">The line number where the ignore is declared (automatically provided).</param>
    /// <returns>A function that takes a parent feature and returns the constructed ignored sub-feature.</returns>
    static member Ignore (subFeatureName, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, TestTags [], Setup Ok, emptyTeardown, fileFullName, lineNumber)


