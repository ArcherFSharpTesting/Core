[<AutoOpen>]
module Archer.Core.FeatureTeardown

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Core.Helpers

type FeatureFactory with
    /// <summary>
    /// Creates a new feature with the specified teardown logic, using the current context for feature name and path.
    /// </summary>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    static member NewFeature (teardown: TeardownIndicator<unit>) =
        let featureName, featurePath = getNames ()
        FeatureFactory.NewFeature (featurePath, featureName, teardown)

    /// <summary>
    /// Creates an ignored feature with the specified teardown logic, using the current context for feature name and path, and source location.
    /// </summary>
    /// <param name="teardown">The teardown logic to run after the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNames ()
        FeatureFactory.Ignore (featurePath, featureName, teardown, fileFullName, lineNumber)