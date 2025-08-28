[<AutoOpen>]
module Archer.Arrows.SubSetup

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers
open Archer.Arrows.Internals

type Sub with

    static member Feature (setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, TestTags [], setup, teardown)

    static member Ignore (setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, TestTags [], setup, teardown, fileFullName, lineNumber)

    static member Feature (setup: SetupIndicator<'featureType, 'subFeatureType>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, TestTags [], setup, emptyTeardown)

    static member Ignore (setup: SetupIndicator<'featureType, 'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, TestTags [], setup, emptyTeardown, fileFullName, lineNumber)
