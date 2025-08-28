[<AutoOpen>]
module Archer.Arrows.SubTeardown

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers

type Sub with

    static member Feature (teardown: TeardownIndicator<unit>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, TestTags [], Setup Ok, teardown)

    static member Ignore (teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, TestTags [], Setup Ok, teardown, fileFullName, lineNumber)
