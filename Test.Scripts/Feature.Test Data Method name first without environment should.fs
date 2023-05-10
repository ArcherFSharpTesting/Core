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
                List.length >> Should.BeEqualTo 3 >> withMessage "Incorrect number of tests"
                
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
                List.length >> Should.BeEqualTo 3 >> withMessage "Incorrect number of tests"
                
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
                
            let getExecutor (test: ITest) =
                test.GetExecutor ()
            
            tests
            |> List.map (getExecutor >> executeFunction >> runIt)
            |> ignore
            
            monitor.NumberOfTimesSetupWasCalled
            |> Should.BeEqualTo 3
            |> withMessage "Setup was not called"
        )
    )
    
// let ``run the test method passed to it when everything is passed`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Test (
//                     "My test",
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     Setup monitor.CallSetup,
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test.GetExecutor ()
//             |> executeFunction
//             |> runIt
//             |> ignore
//             
//             monitor.TestWasCalled
//             |> Should.BeTrue
//             |> withMessage "test was not called"
//         )
//     )
//     
// let ``run the teardown method passed to it when everything is passed`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Test (
//                     "My test",
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     Setup monitor.CallSetup,
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test.GetExecutor ()
//             |> executeFunction
//             |> runIt
//             |> ignore
//             
//             monitor.TeardownWasCalled
//             |> Should.BeTrue
//             |> withMessage "teardown was not called"
//         )
//     )
//
// // Tags, Setup, TestBody!
// let ``return an ITest with everything when given no teardown`` =
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
//                 testFeature.Test (
//                     testName,
//                     TestTags testTags,
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
// let ``run setup method passed to it when given no teardown`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Test (
//                     "My test",
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     Setup monitor.CallSetup,
//                     TestBody (fun _ -> TestSuccess),
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test.GetExecutor ()
//             |> executeFunction
//             |> runIt
//             |> ignore
//             
//             monitor.SetupWasCalled
//             |> Should.BeTrue
//             |> withMessage "Setup was not called"
//         )
//     )
//     
// let ``run the test method passed to it when given no teardown`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Test (
//                     "My test",
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     Setup monitor.CallSetup,
//                     TestBody monitor.CallTestActionWithSetup,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test.GetExecutor ()
//             |> executeFunction
//             |> runIt
//             |> ignore
//             
//             monitor.TestWasCalled
//             |> Should.BeTrue
//             |> withMessage "test was not called"
//         )
//     )
//
// // Tags, TestBody, Teardown!
// let ``return an ITest with everything when given no setup`` =
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
//                 testFeature.Test (
//                     testName,
//                     TestTags testTags,
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
// let ``run the test method passed to it when given no setup`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Test (
//                     "My test",
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test.GetExecutor ()
//             |> executeFunction
//             |> runIt
//             |> ignore
//             
//             monitor.TestWasCalled
//             |> Should.BeTrue
//             |> withMessage "test was not called"
//         )
//     )
//     
// let ``run the teardown method passed to it when given no setup`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Test (
//                     "My test",
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test.GetExecutor ()
//             |> executeFunction
//             |> runIt
//             |> ignore
//             
//             monitor.TeardownWasCalled
//             |> Should.BeTrue
//             |> withMessage "teardown was not called"
//         )
//     )
//
// // Tags, TestBody!
// let ``return an ITest with everything when given no setup or teardown`` =
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
//                 testFeature.Test (
//                     testName,
//                     TestTags testTags,
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
//                 testFeature.Test (
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
// let ``run the test method passed to it when given no setup, teardown, or test body indicator`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Test (
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
//             test.GetExecutor ()
//             |> executeFunction
//             |> runIt
//             |> ignore
//             
//             monitor.TestWasCalled
//             |> Should.BeTrue
//             |> withMessage "test fas not called"
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
//                 testFeature.Test (
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
// let ``run setup method passed to it when given no tags`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Test (
//                     "My test",
//                     Setup monitor.CallSetup,
//                     TestBody (fun _ -> TestSuccess),
//                     emptyTeardown,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test.GetExecutor ()
//             |> executeFunction
//             |> runIt
//             |> ignore
//             
//             monitor.SetupWasCalled
//             |> Should.BeTrue
//             |> withMessage "Setup was not called"
//         )
//     )
//     
// let ``run the test method passed to it when given no tags`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Test (
//                     "My test",
//                     Setup monitor.CallSetup,
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test.GetExecutor ()
//             |> executeFunction
//             |> runIt
//             |> ignore
//             
//             monitor.TestWasCalled
//             |> Should.BeTrue
//             |> withMessage "test was not called"
//         )
//     )
//     
// let ``run the teardown method passed to it when given no tags`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Test (
//                     "My test",
//                     Setup monitor.CallSetup,
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test.GetExecutor ()
//             |> executeFunction
//             |> runIt
//             |> ignore
//             
//             monitor.TeardownWasCalled
//             |> Should.BeTrue
//             |> withMessage "teardown was not called"
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
//                 testFeature.Test (
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
// let ``run setup method passed to it when given no tags, no teardown`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Test (
//                     "My test",
//                     Setup monitor.CallSetup,
//                     TestBody (fun _ -> TestSuccess),
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test.GetExecutor ()
//             |> executeFunction
//             |> runIt
//             |> ignore
//             
//             monitor.SetupWasCalled
//             |> Should.BeTrue
//             |> withMessage "Setup was not called"
//         )
//     )
//     
// let ``run the test method passed to it when given no tags, no teardown`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Test (
//                     "My test",
//                     Setup monitor.CallSetup,
//                     TestBody monitor.CallTestActionWithSetup,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test.GetExecutor ()
//             |> executeFunction
//             |> runIt
//             |> ignore
//             
//             monitor.TestWasCalled
//             |> Should.BeTrue
//             |> withMessage "test was not called"
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
//                 testFeature.Test (
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
// let ``run the test method passed to it when given no tags, no setup`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Test (
//                     "My test",
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test.GetExecutor ()
//             |> executeFunction
//             |> runIt
//             |> ignore
//             
//             monitor.TestWasCalled
//             |> Should.BeTrue
//             |> withMessage "test was not called"
//         )
//     )
//     
// let ``run the teardown method passed to it when given no tags, no setup`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Test (
//                     "My test",
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test.GetExecutor ()
//             |> executeFunction
//             |> runIt
//             |> ignore
//             
//             monitor.TeardownWasCalled
//             |> Should.BeTrue
//             |> withMessage "teardown was not called"
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
//                 testFeature.Test (
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
// let ``run the test method passed to it when given no tags, no setup, no teardown`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Test (
//                     "My test",
//                     TestBody monitor.CallTestActionWithSetup,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test.GetExecutor ()
//             |> executeFunction
//             |> runIt
//             |> ignore
//             
//             monitor.TestWasCalled
//             |> Should.BeTrue
//             |> withMessage "test was not called"
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
//                 testFeature.Test (
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
// let ``run the test method passed to it when given no tags, no setup, no teardown, no test body indicator`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Test (
//                     "My test",
//                     monitor.CallTestActionWithSetup,
//                     "D:\\dog.bark",
//                     73
//                 )
//                 
//             test.GetExecutor ()
//             |> executeFunction
//             |> runIt
//             |> ignore
//             
//             monitor.TestWasCalled
//             |> Should.BeTrue
//             |> withMessage "test was not called"
//         )
//     )

let ``Test Cases`` = feature.GetTests ()