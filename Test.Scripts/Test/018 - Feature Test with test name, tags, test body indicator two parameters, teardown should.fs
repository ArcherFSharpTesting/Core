module Archer.Arrows.Tests.Test.``018 - Feature Test with test name, tags, test body indicator two parameters, teardown should``

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
    feature.Test (
        TestBody (fun (_, testFeature: IFeature<string>) ->
            let (_, test), (tags, testName), (path, fileName, lineNumber) =
                TestBuilder.BuildTestWithTestNameTagsTestBodyTwoParametersTeardown testFeature
                
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
    )

let ``Call setup when executed`` =
    feature.Test (
        TestBody (fun (_, testFeature: IFeature<string>) ->
            let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTagsTestBodyTwoParametersTeardown testFeature

            test
            |> silentlyRunTest
            
            monitor.SetupFunctionParameterValues
            |> Should.BeEqualTo []
            |> withMessage "Setup was not called"
        ) 
    )

let ``Call Test when executed`` =
    feature.Test (
        TestBody (fun (featureSetupValue, testFeature: IFeature<string>) ->
            let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTagsTestBodyTwoParametersTeardown testFeature

            test
            |> silentlyRunTest
            
            monitor
            |> Should.PassAllOf [
                numberOfTimesTestFunctionWasCalled >> Should.BeEqualTo 1 >> withFailureComment "Incorrect number of test calls"
                
                noTestWasCalledWithData
                
                allTestFunctionsShouldHaveBeenCalledWithFeatureSetupValueOf featureSetupValue
                
                noTestWasCalledWithATestSetupValue
            ]
            |> withMessage "Test was not called"
        ) 
    )

let ``Call Test with test environment when executed`` =
    feature.Test (
        TestBody (fun (_, testFeature: IFeature<string>) ->
            let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTagsTestBodyTwoParametersTeardown testFeature
                
            test
            |> silentlyRunTest
            
            let getValue v =
                match v with
                | Some value -> value
                | _ -> failwith "No Value"
            
            monitor
            |> testFunctionEnvironmentParameterValues
            |> Should.PassAllOf [
               ListShould.HaveLengthOf 1 >> withMessage "Incorrect number of calls to test"
               
               ListShould.HaveAllValuesPassTestOf <@hasValue@>
               
               ListShould.HaveAllValuesPassAllOf [
                   getValue >> (fun env -> env.ApiEnvironment.ApiName) >> Should.BeEqualTo "Archer.Arrows"
                   getValue >> (fun env -> env.TestInfo) >> Should.BeEqualTo test
               ] 
           ]
        ) 
    )
    
let ``Call teardown when executed`` =
    feature.Test (
        TestBody (fun (_, testFeature: IFeature<string>) ->
            let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTagsTestBodyTwoParametersTeardown testFeature
                
            test
            |> silentlyRunTest
            
            monitor.HasTeardownBeenCalled
            |> Should.BeTrue
            |> withMessage "Teardown was not called"
        ) 
    )

let ``Test Cases`` = feature.GetTests ()