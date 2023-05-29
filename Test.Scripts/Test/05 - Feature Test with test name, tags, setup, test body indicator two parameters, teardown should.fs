module Archer.Arrows.Tests.Test.``05 - Feature Test with test name, tags, setup, test body indicator two parameters, teardown should``

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
    ]
)

let private rand = Random ()

let private getContainerName (test: ITest) =
    $"%s{test.ContainerPath}.%s{test.ContainerName}"
    
let ``Create a valid ITest`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let (_monitor, test), (tags, _setupValue, testName), (path, fileName, lineNumber) =
                TestBuilder.BuildTestWithTestNameTagsSetupTestBodyTwoParametersTeardown testFeature
                
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
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTagsSetupTestBodyTwoParametersTeardown testFeature

            test
            |> silentlyRunTest
            
            monitor.NumberOfTimesSetupWasCalled
            |> Should.BeEqualTo 1
            |> withMessage "Setup was not called"
        ) 
    )

let ``Call Test when executed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let (monitor, tests), _, _ = TestBuilder.BuildTestWithTestNameTagsSetupTestBodyTwoParametersTeardown testFeature

            tests
            |> silentlyRunTest
            
            monitor.NumberOfTimesTestWasCalled
            |> Should.BeEqualTo 1
            |> withMessage "Test was not called"
        ) 
    )

let ``Call Test with return value of setup when executed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let (monitor, test), (_, setupValue, _), _ =
                TestBuilder.BuildTestWithTestNameTagsSetupTestBodyTwoParametersTeardown testFeature

            test
            |> silentlyRunTest
            
            monitor.TestInputSetupWas
            |> Should.BeEqualTo [ setupValue ]
            |> withMessage "Test was not called"
        ) 
    )

let ``Call Test with test environment when executed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTagsSetupTestBodyTwoParametersTeardown testFeature
                
            test
            |> silentlyRunTest
            
            monitor.TestEnvironmentWas
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 1 >> withMessage "Incorrect number of calls to test"
                
                List.head >> (fun env -> env.ApiEnvironment.ApiName) >> Should.BeEqualTo "Archer.Arrows"
                List.head >> (fun env -> env.TestInfo) >> Should.BeEqualTo test
            ]
        ) 
    )

let ``Call Test with test data when executed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTagsSetupTestBodyTwoParametersTeardown testFeature
                
            test
            |> silentlyRunTest
            
            monitor.TestDataWas
            |> Should.BeEqualTo []
        ) 
    )
    
let ``Call teardown when executed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let (monitor, test), _, _ = TestBuilder.BuildTestWithTestNameTagsSetupTestBodyTwoParametersTeardown testFeature
                
            test
            |> silentlyRunTest
            
            monitor.TeardownWasCalled
            |> Should.BeTrue
            |> withMessage "Teardown was not called"
        ) 
    )
    
let ``Test Cases`` = feature.GetTests ()