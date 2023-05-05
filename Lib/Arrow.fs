namespace Archer.Arrows

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
        Arrow.NewFeature (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()))
        
    static member Ignore (featurePath, featureName, setup: SetupIndicator<unit, 'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.Ignore (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
        
    static member NewFeature (featurePath, featureName, teardown: TeardownIndicator<unit>) =
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()) , teardown)
        
    static member Ignore (featurePath, featureName, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.Ignore (featurePath, featureName, Setup (fun () -> Ok ()) , teardown, fileFullName, lineNumber)
        
    static member NewFeature (featurePath, featureName) =
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()))

    static member Ignore (featurePath, featureName, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.Ignore (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)

    static member Ignore (featureInfo: string * string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featurePath, featureName = featureInfo
        Arrow.Ignore (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)

    // ------- featureName -------
    static member NewFeature (featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, setup, teardown)
        
    static member Ignore (featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, setup, teardown, fileFullName, lineNumber)
        
    static member NewFeature (featureName, setup: SetupIndicator<unit, 'a>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()))
        
    static member Ignore (featureName, setup: SetupIndicator<unit, 'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
        
    static member NewFeature (featureName, teardown: TeardownIndicator<unit>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()), teardown)
        
    static member Ignore (featureName, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, Setup (fun () -> Ok ()), teardown, fileFullName, lineNumber)
        
    static member NewFeature featureName =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()))

    static member Ignore (featureName, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)

    // ------- setup -------
    static member NewFeature (setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, setup, teardown)
        
    static member Ignore (setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, setup, teardown, fileFullName, lineNumber)
        
    static member NewFeature (setup: SetupIndicator<unit, 'a>) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, setup)
        
    static member Ignore (setup: SetupIndicator<unit, 'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, setup, fileFullName, lineNumber)
        
    // ------- teardown -------
    static member NewFeature (teardown: TeardownIndicator<unit>) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, teardown)
        
    static member Ignore (teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, teardown, fileFullName, lineNumber)
        
    // ------- () -------
    static member NewFeature () =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName)
        
    static member Ignore ([<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, fileFullName, lineNumber)
        
    // ------- featurePath -------
    static member Tests (featurePath, featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, testBuilder: IScriptFeature<'a> -> unit) =
        let t = baseTransformer setup teardown
        let feature = Feature (featurePath, featureName, [], t)
        feature :> IScriptFeature<'a> |> testBuilder 
        feature.GetTests ()
        
    static member IgnoreTests (featurePath, featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, testBuilder: IScriptFeature<'a> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let t = baseTransformer setup teardown
        let location = getLocation fileFullName lineNumber
        let feature = IgnoreFeature (featurePath, featureName, [], t, location)
        feature :> IScriptFeature<'a> |> testBuilder
        feature.GetTests ()
        
    static member Tests (featurePath, featureName, setup: SetupIndicator<unit, 'a>, testBuilder: IScriptFeature<'a> -> unit) =
        Arrow.Tests (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member IgnoreTests (featurePath, featureName, setup: SetupIndicator<unit, 'a>, testBuilder: IScriptFeature<'a> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.IgnoreTests (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()), testBuilder, fileFullName, lineNumber)
        
    static member Tests (featurePath, featureName, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit) =
        Arrow.Tests (featurePath, featureName, Setup (fun () -> Ok ()), teardown, testBuilder)
        
    static member IgnoreTests (featurePath, featureName, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.IgnoreTests (featurePath, featureName, Setup (fun () -> Ok ()), teardown, testBuilder, fileFullName, lineNumber)
        
    static member Tests (featurePath, featureName, testBuilder: IScriptFeature<unit> -> unit) =
        Arrow.Tests (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member IgnoreTests (featurePath, featureName, testBuilder: IScriptFeature<unit> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.IgnoreTests (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder, fileFullName, lineNumber)
        
    // ------- featureName -------
    static member Tests (featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, testBuilder: IScriptFeature<'a> -> unit) =
        let _, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, setup, teardown, testBuilder)
        
    static member IgnoreTests (featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, testBuilder: IScriptFeature<'a> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNamesAt 3
        Arrow.IgnoreTests (featurePath, featureName, setup, teardown, testBuilder, fileFullName, lineNumber)
        
    static member Tests (featureName, setup: SetupIndicator<unit, 'a>, testBuilder: IScriptFeature<'a> -> unit) =
        let _, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member IgnoreTests (featureName, setup: SetupIndicator<unit, 'a>, testBuilder: IScriptFeature<'a> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNamesAt 3
        Arrow.IgnoreTests (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()), testBuilder, fileFullName, lineNumber)
        
    static member Tests (featureName, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit) =
        let _, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, Setup (fun () -> Ok ()), teardown, testBuilder)
        
    static member IgnoreTests (featureName, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNamesAt 3
        Arrow.IgnoreTests (featurePath, featureName, Setup (fun () -> Ok ()), teardown, testBuilder, fileFullName, lineNumber)
        
    static member Tests (featureName, testBuilder: IScriptFeature<unit> -> unit) =
        let _, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member IgnoreTests (featureName, testBuilder: IScriptFeature<unit> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNamesAt 3
        Arrow.IgnoreTests (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder, fileFullName, lineNumber)
        
    // ------- setup -------
    static member Tests (setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, testBuilder: IScriptFeature<'a> -> unit) =
        let featureName, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, setup, teardown, testBuilder)
        
    static member IgnoreTests (setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, testBuilder: IScriptFeature<'a> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNamesAt 3
        Arrow.IgnoreTests (featurePath, featureName, setup, teardown, testBuilder, fileFullName, lineNumber)
        
    static member Tests (setup: SetupIndicator<unit, 'a>, testBuilder: IScriptFeature<'a> -> unit) =
        let featureName, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member IgnoreTests (setup: SetupIndicator<unit, 'a>, testBuilder: IScriptFeature<'a> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNamesAt 3
        Arrow.IgnoreTests (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()), testBuilder, fileFullName, lineNumber)
        
    // ------- teardown -------
    static member Tests (teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit) =
        let featureName, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, Setup (fun () -> Ok ()), teardown, testBuilder)
        
    static member IgnoreTests (teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNamesAt 3
        Arrow.IgnoreTests (featurePath, featureName, Setup (fun () -> Ok ()), teardown, testBuilder, fileFullName, lineNumber)
        
    // ------- testBuilder -------
    static member Tests (testBuilder: IScriptFeature<unit> -> unit) =
        let featureName, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member IgnoreTests (testBuilder: IScriptFeature<unit> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNamesAt 3
        Arrow.IgnoreTests (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder, fileFullName, lineNumber)
        
