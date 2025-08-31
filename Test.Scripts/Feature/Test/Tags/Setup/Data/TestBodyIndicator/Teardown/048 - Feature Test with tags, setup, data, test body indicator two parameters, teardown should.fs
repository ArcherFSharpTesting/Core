module Archer.Core.Tests.Feature.Test.Tags.Setup.Data.TestBodyIndicator.Teardown.``048 - Feature Test with tags, setup, data, test body indicator two parameters, teardown should``

open System
open Archer
open Archer.Core
open Archer.Core.Internal.Types
open Archer.Core.Tests
open Archer.Core.Tests.TestBuilders
open Archer.CoreTypes.InternalTypes
open Archer.MicroLang.Verification

let private feature = FeatureBuilder.NewFeature (
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
    feature.Test (fun (_, testFeature: IFeature<string>) ->
       let (_, tests), (tags, _, data, testNameRoot), (path, fileName, lineNumber) =
           TestBuilder.BuildTestWithTagsSetupDataTestBodyTwoParametersTeardownNameHints testFeature
            
       let name1, name2, name3 = TestBuilder.GetTestNames (fun _ -> sprintf "%s %s" testNameRoot) data
        
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
       let (_, tests), (_, _, data, testNameRoot), _ =
           TestBuilder.BuildTestWithTagsSetupDataTestBodyTwoParametersTeardownNameHints (testFeature, true)
        
       let name1, name2, name3 = TestBuilder.GetTestNames (fun i v -> sprintf "%s %s%s" testNameRoot v (if 0 = i then "" else $"^%i{i}")) data

       tests
       |> Should.PassAllOf [
           List.head >> getTestName >> Should.BeEqualTo name1 >> withMessage "Test Name"
           List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo name2 >> withMessage "Test Name"
           List.last >> getTestName >> Should.BeEqualTo name3 >> withMessage "Test Name"
       ]
   )

let ``Create a test name with no name hints`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
       let (_, tests), (_, _, data, testName), _ =
           TestBuilder.BuildTestWithTagsSetupDataTestBodyTwoParametersTeardown testFeature
        
       let name1, name2, name3 = TestBuilder.GetTestNames (fun _ -> sprintf "%s (%A)" testName) data

       tests
       |> Should.PassAllOf [
           List.head >> getTestName >> Should.BeEqualTo name1 >> withMessage "Test Name"
           List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo name2 >> withMessage "Test Name"
           List.last >> getTestName >> Should.BeEqualTo name3 >> withMessage "Test Name"
       ]
   )

let ``Create a test name with no name hints same data repeated`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
       let (_, tests), (_, _, data, testName), _ =
           TestBuilder.BuildTestWithTagsSetupDataTestBodyTwoParametersTeardown (testFeature, true)
        
       let name1, name2, name3 = TestBuilder.GetTestNames (fun i v -> sprintf "%s (%A)%s" testName v (if 0 = i then "" else $"^%i{i}")) data

       tests
       |> Should.PassAllOf [
           List.head >> getTestName >> Should.BeEqualTo name1 >> withMessage "Test Name"
           List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo name2 >> withMessage "Test Name"
           List.last >> getTestName >> Should.BeEqualTo name3 >> withMessage "Test Name"
       ]
   )

let ``Call setup when executed`` =
    feature.Test (fun (featureSetupValue, testFeature: IFeature<string>) ->
       let (monitor, tests), _, _ = TestBuilder.BuildTestWithTagsSetupDataTestBodyTwoParametersTeardown testFeature

       tests
       |> silentlyRunAllTests
        
       monitor
        |> Should.PassAllOf [
            numberOfTimesSetupFunctionWasCalled >> Should.BeEqualTo tests.Length >> withFailureComment "Setup was called an incorrect number of times"
            
            verifyAllSetupFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf featureSetupValue
        ]
   )

let ``Call Test when executed`` =
    feature.Test (fun (featureSetupValue, testFeature: IFeature<string>) ->
        let (monitor, tests), (_, setupValue, data, _), _ = TestBuilder.BuildTestWithTagsSetupDataTestBodyTwoParametersTeardown testFeature

        tests
        |> silentlyRunAllTests


        monitor
        |> Should.PassAllOf [
            numberOfTimesTestFunctionWasCalled >> Should.BeEqualTo 3
            
            testFunctionDataParameterValues >> Should.BeEqualTo (data |> List.map Some)
            
            verifyAllTestFunctionShouldHaveBeenCalledWithTestSetupValueOf setupValue
            
            verifyAllTestFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf featureSetupValue
            
            verifyNoTestFunctionWasCalledWithTestEnvironment
        ]
        |> withMessage "Test was not called"
    )
    
let ``Call teardown when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, tests), _, _ = TestBuilder.BuildTestWithTagsSetupDataTestBodyTwoParametersTeardown testFeature

        tests
        |> silentlyRunAllTests

        monitor.NumberOfTimesTeardownFunctionWasCalled
        |> Should.BeEqualTo 3
        |> withMessage "Teardown was called an incorrect number of times"
    )
    
let ``Test Cases`` = feature.GetTests ()