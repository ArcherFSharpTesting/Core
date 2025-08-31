[<AutoOpen>]
module Archer.Core.FeatureTagsSetup

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Core.Helpers

type FeatureFactory with

    /// <summary>
    /// Creates a new feature with the specified tags, setup, and teardown logic, using the current context for feature name and path.
    /// </summary>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    static member NewFeature (featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let featureName, featurePath = getNames ()
        FeatureFactory.NewFeature (featurePath, featureName, featureTags, setup, teardown)

    /// <summary>
    /// Creates an ignored feature with the specified tags, setup, teardown, and source location, using the current context for feature name and path.
    /// </summary>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNames ()
        FeatureFactory.Ignore (featurePath, featureName, featureTags, setup, teardown, fileFullName, lineNumber)

    /// <summary>
    /// Creates a new feature with the specified tags and setup logic, and no teardown, using the current context for feature name and path.
    /// </summary>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    static member NewFeature (featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>) =
        let featureName, featurePath = getNames ()
        FeatureFactory.NewFeature (featurePath, featureName, featureTags, setup)

    /// <summary>
    /// Creates an ignored feature with the specified tags and setup logic, and no teardown, using the current context for feature name and path, and source location.
    /// </summary>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNames ()
        FeatureFactory.Ignore (featurePath, featureName, featureTags, setup, fileFullName, lineNumber)