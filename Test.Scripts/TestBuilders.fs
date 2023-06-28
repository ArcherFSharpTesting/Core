[<AutoOpen>]
module Archer.Arrows.Tests.TestBuilders

open Archer
open Archer.Arrows
open Archer.Arrows.Internal.Types
open Archer.Arrows.Tests.TestMonitors
open Microsoft.FSharp.Core
open System.Runtime.InteropServices

let private rand = System.Random ()

let private getBaseTestParts () =
    let path = $"%s{randomCapitalLetter ()}:\\"
    let fileName = $"%s{randomWord (rand.Next (1, 5))}.%s{randomLetter ()}"
    let fullPath = $"%s{path}%s{fileName}"
    let lineNumber = rand.Next ()

    let tags = [
        Category $"%s{randomWord (rand.Next (3, 8))}"
        if rand.Next () % 2 = 0 then Only else (Category $"%s{randomWord (rand.Next (3, 8))}")
        if rand.Next () % 2 = 0 then Serial else (Category $"%s{randomWord (rand.Next (3, 8))}")
    ]

    tags, (path, fileName, fullPath, lineNumber)
    
let private getData repeatedData =
    if repeatedData then
        let l = randomLetter ()
        [l; l; l]
    else
        randomDistinctLetters 3
    
let private getMonitorBaseTestParts () =
    let tags, (path, fileName, fullPath, lineNumber) = getBaseTestParts ()
    let testSetupValue = rand.Next ()
    
    let monitor = getTestMonitor<string, string, int> ()
    
    monitor, (tags, testSetupValue), (path, fileName, fullPath, lineNumber)
    
let private getDataTestParts repeat =
    let testNameRoot = $"My %s{randomWord 5} Test"
    let testName = $"%s{testNameRoot} %%s"
    
    let monitor, (tags, testSetupValue), (path, fileName, fullPath, lineNumber) = getMonitorBaseTestParts ()

    let data = getData repeat

    monitor, (testNameRoot, testName), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber)
    
let private getTestParts () =
    let testName = $"My %s{randomWord 5} Test"
    
    let monitor, (tags, testSetupValue), (path, fileName, fullPath, lineNumber) = getMonitorBaseTestParts ()

    monitor, (testName, tags, testSetupValue), (path, fileName, fullPath, lineNumber)
    
type TestBuilder =
    static member GetTestNames (f: int -> 'a -> string) data =
        let [a; b; c] =
            data
            |> List.mapi f
            
        (a, b, c)
        
    //test name, tags, setup, data, test body indicator, teardown
    static member BuildTestWithTestNameTagsSetupDataTestBodyThreeParametersTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, testSetupValue, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsSetupDataTestBodyThreeParametersTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, testSetupValue, data, testName), (path, fileName, lineNumber)
    
    static member BuildTestWithTestNameTagsSetupDataTestBodyTwoParametersTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataTwoParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, testSetupValue, data, testNameRoot), (path, fileName, lineNumber)
    
    static member BuildTestWithTestNameTagsSetupDataTestBodyTwoParametersTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataTwoParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, testSetupValue, data, testName), (path, fileName, lineNumber)
        
    //test name, tags, setup, data, test body indicator
    static member BuildTestWithTestNameTagsSetupDataTestBodyThreeParametersNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts  repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, testSetupValue, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsSetupDataTestBodyThreeParameters (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, testSetupValue, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsSetupDataTestBodyTwoParametersNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataTwoParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, testSetupValue, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsSetupDataTestBodyTwoParameters (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataTwoParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, testSetupValue, data, testName), (path, fileName, lineNumber)
    
    //test name, tags, setup, test body indicator, teardown
    static member BuildTestWithTestNameTagsSetupTestBodyTwoParametersTeardown (testFeature: IFeature<string>) =
        let monitor, (testName, tags, testSetupValue), (path, fileName, fullPath, lineNumber) = getTestParts ()
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureTwoParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let test =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup setup,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (tags, testSetupValue, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsSetupTestBodyOneParameterTeardown (testFeature: IFeature<string>) =
        let monitor, (testName, tags, testSetupValue), (path, fileName, fullPath, lineNumber) = getTestParts ()
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureOneParameterSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let test =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup setup,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (tags, testSetupValue, testName), (path, fileName, lineNumber)

    //test name, tags, setup, test body indicator
    static member BuildTestWithTestNameTagsSetupTestBodyTwoParameters (testFeature: IFeature<string>) =
        let monitor, (testName, tags, testSetupValue), (path, fileName, fullPath, lineNumber) = getTestParts ()
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureTwoParametersSuccess
        
        let test =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup setup,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (tags, testSetupValue, testName), (path, fileName, lineNumber)

    static member BuildTestWithTestNameTagsSetupTestBodyOneParameter (testFeature: IFeature<string>) =
        let monitor, (testName, tags, testSetupValue), (path, fileName, fullPath, lineNumber) = getTestParts ()
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureOneParameterSuccess
        
        let test =
            testFeature.Test (
                testName,
                TestTags tags,
                Setup setup,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (tags, testSetupValue, testName), (path, fileName, lineNumber)
        
    // test name, tags, data, test body indicator, teardown
    static member BuildTestWithTestNameTagsDataTestBodyThreeParametersTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestBodyThreeParametersTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestBodyTwoParametersTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestPassThroughDataTwoParametersSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestBodyTwoParametersTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestPassThroughDataTwoParametersSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestBodyOneParameterTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestDataOneParameterSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestBodyOneParameterTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestDataOneParameterSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    // test name, tags, data, test body indicator
    static member BuildTestWithTestNameTagsDataTestBodyThreeParametersNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
    
    static member BuildTestWithTestNameTagsDataTestBodyThreeParameters (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestBodyTwoParametersNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestPassThroughDataTwoParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
    
    static member BuildTestWithTestNameTagsDataTestBodyTwoParameters (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestPassThroughDataTwoParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestBodyOneParameterNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestDataOneParameterSuccess
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
    
    static member BuildTestWithTestNameTagsDataTestBodyOneParameter (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestDataOneParameterSuccess
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    //test name, tags, data, test function
    static member BuildTestWithTestNameTagsDataTestFunctionThreeParametersNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                testBody,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestFunctionThreeParameters (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                testBody,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)

    static member BuildTestWithTestNameTagsDataTestFunctionTwoParametersNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestPassThroughDataTwoParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                testBody,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)

    static member BuildTestWithTestNameTagsDataTestFunctionTwoParameters (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestPassThroughDataTwoParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                testBody,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestFunctionOneParameterNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestDataOneParameterSuccess
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                testBody,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)

    static member BuildTestWithTestNameTagsDataTestFunctionOneParameter (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestDataOneParameterSuccess
        
        let tests =
            testFeature.Test (
                testName,
                TestTags tags,
                Data data,
                testBody,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    // test name, tags, test body indicator, teardown
    static member BuildTestWithTestNameTagsTestBodyTwoParametersTeardown (testFeature: IFeature<string>) =
        let monitor, (testName, tags, _), (path, fileName, fullPath, lineNumber) =
            getTestParts ()

        let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let test =
            testFeature.Test (
                testName,
                TestTags tags,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )

        (monitor, test), (tags, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsTestBodyOneParameterTeardown (testFeature: IFeature<string>) =
        let monitor, (testName, tags, _), (path, fileName, fullPath, lineNumber) =
            getTestParts ()

        let testBody = monitor.FunctionTestPassThroughOneParameterSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let test =
            testFeature.Test (
                testName,
                TestTags tags,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )

        (monitor, test), (tags, testName), (path, fileName, lineNumber)
        
    // test name, tags, test body indicator
    static member BuildTestWithTestNameTagsTestBodyTwoParameters (testFeature: IFeature<string>) =
        let monitor, (testName, tags, _), (path, fileName, fullPath, lineNumber) =
            getTestParts ()

        let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
        
        let test =
            testFeature.Test (
                testName,
                TestTags tags,
                TestBody testBody,
                fullPath,
                lineNumber
            )

        (monitor, test), (tags, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsTestBodyOneParameter (testFeature: IFeature<string>) =
        let monitor, (testName, tags, _), (path, fileName, fullPath, lineNumber) =
            getTestParts ()

        let testBody = monitor.FunctionTestPassThroughOneParameterSuccess
        
        let test =
            testFeature.Test (
                testName,
                TestTags tags,
                TestBody testBody,
                fullPath,
                lineNumber
            )

        (monitor, test), (tags, testName), (path, fileName, lineNumber)

    // test name, tags, test function
    static member BuildTestWithTestNameTagsTestFunctionTwoParameters (testFeature: IFeature<string>) =
        let monitor, (testName, tags, _), (path, fileName, fullPath, lineNumber) =
            getTestParts ()

        let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
        
        let test =
            testFeature.Test (
                testName,
                TestTags tags,
                testBody,
                fullPath,
                lineNumber
            )

        (monitor, test), (tags, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsTestFunctionOneParameter (testFeature: IFeature<string>) =
        let monitor, (testName, tags, _), (path, fileName, fullPath, lineNumber) =
            getTestParts ()

        let testBody = monitor.FunctionTestPassThroughOneParameterSuccess
        
        let test =
            testFeature.Test (
                testName,
                TestTags tags,
                testBody,
                fullPath,
                lineNumber
            )

        (monitor, test), (tags, testName), (path, fileName, lineNumber)

    // test name, setup, data, test body indicator, teardown
    static member BuildTestWithTestNameSetupDataTestBodyThreeParametersTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (_, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let setup = monitor.FunctionSetupFeatureWith  testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let tests =
            testFeature.Test (
                testName,
                Setup setup,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )

        (monitor, tests), (testSetupValue, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameSetupDataTestBodyThreeParametersTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (_, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let setup = monitor.FunctionSetupFeatureWith  testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let tests =
            testFeature.Test (
                testName,
                Setup setup,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )

        (monitor, tests), (testSetupValue, data, testName), (path, fileName, lineNumber)

    static member BuildTestWithTestNameSetupDataTestBodyTwoParametersTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (_, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let setup = monitor.FunctionSetupFeatureWith  testSetupValue
        let testBody = monitor.FunctionTestFeatureDataTwoParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let tests =
            testFeature.Test (
                testName,
                Setup setup,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )

        (monitor, tests), (testSetupValue, data, testNameRoot), (path, fileName, lineNumber)

    static member BuildTestWithTestNameSetupDataTestBodyTwoParametersTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (_, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let setup = monitor.FunctionSetupFeatureWith  testSetupValue
        let testBody = monitor.FunctionTestFeatureDataTwoParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let tests =
            testFeature.Test (
                testName,
                Setup setup,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )

        (monitor, tests), (testSetupValue, data, testName), (path, fileName, lineNumber)

    // test name, setup, data, test body indicator
    static member BuildTestWithTestNameSetupDataTestBodyThreeParametersNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (_, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith  testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                Setup setup,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (testSetupValue, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameSetupDataTestBodyThreeParameters (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (_, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith  testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                Setup setup,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (testSetupValue, data, testName), (path, fileName, lineNumber)

    static member BuildTestWithTestNameSetupDataTestBodyTwoParametersNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (_, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith  testSetupValue
        let testBody = monitor.FunctionTestFeatureDataTwoParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                Setup setup,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (testSetupValue, data, testNameRoot), (path, fileName, lineNumber)

    static member BuildTestWithTestNameSetupDataTestBodyTwoParameters (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (_, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith  testSetupValue
        let testBody = monitor.FunctionTestFeatureDataTwoParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                Setup setup,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (testSetupValue, data, testName), (path, fileName, lineNumber)

    // test name, setup, test body indicator, teardown
    static member BuildTestWithTestNameSetupTestBodyTwoParametersTeardown (testFeature: IFeature<string>) =
        let monitor, (testName, _, testSetupValue), (path, fileName, fullPath, lineNumber) =
            getTestParts ()

        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureTwoParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let test =
            testFeature.Test (
                testName,
                Setup setup,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )

        (monitor, test), (testSetupValue, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameSetupTestBodyOneParameterTeardown (testFeature: IFeature<string>) =
        let monitor, (testName, _, testSetupValue), (path, fileName, fullPath, lineNumber) =
            getTestParts ()

        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureOneParameterSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let test =
            testFeature.Test (
                testName,
                Setup setup,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )

        (monitor, test), (testSetupValue, testName), (path, fileName, lineNumber)
        
    // test name, setup, test body indicator
    static member BuildTestWithTestNameSetupTestBodyTwoParameters (testFeature: IFeature<string>) =
        let monitor, (testName, _, testSetupValue), (path, fileName, fullPath, lineNumber) =
            getTestParts ()

        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureTwoParametersSuccess
        
        let test =
            testFeature.Test (
                testName,
                Setup setup,
                TestBody testBody,
                fullPath,
                lineNumber
            )

        (monitor, test), (testSetupValue, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameSetupTestBodyOneParameter (testFeature: IFeature<string>) =
        let monitor, (testName, _, testSetupValue), (path, fileName, fullPath, lineNumber) =
            getTestParts ()

        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureOneParameterSuccess
        
        let test =
            testFeature.Test (
                testName,
                Setup setup,
                TestBody testBody,
                fullPath,
                lineNumber
            )

        (monitor, test), (testSetupValue, testName), (path, fileName, lineNumber)

    //test name, data, test body indicator, teardown
    static member BuildTestWithTestNameDataTestBodyThreeParametersTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (_, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameDataTestBodyThreeParametersTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (_, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)

    static member BuildTestWithTestNameDataTestBodyTwoParametersTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (_, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameRoot), (path, fileName, lineNumber)

    static member BuildTestWithTestNameDataTestBodyTwoParametersTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (_, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataTwoParametersSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)

    static member BuildTestWithTestNameDataTestBodyOneParameterTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (_, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestDataOneParameterSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameRoot), (path, fileName, lineNumber)

    static member BuildTestWithTestNameDataTestBodyOneParameterTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (_, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestDataOneParameterSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)
        
    // test name, data, test body indicator
    static member BuildTestWithTestNameDataTestBodyThreeParametersNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (_, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameRoot), (path, fileName, lineNumber)

    static member BuildTestWithTestNameDataTestBodyThreeParameters (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (_, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)

    static member BuildTestWithTestNameDataTestBodyTwoParametersNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (_, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataTwoParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameRoot), (path, fileName, lineNumber)

    static member BuildTestWithTestNameDataTestBodyTwoParameters (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (_, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataTwoParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)

    static member BuildTestWithTestNameDataTestBodyOneParameterNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (_, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestDataOneParameterSuccess
        
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameRoot), (path, fileName, lineNumber)

    static member BuildTestWithTestNameDataTestBodyOneParameter (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (_, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestDataOneParameterSuccess
        
        let tests =
            testFeature.Test (
                testName,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)
        
    // test name, data, test function
    static member BuildTestWithTestNameDataTestFunctionThreeParametersNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (_, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                Data data,
                testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameDataTestFunctionThreeParameters (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (_, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                Data data,
                testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameDataTestFunctionTwoParametersNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (_, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataTwoParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                Data data,
                testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameDataTestFunctionTwoParameters (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (_, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataTwoParametersSuccess
        
        let tests =
            testFeature.Test (
                testName,
                Data data,
                testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameDataTestFunctionOneParameterNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (_, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestDataOneParameterSuccess
        
        let tests =
            testFeature.Test (
                testName,
                Data data,
                testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameDataTestFunctionOneParameter (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (_, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestDataOneParameterSuccess
        
        let tests =
            testFeature.Test (
                testName,
                Data data,
                testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)
    
    // test name, test body indicator, teardown
    static member BuildTestWithTestNameTestBodyTwoParametersTearDown (testFeature: IFeature<string>) =
        let monitor, (testName, _, _), (path, fileName, fullPath, lineNumber) =
            getTestParts ()
    
        let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let test =
            testFeature.Test (
                testName,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, test), testName, (path, fileName, lineNumber)

    static member BuildTestWithTestNameTestBodyOneParameterTearDown (testFeature: IFeature<string>) =
        let monitor, (testName, _, _), (path, fileName, fullPath, lineNumber) =
            getTestParts ()
    
        let testBody = monitor.FunctionTestPassThroughOneParameterSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let test =
            testFeature.Test (
                testName,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, test), testName, (path, fileName, lineNumber)
    
    // test name, test body indicator
    static member BuildTestWithTestNameTestBodyTwoParameters (testFeature: IFeature<string>) =
        let monitor, (testName, _, _), (path, fileName, fullPath, lineNumber) =
            getTestParts ()
    
        let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
        
        let test =
            testFeature.Test (
                testName,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, test), testName, (path, fileName, lineNumber)

    static member BuildTestWithTestNameTestBodyOneParameter (testFeature: IFeature<string>) =
        let monitor, (testName, _, _), (path, fileName, fullPath, lineNumber) =
            getTestParts ()
    
        let testBody = monitor.FunctionTestPassThroughOneParameterSuccess
        
        let test =
            testFeature.Test (
                testName,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, test), testName, (path, fileName, lineNumber)

    // test name, test function
    static member BuildTestWithTestNameTestFunctionTwoParameters (testFeature: IFeature<string>) =
        let monitor, (testName, _, _), (path, fileName, fullPath, lineNumber) =
            getTestParts ()

        let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
        
        let test =
            testFeature.Test (
                testName,
                testBody,
                fullPath,
                lineNumber
            )

        (monitor, test), testName, (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTestFunctionOneParameter (testFeature: IFeature<string>) =
        let monitor, (testName, _, _), (path, fileName, fullPath, lineNumber) =
            getTestParts ()

        let testBody = monitor.FunctionTestPassThroughOneParameterSuccess
        
        let test =
            testFeature.Test (
                testName,
                testBody,
                fullPath,
                lineNumber
            )

        (monitor, test), testName, (path, fileName, lineNumber)
        
    // tags, setup, data, test body indicator, teardown
    static member BuildTestWithTagsSetupDataTestBodyThreeParametersTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                Teardown teardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, testSetupValue, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsSetupDataTestBodyThreeParametersTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                Teardown teardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, testSetupValue, data, testName), (path, fileName, lineNumber)
    
    static member BuildTestWithTagsSetupDataTestBodyTwoParametersTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataTwoParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                Teardown teardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, testSetupValue, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsSetupDataTestBodyTwoParametersTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataTwoParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                Teardown teardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, testSetupValue, data, testName), (path, fileName, lineNumber)
        
    // tags, setup, data, test body indicator
    static member BuildTestWithTagsSetupDataTestBodyThreeParametersNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts  repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, testSetupValue, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsSetupDataTestBodyThreeParameters (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, testSetupValue, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsSetupDataTestBodyTwoParametersNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataTwoParametersSuccess
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, testSetupValue, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsSetupDataTestBodyTwoParameters (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataTwoParametersSuccess
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, testSetupValue, data, testName), (path, fileName, lineNumber)
    
    // tags, setup, test body indicator, teardown
    static member BuildTestWithTagsSetupTestBodyTwoParametersTeardown (testFeature: IFeature<string>) =
        let monitor, (testName, tags, testSetupValue), (path, fileName, fullPath, lineNumber) = getTestParts ()
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureTwoParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let test =
            testFeature.Test (
                TestTags tags,
                Setup setup,
                TestBody testBody,
                Teardown teardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (tags, testSetupValue, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsSetupTestBodyOneParameterTeardown (testFeature: IFeature<string>) =
        let monitor, (testName, tags, testSetupValue), (path, fileName, fullPath, lineNumber) = getTestParts ()
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureOneParameterSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let test =
            testFeature.Test (
                TestTags tags,
                Setup setup,
                TestBody testBody,
                Teardown teardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (tags, testSetupValue, testName), (path, fileName, lineNumber)
        
    // tags, setup, test body indicator
    static member BuildTestWithTagsSetupTestBodyTwoParameters (testFeature: IFeature<string>) =
        let monitor, (testName, tags, testSetupValue), (path, fileName, fullPath, lineNumber) = getTestParts ()
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureTwoParametersSuccess
        
        let test =
            testFeature.Test (
                TestTags tags,
                Setup setup,
                TestBody testBody,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (tags, testSetupValue, testName), (path, fileName, lineNumber)

    static member BuildTestWithTagsSetupTestBodyOneParameter (testFeature: IFeature<string>) =
        let monitor, (testName, tags, testSetupValue), (path, fileName, fullPath, lineNumber) = getTestParts ()
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureOneParameterSuccess
        
        let test =
            testFeature.Test (
                TestTags tags,
                Setup setup,
                TestBody testBody,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (tags, testSetupValue, testName), (path, fileName, lineNumber)
        
    // tags, data, test body indicator, teardown
    static member BuildTestWithTagsDataTestBodyThreeParametersTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody testBody,
                Teardown teardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestBodyThreeParametersTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody testBody,
                Teardown teardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestBodyTwoParametersTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestPassThroughDataTwoParametersSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody testBody,
                Teardown teardown,
                testName,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestBodyTwoParametersTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestPassThroughDataTwoParametersSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody testBody,
                Teardown teardown,
                testName,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestBodyOneParameterTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestDataOneParameterSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody testBody,
                Teardown teardown,
                testName,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestBodyOneParameterTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestDataOneParameterSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody testBody,
                Teardown teardown,
                testName,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
    
    // tags, data, test body indicator
    static member BuildTestWithTagsDataTestBodyThreeParametersNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody testBody,
                testName,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
    
    static member BuildTestWithTagsDataTestBodyThreeParameters (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue

        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody testBody,
                testName,
                fullPath,
                lineNumber
            )

        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestBodyTwoParametersNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataTwoParametersSuccess
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody testBody,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
    
    static member BuildTestWithTagsDataTestBodyTwoParameters (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataTwoParametersSuccess
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody testBody,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestBodyOneParameterNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestDataOneParameterSuccess
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody testBody,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
    
    static member BuildTestWithTagsDataTestBodyOneParameter (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestDataOneParameterSuccess
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                TestBody testBody,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    // tags, data, test function
    static member BuildTestWithTagsDataTestFunctionThreeParametersNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                testBody,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestFunctionThreeParameters (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                testBody,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)

    static member BuildTestWithTagsDataTestFunctionTwoParametersNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataTwoParametersSuccess
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                testBody,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)

    static member BuildTestWithTagsDataTestFunctionTwoParameters (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataTwoParametersSuccess
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                testBody,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsDataTestFunctionOneParameterNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestDataOneParameterSuccess
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                testBody,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)

    static member BuildTestWithTagsDataTestFunctionOneParameter (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestDataOneParameterSuccess
        
        let tests =
            testFeature.Test (
                TestTags tags,
                Data data,
                testBody,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    // tags, test body indicator, teardown
    static member BuildTestWithTagsTestBodyTwoParametersTeardown (testFeature: IFeature<string>) =
        let monitor, (testName, tags, _), (path, fileName, fullPath, lineNumber) =
            getTestParts ()
    
        let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let test =
            testFeature.Test (
                TestTags tags,
                TestBody testBody,
                Teardown teardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (tags, testName), (path, fileName, lineNumber)
        
    static member BuildTestWithTagsTestBodyOneParameterTeardown (testFeature: IFeature<string>) =
        let monitor, (testName, tags, _), (path, fileName, fullPath, lineNumber) =
            getTestParts ()
    
        let testBody = monitor.FunctionTestPassThroughOneParameterSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let test =
            testFeature.Test (
                TestTags tags,
                TestBody testBody,
                Teardown teardown,
                testName,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (tags, testName), (path, fileName, lineNumber)