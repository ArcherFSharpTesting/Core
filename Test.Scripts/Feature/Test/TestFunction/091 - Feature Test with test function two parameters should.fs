module Archer.Arrows.Tests.Feature.Test.TestFunction.``091 - Feature Test with test function two parameters should``

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
            TestBuilder.BuildTestWithTestFunctionTwoParameters testFeature

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

let ``Call Test when executed`` =
    feature.Test (fun (featureSetupValue, testFeature: IFeature<string>) ->
        let (monitor, test), _, _ = TestBuilder.BuildTestWithTestFunctionTwoParameters testFeature

        test
        |> silentlyRunTest

        monitor
        |> Should.PassAllOf [
            hasTestFunctionBeenCalled >> Should.BeTrue >> withFailureComment "Test function not called"

            verifyAllTestFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf featureSetupValue

            verifyNoTestWasCalledWithATestSetupValue
        ]
        |> withMessage "Test was not called"
    )

let ``Not call Test with test environment when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, test), _, _ = TestBuilder.BuildTestWithTestFunctionTwoParameters testFeature

        test
        |> silentlyRunTest

        monitor.HasTestFunctionBeenCalledWithEnvironmentParameter
        |> Should.BeTrue
        |> withFailureComment "Test not called with environment"
    )

let ``Test Cases`` = feature.GetTests ()