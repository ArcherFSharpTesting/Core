module Archer.Arrows.Tests.Test.``Feature Test with tags setup data TestFunctionThreeParameters should``

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
            let testNameBase = $"My %s{randomWord 5} Test"
            let testName = $"%s{testNameBase} %%s"
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
            
            let data = randomDistinctLetters 3
            
            let [ name1; name2; name3 ] =
                data
                |> List.map (sprintf "%s %s" testNameBase)
            
            let tests =
                testFeature.Test (
                    TestTags tags,
                    Setup monitor.CallSetup,
                    Data data,
                    TestBody monitor.CallTestActionWithDataSetupEnvironment,
                    testName,
                    fullPath,
                    lineNumber
                )
                
            tests
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Number of tests"
                
                List.head >> getTags >> Should.BeEqualTo tags >> withMessage "Test Tags"
                List.head >> getTestName >> Should.BeEqualTo name1 >> withMessage "Test Name"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "File Path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Name"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo tags >> withMessage "Test Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo name2 >> withMessage "Test Name"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "File Path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Name"
                
                List.last >> getTags >> Should.BeEqualTo tags >> withMessage "Test Tags"
                List.last >> getTestName >> Should.BeEqualTo name3 >> withMessage "Test Name"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "File Path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Name"
            ]
        ) 
    )

let ``Create a test name with name hints and repeating data`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testNameBase = $"My %s{randomWord 5} Test"
            let testName = $"%s{testNameBase} %%s"
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
            
            let data =
                let l = randomLetter ()
                [l; l; l]
            
            let [ name1; name2; name3 ] =
                data
                |> List.mapi (fun i v -> sprintf "%s %s%s" testNameBase v (if 0 = i then "" else $"^%i{i}"))
            
            let tests =
                testFeature.Test (
                    TestTags tags,
                    Setup monitor.CallSetup,
                    Data data,
                    TestBody monitor.CallTestActionWithDataSetupEnvironment,
                    testName,
                    fullPath,
                    lineNumber
                )
                
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo name1 >> withMessage "Test Name"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo name2 >> withMessage "Test Name"
                List.last >> getTestName >> Should.BeEqualTo name3 >> withMessage "Test Name"
            ]
        ) 
    )

let ``Create a test name with no name hints`` =
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
            
            let data = randomDistinctLetters 3
            
            let [ name1; name2; name3 ] =
                data
                |> List.map (sprintf "%s (%A)" testName)
            
            let tests =
                testFeature.Test (
                    TestTags tags,
                    Setup monitor.CallSetup,
                    Data data,
                    TestBody monitor.CallTestActionWithDataSetupEnvironment,
                    testName,
                    fullPath,
                    lineNumber
                )
                
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo name1 >> withMessage "Test Name"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo name2 >> withMessage "Test Name"
                List.last >> getTestName >> Should.BeEqualTo name3 >> withMessage "Test Name"
            ]
        ) 
    )

let ``Create a test name with no name hints same data repeated`` =
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
            
            let data =
                let l = randomLetter ()
                [l; l; l]
            
            let [ name1; name2; name3 ] =
                data
                |> List.mapi (fun i v -> sprintf "%s (%A)%s" testName v (if 0 = i then "" else $"^%i{i}"))
            
            let tests =
                testFeature.Test (
                    TestTags tags,
                    Setup monitor.CallSetup,
                    Data data,
                    TestBody monitor.CallTestActionWithDataSetupEnvironment,
                    testName,
                    fullPath,
                    lineNumber
                )
                
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo name1 >> withMessage "Test Name"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo name2 >> withMessage "Test Name"
                List.last >> getTestName >> Should.BeEqualTo name3 >> withMessage "Test Name"
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
            
            let data = randomDistinctLetters 3
            
            let tags = [
                Category "Not important"
                Only
                Serial
            ]
            
            let tests =
                testFeature.Test (
                    TestTags tags,
                    Setup monitor.CallSetup,
                    Data data,
                    TestBody monitor.CallTestActionWithDataSetupEnvironment,
                    testName,
                    fullPath,
                    lineNumber
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.NumberOfTimesSetupWasCalled
            |> Should.BeEqualTo 3
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
            
            let data = randomDistinctLetters 3     
            
            let tags = [
                Category "Not important"
                Only
                Serial
            ]
            
            let tests =
                testFeature.Test (
                    TestTags tags,
                    Setup monitor.CallSetup,
                    Data data,
                    TestBody monitor.CallTestActionWithDataSetupEnvironment,
                    testName,
                    fullPath,
                    lineNumber
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.NumberOfTimesTestWasCalled
            |> Should.BeEqualTo 3
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
            
            let data = randomDistinctLetters 3
            
            let tags = [
                Category "Not important"
                Only
                Serial
            ]
            
            let tests =
                testFeature.Test (
                    TestTags tags,
                    Setup monitor.CallSetup,
                    Data data,
                    TestBody monitor.CallTestActionWithDataSetupEnvironment,
                    testName,
                    fullPath,
                    lineNumber
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.TestInputSetupWas
            |> Should.BeEqualTo [setupValue; setupValue; setupValue]
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
            
            let data = randomDistinctLetters 3
            
            let tags = [
                Category "Not important"
                Only
                Serial
            ]
            
            let tests =
                testFeature.Test (
                    TestTags tags,
                    Setup monitor.CallSetup,
                    Data data,
                    TestBody monitor.CallTestActionWithDataSetupEnvironment,
                    testName,
                    fullPath,
                    lineNumber
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.TestEnvironmentWas
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of calls to test"
                
                List.head >> (fun env -> env.ApiEnvironment.ApiName) >> Should.BeEqualTo "Archer.Arrows"
                List.head >> (fun env -> env.TestInfo) >> Should.BeEqualTo (tests |> List.head :> ITestInfo)
                
                List.skip 1 >> List.head >> (fun env -> env.ApiEnvironment.ApiName) >> Should.BeEqualTo "Archer.Arrows"
                List.skip 1 >> List.head >> (fun env -> env.TestInfo) >> Should.BeEqualTo (tests |> List.skip 1 |> List.head :> ITestInfo)
                
                List.last >> (fun env -> env.ApiEnvironment.ApiName) >> Should.BeEqualTo "Archer.Arrows"
                List.last >> (fun env -> env.TestInfo) >> Should.BeEqualTo (tests |> List.last :> ITestInfo)
            ]
        ) 
    )

let ``Call Test with test data when executed`` =
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
            
            let data = randomDistinctLetters 3
            
            let tags = [
                Category "Not important"
                Only
                Serial
            ]
            
            let tests =
                testFeature.Test (
                    TestTags tags,
                    Setup monitor.CallSetup,
                    Data data,
                    TestBody monitor.CallTestActionWithDataSetupEnvironment,
                    testName,
                    fullPath,
                    lineNumber
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.TestDataWas
            |> Should.BeEqualTo data
        ) 
    )

let ``Test Cases`` = feature.GetTests ()