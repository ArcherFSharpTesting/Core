[<AutoOpen>]
module Archer.Arrows.ArrowTags

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers
open Archer.Arrows.Internal.Types
open Archer.Arrows.Internals

type Arrow with
    static member NewFeature (featurePath, featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let t = baseTransformer setup teardown
        let (TestTags tags) = featureTags
        Feature (featurePath, featureName, tags, t)
        :> IFeature<'a>
        
    static member Ignore (featurePath, featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let t = baseTransformer setup teardown
        let location = getLocation fileFullName lineNumber
        let (TestTags tags) = featureTags
        IgnoreFeature (featurePath, featureName, tags, t, location)
        :> IFeature<'a>
        
    static member NewFeature (featurePath, featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>) =
        Arrow.NewFeature (featurePath, featureName, featureTags, setup, Teardown (fun _ _ -> Ok ()))
        
    static member Ignore (featurePath, featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.Ignore (featurePath, featureName, featureTags, setup, Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
        
    static member NewFeature (featurePath, featureName, featureTags: TagsIndicator, teardown: TeardownIndicator<unit>) =
        Arrow.NewFeature (featurePath, featureName, featureTags, Setup (fun () -> Ok ()) , teardown)
         
    static member Ignore (featurePath, featureName, featureTags: TagsIndicator, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.Ignore (featurePath, featureName, featureTags, Setup (fun () -> Ok ()) , teardown, fileFullName, lineNumber)
        
    static member NewFeature (featurePath, featureName, featureTags: TagsIndicator) =
        Arrow.NewFeature (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()))
    
    static member Ignore (featurePath, featureName, featureTags: TagsIndicator, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.Ignore (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
    
    static member Ignore (featureInfo: string * string, featureTags: TagsIndicator, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featurePath, featureName = featureInfo
        Arrow.Ignore (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
    
    // ------- featureName -------
    static member NewFeature (featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, featureTags, setup, teardown)
        
    static member Ignore (featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, featureTags, setup, teardown, fileFullName, lineNumber)
        
    static member NewFeature (featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, featureTags, setup, Teardown (fun _ _ -> Ok ()))
        
    static member Ignore (featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, featureTags, setup, Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
        
    static member NewFeature (featureName, featureTags: TagsIndicator, teardown: TeardownIndicator<unit>) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), teardown)
        
    static member Ignore (featureName, featureTags: TagsIndicator, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), teardown, fileFullName, lineNumber)
        
    static member NewFeature (featureName, featureTags: TagsIndicator) =
        let _, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()))
    
    static member Ignore (featureName, featureTags: TagsIndicator, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
    
    // // ------- setup -------
    static member NewFeature (featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, featureTags, setup, teardown)
        
    static member Ignore (featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, featureTags, setup, teardown, fileFullName, lineNumber)
        
    static member NewFeature (featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, featureTags, setup)
        
    static member Ignore (featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, featureTags, setup, fileFullName, lineNumber)
        
    // // ------- teardown -------
    static member NewFeature (featureTags: TagsIndicator, teardown: TeardownIndicator<unit>) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, featureTags, teardown)
        
    static member Ignore (featureTags: TagsIndicator, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, featureTags, teardown, fileFullName, lineNumber)
        
    // ------- () -------
    static member NewFeature (featureTags: TagsIndicator) =
        let featureName, featurePath = getNames ()
        Arrow.NewFeature (featurePath, featureName, featureTags)
        
    static member Ignore (featureTags: TagsIndicator, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNames ()
        Arrow.Ignore (featurePath, featureName, featureTags, fileFullName, lineNumber)
        
    // ------- featurePath -------
    static member Tests (featurePath, featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, testBuilder: IScriptFeature<'a> -> unit) =
        let t = baseTransformer setup teardown
        let (TestTags featureTags) = featureTags
        let feature = Feature (featurePath, featureName, featureTags, t)
        feature :> IScriptFeature<'a> |> testBuilder 
        feature.GetTests ()
        
    static member IgnoreTests (featurePath, featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, testBuilder: IScriptFeature<'a> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let t = baseTransformer setup teardown
        let location = getLocation fileFullName lineNumber
        let (TestTags featureTags) = featureTags
        let feature = IgnoreFeature (featurePath, featureName, featureTags, t, location)
        feature :> IScriptFeature<'a> |> testBuilder
        feature.GetTests ()
        
    static member Tests (featurePath, featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, testBuilder: IScriptFeature<'a> -> unit) =
        Arrow.Tests (featurePath, featureName, featureTags, setup, Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member IgnoreTests (featurePath, featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, testBuilder: IScriptFeature<'a> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.IgnoreTests (featurePath, featureName, featureTags, setup, Teardown (fun _ _ -> Ok ()), testBuilder, fileFullName, lineNumber)
        
    static member Tests (featurePath, featureName, featureTags: TagsIndicator, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit) =
        Arrow.Tests (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), teardown, testBuilder)
        
    static member IgnoreTests (featurePath, featureName, featureTags: TagsIndicator, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.IgnoreTests (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), teardown, testBuilder, fileFullName, lineNumber)
        
    static member Tests (featurePath, featureName, featureTags: TagsIndicator, testBuilder: IScriptFeature<unit> -> unit) =
        Arrow.Tests (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member IgnoreTests (featurePath, featureName, featureTags: TagsIndicator, testBuilder: IScriptFeature<unit> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.IgnoreTests (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder, fileFullName, lineNumber)
        
    // ------- featureName -------
    static member Tests (featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, testBuilder: IScriptFeature<'a> -> unit) =
        let _, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, featureTags, setup, teardown, testBuilder)
        
    static member IgnoreTests (featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, testBuilder: IScriptFeature<'a> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNamesAt 3
        Arrow.IgnoreTests (featurePath, featureName, featureTags, setup, teardown, testBuilder, fileFullName, lineNumber)
        
    static member Tests (featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, testBuilder: IScriptFeature<'a> -> unit) =
        let _, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, featureTags, setup, Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member IgnoreTests (featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, testBuilder: IScriptFeature<'a> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNamesAt 3
        Arrow.IgnoreTests (featurePath, featureName, featureTags, setup, Teardown (fun _ _ -> Ok ()), testBuilder, fileFullName, lineNumber)
        
    static member Tests (featureName, featureTags: TagsIndicator, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit) =
        let _, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), teardown, testBuilder)
        
    static member IgnoreTests (featureName, featureTags: TagsIndicator, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNamesAt 3
        Arrow.IgnoreTests (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), teardown, testBuilder, fileFullName, lineNumber)
        
    static member Tests (featureName, featureTags: TagsIndicator, testBuilder: IScriptFeature<unit> -> unit) =
        let _, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member IgnoreTests (featureName, featureTags: TagsIndicator, testBuilder: IScriptFeature<unit> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let _, featurePath = getNamesAt 3
        Arrow.IgnoreTests (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder, fileFullName, lineNumber)
        
    // ------- setup -------
    static member Tests (featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, testBuilder: IScriptFeature<'a> -> unit) =
        let featureName, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, featureTags, setup, teardown, testBuilder)
        
    static member IgnoreTests (featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, teardown: TeardownIndicator<'a>, testBuilder: IScriptFeature<'a> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNamesAt 3
        Arrow.IgnoreTests (featurePath, featureName, featureTags, setup, teardown, testBuilder, fileFullName, lineNumber)
        
    static member Tests (featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, testBuilder: IScriptFeature<'a> -> unit) =
        let featureName, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, featureTags, setup, Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member IgnoreTests (featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, testBuilder: IScriptFeature<'a> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNamesAt 3
        Arrow.IgnoreTests (featurePath, featureName, featureTags, setup, Teardown (fun _ _ -> Ok ()), testBuilder, fileFullName, lineNumber)
        
    // ------- teardown -------
    static member Tests (featureTags: TagsIndicator, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit) =
        let featureName, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), teardown, testBuilder)
        
    static member IgnoreTests (featureTags: TagsIndicator, teardown: TeardownIndicator<unit>, testBuilder: IScriptFeature<unit> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNamesAt 3
        Arrow.IgnoreTests (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), teardown, testBuilder, fileFullName, lineNumber)
        
    // ------- testBuilder -------
    static member Tests (featureTags: TagsIndicator, testBuilder: IScriptFeature<unit> -> unit) =
        let featureName, featurePath = getNamesAt 3
        Arrow.Tests (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder)
        
    static member IgnoreTests (featureTags: TagsIndicator, testBuilder: IScriptFeature<unit> -> unit, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featureName, featurePath = getNamesAt 3
        Arrow.IgnoreTests (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), Teardown (fun _ _ -> Ok ()), testBuilder, fileFullName, lineNumber)
