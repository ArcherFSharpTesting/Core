[<AutoOpen>]
module Archer.Arrows.ArrowFeatureName

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers
open Archer.Arrows.Internal.Types
open Archer.Arrows.Internals

type Arrow with

    /// <summary>
    /// Creates a new feature with the specified feature name, setup, and teardown logic.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    static member NewFeature (featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, setup, teardown)

    /// <summary>
    /// Creates an ignored feature with the specified feature name, setup, teardown, and source location.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, setup, teardown, fileFullName, lineNumber)

    /// <summary>
    /// Creates a new feature with the specified feature name and setup logic, and no teardown.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    static member NewFeature (featureName, setup: SetupIndicator<unit, 'a>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, setup, emptyTeardown)

    /// <summary>
    /// Creates an ignored feature with the specified feature name and setup logic, and no teardown.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="setup">The setup logic to run before the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featureName, setup: SetupIndicator<unit, 'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, setup, emptyTeardown, fileFullName, lineNumber)

    /// <summary>
    /// Creates a new feature with the specified feature name and teardown logic, and default setup.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    static member NewFeature (featureName, teardown: TeardownIndicator<unit>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()), teardown)

    /// <summary>
    /// Creates an ignored feature with the specified feature name and teardown logic, and default setup.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featureName, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, Setup (fun () -> Ok ()), teardown, fileFullName, lineNumber)

    /// <summary>
    /// Creates a new feature with the specified feature name and default setup and teardown.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    static member NewFeature featureName =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()), emptyTeardown)

    /// <summary>
    /// Creates an ignored feature with the specified feature name and default setup and teardown.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featureName, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, Setup (fun () -> Ok ()), emptyTeardown, fileFullName, lineNumber)