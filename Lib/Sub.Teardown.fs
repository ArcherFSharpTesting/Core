[<AutoOpen>]
module Archer.Core.SubTeardown

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Core.Helpers

type Sub with

    /// <summary>
    /// Creates a sub-feature using the current context's feature name and the specified teardown logic, with no tags and a default setup.
    /// </summary>
    /// <param name="teardown">Teardown logic for the sub-feature.</param>
    /// <returns>A function that takes a parent feature and returns the constructed sub-feature.</returns>
    static member Feature (teardown: TeardownIndicator<unit>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, TestTags [], Setup Ok, teardown)

    /// <summary>
    /// Creates an ignored sub-feature using the current context's feature name and the specified teardown logic, with no tags and a default setup.
    /// </summary>
    /// <param name="teardown">Teardown logic for the sub-feature.</param>
    /// <param name="fileFullName">The full file path where the ignore is declared (automatically provided).</param>
    /// <param name="lineNumber">The line number where the ignore is declared (automatically provided).</param>
    /// <returns>A function that takes a parent feature and returns the constructed ignored sub-feature.</returns>
    static member Ignore (teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, TestTags [], Setup Ok, teardown, fileFullName, lineNumber)
