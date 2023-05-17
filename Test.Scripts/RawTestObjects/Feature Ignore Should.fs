module Archer.Arrows.Tests.RawTestObjects.``Feature Ignore Should``

open Archer
open Archer.Arrows
open Archer.Arrows.Internals
open Archer.Arrows.Tests
open Archer.Fletching.Types.Internal
open Archer.MicroLang.Verification

let private feature = Arrow.NewFeature (
    TestTags [
        Category "Feature"
        Category "Ignore"
    ]
)

let private failureBuilder = TestResultFailureBuilder TestExecutionResult

// --------------------------------------------------------------
// -                 All items with environment                 -
// --------------------------------------------------------------
let ``return a test with correct tags when given tags, setup, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let tags = 
            [
                Category "First"
                Category "Something Else"
                Category "Is Ignored by me at feature level"
            ]
            
        let test = fut.Ignore (
            TestTags tags,
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Tags
        |> Should.BeEqualTo tags
    )

let ``return a test with correct test name when given tags, setup, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.TestName
        |> Should.BeEqualTo "The ignored test"
    )

let ``return a test with correct file path when given tags, setup, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FilePath
        |> Should.BeEqualTo "S:\\path"
    )

let ``return a test with correct file name when given tags, setup, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FileName
        |> Should.BeEqualTo "file.f"
    )

let ``return a test with correct line number when given tags, setup, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            32
        )
        
        test.Location.LineNumber
        |> Should.BeEqualTo 32
    )

let ``return a test that returns an "Ignored" result when given tags, setup, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let expected = failureBuilder.IgnoreFailure ("S:\\path\\file.f", 3)
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        let result =
            test
            |> runTest
        
        result
        |> Should.BeEqualTo expected
    )

let ``return a test that does not call setup when run when given tags, setup, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup monitor.CallSetup,
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.SetupWasCalled
        |> Should.BeFalse
        |> withMessage "Setup was run when it should not have been"
    )

let ``return a test that does not call the test action when run when given tags, setup, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
            emptyTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.TestWasCalled
        |> Should.BeFalse
        |> withMessage "Test action was run when it should not have been"
    )

let ``return a test that does not call the teardown when run when given tags, setup, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            Teardown monitor.CallTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.TeardownWasCalled
        |> Should.BeFalse
        |> withMessage "Teardown was run when it should not have been"
    )
    
// --------------------------------------------------------------
// -       No setup, test body with environment, teardown       -
// --------------------------------------------------------------
let ``return a test with correct tags when given tags, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let tags = 
            [
                Category "First"
                Category "Something Else"
                Category "Is Ignored by me at feature level"
            ]
            
        let test = fut.Ignore (
            TestTags tags,
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Tags
        |> Should.BeEqualTo tags
    )

let ``return a test with correct test name when given tags, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.TestName
        |> Should.BeEqualTo "The ignored test"
    )

let ``return a test with correct file path when given tags, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FilePath
        |> Should.BeEqualTo "S:\\path"
    )

let ``return a test with correct file name when given tags, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FileName
        |> Should.BeEqualTo "file.f"
    )

let ``return a test with correct line number when given tags, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            32
        )
        
        test.Location.LineNumber
        |> Should.BeEqualTo 32
    )

let ``return a test that returns an "Ignored" result when given tags, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let expected = failureBuilder.IgnoreFailure ("S:\\path\\file.f", 3)
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        let result =
            test.GetExecutor ()
            |> executeFunction
            |> runIt
        
        result
        |> Should.BeEqualTo expected
    )

let ``return a test that does not call the test action when run when given tags, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
            emptyTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.TestWasCalled
        |> Should.BeFalse
        |> withMessage "Test action was run when it should not have been"
    )

let ``return a test that does not call the teardown when run when given tags, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            Teardown monitor.CallTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.TeardownWasCalled
        |> Should.BeFalse
        |> withMessage "Teardown was run when it should not have been"
    )

// ---------------------------------------------------------------
// -      No teardown, test body with environment, teardown      -
// ---------------------------------------------------------------
let ``return a test with correct tags when given tags, setup, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let tags = 
            [
                Category "First"
                Category "Something Else"
                Category "Is Ignored by me at feature level"
            ]
            
        let test = fut.Ignore (
            TestTags tags,
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Tags
        |> Should.BeEqualTo tags
    )

let ``return a test with correct test name when given tags, setup, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.TestName
        |> Should.BeEqualTo "The ignored test"
    )

let ``return a test with correct file path when given tags, setup, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FilePath
        |> Should.BeEqualTo "S:\\path"
    )

let ``return a test with correct file name when given tags, setup, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FileName
        |> Should.BeEqualTo "file.f"
    )

let ``return a test with correct line number when given tags, setup, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            32
        )
        
        test.Location.LineNumber
        |> Should.BeEqualTo 32
    )

let ``return a test that returns an "Ignored" result when given tags, setup, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let expected = failureBuilder.IgnoreFailure ("S:\\path\\file.f", 3)
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        let result =
            test.GetExecutor ()
            |> executeFunction
            |> runIt
        
        result
        |> Should.BeEqualTo expected
    )

let ``return a test that does not call setup when run when given tags, setup, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup monitor.CallSetup,
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.SetupWasCalled
        |> Should.BeFalse
        |> withMessage "Setup was run when it should not have been"
    )

let ``return a test that does not call the test action when run when given tags, setup, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.TestWasCalled
        |> Should.BeFalse
        |> withMessage "Test action was run when it should not have been"
    )
    
// ---------------------------------------------------------------
// -      No setup, test body with environment, no teardown      -
// ---------------------------------------------------------------
let ``return a test with correct tags when given tags, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let tags = 
            [
                Category "First"
                Category "Something Else"
                Category "Is Ignored by me at feature level"
            ]
            
        let test = fut.Ignore (
            TestTags tags,
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Tags
        |> Should.BeEqualTo tags
    )

let ``return a test with correct test name when given tags, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.TestName
        |> Should.BeEqualTo "The ignored test"
    )

let ``return a test with correct file path when given tags, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FilePath
        |> Should.BeEqualTo "S:\\path"
    )

let ``return a test with correct file name when given tags, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FileName
        |> Should.BeEqualTo "file.f"
    )

let ``return a test with correct line number when given tags, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            32
        )
        
        test.Location.LineNumber
        |> Should.BeEqualTo 32
    )

let ``return a test that returns an "Ignored" result when given tags, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let expected = failureBuilder.IgnoreFailure ("S:\\path\\file.f", 3)
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        let result =
            test.GetExecutor ()
            |> executeFunction
            |> runIt
        
        result
        |> Should.BeEqualTo expected
    )

let ``return a test that does not call the test action when run when given tags, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.TestWasCalled
        |> Should.BeFalse
        |> withMessage "Test action was run when it should not have been"
    )

// ----------------------------------------------------------------
// -               No teardown, test body, teardown               -
// ----------------------------------------------------------------
let ``return a test with correct tags when given tags, setup, test body, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let tags = 
            [
                Category "First"
                Category "Something Else"
                Category "Is Ignored by me at feature level"
            ]
            
        let test = fut.Ignore (
            TestTags tags,
            Setup (fun _ -> Ok ()),
            TestBody (fun _ -> TestSuccess),
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Tags
        |> Should.BeEqualTo tags
    )

let ``return a test with correct test name when given tags, setup, test body, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBody (fun _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.TestName
        |> Should.BeEqualTo "The ignored test"
    )

let ``return a test with correct file path when given tags, setup, test body, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBody (fun _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FilePath
        |> Should.BeEqualTo "S:\\path"
    )

let ``return a test with correct file name when given tags, setup, test body, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBody (fun _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FileName
        |> Should.BeEqualTo "file.f"
    )

let ``return a test with correct line number when given tags, setup, test body, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBody (fun _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            32
        )
        
        test.Location.LineNumber
        |> Should.BeEqualTo 32
    )

let ``return a test that returns an "Ignored" result when given tags, setup, test body, test name, path, and line number`` =
    feature.Test (fun _ ->
        let expected = failureBuilder.IgnoreFailure ("S:\\path\\file.f", 3)
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBody (fun _ -> TestSuccess),
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        let result =
            test.GetExecutor ()
            |> executeFunction
            |> runIt
        
        result
        |> Should.BeEqualTo expected
    )

let ``return a test that does not call setup when run when given tags, setup, test body, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup monitor.CallSetup,
            TestBody (fun _ -> TestSuccess),
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.SetupWasCalled
        |> Should.BeFalse
        |> withMessage "Setup was run when it should not have been"
    )

let ``return a test that does not call the test action when run when given tags, setup, test body, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup (fun _ -> Ok ()),
            TestBody monitor.CallTestActionWithSetup,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.TestWasCalled
        |> Should.BeFalse
        |> withMessage "Test action was run when it should not have been"
    )
    
// --------------------------------------------------------------
// -                    No tags, environment                    -
// --------------------------------------------------------------
let ``return a test with correct test name when given setup, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.TestName
        |> Should.BeEqualTo "The ignored test"
    )

let ``return a test with correct file path when given setup, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FilePath
        |> Should.BeEqualTo "S:\\path"
    )

let ``return a test with correct file name when given setup, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FileName
        |> Should.BeEqualTo "file.f"
    )

let ``return a test with correct line number when given setup, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            32
        )
        
        test.Location.LineNumber
        |> Should.BeEqualTo 32
    )

let ``return a test that returns an "Ignored" result when given setup, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let expected = failureBuilder.IgnoreFailure ("S:\\path\\file.f", 3)
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        let result =
            test.GetExecutor ()
            |> executeFunction
            |> runIt
        
        result
        |> Should.BeEqualTo expected
    )

let ``return a test that does not call setup when run when given setup, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            Setup monitor.CallSetup,
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.SetupWasCalled
        |> Should.BeFalse
        |> withMessage "Setup was run when it should not have been"
    )

let ``return a test that does not call the test action when run when given setup, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
            emptyTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.TestWasCalled
        |> Should.BeFalse
        |> withMessage "Test action was run when it should not have been"
    )

let ``return a test that does not call the teardown when run when given setup, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            Teardown monitor.CallTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.TeardownWasCalled
        |> Should.BeFalse
        |> withMessage "Teardown was run when it should not have been"
    )

// ----------------------------------------------------------------
// -       No tags, test body with environment, No teardown       -
// ----------------------------------------------------------------
let ``return a test with correct test name when given setup, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.TestName
        |> Should.BeEqualTo "The ignored test"
    )

let ``return a test with correct file path when given setup, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FilePath
        |> Should.BeEqualTo "S:\\path"
    )

let ``return a test with correct file name when given setup, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FileName
        |> Should.BeEqualTo "file.f"
    )

let ``return a test with correct line number when given setup, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            32
        )
        
        test.Location.LineNumber
        |> Should.BeEqualTo 32
    )

let ``return a test that returns an "Ignored" result when given setup, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let expected = failureBuilder.IgnoreFailure ("S:\\path\\file.f", 3)
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        let result =
            test.GetExecutor ()
            |> executeFunction
            |> runIt
        
        result
        |> Should.BeEqualTo expected
    )

let ``return a test that does not call setup when run when given setup, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            Setup monitor.CallSetup,
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.SetupWasCalled
        |> Should.BeFalse
        |> withMessage "Setup was run when it should not have been"
    )

let ``return a test that does not call the test action when run when given setup, test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            Setup (fun _ -> Ok ()),
            TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.TestWasCalled
        |> Should.BeFalse
        |> withMessage "Test action was run when it should not have been"
    )

// --------------------------------------------------------------
// -       No setup, test body with environment, teardown       -
// --------------------------------------------------------------
let ``return a test with correct test name when given test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.TestName
        |> Should.BeEqualTo "The ignored test"
    )

let ``return a test with correct file path when given test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FilePath
        |> Should.BeEqualTo "S:\\path"
    )

let ``return a test with correct file name when given test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FileName
        |> Should.BeEqualTo "file.f"
    )

let ``return a test with correct line number when given test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            32
        )
        
        test.Location.LineNumber
        |> Should.BeEqualTo 32
    )

let ``return a test that returns an "Ignored" result when given test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let expected = failureBuilder.IgnoreFailure ("S:\\path\\file.f", 3)
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            emptyTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        let result =
            test.GetExecutor ()
            |> executeFunction
            |> runIt
        
        result
        |> Should.BeEqualTo expected
    )

let ``return a test that does not call the test action when run when given test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestBodyTwoParameters monitor.CallTestActionWithSetupEnvironment,
            emptyTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.TestWasCalled
        |> Should.BeFalse
        |> withMessage "Test action was run when it should not have been"
    )

let ``return a test that does not call the teardown when run when given test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestBodyTwoParameters (fun _ _ -> TestSuccess),
            Teardown monitor.CallTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.TeardownWasCalled
        |> Should.BeFalse
        |> withMessage "Teardown was run when it should not have been"
    )
    
// ----------------------------------------------------------------
// -            No tags, no setup, test body, teardown            -
// ----------------------------------------------------------------
let ``return a test with correct test name when given test body, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestBody (fun _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.TestName
        |> Should.BeEqualTo "The ignored test"
    )

let ``return a test with correct file path when given test body, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestBody (fun _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FilePath
        |> Should.BeEqualTo "S:\\path"
    )

let ``return a test with correct file name when given test body, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestBody (fun _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FileName
        |> Should.BeEqualTo "file.f"
    )

let ``return a test with correct line number when given test body, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestBody (fun _ -> TestSuccess),
            emptyTeardown,
            "The ignored test",
            "S:\\path\\file.f",
            32
        )
        
        test.Location.LineNumber
        |> Should.BeEqualTo 32
    )

let ``return a test that returns an "Ignored" result when given test body, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let expected = failureBuilder.IgnoreFailure ("S:\\path\\file.f", 3)
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestBody (fun _ -> TestSuccess),
            emptyTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        let result =
            test.GetExecutor ()
            |> executeFunction
            |> runIt
        
        result
        |> Should.BeEqualTo expected
    )

let ``return a test that does not call the test action when run when given test body, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestBody monitor.CallTestActionWithSetup,
            emptyTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.TestWasCalled
        |> Should.BeFalse
        |> withMessage "Test action was run when it should not have been"
    )

let ``return a test that does not call the teardown when run when given test body, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestBody (fun _ -> TestSuccess),
            Teardown monitor.CallTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.TeardownWasCalled
        |> Should.BeFalse
        |> withMessage "Teardown was run when it should not have been"
    )
    
// ---------------------------------------------------------------
// -      No setup, test body with environment, No teardown      -
// ---------------------------------------------------------------
let ``return a test with correct test name when given test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            (fun _ _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.TestName
        |> Should.BeEqualTo "The ignored test"
    )

let ``return a test with correct file path when given test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            (fun _ _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FilePath
        |> Should.BeEqualTo "S:\\path"
    )

let ``return a test with correct file name when given test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            (fun _ _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FileName
        |> Should.BeEqualTo "file.f"
    )

let ``return a test with correct line number when given test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            (fun _ _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            32
        )
        
        test.Location.LineNumber
        |> Should.BeEqualTo 32
    )

let ``return a test that returns an "Ignored" result when given test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let expected = failureBuilder.IgnoreFailure ("S:\\path\\file.f", 3)
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            (fun _ _ -> TestSuccess),
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        let result =
            test.GetExecutor ()
            |> executeFunction
            |> runIt
        
        result
        |> Should.BeEqualTo expected
    )

let ``return a test that does not call the test action when run when given test body with environment, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            monitor.CallTestActionWithSetupEnvironment,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.TestWasCalled
        |> Should.BeFalse
        |> withMessage "Test action was run when it should not have been"
    )
    
// ----------------------------------------------------------------
// -            No tags, no setup, test body, teardown            -
// ----------------------------------------------------------------
let ``return a test with correct test name when given test body, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            (fun _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.TestName
        |> Should.BeEqualTo "The ignored test"
    )

let ``return a test with correct file path when given test body, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            (fun _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FilePath
        |> Should.BeEqualTo "S:\\path"
    )

let ``return a test with correct file name when given test body, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            (fun _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.Location.FileName
        |> Should.BeEqualTo "file.f"
    )

let ``return a test with correct line number when given test body, test name, path, and line number`` =
    feature.Test (fun _ ->
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            (fun _ -> TestSuccess),
            "The ignored test",
            "S:\\path\\file.f",
            32
        )
        
        test.Location.LineNumber
        |> Should.BeEqualTo 32
    )

let ``return a test that returns an "Ignored" result when given test body, test name, path, and line number`` =
    feature.Test (fun _ ->
        let expected = failureBuilder.IgnoreFailure ("S:\\path\\file.f", 3)
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            (fun _ -> TestSuccess),
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        let result =
            test.GetExecutor ()
            |> executeFunction
            |> runIt
        
        result
        |> Should.BeEqualTo expected
    )

let ``return a test that does not call the test action when run when given test body, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            monitor.CallTestActionWithSetup,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test
        |> silentlyRunTest
        
        monitor.TestWasCalled
        |> Should.BeFalse
        |> withMessage "Test action was run when it should not have been"
    )
    
let ``Test Cases`` = feature.GetTests ()