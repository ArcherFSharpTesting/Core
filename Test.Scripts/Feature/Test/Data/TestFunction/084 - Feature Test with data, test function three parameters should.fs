module Archer.Arrows.Tests.Feature.Test.Data.TestFunction.``084 - Feature Test with data, test function three parameters should``

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
        let (_, tests), (data, testNameRoot), (path, fileName, lineNumber) =
            TestBuilder.BuildTestWithDataTestFunctionThreeParametersNameHints testFeature

        let name1, name2, name3 = TestBuilder.GetTestNames (fun _ -> sprintf "%s %s" testNameRoot) data

        tests
        |> Should.PassAllOf [
            ListShould.HaveLengthOf 3 >> withMessage "Number of tests"
            
            ListShould.HaveAllValuesPassAllOf [
                getTags >> SeqShould.HaveLengthOf 0 >> withFailureComment "has tags"
                getFilePath >> Should.BeEqualTo path >> withMessage "File Path"
                getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Name"
            ]

            List.head >> getTestName >> Should.BeEqualTo name1 >> withMessage "Test Name"
            List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo name2 >> withMessage "Test Name"
            List.last >> getTestName >> Should.BeEqualTo name3 >> withMessage "Test Name"
        ]
    )

let ``Create a test name with name hints and repeating data`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (_, tests), (data, testNameRoot), _ =
            TestBuilder.BuildTestWithDataTestFunctionThreeParametersNameHints (testFeature, true)

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
            TestBuilder.BuildTestWithDataTestFunctionThreeParameters testFeature

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
            TestBuilder.BuildTestWithDataTestFunctionThreeParameters (testFeature, true)

        let name1, name2, name3 = TestBuilder.GetTestNames (fun i v -> sprintf "%s (%A)%s" testName v (if 0 = i then "" else $"^%i{i}")) data

        tests
        |> Should.PassAllOf [
            List.head >> getTestName >> Should.BeEqualTo name1 >> withMessage "Test Name"
            List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo name2 >> withMessage "Test Name"
            List.last >> getTestName >> Should.BeEqualTo name3 >> withMessage "Test Name"
        ]
    )

let ``Call setup when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, tests), _, _ = TestBuilder.BuildTestWithDataTestFunctionThreeParameters testFeature

        tests
        |> silentlyRunAllTests

        monitor.SetupFunctionParameterValues
        |> Should.BeEqualTo []
        |> withMessage "Setup was not called"
    )

let ``Call Test when executed`` =
    feature.Test (fun (featureSetupValue, testFeature: IFeature<string>) ->
        let (monitor, tests), (data, _), _ = TestBuilder.BuildTestWithDataTestFunctionThreeParameters testFeature

        tests
        |> silentlyRunAllTests

        monitor
        |> Should.PassAllOf [
            numberOfTimesTestFunctionWasCalled >> Should.BeEqualTo 3 >> withFailureComment "Incorrect number of test calls"

            verifyAllTestFunctionShouldHaveBeenCalledWithDataOf data

            verifyAllTestFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf featureSetupValue

            verifyNoTestWasCalledWithATestSetupValue
        ]
        |> withMessage "Test was not called"
    )

let ``Call Test with test environment when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, tests), _, _ = TestBuilder.BuildTestWithDataTestFunctionThreeParameters testFeature

        tests
        |> silentlyRunAllTests

        let getValue v =
            match v with
            | Some value -> value
            | _ -> failwith "No value"

        monitor
        |> testFunctionEnvironmentParameterValues
        |> Should.PassAllOf [
            ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of calls to test"

            ListShould.HaveAllValuesPassTestOf <@hasValue@>

            ListShould.HaveAllValuesPassAllOf [
                getValue >> (fun env -> env.ApiEnvironment.ApiName) >> Should.BeEqualTo "Archer.Arrows"
                getValue >> (fun env -> env.TestInfo) >> (fun ti -> tests |> List.map (fun t -> t :> ITestInfo) |> ListShould.Contain ti)
            ]

            List.map (getValue >> (fun env -> env.TestInfo)) >> List.distinct >> ListShould.HaveLengthOf tests.Length >> withFailureComment "tests not distinct" 
        ]
    )
    
let ``Call teardown when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, tests), _, _ = TestBuilder.BuildTestWithDataTestFunctionThreeParameters testFeature

        tests
        |> silentlyRunAllTests

        monitor.HasTeardownBeenCalled
        |> Should.BeFalse
        |> withMessage "Teardown was called"
    )

let ``Test Cases`` = feature.GetTests ()