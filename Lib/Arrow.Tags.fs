[<AutoOpen>]
module Archer.Arrows.ArrowTags

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers

type Arrow with

    // ------- () -------
    static member NewFeature (featureTags: TagsIndicator) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, featureTags)

    static member Ignore (featureTags: TagsIndicator, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, featureTags, fileFullName, lineNumber)