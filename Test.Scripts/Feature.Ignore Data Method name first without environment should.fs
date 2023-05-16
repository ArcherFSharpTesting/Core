module Archer.Arrows.Tests.Feature.``Ignore Data Method name first without environment should``

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
        Category "Ignore"
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
            
            let tests =
                testFeature.Ignore (
                    testName,
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data (seq { 3..5 }),
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
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
                List.head >> getTestName >> Should.BeEqualTo "My test 3" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test 4" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test 5" >> withMessage "TestName"
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
            
            let tests =
                testFeature.Ignore (
                    testName,
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data (seq { 3..5 }),
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    emptyTeardown,
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (3)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (4)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (5)" >> withMessage "TestName"
            ]
        )
    )
    
let ``not run setup method passed to it when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data (seq{ 3..5 }),
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    emptyTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            test
            |> silentlyRunAllTests
            
            monitor.SetupWasCalled
            |> Should.BeFalse
            |> withMessage "Setup was called"
        )
    )
    
let ``not run the test method passed to it when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data (seq{ 3..5 }),
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            test
            |> silentlyRunAllTests
            
            monitor.TestWasCalled
            |> Should.BeFalse
            |> withMessage "Test was called"
        )
    )
    
let ``return an ignored failure upon test being executed executed when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data (seq { 3..5 }),
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "D:\\dog.bark",
                    73
                )
            
            let results =     
                tests |> runAllTests
            
            results
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of results"
                ListShould.HaveAllValuesPassTestOf resultIsIgnored
            ]
        )
    )
    
let ``not run the teardown method passed to it when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data (seq{ 3..5 }),
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.TeardownWasCalled
            |> Should.BeFalse
            |> withMessage "teardown was called"
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
    
            let testName = "My test %i"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Ignore (
                    testName,
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data (seq{ 7..9 }),
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            tests
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test 7" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test 8" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test 9" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``return an ITest with everything when given no teardown, no name hints`` =
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
                testFeature.Ignore (
                    testName,
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data (seq{ 7..9 }),
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (7)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (8)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (9)" >> withMessage "TestName"
            ]
        )
    )
    
let ``not run setup method passed to it when given no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data (seq{ 7..9 }),
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.SetupWasCalled
            |> Should.BeFalse
            |> withMessage "Setup called"
        )
    )
    
let ``not run the test method passed to it when given no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data (seq{ 7..9 }),
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.TestWasCalled
            |> Should.BeFalse
            |> withMessage "Test was called"
        )
    )
    
let ``return an ignored failure upon test being executed executed when given no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data (seq{ 7..9 }),
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    "D:\\dog.bark",
                    73
                )
                
            let results =     
                tests |> runAllTests
            
            results
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of results"
                ListShould.HaveAllValuesPassTestOf resultIsIgnored
            ]
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
           
            let testName = "My test %i"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Ignore (
                    testName,
                    TestTags testTags,
                    Data (seq { 10..-1..8 }),
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
                List.head >> getTestName >> Should.BeEqualTo "My test 10" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test 9" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test 8" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``return an ITest with everything when given no setup, no name hints`` =
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
                testFeature.Ignore (
                    testName,
                    TestTags testTags,
                    Data (seq { 10..-1..8 }),
                    TestBody (fun _ -> TestSuccess),
                    emptyTeardown,
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (10)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (9)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (8)" >> withMessage "TestName"
            ]
        )
    )
    
let ``not run the test method passed to it when given no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Data (seq {10..-1..8}),
                    TestBody monitor.CallTestActionWithData,
                    Teardown monitor.CallTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.TestWasCalled
            |> Should.BeFalse
            |> withMessage "Test was called"
        )
    )
    
let ``return an ignored failure upon test being executed executed when given no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Data (seq { 10..-1..8 }),
                    TestBody monitor.CallTestActionWithData,
                    Teardown monitor.CallTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            let results =     
                tests |> runAllTests
            
            results
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of results"
                ListShould.HaveAllValuesPassTestOf resultIsIgnored
            ]
        )
    )
    
let ``not run the teardown method passed to it when given no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    "My test",
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Data (seq{ 10..-1..8 }),
                    TestBody monitor.CallTestActionWithData,
                    Teardown monitor.CallTeardown,
                    "D:\\dog.bark",
                    73
                )
                
            test
            |> silentlyRunAllTests
            
            monitor.TeardownWasCalled
            |> Should.BeFalse
            |> withMessage "teardown was called"
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
                
            let testName = "My test %i"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Ignore (
                    testName,
                    TestTags testTags,
                    Data (seq{ 100..102 }),
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
                List.head >> getTestName >> Should.BeEqualTo "My test 100" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test 101" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test 102" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``return an ITest with everything when given no setup, no teardown, no name hints`` =
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
                testFeature.Ignore (
                    testName,
                    TestTags testTags,
                    Data (seq{ 100..102 }),
                    TestBody (fun _ -> TestSuccess),
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (100)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (101)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (102)" >> withMessage "TestName"
            ]
        )
    )
    
// let ``not run the test method passed to it when given no setup or teardown`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     "My test",
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     TestBody monitor.CallTestActionWithSetup,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test
//             |> silentlyRunTest
//             
//             monitor.TestWasCalled
//             |> Should.BeFalse
//             |> withMessage "test was called"
//         )
//     )
//
// let ``return an ITest with everything when given no setup, teardown, or test body indicator`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let testTags = 
//                 [
//                    Only
//                    Category "My Category"
//                ]
//             let testName = "My test"
//             
//             let fileName = "dog.bark"
//             let path = "D:\\"
//             let fullPath = $"%s{path}%s{fileName}"
//             let lineNumber = 73
//             
//             let test =
//                 testFeature.Ignore (
//                     testName,
//                     TestTags testTags,
//                     (fun _ -> TestSuccess),
//                     fullPath,
//                     lineNumber
//                 )
//         
//             let getContainerName (test: ITest) =
//                 $"%s{test.ContainerPath}.%s{test.ContainerName}"
//                 
//             test
//             |> Should.PassAllOf [
//                 getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
//                 getTestName >> Should.BeEqualTo testName >> withMessage "TestName"
//                 getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
//                 getFilePath >> Should.BeEqualTo path >> withMessage "file path"
//                 getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
//                 getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
//             ]
//         )
//     )
//     
// let ``not run the test method passed to it when given no setup, teardown, or test body indicator`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     "My test",
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     monitor.CallTestActionWithSetup,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test
//             |> silentlyRunTest
//             
//             monitor.TestWasCalled
//             |> Should.BeFalse
//             |> withMessage "test was called"
//         )
//     )
//
// let ``return an ignored failure upon test being executed executed when given no setup or teardown`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     "My test",
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     TestBody monitor.CallTestActionWithSetup,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             let result =     
//                 test.GetExecutor ()
//                 |> executeFunction
//                 |> runIt
//             
//             match result with
//             | TestExecutionResult (TestFailure (TestIgnored _)) ->
//                 TestSuccess
//             | _ ->
//                 { ExpectedValue = "TestExecutionResult (TestFailure (TestIgnored _))"; ActualValue = result.ToString () }
//                 |> newFailure.With.TestValidationFailure
//                 |> TestFailure
//         )
//     )
//
// let ``return an ignored failure upon test being executed executed when given no setup, teardown, or test body indicator`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     "My test",
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     monitor.CallTestActionWithSetup,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             let result =     
//                 test.GetExecutor ()
//                 |> executeFunction
//                 |> runIt
//             
//             match result with
//             | TestExecutionResult (TestFailure (TestIgnored _)) ->
//                 TestSuccess
//             | _ ->
//                 { ExpectedValue = "TestExecutionResult (TestFailure (TestIgnored _))"; ActualValue = result.ToString () }
//                 |> newFailure.With.TestValidationFailure
//                 |> TestFailure
//         )
//     )
//
// // Setup, TestBody, Teardown
// let ``return an ITest with everything when given no tags`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let testName = "My test"
//             
//             let fileName = "dog.bark"
//             let path = "D:\\"
//             let fullPath = $"%s{path}%s{fileName}"
//             let lineNumber = 73
//             
//             let test =
//                 testFeature.Ignore (
//                     testName,
//                     Setup (fun _ -> Ok ()),
//                     TestBody (fun _ -> TestSuccess),
//                     emptyTeardown,
//                     fullPath,
//                     lineNumber
//                 )
//         
//             let getContainerName (test: ITest) =
//                 $"%s{test.ContainerPath}.%s{test.ContainerName}"
//                 
//             test
//             |> Should.PassAllOf [
//                 getTags >> Should.BeEqualTo [] >> withMessage "Tags"
//                 getTestName >> Should.BeEqualTo testName >> withMessage "TestName"
//                 getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
//                 getFilePath >> Should.BeEqualTo path >> withMessage "file path"
//                 getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
//                 getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
//             ]
//         )
//     )
//     
// let ``not run setup method passed to it when given no tags`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     "My test",
//                     Setup monitor.CallSetup,
//                     TestBody (fun _ -> TestSuccess),
//                     emptyTeardown,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test
//             |> silentlyRunTest
//             
//             monitor.SetupWasCalled
//             |> Should.BeFalse
//             |> withMessage "Setup called"
//         )
//     )
//     
// let ``not run the test method passed to it when given no tags`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     "My test",
//                     Setup monitor.CallSetup,
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test
//             |> silentlyRunTest
//             
//             monitor.TestWasCalled
//             |> Should.BeFalse
//             |> withMessage "Test was called"
//         )
//     )
//     
// let ``return an ignored failure upon test being executed executed when given no tags`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     "My test",
//                     Setup monitor.CallSetup,
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             let result =     
//                 test.GetExecutor ()
//                 |> executeFunction
//                 |> runIt
//             
//             match result with
//             | TestExecutionResult (TestFailure (TestIgnored _)) ->
//                 TestSuccess
//             | _ ->
//                 { ExpectedValue = "TestExecutionResult (TestFailure (TestIgnored _))"; ActualValue = result.ToString () }
//                 |> newFailure.With.TestValidationFailure
//                 |> TestFailure
//         )
//     )
//     
// let ``not run the teardown method passed to it when given no tags`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     "My test",
//                     Setup monitor.CallSetup,
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test
//             |> silentlyRunTest
//             
//             monitor.TeardownWasCalled
//             |> Should.BeFalse
//             |> withMessage "teardown was called"
//         )
//     )
//
// // Setup, TestBody
// let ``return an ITest with everything when given no tags, no teardown`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let testName = "My test"
//             
//             let fileName = "dog.bark"
//             let path = "D:\\"
//             let fullPath = $"%s{path}%s{fileName}"
//             let lineNumber = 73
//             
//             let test =
//                 testFeature.Ignore (
//                     testName,
//                     Setup (fun _ -> Ok ()),
//                     TestBody (fun _ -> TestSuccess),
//                     fullPath,
//                     lineNumber
//                 )
//         
//             let getContainerName (test: ITest) =
//                 $"%s{test.ContainerPath}.%s{test.ContainerName}"
//                 
//             test
//             |> Should.PassAllOf [
//                 getTags >> Should.BeEqualTo [] >> withMessage "Tags"
//                 getTestName >> Should.BeEqualTo testName >> withMessage "TestName"
//                 getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
//                 getFilePath >> Should.BeEqualTo path >> withMessage "file path"
//                 getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
//                 getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
//             ]
//         )
//     )
//     
// let ``not run setup method passed to it when given no tags, no teardown`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     "My test",
//                     Setup monitor.CallSetup,
//                     TestBody (fun _ -> TestSuccess),
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test
//             |> silentlyRunTest
//             
//             monitor.SetupWasCalled
//             |> Should.BeFalse
//             |> withMessage "Setup called"
//         )
//     )
//     
// let ``not run the test method passed to it when given no tags, no teardown`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     "My test",
//                     Setup monitor.CallSetup,
//                     TestBody monitor.CallTestActionWithSetup,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test
//             |> silentlyRunTest
//             
//             monitor.TestWasCalled
//             |> Should.BeFalse
//             |> withMessage "Test was called"
//         )
//     )
//
// let ``return an ignored failure upon test being executed executed when no teardown`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     "My test",
//                     Setup monitor.CallSetup,
//                     TestBody monitor.CallTestActionWithSetup,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             let result =     
//                 test.GetExecutor ()
//                 |> executeFunction
//                 |> runIt
//             
//             match result with
//             | TestExecutionResult (TestFailure (TestIgnored _)) ->
//                 TestSuccess
//             | _ ->
//                 { ExpectedValue = "TestExecutionResult (TestFailure (TestIgnored _))"; ActualValue = result.ToString () }
//                 |> newFailure.With.TestValidationFailure
//                 |> TestFailure
//         )
//     )
//
// // TestBody, Teardown
// let ``return an ITest with everything when given no tags, no setup`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let testName = "My test"
//             
//             let fileName = "dog.bark"
//             let path = "D:\\"
//             let fullPath = $"%s{path}%s{fileName}"
//             let lineNumber = 73
//             
//             let test =
//                 testFeature.Ignore (
//                     testName,
//                     TestBody (fun _ -> TestSuccess),
//                     emptyTeardown,
//                     fullPath,
//                     lineNumber
//                 )
//         
//             let getContainerName (test: ITest) =
//                 $"%s{test.ContainerPath}.%s{test.ContainerName}"
//                 
//             test
//             |> Should.PassAllOf [
//                 getTags >> Should.BeEqualTo [] >> withMessage "Tags"
//                 getTestName >> Should.BeEqualTo testName >> withMessage "TestName"
//                 getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
//                 getFilePath >> Should.BeEqualTo path >> withMessage "file path"
//                 getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
//                 getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
//             ]
//         )
//     )
//     
// let ``not run the test method passed to it when given no tags, no setup`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     "My test",
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test
//             |> silentlyRunTest
//             
//             monitor.TestWasCalled
//             |> Should.BeFalse
//             |> withMessage "test called"
//         )
//     )
//     
// let ``return an ignored failure upon test being executed executed when given no tags, no setup`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     "My test",
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             let result =     
//                 test.GetExecutor ()
//                 |> executeFunction
//                 |> runIt
//             
//             match result with
//             | TestExecutionResult (TestFailure (TestIgnored _)) ->
//                 TestSuccess
//             | _ ->
//                 { ExpectedValue = "TestExecutionResult (TestFailure (TestIgnored _))"; ActualValue = result.ToString () }
//                 |> newFailure.With.TestValidationFailure
//                 |> TestFailure
//         )
//     )
//     
// let ``not run the teardown method passed to it when given no tags, no setup`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     "My test",
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test
//             |> silentlyRunTest
//             
//             monitor.TeardownWasCalled
//             |> Should.BeFalse
//             |> withMessage "teardown was called"
//         )
//     )
//
// // TestBody
// let ``return an ITest with everything when given no tags, no setup, no teardown`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let testName = "My test"
//             
//             let fileName = "dog.bark"
//             let path = "D:\\"
//             let fullPath = $"%s{path}%s{fileName}"
//             let lineNumber = 73
//             
//             let test =
//                 testFeature.Ignore (
//                     testName,
//                     TestBody (fun _ -> TestSuccess),
//                     fullPath,
//                     lineNumber
//                 )
//         
//             let getContainerName (test: ITest) =
//                 $"%s{test.ContainerPath}.%s{test.ContainerName}"
//                 
//             test
//             |> Should.PassAllOf [
//                 getTags >> Should.BeEqualTo [] >> withMessage "Tags"
//                 getTestName >> Should.BeEqualTo testName >> withMessage "TestName"
//                 getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
//                 getFilePath >> Should.BeEqualTo path >> withMessage "file path"
//                 getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
//                 getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
//             ]
//         )
//     )
//     
// let ``not run the test method passed to it when given no tags, no setup, no teardown`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     "My test",
//                     TestBody monitor.CallTestActionWithSetup,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test
//             |> silentlyRunTest
//             
//             monitor.TestWasCalled
//             |> Should.BeFalse
//             |> withMessage "test was called"
//         )
//     )
//     
// let ``return an ignored failure upon test being executed executed when given no tags, no setup, no teardown`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     "My test",
//                     TestBody monitor.CallTestActionWithSetup,
//                     "D:\\dog.bark",
//                     73
//                 )
//             
//             let result =    
//                 test.GetExecutor ()
//                 |> executeFunction
//                 |> runIt
//             
//             match result with
//             | TestExecutionResult (TestFailure (TestIgnored _)) ->
//                 TestSuccess
//             | _ ->
//                 { ExpectedValue = "TestExecutionResult (TestFailure (TestIgnored _))"; ActualValue = result.ToString () }
//                 |> newFailure.With.TestValidationFailure
//                 |> TestFailure
//         )
//     )
//
// let ``return an ITest with everything when given no tags, no setup, no teardown, no test body indicator`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let testName = "My test"
//             
//             let fileName = "dog.bark"
//             let path = "D:\\"
//             let fullPath = $"%s{path}%s{fileName}"
//             let lineNumber = 73
//             
//             let test =
//                 testFeature.Ignore (
//                     testName,
//                     (fun _ -> TestSuccess),
//                     fullPath,
//                     lineNumber
//                 )
//         
//             let getContainerName (test: ITest) =
//                 $"%s{test.ContainerPath}.%s{test.ContainerName}"
//                 
//             test
//             |> Should.PassAllOf [
//                 getTags >> Should.BeEqualTo [] >> withMessage "Tags"
//                 getTestName >> Should.BeEqualTo testName >> withMessage "TestName"
//                 getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
//                 getFilePath >> Should.BeEqualTo path >> withMessage "file path"
//                 getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
//                 getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
//             ]
//         )
//     )
//     
// let ``not run the test method passed to it when given no tags, no setup, no teardown, no test body indicator`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     "My test",
//                     monitor.CallTestActionWithSetup,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test
//             |> silentlyRunTest
//             
//             monitor.TestWasCalled
//             |> Should.BeFalse
//             |> withMessage "test was called"
//         )
//     )
//     
// let ``return an ignored failure upon test being executed executed when given no tags, no setup, no teardown, no test body indicator`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     "My test",
//                     monitor.CallTestActionWithSetup,
//                     "D:\\dog.bark",
//                     73
//                 )
//             
//             let result =    
//                 test.GetExecutor ()
//                 |> executeFunction
//                 |> runIt
//             
//             match result with
//             | TestExecutionResult (TestFailure (TestIgnored _)) ->
//                 TestSuccess
//             | _ ->
//                 { ExpectedValue = "TestExecutionResult (TestFailure (TestIgnored _))"; ActualValue = result.ToString () }
//                 |> newFailure.With.TestValidationFailure
//                 |> TestFailure
//         )
//     )

let ``Test Cases`` = feature.GetTests ()