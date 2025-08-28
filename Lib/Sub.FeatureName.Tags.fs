[<AutoOpen>]
module Archer.Arrows.SubFeatureNameTags

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers
open Archer.Arrows.Internals

type Sub with

    static member Feature (subFeatureName, featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>) =
        Sub.Feature (subFeatureName, featureTags, setup, emptyTeardown)

    static member Ignore (subFeatureName, featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, featureTags, setup, emptyTeardown, fileFullName, lineNumber)

    static member Feature (subFeatureName, featureTags, teardown: TeardownIndicator<unit>) =
        Sub.Feature (subFeatureName, featureTags, Setup Ok, teardown)

    static member Ignore (subFeatureName, featureTags, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, featureTags, Setup Ok, teardown, fileFullName, lineNumber)

    static member Feature (subFeatureName, featureTags) =
        Sub.Feature (subFeatureName, featureTags, Setup Ok, emptyTeardown)

    static member Ignore (subFeatureName, featureTags, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Sub.Ignore (subFeatureName, featureTags, Setup Ok, emptyTeardown, fileFullName, lineNumber)