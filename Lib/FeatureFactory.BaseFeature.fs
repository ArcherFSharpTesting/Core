
[<AutoOpen>]
module Archer.Core.FeatureBaseFeature

open Archer.Core
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Core.Helpers
open Archer.Core.Internal.Types
open Archer.Core.Internals

/// <summary>
/// Provides static methods for creating and ignoring features in the Archer Feature test framework.
/// </summary>
type FeatureFactory =
    /// <summary>
    /// Creates a new feature with the specified setup and teardown logic.
    /// </summary>
    /// <param name="featurePath">The path or category of the feature.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    static member NewFeature (featurePath, featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let t = baseTransformer setup teardown
        Feature (featurePath, featureName, [], t)
        :> IFeature<'a>

    /// <summary>
    /// Creates an ignored feature with the specified setup and teardown logic, and source location.
    /// </summary>
    /// <param name="featurePath">The path or category of the feature.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featurePath, featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let t = baseTransformer setup teardown
        let location = getLocation fileFullName lineNumber
        IgnoreFeature (featurePath, featureName, [], t, location)
        :> IFeature<'a>

    /// <summary>
    /// Creates a new feature with the specified setup logic and no teardown.
    /// </summary>
    /// <param name="featurePath">The path or category of the feature.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    static member NewFeature (featurePath, featureName, setup: SetupIndicator<unit, 'a>) =
        FeatureFactory.NewFeature (featurePath, featureName, setup, emptyTeardown)

    /// <summary>
    /// Creates an ignored feature with the specified setup logic and no teardown.
    /// </summary>
    /// <param name="featurePath">The path or category of the feature.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featurePath, featureName, setup: SetupIndicator<unit, 'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        FeatureFactory.Ignore (featurePath, featureName, setup, emptyTeardown, fileFullName, lineNumber)

    /// <summary>
    /// Creates a new feature with the specified teardown logic and default setup.
    /// </summary>
    /// <param name="featurePath">The path or category of the feature.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    static member NewFeature (featurePath, featureName, teardown: TeardownIndicator<unit>) =
        FeatureFactory.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()) , teardown)

    /// <summary>
    /// Creates an ignored feature with the specified teardown logic and default setup.
    /// </summary>
    /// <param name="featurePath">The path or category of the feature.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featurePath, featureName, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        FeatureFactory.Ignore (featurePath, featureName, Setup (fun () -> Ok ()) , teardown, fileFullName, lineNumber)

    /// <summary>
    /// Creates a new feature with default setup and teardown.
    /// </summary>
    /// <param name="featurePath">The path or category of the feature.</param>
    /// <param name="featureName">The name of the feature.</param>
    static member NewFeature (featurePath, featureName) =
        FeatureFactory.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()), emptyTeardown)

    /// <summary>
    /// Creates an ignored feature with default setup and teardown.
    /// </summary>
    /// <param name="featurePath">The path or category of the feature.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featurePath, featureName, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        FeatureFactory.Ignore (featurePath, featureName, Setup (fun () -> Ok ()), emptyTeardown, fileFullName, lineNumber)

    /// <summary>
    /// Creates an ignored feature from a tuple of feature path and name, with default setup and teardown.
    /// </summary>
    /// <param name="featureInfo">A tuple containing the feature path and feature name.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featureInfo: string * string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featurePath, featureName = featureInfo
        FeatureFactory.Ignore (featurePath, featureName, Setup (fun () -> Ok ()), emptyTeardown, fileFullName, lineNumber)