module Archer.Arrows.Tests.Test.``06 - Feature Test with test name, tags, setup, test body indicator one parameter, teardown should``

open System
open Archer
open Archer.Arrows
open Archer.Arrows.Internal.Types
open Archer.Arrows.Tests
open Archer.CoreTypes.InternalTypes
open Archer.MicroLang.Verification

let private feature = Arrow.NewFeature (
    TestTags [
        Category "Feature"
        Category "Test"
    ],
    Setup setupFeatureUnderTest
)

let private rand = Random ()

let private getContainerName (test: ITest) =
    $"%s{test.ContainerPath}.%s{test.ContainerName}"
    
let ``Create a valid ITest`` =
    feature.Test (
        TestBody (fun (_, testFeature: IFeature<string>) ->
            let (_monitor, test), (tags, _setupValue, testName), (path, fileName, lineNumber) =
                TestBuilder.BuildTestWithTestNameTagsSetupTestBodyOneParameterTeardown testFeature
                
            test
            |> Should.PassAllOf [
                getTags >> Should.BeEqualTo tags >> withMessage "Test Tags"
                getTestName >> Should.BeEqualTo testName >> withMessage "Test Name"
                getFilePath >> Should.BeEqualTo path >> withMessage "File Path"
                getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Name"
            ]
        ) 
    )

let ``Call setup when executed`` =
    feature.Test (
        TestBody (fun (featureSetupValue, testFeature: IFeature<string>) ->
            let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTagsSetupTestBodyOneParameterTeardown testFeature

            test
            |> silentlyRunTest
            
            monitor.SetupFunctionWasCalledWith
            |> Should.BeEqualTo [featureSetupValue]
            |> withMessage "Setup was not called"
        ) 
    )

let ``Call Test when executed`` =
    feature.Test (
        TestBody (fun (featureSetupValue, testFeature: IFeature<string>) ->
            let (monitor, tests), (_, setupValue, _), _ = TestBuilder.BuildTestWithTestNameTagsSetupTestBodyOneParameterTeardown testFeature

            tests
            |> silentlyRunTest
            
            monitor.TestFunctionWasCalledWith
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 1
                List.map (fun (a, _, _) -> a) >> Should.BeEqualTo [ None ]
                List.map (fun (_, b, _) -> b) >> Should.BeEqualTo [
                    Some (Some featureSetupValue, setupValue)
                ]
            ]
            |> withMessage "Test was not called"
        ) 
    )

let ``Call Test with test environment when executed`` =
    feature.Test (
        TestBody (fun (_, testFeature: IFeature<string>) ->
            let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTagsSetupTestBodyOneParameterTeardown testFeature
                
            test
            |> silentlyRunTest
            
            monitor.TestFunctionWasCalledWith
            |> List.map (fun (_, _, c) -> c)
            |> Should.BeEqualTo [ None ]
        ) 
    )
    
let ``Call teardown when executed`` =
    feature.Test (
        TestBody (fun (_, testFeature: IFeature<string>) ->
            let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTagsSetupTestBodyOneParameterTeardown testFeature
                
            test
            |> silentlyRunTest
            
            monitor.HasTeardownBeenCalled
            |> Should.BeTrue
            |> withMessage "Teardown was not called"
        ) 
    )
    
let ``Test Cases`` = feature.GetTests ()