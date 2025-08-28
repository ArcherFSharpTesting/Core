[<AutoOpen>]
module Archer.Arrows.ArrowTagsNoParam

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers

type Arrow with

    /// <summary>
    /// Creates a new feature with the specified tags and no setup or teardown parameters, using the current context for feature name and path.
    /// </summary>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    static member NewFeature (featureTags: TagsIndicator) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, featureTags)

    /// <summary>
    /// Creates an ignored feature with the specified tags and no setup or teardown parameters, using the current context for feature name and path, and source location.
    /// </summary>
    /// <param name="featureTags">The tags to associate with the feature.</param>
    /// <param name="fileFullName">The full file path of the source (auto-filled).</param>
    /// <param name="lineNumber">The line number in the source file (auto-filled).</param>
    static member Ignore (featureTags: TagsIndicator, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, featureTags, fileFullName, lineNumber)
