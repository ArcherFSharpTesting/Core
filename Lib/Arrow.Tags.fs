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
        Arrow.NewFeature (featurePath, featureName, featureTags, setup, emptyTeardown)
        
    static member Ignore (featurePath, featureName, featureTags: TagsIndicator, setup: SetupIndicator<unit, 'a>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.Ignore (featurePath, featureName, featureTags, setup, emptyTeardown, fileFullName, lineNumber)
        
    static member NewFeature (featurePath, featureName, featureTags: TagsIndicator, teardown: TeardownIndicator<unit>) =
        Arrow.NewFeature (featurePath, featureName, featureTags, Setup (fun () -> Ok ()) , teardown)
         
    static member Ignore (featurePath, featureName, featureTags: TagsIndicator, teardown: TeardownIndicator<unit>, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.Ignore (featurePath, featureName, featureTags, Setup (fun () -> Ok ()) , teardown, fileFullName, lineNumber)
        
    static member NewFeature (featurePath, featureName, featureTags: TagsIndicator) =
        Arrow.NewFeature (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), emptyTeardown)
    
    static member Ignore (featurePath, featureName, featureTags: TagsIndicator, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        Arrow.Ignore (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), emptyTeardown, fileFullName, lineNumber)
    
    static member Ignore (featureInfo: string * string, featureTags: TagsIndicator, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let featurePath, featureName = featureInfo
        Arrow.Ignore (featurePath, featureName, featureTags, Setup (fun () -> Ok ()), emptyTeardown, fileFullName, lineNumber)
    
    // ------- featureName -------
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