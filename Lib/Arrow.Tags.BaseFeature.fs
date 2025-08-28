[<AutoOpen>]
module Archer.Arrows.ArrowTagsBaseFeature

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers
open Archer.Arrows.Internal.Types
open Archer.Arrows.Internals

type Arrow with
    /// <summary>
    /// Creates a new feature with the specified path, name, tags, setup, and teardown logic.
    /// </summary>
    /// <param name="featurePath">The path or category of the feature.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    static member NewFeature (featurePath, featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let t = baseTransformer setup teardown
        let (TestTags tags) = featureTags
        Feature (featurePath, featureName, tags, t)
        :> IFeature<'a>

    /// <summary>
    /// Creates an ignored feature with the specified path, name, tags, setup, teardown, and source location.
    /// </summary>
    /// <param name="featurePath">The path or category of the feature.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featurePath, featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let t = baseTransformer setup teardown
        let location = getLocation fileFullName lineNumber
        let (TestTags tags) = featureTags
        IgnoreFeature (featurePath, featureName, tags, t, location)
        :> IFeature<'a>

    /// <summary>
    /// Creates a new feature with the specified path, name, tags, and setup logic, and no teardown.
    /// </summary>
    /// <param name="featurePath">The path or category of the feature.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    static member NewFeature (featurePath, featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>) =
        Arrow.NewFeature (featurePath, featureName, featureTags, setup, emptyTeardown)

    /// <summary>
    /// Creates an ignored feature with the specified path, name, tags, and setup logic, and no teardown, and source location.
    /// </summary>
    /// <param name="featurePath">The path or category of the feature.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featurePath, featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.Ignore (featurePath, featureName, featureTags, setup, emptyTeardown, fileFullName, lineNumber)

    /// <summary>
    /// Creates a new feature with the specified path, name, tags, and teardown logic, and default setup.
    /// </summary>
    /// <param name="featurePath">The path or category of the feature.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    static member NewFeature (featurePath, featureName, featureTags: TagsIndicator, teardown: TeardownIndicator<unit>) =
        Arrow.NewFeature (featurePath, featureName, featureTags, Setup (fun () -> Ok ()) , teardown)

    /// <summary>
    /// Creates an ignored feature with the specified path, name, tags, and teardown logic, and default setup, and source location.
    /// </summary>
    /// <param name="featurePath">The path or category of the feature.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featurePath, featureName, featureTags: TagsIndicator, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.Ignore (featurePath, featureName, featureTags, Setup (fun () -> Ok ()) , teardown, fileFullName, lineNumber)

    /// <summary>
    /// Creates a new feature with the specified path, name, and tags, and default setup and teardown.
    /// </summary>
    /// <param name="featurePath">The path or category of the feature.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    static member NewFeature (featurePath, featureName, featureTags: TagsIndicator) =
        Arrow.NewFeature (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), emptyTeardown)

    /// <summary>
    /// Creates an ignored feature with the specified path, name, and tags, and default setup and teardown, and source location.
    /// </summary>
    /// <param name="featurePath">The path or category of the feature.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featurePath, featureName, featureTags: TagsIndicator, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.Ignore (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), emptyTeardown, fileFullName, lineNumber)

    /// <summary>
    /// Creates an ignored feature from a tuple of feature path and name, with tags, and default setup and teardown, and source location.
    /// </summary>
    /// <param name="featureInfo">A tuple containing the feature path and feature name.</param>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featureInfo: string * string, featureTags: TagsIndicator, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featurePath, featureName = featureInfo
        Arrow.Ignore (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), emptyTeardown, fileFullName, lineNumber)
