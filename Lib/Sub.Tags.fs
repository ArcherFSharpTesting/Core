[<AutoOpen>]
module Archer.Arrows.SubTags

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers
open Archer.Arrows.Internals

type Sub with

    //---------- Teardown ----------
    static member Feature (featureTags, teardown: TeardownIndicator<unit>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, featureTags, Setup Ok, teardown)

    static member Ignore (featureTags, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, featureTags, Setup Ok, teardown, fileFullName, lineNumber)

    //---------- () ----------
    static member Feature featureTags =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, featureTags, Setup Ok, emptyTeardown)

    static member Ignore (featureTags, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, featureTags, Setup Ok, emptyTeardown, fileFullName, lineNumber)