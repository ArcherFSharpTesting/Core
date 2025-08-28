[<AutoOpen>]
module Archer.Arrows.ArrowTagsTeardown

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers

type Arrow with

    static member NewFeature (featureTags: TagsIndicator, teardown: TeardownIndicator<unit>) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, featureTags, teardown)

    static member Ignore (featureTags: TagsIndicator, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, featureTags, teardown, fileFullName, lineNumber)
