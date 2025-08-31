
[<AutoOpen>]
module Archer.Core.FeatureNoParam

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Core.Helpers

type FeatureFactory with

    /// <summary>
    /// Creates a new feature with no setup or teardown parameters, using the current context for feature name and path.
    /// </summary>
    static member NewFeature () =
        let featureName, featurePath = getNames ()
        FeatureFactory.NewFeature (featurePath, featureName)

    /// <summary>
    /// Creates an ignored feature with no setup or teardown parameters, using the current context for feature name and path, and source location.
    /// </summary>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore ([<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNames ()
        FeatureFactory.Ignore (featurePath, featureName, fileFullName, lineNumber)
