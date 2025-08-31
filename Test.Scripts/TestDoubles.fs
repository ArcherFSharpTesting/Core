[<AutoOpen>]
module Archer.Core.Tests.TestDoubles

open System
open Archer
open Archer.Types.InternalTypes
open Archer.MicroLang.Types.TypeSupport
open Archer.MicroLang.Lang
    
let getFakeCodeLocation () =
    let getRandomLetter () =
        let letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
        let i = random0To letters.Length
        letters[i].ToString ()
        
    let getRandomDriveLetter () =
        getRandomLetter().ToUpperInvariant ()
        
    let getRandomWord () =
        let count = random0To 9 |> (+) 1
        let letters = [ for _ in 1..count -> getRandomLetter() ]
        String.Join ("", letters)
        
    let getRandomPath () =
        let count = random0To 4 |> (+) 1
        let words = [ for _ in 1..count -> getRandomWord () ]
        let b = String.Join ("\\", words)
        $"%s{getRandomDriveLetter ()}:\\\\FakePath\\%s{b}"
        
    let getRandomFileName () =
        $"%s{getRandomWord ()}.fs"
        
    {
        FilePath = getRandomPath ()
        FileName = getRandomFileName ()
        LineNumber = randomInt () 
    }

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
    let location = getFakeCodeLocation ()
    getDummyTest (ignoreString ()) (ignoreString ()) (ignoreString ()) $"%s{location.FilePath}\\%s{location.FileName}" location.LineNumber
    
let getFakeEnvironment () =
    {
        RunnerName = ignoreString ()
        RunnerVersion = Version "0.0.0.0"
        TestInfo = getEmptyDummyTest ()
    }
    
let resultIsIgnored =
    <@fun result ->
        match result with
        | TestExecutionResult (TestFailure (TestIgnored _)) -> true
        | _ -> false
    @>