module Archer.Arrows.Tests.Feature.Test.Setup.TestBodyIndicator.``076 - Feature Test with setup, test body indicator two parameters should``

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
       let (_, test), (_, testName), (path, fileName, lineNumber) =
           TestBuilder.BuildTestWithSetupTestBodyTwoParameters testFeature
            
       test
       |> Should.PassAllOf [
           getTags >> SeqShould.HaveLengthOf 0 >> withMessage "Test Tags"
           getTestName >> Should.BeEqualTo testName >> withMessage "Test Name"
           getFilePath >> Should.BeEqualTo path >> withMessage "File Path"
           getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
           getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
           getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Name"
       ]
   )

let ``Call setup when executed`` =
    feature.Test (fun (featureSetupValue, testFeature: IFeature<string>) ->
       let (monitor, test), _, _ = TestBuilder.BuildTestWithSetupTestBodyTwoParameters testFeature

       test
       |> silentlyRunTest
        
       monitor.SetupFunctionParameterValues
       |> Should.BeEqualTo [featureSetupValue]
       |> withMessage "Setup was not called"
   )

let ``Call Test when executed`` =
    feature.Test (fun (featureSetupValue, testFeature: IFeature<string>) ->
       let (monitor, test), (setupValue, _), _ = TestBuilder.BuildTestWithSetupTestBodyTwoParameters testFeature

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
    
let ``Test Cases`` = feature.GetTests ()