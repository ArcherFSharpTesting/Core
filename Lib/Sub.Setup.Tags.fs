[<AutoOpen>]
module Archer.Arrows.SubSetupTags

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers
open Archer.Arrows.Internals

type Sub with

    static member Feature (featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, featureTags, setup, emptyTeardown)

    static member Ignore (featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, featureTags, setup, emptyTeardown, fileFullName, lineNumber)