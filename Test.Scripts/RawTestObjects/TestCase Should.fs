module Archer.Arrow.Tests.RawTestObjects.``TestCase Should``

open System
open Archer
open Archer.MicroLang
open Archer.Arrow.Internal

let private container = suite.Container()
let private random = Random ()

let ``has all the data passed to it`` =
    container.Test (fun _ ->
        let expectedContainerPath = $"Container Path %d{random.Next ()}"
        let expectedContainerName = $"Container Name %d{random.Next ()}"
        let expectedTestName = $"Test Name %d{random.Next ()}"
        let expectedFilePath = $"File Path %d{random.Next ()}"
        let expectedFileName = $"FileName%d{random.Next (1, 9999)}.fs"
        let expectedLineNumber = random.Next (1, 9999)
        let expectedTags = [ Category $"Test Category Tag %d{random.Next ()}" ]
        let parts = { Setup = (fun _ -> Ok ()); TestAction = (fun _ _ -> TestSuccess); TearDown = (fun _ _ -> Ok ()) }
        
        let test = TestCase (expectedContainerPath, expectedContainerName, expectedTestName, parts, expectedTags, expectedFilePath, expectedFileName, expectedLineNumber)
        
        test.ContainerPath |> expects.ToBe expectedContainerPath
        |> andResult (test.ContainerName |> expects.ToBe expectedContainerName)
        |> andResult (test.TestName |> expects.ToBe expectedTestName)
        |> andResult (test.Location.FilePath |> expects.ToBe expectedFilePath)
        |> andResult (test.Location.FileName |> expects.ToBe expectedFileName)
        |> andResult (test.Location.LineNumber |> expects.ToBe expectedLineNumber)
        |> andResult (test.Tags |> expects.ToBe expectedTags)
    )
    
let ``Test Cases`` = container.Tests