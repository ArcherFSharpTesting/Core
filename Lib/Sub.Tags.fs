[<AutoOpen>]
module Archer.Core.SubTags

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Core.Helpers
open Archer.Core.Internals

type Sub with

    /// <summary>
    /// Creates a sub-feature using the current context's feature name and the specified tags, with a default setup and an empty teardown.
    /// </summary>
    /// <param name="featureTags">Tags to associate with the sub-feature.</param>
    /// <returns>A function that takes a parent feature and returns the constructed sub-feature.</returns>
    static member Feature featureTags =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, featureTags, Setup Ok, emptyTeardown)

    /// <summary>
    /// Creates an ignored sub-feature using the current context's feature name and the specified tags, with a default setup and an empty teardown.
    /// </summary>
    /// <param name="featureTags">Tags to associate with the sub-feature.</param>
    /// <param name="fileFullName">The full file path where the ignore is declared (automatically provided).</param>
    /// <param name="lineNumber">The line number where the ignore is declared (automatically provided).</param>
    /// <returns>A function that takes a parent feature and returns the constructed ignored sub-feature.</returns>
    static member Ignore (featureTags, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, featureTags, Setup Ok, emptyTeardown, fileFullName, lineNumber)