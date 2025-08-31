module Archer.Core.Tests.Feature.Ignore.TestName.``016 - Feature Ignore with test name, test should``

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
        let (_, test), testName, (path, fileName, lineNumber) =
            IgnoreBuilder.BuildTestWithTestNameTestBody testFeature

        test
        |> Should.PassAllOf [
            getTags >> Should.BeEqualTo [] >> withMessage "Test Tags"
            getTestName >> Should.BeEqualTo testName >> withMessage "Test Name"
            getFilePath >> Should.BeEqualTo path >> withMessage "File Path"
            getFileName >> Should.BeEqualTo fileName >> withMessage "File Name"
            getLineNumber >> Should.BeEqualTo lineNumber >> withMessage "Line Number"
            getContainerName >> Should.BeEqualTo (testFeature.ToString ()) >> withMessage "Container Name"
        ]
    )

let ``Not call Test when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, test), _, _ = IgnoreBuilder.BuildTestWithTestNameTestBody testFeature

        test
        |> silentlyRunTest

        monitor
        |> verifyNoTestFunctionsHaveBeenCalled
    )

let ``Test Cases`` = feature.GetTests ()