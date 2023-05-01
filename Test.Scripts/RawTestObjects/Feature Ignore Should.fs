module Archer.Arrows.Tests.RawTestObjects.``Feature Ignore Should``

open Archer
open Archer.Arrows
open Archer.Arrows.Tests
open Archer.Fletching.Types.Internal
open Archer.MicroLang.Verification

let private feature = Arrow.NewFeature ()
let private failureBuilder = TestResultFailureBuilder TestExecutionResult

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
            TestWithEnvironmentBody (fun _ _ -> TestSuccess),
            Teardown (fun _ _ -> Ok ()),
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
            TestWithEnvironmentBody (fun _ _ -> TestSuccess),
            Teardown (fun _ _ -> Ok ()),
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
            TestWithEnvironmentBody (fun _ _ -> TestSuccess),
            Teardown (fun _ _ -> Ok ()),
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
            TestWithEnvironmentBody (fun _ _ -> TestSuccess),
            Teardown (fun _ _ -> Ok ()),
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
            TestWithEnvironmentBody (fun _ _ -> TestSuccess),
            Teardown (fun _ _ -> Ok ()),
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
            TestWithEnvironmentBody (fun _ _ -> TestSuccess),
            Teardown (fun _ _ -> Ok ()),
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

let ``return a test that does not call setup when run when given tags, setup, test body with environment, teardown, test name, path, and line number`` =
    feature.Test (fun _ ->
        let monitor = Monitor (Ok ())
        let fut = Arrow.NewFeature ()
        
        let test = fut.Ignore (
            TestTags [],
            Setup monitor.CallSetup,
            TestWithEnvironmentBody (fun _ _ -> TestSuccess),
            Teardown (fun _ _ -> Ok ()),
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.GetExecutor ()
        |> executeFunction
        |> runIt
        |> ignore
        
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
            TestWithEnvironmentBody monitor.CallTestActionWithEnvironment,
            Teardown (fun _ _ -> Ok ()),
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.GetExecutor ()
        |> executeFunction
        |> runIt
        |> ignore
        
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
            TestWithEnvironmentBody (fun _ _ -> TestSuccess),
            Teardown monitor.CallTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.GetExecutor ()
        |> executeFunction
        |> runIt
        |> ignore
        
        monitor.TeardownWasCalled
        |> Should.BeFalse
        |> withMessage "Teardown was run when it should not have been"
    )
    
// ---------------------------------------------------------------
// -             Tags, no setup, test body, teardown             -
// ---------------------------------------------------------------

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
            TestWithEnvironmentBody (fun _ _ -> TestSuccess),
            Teardown (fun _ _ -> Ok ()),
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
            TestWithEnvironmentBody (fun _ _ -> TestSuccess),
            Teardown (fun _ _ -> Ok ()),
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
            TestWithEnvironmentBody (fun _ _ -> TestSuccess),
            Teardown (fun _ _ -> Ok ()),
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
            TestWithEnvironmentBody (fun _ _ -> TestSuccess),
            Teardown (fun _ _ -> Ok ()),
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
            TestWithEnvironmentBody (fun _ _ -> TestSuccess),
            Teardown (fun _ _ -> Ok ()),
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
            TestWithEnvironmentBody (fun _ _ -> TestSuccess),
            Teardown (fun _ _ -> Ok ()),
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
            TestWithEnvironmentBody monitor.CallTestActionWithEnvironment,
            Teardown (fun _ _ -> Ok ()),
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.GetExecutor ()
        |> executeFunction
        |> runIt
        |> ignore
        
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
            TestWithEnvironmentBody (fun _ _ -> TestSuccess),
            Teardown monitor.CallTeardown,
            "My ignored test",
            "S:\\path\\file.f",
            3
        )
        
        test.GetExecutor ()
        |> executeFunction
        |> runIt
        |> ignore
        
        monitor.TeardownWasCalled
        |> Should.BeFalse
        |> withMessage "Teardown was run when it should not have been"
    )

let ``Test Cases`` = feature.GetTests ()