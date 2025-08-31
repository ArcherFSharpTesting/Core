module Archer.Core.Tests.Feature.Test.Setup.TestBodyIndicator.Teardown.``074 - Feature Test with setup, test body indicator two parameters, teardown should``

open System
open Archer
open Archer.Core
open Archer.Core.Internal.Types
open Archer.Core.Tests
open Archer.Core.Tests.TestBuilders
open Archer.Types.InternalTypes
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
            TestBuilder.BuildTestWithSetupTestBodyTwoParametersTeardown testFeature

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
        let (monitor, test), _, _ = TestBuilder.BuildTestWithSetupTestBodyTwoParametersTeardown testFeature

        test
        |> silentlyRunTest

        monitor
        |> verifyAllSetupFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf featureSetupValue
    )

let ``Call Test when executed`` =
    feature.Test (fun (featureSetupValue, testFeature: IFeature<string>) ->
        let (monitor, test), (testSetupValue, _), _ = TestBuilder.BuildTestWithSetupTestBodyTwoParametersTeardown testFeature

        test
        |> silentlyRunTest

        monitor
        |> Should.PassAllOf [
            numberOfTimesTestFunctionWasCalled >> Should.BeEqualTo 1 >> withFailureComment "Incorrect number of tests"

            verifyNoTestFunctionWasCalledWithData

            verifyAllTestFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf featureSetupValue

            verifyAllTestFunctionShouldHaveBeenCalledWithTestSetupValueOf testSetupValue
            
            verifyAllTestFunctionsWereCalledWithTestEnvironmentContaining [test]
        ]
        |> withMessage "Test was not called"
    )
    
let ``Call teardown when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, test), _, _ = TestBuilder.BuildTestWithSetupTestBodyTwoParametersTeardown testFeature

        test
        |> silentlyRunTest

        monitor.HasTeardownBeenCalled
        |> Should.BeTrue
        |> withMessage "Teardown was not called"
    )

let ``Test Cases`` = feature.GetTests ()