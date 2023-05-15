module Archer.Arrows.Tests.Feature.``Ignore Data Method without environment should``

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
            let testName = "Test with %i"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Ignore (
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data (seq { 0..-1..-2 }),
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    emptyTeardown,
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            tests
            |> Should.PassAllOf [
                List.length >> Should.BeEqualTo 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "Test with 0" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.skip 1 >> List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "Test with -1" >> withMessage "TestName"
                List.skip 1 >> List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.skip 1 >> List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.skip 1 >> List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.skip 1 >> List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
                
                List.last >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.last >> getTestName >> Should.BeEqualTo "Test with -2" >> withMessage "TestName"
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
            let testName = "Test with"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Ignore (
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data (seq { 0..-1..-2 }),
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    emptyTeardown,
                    testName,
                    fullPath,
                    lineNumber
                )
        
            tests
            |> Should.PassAllOf [
                List.head >> getTestName >> Should.BeEqualTo "Test with (0)" >> withMessage "TestName"
                List.skip 1 >> List.head >> getTestName >> Should.BeEqualTo "Test with (-1)" >> withMessage "TestName"
                List.last >> getTestName >> Should.BeEqualTo "Test with (-2)" >> withMessage "TestName"
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
                    Data (seq { 0..-1..-2 }),
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
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
                    Data (seq { 0..-1..-2 }),
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
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
                    Data (seq { 0..-1..-2 }),
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
            
            let results =
                tests
                |> runAllTests
            
            let isIgnoredResult =
                <@fun result ->
                    match result with
                    | TestExecutionResult (TestFailure (TestIgnored _)) -> true
                    | _ -> false
                @>
                    
            results
            |> Should.PassAllOf [
                List.length >> Should.BeEqualTo 3 >> withMessage "Incorrect length"
                ListShould.FindAllValuesWith isIgnoredResult
            ]
        )
    )
    
let ``not run the teardown method passed to it when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<int, unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    Data (seq { 0..-1..-2 }),
                    TestBodyTwoParameters monitor.CallTestActionWithDataSetup,
                    Teardown monitor.CallTeardown,
                    "My test",
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
            let testName = "%i) Test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let tests =
                testFeature.Ignore (
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    Data (seq{ 1..3 }),
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            tests
            |> Should.PassAllOf [
                List.length >> Should.BeEqualTo 3 >> withMessage "Incorrect number of tests"
                
                List.head >> getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                List.head >> getTestName >> Should.BeEqualTo "1) Test" >> withMessage "TestName"
                List.head >> getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                List.head >> getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                List.head >> getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                List.head >> getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
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
                    Data (seq{ 1..3 }),
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
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
    
// let ``not run the test method passed to it when given no teardown`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     Setup monitor.CallSetup,
//                     TestBody monitor.CallTestActionWithSetup,
//                     "My test",
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
// let ``return an ignored failure upon test being executed executed when given no teardown`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     Setup monitor.CallSetup,
//                     TestBody monitor.CallTestActionWithSetup,
//                     "My test",
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
//                 { new IVerificationInfo with
//                     member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
//                     member _.Actual = result.ToString ()
//                 }
//                 |> newFailure.With.TestValidationFailure
//                 |> TestFailure
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
//                 testFeature.Ignore (
//                     TestTags testTags,
//                     TestBody (fun _ -> TestSuccess),
//                     emptyTeardown,
//                     testName,
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
// let ``not run the test method passed to it when given no setup`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "My test",
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
// let ``return an ignored failure upon test being executed executed when given no setup`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "My test",
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
//                 { new IVerificationInfo with
//                     member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
//                     member _.Actual = result.ToString ()
//                 }
//                 |> newFailure.With.TestValidationFailure
//                 |> TestFailure
//         )
//     )
//     
// let ``not run the teardown method passed to it when given no setup`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "My test",
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
//                 testFeature.Ignore (
//                     TestTags testTags,
//                     TestBody (fun _ -> TestSuccess),
//                     testName,
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
// let ``not run the test method passed to it when given no setup or teardown`` =
//     feature.Test (
//         Setup setupFeatureUnderTest,
//         TestBody (fun (testFeature: IFeature<unit>) ->
//             let monitor = Monitor<unit, unit, unit> (Ok ())
//             let test =
//                 testFeature.Ignore (
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     TestBody monitor.CallTestActionWithSetup,
//                     "My test",
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
//                     TestTags testTags,
//                     (fun _ -> TestSuccess),
//                     testName,
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
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     monitor.CallTestActionWithSetup,
//                     "My test",
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
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     TestBody monitor.CallTestActionWithSetup,
//                     "My test",
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
//                 { new IVerificationInfo with
//                     member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
//                     member _.Actual = result.ToString ()
//                 }
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
//                     TestTags [
//                                 Only
//                                 Category "My Category"
//                             ],
//                     monitor.CallTestActionWithSetup,
//                     "My test",
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
//                 { new IVerificationInfo with
//                     member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
//                     member _.Actual = result.ToString ()
//                 }
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
//                     Setup (fun _ -> Ok ()),
//                     TestBody (fun _ -> TestSuccess),
//                     emptyTeardown,
//                     testName,
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
//                     Setup monitor.CallSetup,
//                     TestBody (fun _ -> TestSuccess),
//                     emptyTeardown,
//                     "My test",
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
//                     Setup monitor.CallSetup,
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "My test",
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
//                     Setup monitor.CallSetup,
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "My test",
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
//                 { new IVerificationInfo with
//                     member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
//                     member _.Actual = result.ToString ()
//                 }
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
//                     Setup monitor.CallSetup,
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "My test",
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
//                     Setup (fun _ -> Ok ()),
//                     TestBody (fun _ -> TestSuccess),
//                     testName,
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
//                     Setup monitor.CallSetup,
//                     TestBody (fun _ -> TestSuccess),
//                     "My test",
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
//                     Setup monitor.CallSetup,
//                     TestBody monitor.CallTestActionWithSetup,
//                     "My test",
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
//                     Setup monitor.CallSetup,
//                     TestBody monitor.CallTestActionWithSetup,
//                     "My test",
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
//                 { new IVerificationInfo with
//                     member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
//                     member _.Actual = result.ToString ()
//                 }
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
//                     TestBody (fun _ -> TestSuccess),
//                     emptyTeardown,
//                     testName,
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
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "My test",
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
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "My test",
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
//                 { new IVerificationInfo with
//                     member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
//                     member _.Actual = result.ToString ()
//                 }
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
//                     TestBody monitor.CallTestActionWithSetup,
//                     Teardown monitor.CallTeardown,
//                     "My test",
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
//                     TestBody (fun _ -> TestSuccess),
//                     testName,
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
//                     TestBody monitor.CallTestActionWithSetup,
//                     "My test",
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
//                     TestBody monitor.CallTestActionWithSetup,
//                     "My test",
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
//                 { new IVerificationInfo with
//                     member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
//                     member _.Actual = result.ToString ()
//                 }
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
//                     (fun _ -> TestSuccess),
//                     testName,
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
//                     monitor.CallTestActionWithSetup,
//                     "My test",
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
//                     monitor.CallTestActionWithSetup,
//                     "My test",
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
//                 { new IVerificationInfo with
//                     member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
//                     member _.Actual = result.ToString ()
//                 }
//                 |> newFailure.With.TestValidationFailure
//                 |> TestFailure
//         )
//     )

let ``Test Cases`` = feature.GetTests ()