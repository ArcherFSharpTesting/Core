module Archer.Arrows.Tests.Test.TestName.Setup.TestBodyIndicator.``031 - Feature Test with test name, setup, test body indicator one parameter should``

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
        let (_, test), (_, testName), (path, fileName, lineNumber) =
            TestBuilder.BuildTestWithTestNameSetupTestBodyOneParameter testFeature

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
        let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameSetupTestBodyOneParameter testFeature

        test
        |> silentlyRunTest

        monitor
        |> allSetupFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf featureSetupValue
    )

let ``Call Test when executed`` =
    feature.Test (fun (featureSetupValue, testFeature: IFeature<string>) ->
        let (monitor, test), (testSetupValue, _), _ = TestBuilder.BuildTestWithTestNameSetupTestBodyOneParameter testFeature

        test
        |> silentlyRunTest

        monitor
        |> Should.PassAllOf [
            numberOfTimesTestFunctionWasCalled >> Should.BeEqualTo 1 >> withFailureComment "Incorrect number of tests"

            noTestWasCalledWithData

            allTestFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf featureSetupValue

            allTestFunctionShouldHaveBeenCalledWithTestSetupValueOf testSetupValue
        ]
        |> withMessage "Test was not called"
    )

let ``Call Test with test environment when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameSetupTestBodyOneParameter testFeature

        test
        |> silentlyRunTest

        let getValue v =
            match v with
            | Some value -> value
            | _ -> failwith "No Value"

        monitor
        |> noTestWasCalledWithTestEnvironment
    )
    
let ``Call teardown when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameSetupTestBodyOneParameter testFeature

        test
        |> silentlyRunTest

        monitor.HasTeardownBeenCalled
        |> Should.BeFalse
        |> withMessage "Teardown was called"
    )

let ``Test Cases`` = feature.GetTests ()