module Archer.Arrows.Tests.Feature.``Test Data Method name first without environment should``

open Archer
open Archer.Arrows
open Archer.Arrows.Internal.Types
open Archer.Arrows.Internals
open Archer.Arrows.Tests
open Archer.CoreTypes.InternalTypes
open Archer.MicroLang.Verification

let private feature = Arrow.NewFeature (
    TestTags [
        Category "Feature"
        Category "Test"
    ]
)

// Tags, Setup, TestBody, Teardown 
let ``return an ITest with everything when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testTags = 
                [
                   Only
                   Category "My Category"
               ]
            let testName = "My test %i"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    testName,
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data [1; 2; 3],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    emptyTeardown,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test 1" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test 2" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test 3" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``return an ITest with everything when everything is passed no name hints`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testTags = 
                [
                   Only
                   Category "My Category"
               ]
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    testName,
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data [1; 2; 3],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    emptyTeardown,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test (1)" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (2)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test (3)" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``run setup method passed to it when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data [1; 2; 3],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    emptyTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.NumberOfTimesSetupWasCalled
            |> Should.BeEqualTo 3
            |> withMessage "Setup was not called"
        )
    )
    
let ``run the test method passed to it when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data [1;5;9],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.NumberOfTimesTestWasCalled
            |> Should.BeEqualTo 3
            |> withMessage "test was not called"
        )
    )
    
let ``run the test method defined with everything by calling it with data`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data [1;5;9],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.TestDataWas
            |> Should.BeEqualTo [1;5;9]
            |> withMessage "test was not called"
        )
    )
    
let ``run the teardown method passed to it when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data [1; 2],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.NumberOfTimesTeardownWasCalled
            |> Should.BeEqualTo 2
            |> withMessage "teardown was not called"
        )
    )

// Tags, Setup, TestBody!
let ``return an ITest with everything when given no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testTags = 
                [
                   Only
                   Category "My Category"
               ]
            let testName = "My test %c"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    testName,
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data ['Z';'p';'M'],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test Z" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test p" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test M" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``return an ITest with everything when given no teardown no name hints`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testTags = 
                [
                   Only
                   Category "My Category"
               ]
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    testName,
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data ['Z';'p';'p'],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test ('Z')" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test ('p')" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test ('p')^1" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``run setup method passed to it when given no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data ["First"; "Second"; "Last"],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.NumberOfTimesSetupWasCalled
            |> Should.BeEqualTo 3
            |> withMessage "Setup was not called"
        )
    )
    
let ``run the test method passed to it when given no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<string, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data ["One"; "Two"; "Three"],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.NumberOfTimesTestWasCalled
            |> Should.BeEqualTo 3
            |> withMessage "test was not called"
        )
    )
    
let ``run the test method passed to it when given no teardown by calling it with data`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<string, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data ["One"; "Two"; "Three"],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.TestDataWas
            |> Should.BeEqualTo ["One"; "Two"; "Three"]
            |> withMessage "test was not called"
        )
    )

// Tags, TestBody, Teardown!
let ``return an ITest with everything when given no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testTags = 
                [
                   Only
                   Category "My Category"
               ]
            let testName = "My test %s"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Test (
                    testName,
                    TestTags testTags,
                    Data ["Two"; "Four"; "Me"],
                    TestBody (fun _ -> TestSuccess),
                    emptyTeardown,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            tests
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test Two" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test Four" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test Me" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``run the test method passed to it when given no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Data [2; 5; 8;],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
                
            monitor.NumberOfTimesTestWasCalled
            |> Should.BeEqualTo 3
        )
    )
    

    
let ``run the test method passed to it when given no setup by calling it with data`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Data [2; 5; 8;],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
                
            monitor.TestDataWas
            |> Should.BeEqualTo [2; 5; 8]
        )
    )
    
let ``run the teardown method passed to it when given no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test %i",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Data [7; 8; 3],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.NumberOfTimesTeardownWasCalled
            |> Should.BeEqualTo 3
            |> withMessage "teardown was not called"
        )
    )

// Tags, TestBody!
let ``return an ITest with everything when given no setup or teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testTags = 
                [
                   Only
                   Category "My Category"
               ]
            let testName = "My test %s"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Test (
                    testName,
                    TestTags testTags,
                    Data ["Passes"; "Rules"; "is written"],
                    TestBody (fun _ -> TestSuccess),
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            tests
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test Passes" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test Rules" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test is written" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``return an ITest with everything when given no setup or teardown no name hints`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testTags = 
                [
                   Only
                   Category "My Category"
               ]
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Test (
                    testName,
                    TestTags testTags,
                    Data ["Passes"; "Rules"; "is written"],
                    TestBody (fun _ -> TestSuccess),
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (\"Passes\")" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (\"Rules\")" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (\"is written\")" >> withMessage "TestName"
            ]
        )
    )
    
let ``return an ITest with everything when given no setup, teardown, or test body indicator`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testTags = 
                [
                   Only
                   Category "My Category"
               ]
            let testName = "My test %A"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Test (
                    testName,
                    TestTags testTags,
                    Data [(6,"Tin"); (4,"Soldier")],
                    (fun _ -> TestSuccess),
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            tests
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 2 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test (6, \"Tin\")" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test (4, \"Soldier\")" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``return an ITest with everything when given no setup, teardown, or test body indicator, no name hints`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testTags = 
                [
                   Only
                   Category "My Category"
               ]
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Test (
                    testName,
                    TestTags testTags,
                    Data [(6,"Tin"); (4,"Soldier")],
                    (fun _ -> TestSuccess),
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test ((6, \"Tin\"))" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test ((4, \"Soldier\"))" >> withMessage "TestName"
            ]
        )
    )
    
let ``run the test method passed to it when given no setup, teardown, or test body indicator`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int * string, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Data [(1, "Partridge in a Pear Tree"); (2, "Turtle Doves"); (3, "French Hens")],
                    monitor.CallTestActionWithData,
                    "D:\\dog.bark",
                    73
                )
            
            tests
            |> silentlyRunAllTests
            
            monitor.NumberOfTimesTestWasCalled
            |> Should.BeEqualTo 3
            |> withMessage "test fas not called"
        )
    )
    
let ``run the test method passed to it when given no setup, teardown, or test body indicator by calling it with test data`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int * string, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Data [(1, "Partridge in a Pear Tree"); (2, "Turtle Doves"); (3, "French Hens")],
                    monitor.CallTestActionWithData,
                    "D:\\dog.bark",
                    73
                )
            
            tests
            |> silentlyRunAllTests
            
            monitor.TestDataWas
            |> Should.BeEqualTo [(1, "Partridge in a Pear Tree"); (2, "Turtle Doves"); (3, "French Hens")]
            |> withMessage "test fas not called"
        )
    )

// Setup, TestBody, Teardown
let ``return an ITest with everything when given no tags`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test %i"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Test (
                    testName,
                    Setup (fun _ -> Ok ()),
                    Data [6; 2; 8],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    emptyTeardown,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            tests
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect Number of Tests"
                
                List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test 6" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test 2" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test 8" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``return an ITest with everything when given no tags no name hints`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Test (
                    testName,
                    Setup (fun _ -> Ok ()),
                    Data [6; 2; 8],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    emptyTeardown,
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (6)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (2)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (8)" >> withMessage "TestName"
            ]
        )
    )
    
let ``run setup method passed to it when given no tags`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    Setup monitor.CallSetup,
                    Data [5; 1; 7],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    emptyTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.NumberOfTimesSetupWasCalled
            |> Should.BeEqualTo 3
            |> withMessage "Setup was not called"
        )
    )
    
let ``run the test method passed to it when given no tags`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    Setup monitor.CallSetup,
                    Data [5; 9; 3],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.NumberOfTimesTestWasCalled
            |> Should.BeEqualTo 3
            |> withMessage "test was not called"
        )
    )
    
let ``run the test method passed to it when given no tags by calling it with test data`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    Setup monitor.CallSetup,
                    Data [5; 9; 3],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.TestDataWas
            |> Should.BeEqualTo [5; 9; 3]
            |> withMessage "test was not called"
        )
    )
    
let ``run the teardown method passed to it when given no tags`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<char * char, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    Setup monitor.CallSetup,
                    Data [('a', 'Z'); ('z', 'D'); ('d', 'A')],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.NumberOfTimesTeardownWasCalled
            |> Should.BeEqualTo 3
            |> withMessage "teardown was not called"
        )
    )

// Setup, TestBody
let ``return an ITest with everything when given no tags, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test %A"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Test (
                    testName,
                    Setup (fun _ -> Ok ()),
                    Data [('a', 'F'); ('f', 'G'); ('g', 'A')],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            tests
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test ('a', 'F')" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test ('f', 'G')" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test ('g', 'A')" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``return an ITest with everything when given no tags, no teardown, no name hints`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Test (
                    testName,
                    Setup (fun _ -> Ok ()),
                    Data [('a', 'F'); ('f', 'G'); ('g', 'A')],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (('a', 'F'))" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (('f', 'G'))" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (('g', 'A'))" >> withMessage "TestName"
            ]
        )
    )
    
let ``run setup method passed to it when given no tags, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    Setup monitor.CallSetup,
                    Data ['a'; 'b'; 'c'; 'd'],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.NumberOfTimesSetupWasCalled
            |> Should.BeEqualTo 4
            |> withMessage "Setup was not called"
        )
    )
    
let ``run the test method passed to it when given no tags, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<char, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    Setup monitor.CallSetup,
                    Data ['a'; 'b'; 'c'; 'd'],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.NumberOfTimesTestWasCalled
            |> Should.BeEqualTo 4
            |> withMessage "test was not called"
        )
    )
    
let ``run the test method passed to it when given no tags, no teardown by calling it with test data`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<char, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    Setup monitor.CallSetup,
                    Data ['a'; 'b'; 'c'; 'd'],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.TestDataWas
            |> Should.BeEqualTo ['a'; 'b'; 'c'; 'd']
            |> withMessage "test was not called"
        )
    )

// TestBody, Teardown
let ``return an ITest with everything when given no tags, no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test %s"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    testName,
                    Data ["hawk"; "ghost"; "blue"],
                    TestBody (fun _ -> TestSuccess),
                    emptyTeardown,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test hawk" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test ghost" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test blue" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``return an ITest with everything when given no tags, no setup, no name hints`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    testName,
                    Data ["hawk"; "ghost"; "blue"],
                    TestBody (fun _ -> TestSuccess),
                    emptyTeardown,
                    fullPath,
                    lineNumber
                )
        
            test
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (\"hawk\")" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (\"ghost\")" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (\"blue\")" >> withMessage "TestName"
            ]
        )
    )
    
let ``run the test method passed to it when given no tags, no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    Data [99; 100; -22],
                    TestBody monitor.CallTestActionWithData,
                    Teardown monitor.CallTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.NumberOfTimesTestWasCalled
            |> Should.BeEqualTo 3
            |> withMessage "test was not called"
        )
    )
    
let ``run the test method passed to it when given no tags, no setup by calling it with test data`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    Data [99; 100; -22],
                    TestBody monitor.CallTestActionWithData,
                    Teardown monitor.CallTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.TestDataWas
            |> Should.BeEqualTo [99; 100; -22]
            |> withMessage "test was not called"
        )
    )
    
let ``run the teardown method passed to it when given no tags, no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    Data [99; 100; -22],
                    TestBody monitor.CallTestActionWithData,
                    Teardown monitor.CallTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.NumberOfTimesTeardownWasCalled
            |> Should.BeEqualTo 3
            |> withMessage "teardown was not called"
        )
    )

// TestBody
let ``return an ITest with everything when given no tags, no setup, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My %i test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    testName,
                    Data [66; -33; 0],
                    TestBody (fun _ -> TestSuccess),
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My 66 test" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My -33 test" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My 0 test" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``return an ITest with everything when given no tags, no setup, no teardown, no name hints`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    testName,
                    Data [66; -33; 0],
                    TestBody (fun _ -> TestSuccess),
                    fullPath,
                    lineNumber
                )
        
            test
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (66)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (-33)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (0)" >> withMessage "TestName"
            ]
        )
    )
    
let ``run the test method passed to it when given no tags, no setup, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    Data [6; -999; 32],
                    TestBody monitor.CallTestActionWithData,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.NumberOfTimesTestWasCalled
            |> Should.BeEqualTo 3
            |> withMessage "test was not called"
        )
    )
    
let ``run the test method passed to it when given no tags, no setup, no teardown by calling it with test data`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    Data [6; -999; 32],
                    TestBody monitor.CallTestActionWithData,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.TestDataWas
            |> Should.BeEqualTo [6; -999; 32]
            |> withMessage "test was not called"
        )
    )

let ``return an ITest with everything when given no tags, no setup, no teardown, no test body indicator`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test %i"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    testName,
                    Data [0; -753; -6842],
                    (fun _ -> TestSuccess),
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test 0" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test -753" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test -6842" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )

let ``return an ITest with everything when given no tags, no setup, no teardown, no test body indicator, no name hints`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    testName,
                    Data [0; -753; -6842],
                    (fun _ -> TestSuccess),
                    fullPath,
                    lineNumber
                )
        
            test
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (0)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (-753)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (-6842)" >> withMessage "TestName"
            ]
        )
    )
    
let ``run the test method passed to it when given no tags, no setup, no teardown, no test body indicator`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    Data (seq{1..4}),
                    monitor.CallTestActionWithData,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.NumberOfTimesTestWasCalled
            |> Should.BeEqualTo 4
            |> withMessage "test was not called"
        )
    )
    
let ``run the test method passed to it when given no tags, no setup, no teardown, no test body indicator by calling it with test data`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    "My test",
                    Data (seq{1..4}),
                    monitor.CallTestActionWithData,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.TestDataWas
            |> Should.BeEqualTo [1; 2; 3; 4]
            |> withMessage "test was not called"
        )
    )

let ``Test Cases`` = feature.GetTests ()