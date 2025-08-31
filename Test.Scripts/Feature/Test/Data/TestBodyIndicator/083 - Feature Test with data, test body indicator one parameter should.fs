module Archer.Core.Tests.Feature.Test.Data.TestBodyIndicator.``083 - Feature Test with data, test body indicator one parameter should``

open System
open Archer
open Archer.Core
open Archer.Core.Internal.Types
open Archer.Core.Tests
open Archer.Core.Tests.TestBuilders
open Archer.Types.InternalTypes
open Archer.MicroLang.Verification

let private feature = FeatureFactory.NewFeature (
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
        let (_, tests), (data, testNameRoot), (path, fileName, lineNumber) =
            TestBuilder.BuildTestWithDataTestBodyOneParameterNameHints testFeature

        let name1, name2, name3 = TestBuilder.GetTestNames (fun _ -> sprintf "%s %s" testNameRoot) data

        tests
        |> Should.PassAllOf [
            ListShould.HaveLengthOf 3 >> withMessage "Number of tests"

            ListShould.HaveAllValuesPassAllOf [
                getTags >> Should.BeEqualTo [] >> withMessage "Test Tags"
                getFilePath >> Should.BeEqualTo path >> withMessage "File Path"
                getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Name"
                getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]

            List.head >> getTestName >> Should.BeEqualTo name1 >> withMessage "Test Name"

            List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo name2 >> withMessage "Test Name"

            List.last >> getTestName >> Should.BeEqualTo name3 >> withMessage "Test Name"
        ]
    )

let ``Create a test name with name hints and repeating data`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (_, tests), (data, testNameRoot), _ =
            TestBuilder.BuildTestWithDataTestBodyOneParameterNameHints (testFeature, true)

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
        let (_, tests), (data, testName), _ =
            TestBuilder.BuildTestWithDataTestBodyOneParameter testFeature

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
        let (_, tests), (data, testName), _ =
            TestBuilder.BuildTestWithDataTestBodyOneParameter (testFeature, true)

        let name1, name2, name3 = TestBuilder.GetTestNames (fun i v -> sprintf "%s (%A)%s" testName v (if 0 = i then "" else $"^%i{i}")) data

        tests
        |> Should.PassAllOf [
            List.head >> getTestName >> Should.BeEqualTo name1 >> withMessage "Test Name"
            List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo name2 >> withMessage "Test Name"
            List.last >> getTestName >> Should.BeEqualTo name3 >> withMessage "Test Name"
        ]
    )

let ``Call Test when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, tests), (data, _), _ = TestBuilder.BuildTestWithDataTestBodyOneParameter testFeature

        tests
        |> silentlyRunAllTests

        monitor
        |> Should.PassAllOf [
            numberOfTimesTestFunctionWasCalled >> Should.BeEqualTo 3 >> withFailureComment "Incorrect number of test calls"

            verifyAllTestFunctionShouldHaveBeenCalledWithDataOf data

            verifyNoTestWasCalledWithAFeatureSetupValue

            verifyNoTestFunctionWasCalledWithATestSetupValue
            
            verifyNoTestFunctionWasCalledWithTestEnvironment
        ]
        |> withMessage "Test was not called"
    )

let ``Test Cases`` = feature.GetTests ()