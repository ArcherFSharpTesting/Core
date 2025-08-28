[<AutoOpen>]
module Archer.Arrows.Temp

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers
open Archer.Arrows.Internals

type Sub with

    //---------- () ----------
    static member Feature () =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, TestTags [], Setup Ok, emptyTeardown)

    static member Ignore ([<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, TestTags [], Setup Ok, emptyTeardown, fileFullName, lineNumber)