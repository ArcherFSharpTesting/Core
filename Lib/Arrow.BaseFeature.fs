[<AutoOpen>]
module Archer.Arrows.BaseFeature

open Archer.Arrows
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers
open Archer.Arrows.Internal.Types
open Archer.Arrows.Internals

type Arrow =
    static member NewFeature (featurePath, featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let t = baseTransformer setup teardown
        Feature (featurePath, featureName, [], t)
        :> IFeature<'a>
        
    static member Ignore (featurePath, featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let t = baseTransformer setup teardown
        let location = getLocation fileFullName lineNumber
        IgnoreFeature (featurePath, featureName, [], t, location)
        :> IFeature<'a>
        
    static member NewFeature (featurePath, featureName, setup: SetupIndicator<unit, 'a>) =
        Arrow.NewFeature (featurePath, featureName, setup, emptyTeardown)
        
    static member Ignore (featurePath, featureName, setup: SetupIndicator<unit, 'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.Ignore (featurePath, featureName, setup, emptyTeardown, fileFullName, lineNumber)
        
    static member NewFeature (featurePath, featureName, teardown: TeardownIndicator<unit>) =
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()) , teardown)
        
    static member Ignore (featurePath, featureName, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.Ignore (featurePath, featureName, Setup (fun () -> Ok ()) , teardown, fileFullName, lineNumber)
        
    static member NewFeature (featurePath, featureName) =
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()), emptyTeardown)

    static member Ignore (featurePath, featureName, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.Ignore (featurePath, featureName, Setup (fun () -> Ok ()), emptyTeardown, fileFullName, lineNumber)

    static member Ignore (featureInfo: string * string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featurePath, featureName = featureInfo
        Arrow.Ignore (featurePath, featureName, Setup (fun () -> Ok ()), emptyTeardown, fileFullName, lineNumber)