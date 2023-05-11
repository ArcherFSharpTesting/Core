module Archer.Arrows.Tests.Feature.``Test Data Method without environment should``

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
            let testName = "My test with value %i"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data [1; 2; 3],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    emptyTeardown,
                    testName,
                    fullPath,
                    lineNumber
                )
                
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                List.length >> Should.BeEqualTo 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test with value 1" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test with value 2" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test with value 3" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )

let ``return an ITest with everything when everything is passed no name hint`` =
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
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data [1; 2; 3],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    emptyTeardown,
                    testName,
                    fullPath,
                    lineNumber
                )
                
            test
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (1)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (2)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (3)" >> withMessage "TestName"
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
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data [1; 2; 3],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    emptyTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
               
            let test = tests |> List.head
            
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.SetupWasCalled
            |> Should.BeTrue
            |> withMessage "Setup was not called"
        )
    )
    
let ``run the test method passed to it when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<string, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data ["1"; "2"; "3"],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
             
            let test = tests |> List.head
            
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeTrue
            |> withMessage "test was not called"
        )
    )
    
let ``run the test method passed using the setup when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<string, unit, string> (Ok "Hello")
            let tests =
                testFeature.Test (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data ["1"; "2"; "3"],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
             
            tests
            |> List.map (getTestExecutor >> executeFunction >> runIt)
            |> ignore
            
            monitor.TestInputSetupWas
            |> Should.BeEqualTo ["Hello";"Hello";"Hello"]
            |> withMessage "test was not called"
        )
    )
    
let ``run the test method passed using the data when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<string, unit, string> (Ok "Hello")
            let tests =
                testFeature.Test (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data ["1"; "2"; "3"],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            tests
            |> List.map (getTestExecutor >> executeFunction >> runIt)
            |> ignore
            
            monitor.TestDataWas
            |> Should.BeEqualTo ["1"; "2"; "3"]
            |> withMessage "test was not called"
        )
    )
    
let ``have unique names even when data is not unique`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<string, unit, string> (Ok "Hello")
            let tests =
                testFeature.Test (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data ["1"; "1"; "1"],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let names =
                tests
                |> List.map getTestName
                |> Set.ofList
                
            names.Count
            |> Should.BeEqualTo 3
            |> withMessage "non - unique names"
        )
    )
    
let ``run the teardown method passed to it when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data [1; 2; 3],
                    TestBodyThreeParameters monitor.CallTestActionWithDataSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let test = tests |> List.head
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TeardownWasCalled
            |> Should.BeTrue
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
            let testName = "My test with value %s"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data ["Wolf"; "Shepard"; "Mutt"],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                List.length >> Should.BeEqualTo 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test with value Wolf" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test with value Shepard" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test with value Mutt" >> withMessage "TestName"
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
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data ["Wolf"; "Shepard"; "Mutt"],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            test
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (\"Wolf\")" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (\"Shepard\")" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (\"Mutt\")" >> withMessage "TestName"
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
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data [4; 3; 2],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let test = tests |> List.head
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.SetupWasCalled
            |> Should.BeTrue
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
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data ["Sun"; "Moon"; "Mars"],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let test = tests |> List.head
            
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeTrue
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
            let testName = "My test %c"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    TestTags testTags,
                    Data ['a'; 'b'; 'c'],
                    TestBody (fun _ -> TestSuccess),
                    emptyTeardown,
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                List.length >> Should.BeEqualTo 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test a" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test b" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test c" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``return an ITest with everything when given no setup no name hints`` =
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
                    TestTags testTags,
                    Data ['a'; 'b'; 'c'],
                    TestBody (fun _ -> TestSuccess),
                    emptyTeardown,
                    testName,
                    fullPath,
                    lineNumber
                )
        
            test
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test ('a')" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test ('b')" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test ('c')" >> withMessage "TestName"
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
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Data [1; 2; 3],
                    TestBody monitor.CallTestActionWithData,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let test = tests |> List.head
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeTrue
            |> withMessage "test was not called"
        )
    )
    
let ``run the teardown method passed to it when given no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Data [1; 2; 3],
                    TestBody monitor.CallTestActionWithData,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let test = tests |> List.head
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TeardownWasCalled
            |> Should.BeTrue
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
            let testName = "My test says %s"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    TestTags testTags,
                    Data ["hello"; "goodbye"],
                    TestBody (fun _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                List.length >> Should.BeEqualTo 2
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test says hello" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test says goodbye" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``return an ITest with everything when given no setup or teardown no name hint`` =
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
                    TestTags testTags,
                    Data ["hello"; "goodbye"],
                    TestBody (fun _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            test
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (\"hello\")" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (\"goodbye\")" >> withMessage "TestName"
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
            let testName = "My test is %A"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    TestTags testTags,
                    Data [false; true],
                    (fun _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                List.length >> Should.BeEqualTo 2 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test is false" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test is true" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``return an ITest with everything when given no setup, teardown, or test body indicator no name hints`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testTags = 
                [
                   Only
                   Category "My Category"
               ]
            let testName = "A test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    TestTags testTags,
                    Data [false; true],
                    (fun _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                List.length >> Should.BeEqualTo 2 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "A test (false)" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "A test (true)" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``run the test method passed to it when given no setup, teardown, or test body indicator`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<bool, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Data [true; false; false],
                    monitor.CallTestActionWithData,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let test = tests |> List.head
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeTrue
            |> withMessage "test fas not called"
        )
    )

// Setup, TestBody, Teardown
let ``return an ITest with everything when given no tags`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test number %i"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    Setup (fun _ -> Ok ()),
                    Data [2; 4; 6],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    emptyTeardown,
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                List.length >> Should.BeEqualTo 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test number 2" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test number 4" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test number 6" >> withMessage "TestName"
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
            let testName = "A Test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    Setup (fun _ -> Ok ()),
                    Data [2; 4; 6],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    emptyTeardown,
                    testName,
                    fullPath,
                    lineNumber
                )
        
            test
            |> Should.PassAllOf [
                List.length >> Should.BeEqualTo 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTestName >> Should.BeEqualTo "A Test (2)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "A Test (4)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "A Test (6)" >> withMessage "TestName"
            ]
        )
    )
    
let ``run setup method passed to it when given no tags`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<string, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    Setup monitor.CallSetup,
                    Data ["Yes"; "No"; "Maybe"],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    emptyTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let test = tests |> List.head
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.SetupWasCalled
            |> Should.BeTrue
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
                    Setup monitor.CallSetup,
                    Data [7; 8],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let test = tests |> List.head
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeTrue
            |> withMessage "test was not called"
        )
     )
    
let ``run the teardown method passed to it when given no tags`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    Setup monitor.CallSetup,
                    Data [(); ()],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let test = tests |> List.head
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TeardownWasCalled
            |> Should.BeTrue
            |> withMessage "teardown was not called"
        )
    )

// Setup, TestBody
let ``return an ITest with everything when given no tags, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test at %f"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    Setup (fun _ -> Ok ()),
                    Data [3.3; 2.25],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                List.length >> Should.BeEqualTo 2 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test at 3.300000" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "My test at 2.250000" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``return an ITest with everything when given no tags, no teardown no name hints`` =
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
                    Setup (fun _ -> Ok ()),
                    Data [3.3; 2.25],
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            test
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (3.3)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (2.25)" >> withMessage "TestName"
            ]
        )
    )
    
let ``run setup method passed to it when given no tags, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit, unit> (Ok ())
            let test =
                testFeature.Test (
                    Setup monitor.CallSetup,
                    TestBody (fun _ -> TestSuccess),
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.SetupWasCalled
            |> Should.BeTrue
            |> withMessage "Setup was not called"
        )
    )
    
let ``run the test method passed to it when given no tags, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<float, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    Setup monitor.CallSetup,
                    Data [1.2; 2.4; 3.7],
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let test = tests |> List.head
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeTrue
            |> withMessage "test was not called"
        )
    )

// TestBody, Teardown
let ``return an ITest with everything when given no tags, no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test letter %c"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    Data ['z'],
                    TestBody (fun _ -> TestSuccess),
                    emptyTeardown,
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                List.length >> Should.BeEqualTo 1 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "My test letter z" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``return an ITest with everything when given no tags, no setup, no name hint`` =
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
                    Data ['z'],
                    TestBody (fun _ -> TestSuccess),
                    emptyTeardown,
                    testName,
                    fullPath,
                    lineNumber
                )
        
            test
            |> List.head
            |> getTestName
            |> Should.BeEqualTo "My test ('z')"
            |> withMessage "TestName"
        )
    )
    
let ``run the test method passed to it when given no tags, no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    Data [6],
                    TestBody monitor.CallTestActionWithData,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let test = tests |> List.head
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeTrue
            |> withMessage "test was not called"
        )
    )
    
let ``run the teardown method passed to it when given no tags, no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<bool, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    Data [false],
                    TestBody monitor.CallTestActionWithData,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let test = tests |> List.head
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TeardownWasCalled
            |> Should.BeTrue
            |> withMessage "teardown was not called"
        )
    )

// TestBody
let ``return an ITest with everything when given no tags, no setup, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "%c: My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    Data ['Y'; 'Z'],
                    TestBody (fun _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                List.length >> Should.BeEqualTo 2 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "Y: My test" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "Z: My test" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``return an ITest with everything when given no tags, no setup, no teardown no name hints`` =
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
                    Data ['Y'; 'Z'],
                    TestBody (fun _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            test
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test ('Y')" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test ('Z')" >> withMessage "TestName"
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
                    Data [4],
                    TestBody monitor.CallTestActionWithData,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let test = tests |> List.head
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeTrue
            |> withMessage "test was not called"
        )
    )

let ``return an ITest with everything when given no tags, no setup, no teardown, no test body indicator`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "%i: My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Test (
                    Data [6; 9; 1],
                    (fun _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                List.length >> Should.BeEqualTo 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "6: My test" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "9: My test" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "1: My test" >> withMessage "TestName"
                List.last >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.last >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.last >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.last >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )

let ``return an ITest with everything when given no tags, no setup, no teardown, no test body indicator no name hints`` =
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
                    Data [6; 9; 1],
                    (fun _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "My test (6)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "My test (9)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "My test (1)" >> withMessage "TestName"
            ]
        )
    )
    
let ``run the test method passed to it when given no tags, no setup, no teardown, no test body indicator`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int option, unit, unit> (Ok ())
            let tests =
                testFeature.Test (
                    Data [Some 1; None; Some 4],
                    monitor.CallTestActionWithData,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let test = tests |> List.head
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeTrue
            |> withMessage "test was not called"
        )
    )

let ``Test Cases`` = feature.GetTests ()