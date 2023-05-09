module Archer.Arrows.Tests.Feature.``Ignore Method with environment should``

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
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Ignore (
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
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
                getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                getTestName >> Should.BeEqualTo testName >> withMessage "TestName"
                getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``not run setup method passed to it when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    emptyTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.SetupWasCalled
            |> Should.BeFalse
            |> withMessage "Setup was called"
        )
    )
    
let ``not run the test method passed to it when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeFalse
            |> withMessage "Test was called"
        )
    )
    
let ``return an ignored failure upon test being executed executed when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
            
            let result =     
                test.GetExecutor ()
                |> executeFunction
                |> runIt
            
            match result with
            | TestExecutionResult (TestFailure (TestIgnored _)) ->
                TestSuccess
            | _ ->
                { new IVerificationInfo with
                    member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
                    member _.Actual = result.ToString ()
                }
                |> newFailure.With.TestValidationFailure
                |> TestFailure
        )
    )
    
let ``not run the teardown method passed to it when everything is passed`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
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
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Ignore (
                    TestTags testTags,
                    Setup (fun _ -> Ok ()),
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                getTestName >> Should.BeEqualTo testName >> withMessage "TestName"
                getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``not run setup method passed to it when given no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.SetupWasCalled
            |> Should.BeFalse
            |> withMessage "Setup called"
        )
    )
    
let ``not run the test method passed to it when given no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeFalse
            |> withMessage "Test was called"
        )
    )
    
let ``return an ignored failure upon test being executed executed when given no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    Setup monitor.CallSetup,
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let result =     
                test.GetExecutor ()
                |> executeFunction
                |> runIt
            
            match result with
            | TestExecutionResult (TestFailure (TestIgnored _)) ->
                TestSuccess
            | _ ->
                { new IVerificationInfo with
                    member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
                    member _.Actual = result.ToString ()
                }
                |> newFailure.With.TestValidationFailure
                |> TestFailure
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
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Ignore (
                    TestTags testTags,
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
                getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                getTestName >> Should.BeEqualTo testName >> withMessage "TestName"
                getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``not run the test method passed to it when given no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeFalse
            |> withMessage "Test was called"
        )
    )
    
let ``return an ignored failure upon test being executed executed when given no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let result =     
                test.GetExecutor ()
                |> executeFunction
                |> runIt
            
            match result with
            | TestExecutionResult (TestFailure (TestIgnored _)) ->
                TestSuccess
            | _ ->
                { new IVerificationInfo with
                    member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
                    member _.Actual = result.ToString ()
                }
                |> newFailure.With.TestValidationFailure
                |> TestFailure
        )
    )
    
let ``not run the teardown method passed to it when given no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
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
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Ignore (
                    TestTags testTags,
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                getTestName >> Should.BeEqualTo testName >> withMessage "TestName"
                getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``not run the test method passed to it when given no setup or teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeFalse
            |> withMessage "test was called"
        )
    )

let ``return an ignored failure upon test being executed executed when given no setup or teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let result =     
                test.GetExecutor ()
                |> executeFunction
                |> runIt
            
            match result with
            | TestExecutionResult (TestFailure (TestIgnored _)) ->
                TestSuccess
            | _ ->
                { new IVerificationInfo with
                    member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
                    member _.Actual = result.ToString ()
                }
                |> newFailure.With.TestValidationFailure
                |> TestFailure
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
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Ignore (
                    TestTags testTags,
                    (fun _ _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                getTags >> Should.BeEqualTo testTags >> withMessage "Tags"
                getTestName >> Should.BeEqualTo testName >> withMessage "TestName"
                getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``not run the test method passed to it when given no setup, or teardown, or test body indicator`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    monitor.CallTestActionWithSetupEnvironment,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeFalse
            |> withMessage "test was called"
        )
    )

let ``return an ignored failure upon test being executed executed when given no setup, or teardown, or test body indicator`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestTags [
                                Only
                                Category "My Category"
                            ],
                    monitor.CallTestActionWithSetupEnvironment,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let result =     
                test.GetExecutor ()
                |> executeFunction
                |> runIt
            
            match result with
            | TestExecutionResult (TestFailure (TestIgnored _)) ->
                TestSuccess
            | _ ->
                { new IVerificationInfo with
                    member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
                    member _.Actual = result.ToString ()
                }
                |> newFailure.With.TestValidationFailure
                |> TestFailure
        )
    )

// Setup, TestBody, Teardown
let ``return an ITest with everything when given no tags`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Ignore (
                    Setup (fun _ -> Ok ()),
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
                getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                getTestName >> Should.BeEqualTo testName >> withMessage "TestName"
                getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``not run setup method passed to it when given no tags`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    Setup monitor.CallSetup,
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    emptyTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.SetupWasCalled
            |> Should.BeFalse
            |> withMessage "Setup called"
        )
    )
    
let ``not run the test method passed to it when given no tags`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    Setup monitor.CallSetup,
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeFalse
            |> withMessage "Test was called"
        )
    )
    
let ``return an ignored failure upon test being executed executed when given no tags`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    Setup monitor.CallSetup,
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let result =     
                test.GetExecutor ()
                |> executeFunction
                |> runIt
            
            match result with
            | TestExecutionResult (TestFailure (TestIgnored _)) ->
                TestSuccess
            | _ ->
                { new IVerificationInfo with
                    member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
                    member _.Actual = result.ToString ()
                }
                |> newFailure.With.TestValidationFailure
                |> TestFailure
        )
    )
    
let ``not run the teardown method passed to it when given no tags`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    Setup monitor.CallSetup,
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
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
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Ignore (
                    Setup (fun _ -> Ok ()),
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                getTestName >> Should.BeEqualTo testName >> withMessage "TestName"
                getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``not run setup method passed to it when given no tags, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    Setup monitor.CallSetup,
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.SetupWasCalled
            |> Should.BeFalse
            |> withMessage "Setup called"
        )
    )
    
let ``not run the test method passed to it when given no tags, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    Setup monitor.CallSetup,
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeFalse
            |> withMessage "Test was called"
        )
    )

let ``return an ignored failure upon test being executed executed when no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    Setup monitor.CallSetup,
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let result =     
                test.GetExecutor ()
                |> executeFunction
                |> runIt
            
            match result with
            | TestExecutionResult (TestFailure (TestIgnored _)) ->
                TestSuccess
            | _ ->
                { new IVerificationInfo with
                    member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
                    member _.Actual = result.ToString ()
                }
                |> newFailure.With.TestValidationFailure
                |> TestFailure
        )
    )

// TestBody, Teardown
let ``return an ITest with everything when given no tags, no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Ignore (
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
                getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                getTestName >> Should.BeEqualTo testName >> withMessage "TestName"
                getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``not run the test method passed to it when given no tags, no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeFalse
            |> withMessage "test called"
        )
    )
    
let ``return an ignored failure upon test being executed executed when given no tags, no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            let result =     
                test.GetExecutor ()
                |> executeFunction
                |> runIt
            
            match result with
            | TestExecutionResult (TestFailure (TestIgnored _)) ->
                TestSuccess
            | _ ->
                { new IVerificationInfo with
                    member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
                    member _.Actual = result.ToString ()
                }
                |> newFailure.With.TestValidationFailure
                |> TestFailure
        )
    )
    
let ``not run the teardown method passed to it when given no tags, no setup`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    Teardown monitor.CallTeardown,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
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
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Ignore (
                    TestBodyTwoParameters (fun _ _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                getTestName >> Should.BeEqualTo testName >> withMessage "TestName"
                getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``not run the test method passed to it when given no tags, no setup, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeFalse
            |> withMessage "test was called"
        )
    )
    
let ``return an ignored failure upon test being executed executed when given no tags, no setup, no teardown`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
            
            let result =    
                test.GetExecutor ()
                |> executeFunction
                |> runIt
            
            match result with
            | TestExecutionResult (TestFailure (TestIgnored _)) ->
                TestSuccess
            | _ ->
                { new IVerificationInfo with
                    member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
                    member _.Actual = result.ToString ()
                }
                |> newFailure.With.TestValidationFailure
                |> TestFailure
        )
    )

let ``return an ITest with everything when given no tags, no setup, no teardown, no test body indicator`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let testName = "My test"
            
            let fileName = "dog.bark"
            let path = "D:\\"
            let fullPath = $"%s{path}%s{fileName}"
            let lineNumber = 73
            
            let test =
                testFeature.Ignore (
                    (fun _ _ -> TestSuccess),
                    testName,
                    fullPath,
                    lineNumber
                )
        
            let getContainerName (test: ITest) =
                $"%s{test.ContainerPath}.%s{test.ContainerName}"
                
            test
            |> Should.PassAllOf [
                getTags >> Should.BeEqualTo [] >> withMessage "Tags"
                getTestName >> Should.BeEqualTo testName >> withMessage "TestName"
                getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Information"
                getFilePath >> Should.BeEqualTo path >> withMessage "file path"
                getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
                getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            ]
        )
    )
    
let ``not run the test method passed to it when given no tags, no setup, no teardown, no test body indicator`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    monitor.CallTestActionWithSetupEnvironment,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
                
            test.GetExecutor ()
            |> executeFunction
            |> runIt
            |> ignore
            
            monitor.TestWasCalled
            |> Should.BeFalse
            |> withMessage "test was called"
        )
    )
    
let ``return an ignored failure upon test being executed executed when given no tags, no setup, no teardown, no test body indicator`` =
    feature.Test (
        Setup setupFeatureUnderTest,
        TestBody (fun (testFeature: IFeature<unit>) ->
            let monitor = Monitor<unit, unit> (Ok ())
            let test =
                testFeature.Ignore (
                    monitor.CallTestActionWithSetupEnvironment,
                    "My test",
                    "D:\\dog.bark",
                    73
                )
            
            let result =    
                test.GetExecutor ()
                |> executeFunction
                |> runIt
            
            match result with
            | TestExecutionResult (TestFailure (TestIgnored _)) ->
                TestSuccess
            | _ ->
                { new IVerificationInfo with
                    member _.Expected = "TestExecutionResult (TestFailure (TestIgnored _))"
                    member _.Actual = result.ToString () }
                |> newFailure.With.TestValidationFailure
                |> TestFailure
        )
    )

let ``Test Cases`` = feature.GetTests ()