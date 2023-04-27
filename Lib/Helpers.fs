[<AutoOpen>]
module Archer.Arrows.Helpers

open System
open System.Diagnostics
open Archer.Arrows.Internal

let private getNamesAt frame = 
    let trace = StackTrace ()
    let method = trace.GetFrame(frame).GetMethod ()
    let containerName = method.ReflectedType.Name
    let containerPath = method.ReflectedType.Namespace |> fun s -> s.Split ([|"$"|], StringSplitOptions.RemoveEmptyEntries) |> Array.last
            
    containerName, containerPath

let private getNames () =
    getNamesAt 2

type Arrow =
    // ------- featurePath -------
    static member NewFeature (featurePath, featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        Feature (featurePath, featureName, setup, teardown)
        
    static member NewFeature (featurePath, featureName, setup: SetupIndicator<unit, 'a>) =
        Arrow.NewFeature (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()))
        
    static member NewFeature (featurePath, featureName, teardown: TeardownIndicator<unit>) =
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()) , teardown)
        
    static member NewFeature (featurePath, featureName) =
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()))

    // ------- featureName -------
    static member NewFeature (featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, setup, teardown)
        
    static member NewFeature (featureName, setup: SetupIndicator<unit, 'a>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()))
        
    static member NewFeature (featureName, teardown: TeardownIndicator<unit>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()), teardown)
        
    static member NewFeature featureName =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()))

    // ------- setup -------
    static member NewFeature (setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, setup, teardown)
        
    static member NewFeature (setup: SetupIndicator<unit, 'a>) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, setup)
        
    // ------- teardown -------
    static member NewFeature (teardown: TeardownIndicator<unit>) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, teardown)
        
    // ------- () -------
    static member NewFeature () =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName)
        
    // ------- featurePath -------
    static member Tests (featurePath, featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, testBuilder: IScriptFeature<'a> -> unit) =
        let feature = Arrow.NewFeature (featurePath, featureName, setup, teardown)
        feature :> IScriptFeature<'a> |> testBuilder 
        feature.GetTests ()
        
    static member Tests (featurePath, featureName, setup: SetupIndicator<unit, 'a>, testBuilder: IScriptFeature<'a> -> unit) =
        Arrow.Tests (featurePath, featureName, setup, Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member Tests (featurePath, featureName, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit) =
        Arrow.Tests (featurePath, featureName, Setup (fun () -> Ok ()), teardown, testBuilder)
        
    static member Tests (featurePath, featureName, testBuilder: IScriptFeature<unit> -> unit) =
        Arrow.Tests (featurePath, featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder)
        
    // ------- featureName -------
    static member Tests (featureName, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, testBuilder: IScriptFeature<'a> -> unit) =
        let _, featurePath = getNames ()
        Arrow.Tests (featurePath, featureName, setup, teardown, testBuilder)
        
    static member Tests (featureName, setup: SetupIndicator<unit, 'a>, testBuilder: IScriptFeature<'a> -> unit) =
        Arrow.Tests (featureName, setup, Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member Tests (featureName, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit) =
        Arrow.Tests (featureName, Setup (fun () -> Ok ()), teardown, testBuilder)
        
    static member Tests (featureName, testBuilder: IScriptFeature<unit> -> unit) =
        Arrow.Tests (featureName, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder)
        
    // ------- setup -------
    static member Tests (setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, testBuilder: IScriptFeature<'a> -> unit) =
        let featureName, featurePath = getNames ()
        Arrow.Tests (featurePath, featureName, setup, teardown, testBuilder)
        
    static member Tests (setup: SetupIndicator<unit, 'a>, testBuilder: IScriptFeature<'a> -> unit) =
        Arrow.Tests (setup, Teardown (fun _ _ -> Ok ()), testBuilder)
        
    // ------- teardown -------
    static member Tests (teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit) =
        Arrow.Tests (Setup (fun () -> Ok ()), teardown, testBuilder)
        
    // ------- testBuilder -------
    static member Tests (testBuilder: IScriptFeature<unit> -> unit) =
        Arrow.Tests (Teardown (fun _ _ -> Ok ()), testBuilder)