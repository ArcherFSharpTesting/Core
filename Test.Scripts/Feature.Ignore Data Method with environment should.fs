module Archer.Arrows.Tests.Feature.``Ignore Data Method with environment should``

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
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data (seq{ -30..15..0 }),
                    TestBodyThreeParameters (fun _ _ _ -> TestSuccess),
                    emptyTeardown,
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            tests
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test -30" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test -15" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test 0" >> withMessage "TestName"
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
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data (seq{ -30..15..0 }),
                    TestBodyThreeParameters (fun _ _ _ -> TestSuccess),
                    emptyTeardown,
                    testName,
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (-30)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (-15)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (0)" >> withMessage "TestName"
            ]
        )
    )
    
let ``not run setup method passed to it when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data (seq{ -30..15..0 }),
                    TestBodyThreeParameters (fun _ _ _ -> TestSuccess),
                    emptyTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            tests
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
            let tests =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data (seq{ -30..15..0 }),
                    TestBodyThreeParameters monitor.CallTestActionWithDataSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
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
    
let ``return an ignored failure upon test being executed executed when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data (seq{ -30..15..0 }),
                    TestBodyThreeParameters monitor.CallTestActionWithDataSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
            
            let results =
                tests |> runAllTests
            
            results
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
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
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data (seq{ -30..15..0 }),
                    TestBodyThreeParameters monitor.CallTestActionWithDataSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
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
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data (seq{ 12..12..36 }),
                    TestBodyThreeParameters (fun _ _ _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            tests
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test 12" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test 24" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test 36" >> withMessage "TestName"
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
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data (seq{ 12..12..36 }),
                    TestBodyThreeParameters (fun _ _ _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (12)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (24)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (36)" >> withMessage "TestName"
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
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data (seq{ 12..12..36 }),
                    TestBodyThreeParameters (fun _ _ _ -> TestSuccess),
                    "My test",
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
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data (seq{ 12..12..36 }),
                    TestBodyThreeParameters monitor.CallTestActionWithDataSetupEnvironment,
                    "My test",
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
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data (seq{ 12..12..36 }),
                    TestBodyThreeParameters monitor.CallTestActionWithDataSetupEnvironment,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let results =
                tests |> runAllTests
            
            results
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
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
            
            let test =
                testFeature.Ignore (
                    TestTags testTags,
                    Data (seq{ 44..11..66 }),
                    TestBodyTwoParameters (fun _ (_: unit) -> TestSuccess),
                    emptyTeardown,
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test 44" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test 55" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test 66" >> withMessage "TestName"
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
            
            let test =
                testFeature.Ignore (
                    TestTags testTags,
                    Data (seq{ 44..11..66 }),
                    TestBodyTwoParameters (fun _ (_: unit) -> TestSuccess),
                    emptyTeardown,
                    testName,
                    fullPath,
                    lineNumber
                )
        
            test
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (44)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (55)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (66)" >> withMessage "TestName"
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
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Data (seq{ 44..11..66 }),
                    TestBodyTwoParameters monitor.CallTestActionWithDataEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
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
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Data (seq{ 44..11..66 }),
                    TestBodyTwoParameters monitor.CallTestActionWithDataEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
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
            let tests =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Data (seq{ 44..11..66 }),
                    TestBodyTwoParameters monitor.CallTestActionWithDataEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
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
                    TestTags testTags,
                    Data (seq { 7..7..21 }),
                    TestBodyTwoParameters (fun _ (_: unit) -> TestSuccess),
                    testName,
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
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test 14" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test 21" >> withMessage "TestName"
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
                    TestTags testTags,
                    Data (seq { 7..7..21 }),
                    TestBodyTwoParameters (fun _ (_: unit) -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (7)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (14)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (21)" >> withMessage "TestName"
            ]
        )
    )
    
let ``not run the test method passed to it when given no setup or teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Data (seq{ 7..7..21 }),
                    TestBodyTwoParameters monitor.CallTestActionWithDataEnvironment,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.TestWasCalled
            |> Should.BeFalse
            |> withMessage "test was called"
        )
    )

let ``return an ignored failure upon test being executed executed when given no setup or teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Data (seq{ 7..7..21 }),
                    TestBodyTwoParameters monitor.CallTestActionWithDataEnvironment,
                    "My test",
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

let ``return an ITest with everything when given no setup, or teardown, or test body indicator`` =
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
                    TestTags testTags,
                    Data (seq{ 0..55..110 }),
                    (fun _ (_: unit) -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            tests
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test 0" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test 55" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test 110" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )

let ``return an ITest with everything when given no setup, no teardown, no test body indicator, no name hints`` =
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
                    TestTags testTags,
                    Data (seq{ 0..55..110 }),
                    (fun _ (_: unit) -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (0)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (55)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (110)" >> withMessage "TestName"
            ]
        )
    )
    
let ``not run the test method passed to it when given no setup, or teardown, or test body indicator`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Data (seq{ 0..55..110 }),
                    monitor.CallTestActionWithDataEnvironment,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.TestWasCalled
            |> Should.BeFalse
            |> withMessage "test was called"
        )
    )

let ``return an ignored failure upon test being executed executed when given no setup, or teardown, or test body indicator`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Data (seq{ 0..55..110 }),
                    monitor.CallTestActionWithDataEnvironment,
                    "My test",
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
                testFeature.Ignore (
                    Setup (fun _ -> Ok ()),
                    Data (seq { 9..9..27 }),
                    TestBodyThreeParameters (fun _ _ _ -> TestSuccess),
                    emptyTeardown,
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            tests
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test 9" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test 18" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test 27" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``return an ITest with everything when given no tags, no name hints`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Ignore (
                    Setup (fun _ -> Ok ()),
                    Data (seq { 9..9..27 }),
                    TestBodyThreeParameters (fun _ _ _ -> TestSuccess),
                    emptyTeardown,
                    testName,
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (9)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (18)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (27)" >> withMessage "TestName"
            ]
        )
    )
    
let ``not run setup method passed to it when given no tags`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    Setup monitor.CallSetup,
                    Data (seq{ 9..9..27 }),
                    TestBodyThreeParameters (fun _ _ _ -> TestSuccess),
                    emptyTeardown,
                    "My test",
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
    
let ``not run the test method passed to it when given no tags`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    Setup monitor.CallSetup,
                    Data (seq{ 9..9..27 }),
                    TestBodyThreeParameters monitor.CallTestActionWithDataSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
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
    
let ``return an ignored failure upon test being executed executed when given no tags`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    Setup monitor.CallSetup,
                    Data (seq{ 9..9..27 }),
                    TestBodyThreeParameters monitor.CallTestActionWithDataSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
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
    
let ``not run the teardown method passed to it when given no tags`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    Setup monitor.CallSetup,
                    Data (seq{ 9..9..27 }),
                    TestBodyThreeParameters monitor.CallTestActionWithDataSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
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

// Setup, TestBody
let ``return an ITest with everything when given no tags, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test %i"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Ignore (
                    Setup (fun _ -> Ok ()),
                    Data (seq{ 9..-9..-9 }),
                    TestBodyThreeParameters (fun _ _ _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            tests
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test 9" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test 0" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test -9" >> withMessage "TestName"
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
                testFeature.Ignore (
                    Setup (fun _ -> Ok ()),
                    Data (seq{ 9..-9..-9 }),
                    TestBodyThreeParameters (fun _ _ _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (9)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (0)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (-9)" >> withMessage "TestName"
            ]
        )
    )
    
let ``not run setup method passed to it when given no tags, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    Setup monitor.CallSetup,
                    Data (seq{ 9..-9..-9 }),
                    TestBodyThreeParameters (fun _ _ _ -> TestSuccess),
                    "My test",
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
    
let ``not run the test method passed to it when given no tags, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    Setup monitor.CallSetup,
                    Data (seq { 9..-9..-9 }),
                    TestBodyThreeParameters monitor.CallTestActionWithDataSetupEnvironment,
                    "My test",
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

let ``return an ignored failure upon test being executed executed when no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    Setup monitor.CallSetup,
                    Data (seq { 9..-9..-9 }),
                    TestBodyThreeParameters monitor.CallTestActionWithDataSetupEnvironment,
                    "My test",
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

// TestBody, Teardown
let ``return an ITest with everything when given no tags, no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test %i"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Ignore (
                    Data [| 0; 0; 0 |],
                    TestBodyTwoParameters (fun _ () -> TestSuccess),
                    emptyTeardown,
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            tests
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test 0" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test 0^1" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test 0^2" >> withMessage "TestName"
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
            
            let tests =
                testFeature.Ignore (
                    Data [| 0; 0; 0 |],
                    TestBodyTwoParameters (fun _ () -> TestSuccess),
                    emptyTeardown,
                    testName,
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (0)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (0)^1" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (0)^2" >> withMessage "TestName"
            ]
        )
    )
    
let ``not run the test method passed to it when given no tags, no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    Data [| 0; 0; 0 |],
                    TestBodyTwoParameters monitor.CallTestActionWithDataEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.TestWasCalled
            |> Should.BeFalse
            |> withMessage "test called"
        )
    )
    
let ``return an ignored failure upon test being executed executed when given no tags, no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    Data [| 0; 0; 0; |],
                    TestBodyTwoParameters monitor.CallTestActionWithDataEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
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
    
let ``not run the teardown method passed to it when given no tags, no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    Data [| 0; 0; 0 |],
                    TestBodyTwoParameters monitor.CallTestActionWithDataEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
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

// TestBody
let ``return an ITest with everything when given no tags, no setup, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test %i"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Ignore (
                    Data (seq{ -5..-15..-35 }),
                    TestBodyTwoParameters (fun _ () -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            tests
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test -5" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test -20" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test -35" >> withMessage "TestName"
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
            
            let tests =
                testFeature.Ignore (
                    Data (seq{ -5..-15..-35 }),
                    TestBodyTwoParameters (fun _ () -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (-5)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (-20)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (-35)" >> withMessage "TestName"
            ]
        )
    )
    
let ``not run the test method passed to it when given no tags, no setup, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    Data (seq{ -5..-15..-35  }),
                    TestBodyTwoParameters monitor.CallTestActionWithDataEnvironment,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.TestWasCalled
            |> Should.BeFalse
            |> withMessage "test was called"
        )
    )
    
let ``return an ignored failure upon test being executed executed when given no tags, no setup, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    Data (seq { -5..-15..-35 }),
                    TestBodyTwoParameters monitor.CallTestActionWithDataEnvironment,
                    "My test",
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

let ``return an ITest with everything when given no tags, no setup, no teardown, no test body indicator`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test %c"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Ignore (
                    Data (seq{ 'a'..'c' }),
                    (fun _ () -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            tests
            |> Should.PassAllOf [
                ListShould.HaveLengthOf 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test a" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test b" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test c" >> withMessage "TestName"
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
            
            let tests =
                testFeature.Ignore (
                    Data (seq{ 'a'..'c' }),
                    (fun _ () -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test ('a')" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test ('b')" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test ('c')" >> withMessage "TestName"
            ]
        )
    )
    
let ``not run the test method passed to it when given no tags, no setup, no teardown, no test body indicator`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<char, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    Data (seq{ 'a'..'c' }),
                    monitor.CallTestActionWithDataEnvironment,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> silentlyRunAllTests
            
            monitor.TestWasCalled
            |> Should.BeFalse
            |> withMessage "test was called"
        )
    )
    
let ``return an ignored failure upon test being executed executed when given no tags, no setup, no teardown, no test body indicator`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<char, unit, unit> (Ok ())
            let tests =
                testFeature.Ignore (
                    Data (seq{ 'a'..'c' }),
                    monitor.CallTestActionWithDataEnvironment,
                    "My test",
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

let ``Test Cases`` = feature.GetTests ()