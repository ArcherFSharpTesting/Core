module Archer.Arrows.Tests.Test.TestName.Tags.Setup.TestBodyIndicator.``008 - Feature Test with test name, tags, setup, test body indicator one parameter should``

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
       let (_monitor, test), (tags, _setupValue, testName), (path, fileName, lineNumber) =
           TestBuilder.BuildTestWithTestNameTagsSetupTestBodyOneParameter testFeature
            
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

let ``Call setup when executed`` =
    feature.Test (fun (featureSetupValue, testFeature: IFeature<string>) ->
       let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTagsSetupTestBodyOneParameter testFeature

       test
       |> silentlyRunTest
        
       monitor.SetupFunctionParameterValues
       |> Should.BeEqualTo [featureSetupValue]
       |> withMessage "Setup was not called"
   )

let ``Call Test when executed`` =
    feature.Test (fun (featureSetupValue, testFeature: IFeature<string>) ->
       let (monitor, tests), (_, setupValue, _), _ = TestBuilder.BuildTestWithTestNameTagsSetupTestBodyOneParameter testFeature

       tests
       |> silentlyRunTest
        
       monitor
       |> Should.PassAllOf [
           numberOfTimesTestFunctionWasCalled >> Should.BeEqualTo 1
           
           verifyNoTestWasCalledWithData
           
           verifyAllTestFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf featureSetupValue
           
           verifyAllTestFunctionShouldHaveBeenCalledWithTestSetupValueOf setupValue
       ]
       |> withMessage "Test was not called"
   )

let ``Not call Test with test environment when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
       let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTagsSetupTestBodyOneParameter testFeature
            
       test
       |> silentlyRunTest
        
       monitor
       |> verifyNoTestWasCalledWithTestEnvironment
   )
    
let ``Call teardown when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
       let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTagsSetupTestBodyOneParameter testFeature
            
       test
       |> silentlyRunTest
        
       monitor.HasTeardownBeenCalled
       |> Should.BeFalse
       |> withMessage "Teardown was called"
   )
    
let ``Test Cases`` = feature.GetTests ()