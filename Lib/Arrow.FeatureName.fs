[<AutoOpen>]
module Archer.Arrows.ArrowFeatureName

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers
open Archer.Arrows.Internal.Types
open Archer.Arrows.Internals

type Arrow with

    static member NewFeature (featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, setup, teardown)
        
    static member Ignore (featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, setup, teardown, fileFullName, lineNumber)
        
    static member NewFeature (featureName, setup: SetupIndicator<unit, 'a>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, setup, emptyTeardown)
        
    static member Ignore (featureName, setup: SetupIndicator<unit, 'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, setup, emptyTeardown, fileFullName, lineNumber)
        
    static member NewFeature (featureName, teardown: TeardownIndicator<unit>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()), teardown)
        
    static member Ignore (featureName, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, Setup (fun () -> Ok ()), teardown, fileFullName, lineNumber)
        
    static member NewFeature featureName =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()), emptyTeardown)

    static member Ignore (featureName, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, Setup (fun () -> Ok ()), emptyTeardown, fileFullName, lineNumber)