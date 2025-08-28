[<AutoOpen>]
module Archer.Arrows.ArrowTagsFeatureName

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers
open Archer.Arrows.Internal.Types
open Archer.Arrows.Internals

type Arrow with

    static member NewFeature (featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, featureTags, setup, teardown)

    static member Ignore (featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, featureTags, setup, teardown, fileFullName, lineNumber)

    static member NewFeature (featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, featureTags, setup, emptyTeardown)

    static member Ignore (featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, featureTags, setup, emptyTeardown, fileFullName, lineNumber)

    static member NewFeature (featureName, featureTags: TagsIndicator, teardown: TeardownIndicator<unit>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), teardown)

    static member Ignore (featureName, featureTags: TagsIndicator, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), teardown, fileFullName, lineNumber)

    static member NewFeature (featureName, featureTags: TagsIndicator) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), emptyTeardown)

    static member Ignore (featureName, featureTags: TagsIndicator, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), emptyTeardown, fileFullName, lineNumber)