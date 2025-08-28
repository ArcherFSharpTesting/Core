[<AutoOpen>]
module Archer.Arrows.ArrowTags

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows.Helpers
open Archer.Arrows.Internal.Types
open Archer.Arrows.Internals

type Arrow with
    
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