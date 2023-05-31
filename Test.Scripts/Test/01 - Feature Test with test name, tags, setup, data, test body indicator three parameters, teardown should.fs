module Archer.Arrows.Tests.Test.``01 - Feature Test with test name, tags, setup, data, test body indicator three parameters, teardown should``

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
    feature.Test (fun (_, testFeature: IFeature<string>) ->
       let (_monitor, tests), (tags, _setupValue, data, testNameBase), (path, fileName, lineNumber) =
           TestBuilder.BuildTestWithTestNameTagsSetupDataTestBodyThreeParametersTeardownNameHints testFeature
            
       let name1, name2, name3 = TestBuilder.GetTestNames (fun _ -> sprintf "%s %s" testNameBase) data
        
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
       let (_monitor, tests), (_tags, _setupValue, data, testNameBase), _ =
           TestBuilder.BuildTestWithTestNameTagsSetupDataTestBodyThreeParametersTeardownNameHints (testFeature, true)
        
       let name1, name2, name3 = TestBuilder.GetTestNames (fun i v -> sprintf "%s %s%s" testNameBase v (if 0 = i then "" else $"^%i{i}")) data

       tests
       |> Should.PassAllOf [
           List.head >> getTestName >> Should.BeEqualTo name1 >> withMessage "Test Name"
           List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo name2 >> withMessage "Test Name"
           List.last >> getTestName >> Should.BeEqualTo name3 >> withMessage "Test Name"
       ]
   )

let ``Create a test name with no name hints`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
       let (_monitor, tests), (_tags, _setupValue, data, testName), _ =
           TestBuilder.BuildTestWithTestNameTagsSetupDataTestBodyThreeParametersTeardown testFeature
        
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
       let (_monitor, tests), (_tags, _setupValue, data, testName), _ =
           TestBuilder.BuildTestWithTestNameTagsSetupDataTestBodyThreeParametersTeardown (testFeature, true)
        
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
       let (monitor, tests), _, _ = TestBuilder.BuildTestWithTestNameTagsSetupDataTestBodyThreeParametersTeardown testFeature

       tests
       |> silentlyRunAllTests
        
       monitor.SetupFunctionWasCalledWith
       |> Should.BeEqualTo [featureSetupValue; featureSetupValue; featureSetupValue]
       |> withMessage "Setup was not called"
   )

let ``Call Test when executed`` =
    feature.Test (fun (featureSetupValue, testFeature: IFeature<string>) ->
       let (monitor, tests), (_tags, setupValue, data, _testName), _ = TestBuilder.BuildTestWithTestNameTagsSetupDataTestBodyThreeParametersTeardown testFeature

       tests
       |> silentlyRunAllTests
        
       monitor.TestFunctionWasCalledWith
       |> Should.PassAllOf [
           List.map (fun (a, _, _) -> a) >> Should.BeEqualTo (data |> List.map Some)
           List.map (fun (_, b, _) -> b) >> Should.BeEqualTo [
               Some (Some featureSetupValue, Some setupValue)
               Some (Some featureSetupValue, Some setupValue)
               Some (Some featureSetupValue, Some setupValue)
           ]
       ]
       |> withMessage "Test was not called"
   )

let ``Call Test with return value of setup when executed`` =
    feature.Test (fun (featureSetupValue, testFeature: IFeature<string>) ->
       let (monitor, tests), (_tags, setupValue, data, _testName), _ = TestBuilder.BuildTestWithTestNameTagsSetupDataTestBodyThreeParametersTeardown testFeature

       tests
       |> silentlyRunAllTests
        
       monitor.TestFunctionWasCalledWith
       |> Should.PassAllOf [
           List.map (fun (a, _, _) -> a) >> Should.BeEqualTo (data |> List.map Some)
           List.map (fun (_, b, _) -> b) >> Should.BeEqualTo [
               Some (Some featureSetupValue, Some setupValue)
               Some (Some featureSetupValue, Some setupValue)
               Some (Some featureSetupValue, Some setupValue)
           ]
       ]
       |> withMessage "Test was not called"
   )

let ``Call Test with test environment when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
       let (monitor, tests), _, _ = TestBuilder.BuildTestWithTestNameTagsSetupDataTestBodyThreeParametersTeardown testFeature
            
       tests
       |> silentlyRunAllTests
        
       let getValue v =
           match v with
           | Some value -> value
           | _ -> failwith "No value found"
        
       monitor.TestFunctionWasCalledWith
       |> List.map (fun (_, _, c) -> c)
       |> Should.PassAllOf [
           ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of calls to test"
            
           ListShould.HaveAllValuesPassTestOf <@fun v -> match v with | Some _ -> true | _ -> false@>
            
           List.head >> getValue >> (fun env -> env.ApiEnvironment.ApiName) >> Should.BeEqualTo "Archer.Arrows"
           List.head >> getValue >> (fun env -> env.TestInfo) >> Should.BeEqualTo (tests |> List.head :> ITestInfo)
            
           List.skip 1 >> List.head >> getValue >> (fun env -> env.ApiEnvironment.ApiName) >> Should.BeEqualTo "Archer.Arrows"
           List.skip 1 >> List.head >> getValue >> (fun env -> env.TestInfo) >> Should.BeEqualTo (tests |> List.skip 1 |> List.head :> ITestInfo)
            
           List.last >> getValue >> (fun env -> env.ApiEnvironment.ApiName) >> Should.BeEqualTo "Archer.Arrows"
           List.last >> getValue >> (fun env -> env.TestInfo) >> Should.BeEqualTo (tests |> List.last :> ITestInfo)
       ]
   )
    
let ``Call teardown when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
       let (monitor, tests), _, _ = TestBuilder.BuildTestWithTestNameTagsSetupDataTestBodyThreeParametersTeardown testFeature
            
       tests
       |> silentlyRunAllTests
        
       monitor.HasTeardownBeenCalled
       |> Should.BeTrue
       |> withMessage "Teardown was not called"
   )
    
let ``Test Cases`` = feature.GetTests ()