module Archer.Arrows.Tests.RawTestObjects.``TestCase Should``

open System
open Archer
open Archer.Arrows
open Archer.Arrows.Internals
open Archer.MicroLang

let private container = Arrow.NewFeature (
    TestTags [
        Category "TestCase"
    ]
)

let private random = Random ()

let ``has all the data passed to it`` =
    container.Test (fun () ->
        let expectedContainerPath = $"Container Path %d{random.Next ()}"
        let expectedContainerName = $"Container Name %d{random.Next ()}"
        let expectedTestName = $"Test Name %d{random.Next ()}"
        let expectedFilePath = $"File Path %d{random.Next ()}"
        let expectedFileName = $"FileName%d{random.Next (1, 9999)}.fs"
        let expectedLineNumber = random.Next (1, 9999)
        let expectedTags = [ Category $"Test Category Tag %d{random.Next ()}" ]
        
        let workings = SetupTeardownExecutor (successfulUnitSetup, successfulTeardown, fun value env -> env |> successfulEnvironmentTest value |> TestExecutionResult)
        
        let test = TestCase (expectedContainerPath, expectedContainerName, expectedTestName, workings, expectedTags, expectedFilePath, expectedFileName, expectedLineNumber)
        
        test.ContainerPath |> expects.ToBe expectedContainerPath
        |> andResult (test.ContainerName |> expects.ToBe expectedContainerName)
        |> andResult (test.TestName |> expects.ToBe expectedTestName)
        |> andResult (test.Location.FilePath |> expects.ToBe expectedFilePath)
        |> andResult (test.Location.FileName |> expects.ToBe expectedFileName)
        |> andResult (test.Location.LineNumber |> expects.ToBe expectedLineNumber)
        |> andResult (test.Tags |> expects.ToBe expectedTags)
    )
    
let ``have a decent ToString`` =
    container.Test (
        fun () ->
            let feature = Arrow.NewFeature ("TestCase", "ToStringTests")
            let test = feature.Test ((fun _ _ -> TestSuccess), "ToString should")
            
            test.ToString ()
            |> expects.ToBe "TestCase.ToStringTests.ToString should"
    )
    
let ``Test Cases`` = container.GetTests ()