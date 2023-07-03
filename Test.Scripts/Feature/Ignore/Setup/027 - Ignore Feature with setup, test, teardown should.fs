module Archer.Arrows.Tests.Feature.Ignore.Setup.``027 - Ignore Feature with setup, test, teardown should``

open System
open Archer
open Archer.Arrows
open Archer.Arrows.Internal.Types
open Archer.Arrows.Tests
open Archer.Arrows.Tests.IgnoreBuilders
open Archer.CoreTypes.InternalTypes
open Archer.MicroLang.Verification

let private feature = Arrow.NewFeature (
    TestTags [
        Category "Feature"
        Category "Test"
    ],
    Setup setupFeatureUnderTest
)

let private rand = Random ()

let private getContainerName (test: ITest) =
    $"%s{test.ContainerPath}.%s{test.ContainerName}"

let ``Create a valid ITest`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (_, test), testName, (path, fileName, lineNumber) =
            IgnoreBuilder.BuildTestWithSetupTestBodyTeardown testFeature

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

let ``Not call setup when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, test), _, _ = IgnoreBuilder.BuildTestWithSetupTestBodyTeardown testFeature

        test
        |> silentlyRunTest

        monitor
        |> verifyNoSetupFunctionsHaveBeenCalled
    )

let ``Not call Test when executed`` =
    feature.Test (fun (featureSetupValue, testFeature: IFeature<string>) ->
        let (monitor, test), _, _ = IgnoreBuilder.BuildTestWithSetupTestBodyTeardown testFeature

        test
        |> silentlyRunTest

        monitor
        |> verifyNoTestFunctionsHaveBeenCalled
    )
    
let ``Not call teardown when executed`` =
    feature.Test (fun (_, testFeature: IFeature<string>) ->
        let (monitor, test), _, _ = IgnoreBuilder.BuildTestWithSetupTestBodyTeardown testFeature

        test
        |> silentlyRunTest

        monitor
        |> verifyNoTeardownFunctionsHaveBeenCalled
    )

let ``Test Cases`` = feature.GetTests ()