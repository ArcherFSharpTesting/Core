module Archer.Arrows.Tests.Test.``Feature Test with test name, tags, setup, data, test body indicator two parameters, teardown should``
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
            let testName = $"My %s{randomWord 5} Test"
            let path = $"%s{randomCapitalLetter ()}:\\"
            let fileName = $"%s{randomWord (rand.Next (1, 5))}.%s{randomLetter ()}"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = rand.Next ()
            let setupValue = rand.Next ()
            
            let monitor = Monitor (Ok setupValue)
            
            let tags = [
                Category "Not important"
                Only
                Serial
            ]
            
            let test =
                testFeature.Test (
                    testName,
                    TestTags tags,
                    Setup monitor.CallSetup,
                    TestBody monitor.CallTestActionWithSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    fullPath,
                    lineNumber
                )
                
            test
            |> Should.PassAllOf [
                getTags >> Should.BeEqualTo tags >> withMessage "Test Tags"
                getTestName >> Should.BeEqualTo testName >> withMessage "Test Name"
                getFilePath >> Should.BeEqualTo path >> withMessage "File Path"
                getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Name"
            ]
        ) 
    )

let ``Call setup when executed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = $"My %s{randomWord 5} Test"
            let path = $"%s{randomCapitalLetter ()}:\\"
            let fileName = $"%s{randomWord (rand.Next (1, 5))}.%s{randomLetter ()}"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = rand.Next ()
            let setupValue = rand.Next ()
            
            let monitor = Monitor (Ok setupValue)
            
            let tags = [
                Category "Not important"
                Only
                Serial
            ]
            
            let test =
                testFeature.Test (
                    testName,
                    TestTags tags,
                    Setup monitor.CallSetup,
                    TestBody monitor.CallTestActionWithSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    fullPath,
                    lineNumber
                )
                
            test
            |> silentlyRunTest
            
            monitor.SetupWasCalled
            |> Should.BeTrue
            |> withMessage "Setup was not called"
        ) 
    )

let ``Call Test when executed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = $"My %s{randomWord 5} Test"
            let path = $"%s{randomCapitalLetter ()}:\\"
            let fileName = $"%s{randomWord (rand.Next (1, 5))}.%s{randomLetter ()}"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = rand.Next ()
            let setupValue = rand.Next ()
            
            let monitor = Monitor (Ok setupValue)
            
            let tags = [
                Category "Not important"
                Only
                Serial
            ]
            
            let test =
                testFeature.Test (
                    testName,
                    TestTags tags,
                    Setup monitor.CallSetup,
                    TestBody monitor.CallTestActionWithSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    fullPath,
                    lineNumber
                )
                
            test
            |> silentlyRunTest
            
            monitor.TestWasCalled
            |> Should.BeTrue
            |> withMessage "Test was not called"
        ) 
    )

let ``Call Test with return value of setup when executed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = $"My %s{randomWord 5} Test"
            let path = $"%s{randomCapitalLetter ()}:\\"
            let fileName = $"%s{randomWord (rand.Next (1, 5))}.%s{randomLetter ()}"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = rand.Next ()
            let setupValue = rand.Next ()
            
            let monitor = Monitor (Ok setupValue)
            
            let tags = [
                Category "Not important"
                Only
                Serial
            ]
            
            let test =
                testFeature.Test (
                    testName,
                    TestTags tags,
                    Setup monitor.CallSetup,
                    TestBody monitor.CallTestActionWithSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    fullPath,
                    lineNumber
                )
                
            test
            |> silentlyRunTest
            
            monitor.TestInputSetupWas
            |> Should.BeEqualTo [setupValue]
            |> withMessage "Test was not called"
        ) 
    )

let ``Call Test with test environment when executed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = $"My %s{randomWord 5} Test"
            let path = $"%s{randomCapitalLetter ()}:\\"
            let fileName = $"%s{randomWord (rand.Next (1, 5))}.%s{randomLetter ()}"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = rand.Next ()
            let setupValue = rand.Next ()
            
            let monitor = Monitor (Ok setupValue)
            
            let tags = [
                Category "Not important"
                Only
                Serial
            ]
            
            let test =
                testFeature.Test (
                    testName,
                    TestTags tags,
                    Setup monitor.CallSetup,
                    TestBody monitor.CallTestActionWithSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    fullPath,
                    lineNumber
                )
                
            test
            |> silentlyRunTest
            
            monitor.TestEnvironmentWas
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 1 >> withMessage "Incorrect number of calls to test"
                List.head >> (fun env -> env.ApiEnvironment.ApiName) >> Should.BeEqualTo "Archer.Arrows"
                List.head >> (fun env -> env.ApiEnvironment.ApiVersion) >> Should.PassTestOf <@fun v -> 0 < v.ToString().Length@>
                List.head >> (fun env -> env.TestInfo) >> Should.BeEqualTo test
            ]
        ) 
    )
    
let ``Call teardown when executed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = $"My %s{randomWord 5} Test"
            let path = $"%s{randomCapitalLetter ()}:\\"
            let fileName = $"%s{randomWord (rand.Next (1, 5))}.%s{randomLetter ()}"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = rand.Next ()
            let setupValue = rand.Next ()
            
            let monitor = Monitor (Ok setupValue)
            
            let tags = [
                Category "Not important"
                Only
                Serial
            ]
            
            let test =
                testFeature.Test (
                    testName,
                    TestTags tags,
                    Setup monitor.CallSetup,
                    TestBody monitor.CallTestActionWithSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    fullPath,
                    lineNumber
                )
                
            test
            |> silentlyRunTest
            
            monitor.TeardownWasCalled
            |> Should.BeTrue
            |> withMessage "Teardown was not called"
        ) 
    )

let ``Test Cases`` = feature.GetTests ()