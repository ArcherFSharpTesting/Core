[<AutoOpen>]
module Archer.Arrows.SubTags

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers
open Archer.Arrows.Internals

type Sub with

    //---------- Feature Name ----------
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

    //---------- Feature Tags ----------
    static member Feature (featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, featureTags, setup, teardown)

    static member Ignore (featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>, teardown: TeardownIndicator<'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, featureTags, setup, teardown, fileFullName, lineNumber)

    static member Feature (featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, featureTags, setup, emptyTeardown)

    static member Ignore (featureTags, setup: SetupIndicator<'featureType, 'subFeatureType>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, featureTags, setup, emptyTeardown, fileFullName, lineNumber)

    static member Feature (featureTags, teardown: TeardownIndicator<unit>) =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, featureTags, Setup Ok, teardown)

    static member Ignore (featureTags, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, featureTags, Setup Ok, teardown, fileFullName, lineNumber)

    static member Feature featureTags =
        let featureName, _ = getNames ()
        Sub.Feature (featureName, featureTags, Setup Ok, emptyTeardown)

    static member Ignore (featureTags, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, _ = getNames ()
        Sub.Ignore (featureName, featureTags, Setup Ok, emptyTeardown, fileFullName, lineNumber)