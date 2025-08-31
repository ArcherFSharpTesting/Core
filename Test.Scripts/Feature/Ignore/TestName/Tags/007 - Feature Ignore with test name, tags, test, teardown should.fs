module Archer.Core.Tests.Feature.Ignore.TestName.Tags.``007 - Feature Ignore with test name, tags, test, teardown should``

open System
open Archer
open Archer.Core
open Archer.Core.Internal.Types
open Archer.Core.Tests
open Archer.Core.Tests.IgnoreBuilders
open Archer.Types.InternalTypes
open Archer.MicroLang.Verification

let private feature = FeatureFactory.NewFeature (
    TestTags [
        Category "Feature"
        Category "Ignore"
    ],
    Setup setupFeatureUnderTest
)

let private rand = Random ()

let private getContainerName (test: ITest) =
    $"%s{test.ContainerPath}.%s{test.ContainerName}"

let ``Create a valid ITest`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (_, test), (tags, testName), (path, fileName, lineNumber) =
            IgnoreBuilder.BuildTestWithTestNameTagsTestBodyTeardown testFeature

        test
        |> Should.PassAllOf [
            getTags >> Should.BeEqualTo tags >> withMessage "Test Tags"
            getTestName >> Should.BeEqualTo testName >> withMessage "Test Name"
            getFilePath >> Should.BeEqualTo path >> withMessage "File Path"
            getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
            getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Name"
        ]
    )

let ``Not call setup when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, test), _, _ = IgnoreBuilder.BuildTestWithTestNameTagsTestBodyTeardown testFeature

        test
        |> silentlyRunTest

        monitor
        |> verifyNoSetupFunctionsHaveBeenCalled
    )

let ``Not call Test when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, test), _, _ = IgnoreBuilder.BuildTestWithTestNameTagsTestBodyTeardown testFeature

        test
        |> silentlyRunTest

        monitor
        |> verifyNoTestFunctionsHaveBeenCalled
    )

    
let ``Not call teardown when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, test), _, _ = IgnoreBuilder.BuildTestWithTestNameTagsTestBodyTeardown testFeature

        test
        |> silentlyRunTest

        monitor
        |> verifyNoTeardownFunctionsHaveBeenCalled
    )

let ``Test Cases`` = feature.GetTests ()