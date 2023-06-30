module Archer.Arrows.Tests.Feature.Test.TestName.TestBodyIndicator.Teardown.``042 - Feature Test with test name, test body indicator one parameter, teardown should``

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
        let (_, test), testName, (path, fileName, lineNumber) =
            TestBuilder.BuildTestWithTestNameTestBodyOneParameterTearDown testFeature

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

let ``Call Test when executed`` =
    feature.Test (fun (featureSetupValue, testFeature: IFeature<string>) ->
        let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTestBodyOneParameterTearDown testFeature

        test
        |> silentlyRunTest

        monitor
        |> Should.PassAllOf [
            numberOfTimesTestFunctionWasCalled >> Should.BeEqualTo 1 >> withFailureComment "Incorrect number of tests"

            verifyNoTestWasCalledWithData

            verifyAllTestFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf featureSetupValue
            
            verifyNoTestWasCalledWithATestSetupValue
        ]
        |> withMessage "Test was not called"
    )

let ``Call Test with test environment when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTestBodyOneParameterTearDown testFeature

        test
        |> silentlyRunTest

        monitor
        |> verifyNoTestWasCalledWithTestEnvironment
    )
    
let ``Call teardown when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTestBodyOneParameterTearDown testFeature

        test
        |> silentlyRunTest

        monitor.HasTeardownBeenCalled
        |> Should.BeTrue
        |> withMessage "Teardown was not called"
    )

let ``Test Cases`` = feature.GetTests ()