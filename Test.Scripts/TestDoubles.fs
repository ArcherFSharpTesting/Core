[<AutoOpen>]
module Archer.Arrow.Tests.TestDoubles

open System
open Archer
open Archer.CoreTypes.InternalTypes
open Archer.MicroLang.Types.TypeSupport
open Archer.MicroLang.Lang

let getDummyTest containerPath containerName testName fileName lineNumber =
    let info =
        { new ITestInfo with
            member _.ContainerPath with get () = containerPath
            member _.ContainerName with get () = containerName
            member _.TestName with get () = testName
            member _.Location with get () = buildLocation fileName lineNumber
            member _.Tags with get () = []
        }
        
    let test =
        { new ITest with
            member _.ContainerPath with get () = info.ContainerPath
            member _.ContainerName with get () = info.ContainerName
            member _.TestName with get () = info.TestName
            member _.Location with get () = info.Location
            member _.Tags with get () = info.Tags
            member _.GetExecutor () = failwith "Not implemented"
        }
        
    test
    
let getEmptyDummyTest () =
    getDummyTest (ignoreString ()) (ignoreString ()) (ignoreString ()) (ignoreString ()) (ignoreInt ())
    
let getFakeEnvironment () =
    {
        FrameworkName = ignoreString ()
        FrameworkVersion = Version ("0.0.0.0")
        TestInfo = getEmptyDummyTest ()
    }