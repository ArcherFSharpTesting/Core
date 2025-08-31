[<AutoOpen>]
module Archer.Core.FeatureTagsFeatureName

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Core.Helpers
open Archer.Core.Internal.Types
open Archer.Core.Internals

type FeatureFactory with

    /// <summary>
    /// Creates a new feature with the specified feature name, tags, setup, and teardown logic.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    static member NewFeature (featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let _, featurePath = getNames ()
        FeatureFactory.NewFeature (featurePath, featureName, featureTags, setup, teardown)

    /// <summary>
    /// Creates an ignored feature with the specified feature name, tags, setup, teardown, and source location.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        FeatureFactory.Ignore (featurePath, featureName, featureTags, setup, teardown, fileFullName, lineNumber)

    /// <summary>
    /// Creates a new feature with the specified feature name, tags, and setup logic, and no teardown.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    static member NewFeature (featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>) =
        let _, featurePath = getNames ()
        FeatureFactory.NewFeature (featurePath, featureName, featureTags, setup, emptyTeardown)

    /// <summary>
    /// Creates an ignored feature with the specified feature name, tags, and setup logic, and no teardown, and source location.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        FeatureFactory.Ignore (featurePath, featureName, featureTags, setup, emptyTeardown, fileFullName, lineNumber)

    /// <summary>
    /// Creates a new feature with the specified feature name, tags, and teardown logic, and default setup.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    static member NewFeature (featureName, featureTags: TagsIndicator, teardown: TeardownIndicator<unit>) =
        let _, featurePath = getNames ()
        FeatureFactory.NewFeature (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), teardown)

    /// <summary>
    /// Creates an ignored feature with the specified feature name, tags, and teardown logic, and default setup, and source location.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featureName, featureTags: TagsIndicator, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        FeatureFactory.Ignore (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), teardown, fileFullName, lineNumber)

    /// <summary>
    /// Creates a new feature with the specified feature name and tags, and default setup and teardown.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    static member NewFeature (featureName, featureTags: TagsIndicator) =
        let _, featurePath = getNames ()
        FeatureFactory.NewFeature (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), emptyTeardown)

    /// <summary>
    /// Creates an ignored feature with the specified feature name and tags, and default setup and teardown, and source location.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featureName, featureTags: TagsIndicator, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        FeatureFactory.Ignore (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), emptyTeardown, fileFullName, lineNumber)