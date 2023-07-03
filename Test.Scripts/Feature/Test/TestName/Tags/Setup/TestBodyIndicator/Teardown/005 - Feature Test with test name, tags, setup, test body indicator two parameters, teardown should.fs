module Archer.Arrows.Tests.Feature.Test.TestName.Tags.Setup.TestBodyIndicator.Teardown.``005 - Feature Test with test name, tags, setup, test body indicator two parameters, teardown should``

open System
open Archer
open Archer.Arrows
open Archer.Arrows.Internal.Types
open Archer.Arrows.Tests
open Archer.Arrows.Tests.TestBuilders
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
       let (_, test), (tags, _, testName), (path, fileName, lineNumber) =
           TestBuilder.BuildTestWithTestNameTagsSetupTestBodyTwoParametersTeardown testFeature
            
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
       let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTagsSetupTestBodyTwoParametersTeardown testFeature

       test
       |> silentlyRunTest
        
       monitor
        |> Should.PassAllOf [
            numberOfTimesSetupFunctionWasCalled >> Should.BeEqualTo 1 >> withFailureComment "Setup was called an incorrect number of times"
            
            verifyAllSetupFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf featureSetupValue
        ]
   )

let ``Call Test when executed`` =
    feature.Test (fun (featureSetupValue, testFeature: IFeature<string>) ->
       let (monitor, test), (_, setupValue, _), _ = TestBuilder.BuildTestWithTestNameTagsSetupTestBodyTwoParametersTeardown testFeature

       test
       |> silentlyRunTest
        
       monitor
       |> Should.PassAllOf [
           numberOfTimesTestFunctionWasCalled >> Should.BeEqualTo 1
           
           verifyNoTestFunctionWasCalledWithData
           
           verifyAllTestFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf featureSetupValue
           
           verifyAllTestFunctionShouldHaveBeenCalledWithTestSetupValueOf setupValue
           
           verifyAllTestFunctionsWereCalledWithTestEnvironmentContaining [test]
       ]
       |> withMessage "Test was not called"
   )
    
let ``Call teardown when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
       let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTagsSetupTestBodyTwoParametersTeardown testFeature
            
       test
       |> silentlyRunTest
        
       monitor.HasTeardownBeenCalled
       |> Should.BeTrue
       |> withMessage "Teardown was not called"
   )
    
let ``Test Cases`` = feature.GetTests ()