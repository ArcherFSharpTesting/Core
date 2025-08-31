module Archer.Core.Tests.Feature.Test.TestName.Setup.TestBodyIndicator.Teardown.``029 - Feature Test with test name, setup, test body indicator one parameter, teardown should``

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
        let (_, test), (_, testName), (path, fileName, lineNumber) =
            TestBuilder.BuildTestWithTestNameSetupTestBodyOneParameterTeardown testFeature

        test
        |> Should.PassAllOf [
            getTags >> Should.BeEqualTo [] >> withMessage "Test Tags"
            getTestName >> Should.BeEqualTo testName >> withMessage "Test Name"
            getFilePath >> Should.BeEqualTo path >> withMessage "File Path"
            getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
            getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Name"
        ]
    )

let ``Call setup when executed`` =
    feature.Test (fun (featureSetupValue, testFeature: IFeature<string>) ->
        let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameSetupTestBodyOneParameterTeardown testFeature

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
        let (monitor, test), (testSetupValue, _), _ = TestBuilder.BuildTestWithTestNameSetupTestBodyOneParameterTeardown testFeature

        test
        |> silentlyRunTest

        monitor
        |> Should.PassAllOf [
            numberOfTimesTestFunctionWasCalled >> Should.BeEqualTo 1 >> withFailureComment "Incorrect number of tests"

            verifyNoTestFunctionWasCalledWithData

            verifyAllTestFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf featureSetupValue

            verifyAllTestFunctionShouldHaveBeenCalledWithTestSetupValueOf testSetupValue
            
            verifyNoTestFunctionWasCalledWithTestEnvironment
        ]
        |> withMessage "Test was not called"
    )
    
let ``Call teardown when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameSetupTestBodyOneParameterTeardown testFeature

        test
        |> silentlyRunTest

        monitor.HasTeardownBeenCalled
        |> Should.BeTrue
        |> withMessage "Teardown was not called"
    )

let ``Test Cases`` = feature.GetTests ()