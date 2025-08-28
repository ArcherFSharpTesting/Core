[<AutoOpen>]
module Archer.Arrows.SubNoParam

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers
open Archer.Arrows.Internals

type Sub with

    /// <summary>
    /// Creates a sub-feature using the current context's feature name, with no tags, a default setup, and an empty teardown.
    /// </summary>
    /// <returns>A function that takes a parent feature and returns the constructed sub-feature.</returns>
    static member Feature () =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, TestTags [], Setup Ok, emptyTeardown)

    /// <summary>
    /// Creates an ignored sub-feature using the current context's feature name, with no tags, a default setup, and an empty teardown.
    /// </summary>
    /// <param name="fileFullName">The full file path where the ignore is declared (automatically provided).</param>
    /// <param name="lineNumber">The line number where the ignore is declared (automatically provided).</param>
    /// <returns>A function that takes a parent feature and returns the constructed ignored sub-feature.</returns>
    static member Ignore ([<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, TestTags [], Setup Ok, emptyTeardown, fileFullName, lineNumber)
