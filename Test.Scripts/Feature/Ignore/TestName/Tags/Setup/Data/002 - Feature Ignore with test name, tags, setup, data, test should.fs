module Archer.Core.Tests.Ignore.TestName.Tags.Setup.Data.``002 - Feature Ignore with test name, tags, setup, data, test should``

open System
open Archer
open Archer.Core
open Archer.Core.Internal.Types
open Archer.Core.Tests
open Archer.Core.Tests.IgnoreBuilders
open Archer.Types.InternalTypes
open Archer.MicroLang.Verification

let private feature = FeatureFactory.NewFeature (
    TestTags [
        Category "Feature"
        Category "Ignore"
    ],
    Setup setupFeatureUnderTest
)

let private rand = Random ()

let private getContainerName (test: ITest) =
    $"%s{test.ContainerPath}.%s{test.ContainerName}"
    
let ``Create a valid ITest`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (_, tests), (tags, data, testNameRoot), (path, fileName, lineNumber) =
            IgnoreBuilder.BuildTestWithTestNameTagsSetupDataTestBodyNameHints testFeature
            
        let name1, name2, name3 = IgnoreBuilder.GetTestNames (fun _ -> sprintf "%s %s" testNameRoot) data
        
        tests
        |> Should.PassAllOf [
            ListShould.HaveLengthOf 3 >> withMessage "Number of tests"
            
            List.head >> getTags >> Should.BeEqualTo tags >> withMessage "Test Tags"
            List.head >> getTestName >> Should.BeEqualTo name1 >> withMessage "Test Name"
            List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "File Path"
            List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
            List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Name"
            
            List.skip 1 >> List.head >> getTags >> Should.BeEqualTo tags >> withMessage "Test Tags"
            List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo name2 >> withMessage "Test Name"
            List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "File Path"
            List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
            List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Name"
            
            List.last >> getTags >> Should.BeEqualTo tags >> withMessage "Test Tags"
            List.last >> getTestName >> Should.BeEqualTo name3 >> withMessage "Test Name"
            List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "File Path"
            List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
            List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Name"
        ]
    ) 

let ``Create a test name with name hints and repeating data`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (_, tests), (_, data, testNameRoot), _ =
            IgnoreBuilder.BuildTestWithTestNameTagsSetupDataTestBodyNameHints (testFeature, true)
        
        let name1, name2, name3 = IgnoreBuilder.GetTestNames (fun i v -> sprintf "%s %s%s" testNameRoot v (if 0 = i then "" else $"^%i{i}")) data

        tests
        |> Should.PassAllOf [
            List.head >> getTestName >> Should.BeEqualTo name1 >> withMessage "Test Name"
            List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo name2 >> withMessage "Test Name"
            List.last >> getTestName >> Should.BeEqualTo name3 >> withMessage "Test Name"
        ]
    ) 

let ``Create a test name with no name hints`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (_, tests), (_, data, testName), _ =
            IgnoreBuilder.BuildTestWithTestNameTagsSetupDataTestBody testFeature
        
        let name1, name2, name3 = IgnoreBuilder.GetTestNames (fun _ -> sprintf "%s (%A)" testName) data

        tests
        |> Should.PassAllOf [
            List.head >> getTestName >> Should.BeEqualTo name1 >> withMessage "Test Name"
            List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo name2 >> withMessage "Test Name"
            List.last >> getTestName >> Should.BeEqualTo name3 >> withMessage "Test Name"
        ]
    ) 

let ``Create a test name with no name hints same data repeated`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (_, tests), (_, data, testName), _ = IgnoreBuilder.BuildTestWithTestNameTagsSetupDataTestBody (testFeature, true)
        
        let name1, name2, name3 = IgnoreBuilder.GetTestNames (fun i v -> sprintf "%s (%A)%s" testName v (if 0 = i then "" else $"^%i{i}")) data

        tests
        |> Should.PassAllOf [
            List.head >> getTestName >> Should.BeEqualTo name1 >> withMessage "Test Name"
            List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo name2 >> withMessage "Test Name"
            List.last >> getTestName >> Should.BeEqualTo name3 >> withMessage "Test Name"
        ]
    ) 

let ``Not call setup when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, tests), _, _ = IgnoreBuilder.BuildTestWithTestNameTagsSetupDataTestBodyNameHints testFeature

        tests
        |> silentlyRunAllTests
        
        monitor
        |> verifyNoSetupFunctionsHaveBeenCalled
    ) 

let ``Not call Test when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, tests), _, _ = IgnoreBuilder.BuildTestWithTestNameTagsSetupDataTestBodyNameHints testFeature

        tests
        |> silentlyRunAllTests
        
        monitor
        |> verifyNoTestFunctionsHaveBeenCalled
    ) 
    
let ``Test Cases`` = feature.GetTests ()
