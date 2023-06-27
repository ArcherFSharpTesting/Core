module Archer.Arrows.Tests.Test.TestName.Tags.Data.TestBodyIndicator.Teardown.``009 - Feature Test with test name, tags, data, test body indicator three parameters, teardown should``

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
        let (_, tests), (tags, data, testNameRoot), (path, fileName, lineNumber) =
            TestBuilder.BuildTestWithTestNameTagsDataTestBodyThreeParametersTeardownNameHints testFeature

        let name1, name2, name3 = TestBuilder.GetTestNames (fun _ -> sprintf "%s %s" testNameRoot) data

        tests
        |> Should.PassAllOf [
            ListShould.HaveLengthOf 3 >> withMessage "Number of tests"

            List.head >> getTags >> Should.BeEqualTo tags >> withMessage "Test Tags"
            List.head >> getTestName >> Should.BeEqualTo name1 >> withMessage "Test Name"
            List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "File Path"
            List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
            List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Name"

            List.skip 1 >> List.head >> getTags >> Should.BeEqualTo tags >> withMessage "Test Tags"
            List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo name2 >> withMessage "Test Name"
            List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "File Path"
            List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
            List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Name"

            List.last >> getTags >> Should.BeEqualTo tags >> withMessage "Test Tags"
            List.last >> getTestName >> Should.BeEqualTo name3 >> withMessage "Test Name"
            List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "File Path"
            List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
            List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Name"
        ]
)

let ``Create a test name with name hints and repeating data`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (_, tests), (_, data, testNameRoot), _ =
            TestBuilder.BuildTestWithTestNameTagsDataTestBodyThreeParametersTeardownNameHints (testFeature, true)

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
        let (_, tests), (_, data, testName), _ =
            TestBuilder.BuildTestWithTestNameTagsDataTestBodyThreeParametersTeardown testFeature

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
        let (_, tests), (_, data, testName), _ =
            TestBuilder.BuildTestWithTestNameTagsDataTestBodyThreeParametersTeardown (testFeature, true)

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
        let (monitor, tests), _, _ = TestBuilder.BuildTestWithTestNameTagsDataTestBodyThreeParametersTeardown testFeature

        tests
        |> silentlyRunAllTests

        monitor.SetupFunctionParameterValues
        |> Should.BeEqualTo []
        |> withMessage "Setup was not called"
    )

let ``Call Test when executed`` =
    feature.Test (fun (featureSetupValue, testFeature: IFeature<string>) ->
        let (monitor, tests), (_, data, _), _ = TestBuilder.BuildTestWithTestNameTagsDataTestBodyThreeParametersTeardown testFeature

        tests
        |> silentlyRunAllTests

        monitor//.TestFunctionWasCalledWith
        |> Should.PassAllOf [
            numberOfTimesTestFunctionWasCalled >> Should.BeEqualTo 3
            
            verifyAllTestFunctionShouldHaveBeenCalledWithDataOf data
            
            verifyAllTestFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf featureSetupValue
            
            verifyNoTestWasCalledWithATestSetupValue
        ]
        |> withMessage "Test was not called"
    )

let ``Call Test with return value of setup when executed`` =
    feature.Test (fun (featureSetupValue, testFeature: IFeature<string>) ->
        let (monitor, tests), (_, data, _), _ = TestBuilder.BuildTestWithTestNameTagsDataTestBodyThreeParametersTeardown testFeature

        tests
        |> silentlyRunAllTests

        monitor//.TestFunctionWasCalledWith
        |> Should.PassAllOf [
            numberOfTimesTestFunctionWasCalled >> Should.BeEqualTo 3
            
            verifyAllTestFunctionShouldHaveBeenCalledWithDataOf data
            
            verifyAllTestFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf featureSetupValue
        
            verifyNoTestWasCalledWithATestSetupValue
        ]
        |> withMessage "Test was not called"
    )

let ``Call Test with test environment when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, tests), _, _ = TestBuilder.BuildTestWithTestNameTagsDataTestBodyThreeParametersTeardown testFeature

        tests
        |> silentlyRunAllTests

        let getValue v =
            match v with
            | Some value -> value
            | _ -> failwith "No value found"

        monitor
        |> testFunctionEnvironmentParameterValues
        |> Should.PassAllOf [
            ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of calls to test"

            ListShould.HaveAllValuesPassTestOf <@hasValue@>
            
            ListShould.HaveAllValuesPassAllOf [
                getValue >> (fun env -> env.ApiEnvironment.ApiName) >> Should.BeEqualTo "Archer.Arrows"
                getValue >> (fun env -> env.TestInfo) >> (fun testInfo -> tests |> List.map (fun t -> t :> ITestInfo) |> ListShould.Contain testInfo )
            ]
            
            List.map (getValue >> (fun env -> env.TestInfo)) >> List.distinct >> List.length >> Should.BeEqualTo tests.Length >> withFailureComment "Not all tests are distinct" 
        ]
    )

let ``Call teardown when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, tests), _, _ = TestBuilder.BuildTestWithTestNameTagsDataTestBodyThreeParametersTeardown testFeature

        tests
        |> silentlyRunAllTests

        monitor.NumberOfTimesTeardownFunctionWasCalled
        |> Should.BeEqualTo 3
        |> withMessage "Teardown was called an incorrect number of times"
    )

let ``Test Cases`` = feature.GetTests ()