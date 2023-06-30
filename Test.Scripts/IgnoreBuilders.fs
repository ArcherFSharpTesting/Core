module Archer.Arrows.Tests.IgnoreBuilders

open Archer.Arrows
open Archer.Arrows.Internal.Types
open Archer.Arrows.Tests.TestBuilders
open Microsoft.FSharp.Core
open System.Runtime.InteropServices

type IgnoreBuilder =
    static member GetTestNames (f: int -> 'a -> string) data =
        getNamesForThreeTests f data
        
    //test name, tags, setup, data, test body indicator, teardown
    static member BuildTestWithTestNameTagsSetupDataTestBodyTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let tests =
            testFeature.Ignore (
                testName,
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsSetupDataTestBodyTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let tests =
            testFeature.Ignore (
                testName,
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
         
    //test name, tags, setup, data, test body indicator
    static member BuildTestWithTestNameTagsSetupDataTestBodyNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts  repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        
        let tests =
            testFeature.Ignore (
                testName,
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsSetupDataTestBody (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        
        let tests =
            testFeature.Ignore (
                testName,
                TestTags tags,
                Setup setup,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
    
    //test name, tags, setup, test body indicator, teardown
    static member BuildTestWithTestNameTagsSetupTestBodyTeardown (testFeature: IFeature<string>) =
        let monitor, (testName, tags, testSetupValue), (path, fileName, fullPath, lineNumber) = getTestParts ()
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureTwoParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let test =
            testFeature.Ignore (
                testName,
                TestTags tags,
                Setup setup,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (tags, testName), (path, fileName, lineNumber)
    
    //test name, tags, setup, test body indicator
    static member BuildTestWithTestNameTagsSetupTestBody (testFeature: IFeature<string>) =
        let monitor, (testName, tags, testSetupValue), (path, fileName, fullPath, lineNumber) = getTestParts ()
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureTwoParametersSuccess
        
        let test =
            testFeature.Ignore (
                testName,
                TestTags tags,
                Setup setup,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (tags, testName), (path, fileName, lineNumber)
        
    //test name, tags, data, test body indicator, teardown
    static member BuildTestWithTestNameTagsDataTestBodyTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Ignore (
                testName,
                TestTags tags,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameTagsDataTestBodyTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let tests =
            testFeature.Ignore (
                testName,
                TestTags tags,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    //test name, tags, data, test body indicator
    static member BuildTestWithTestNameTagsDataTestBodyNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        
        let tests =
            testFeature.Ignore (
                testName,
                TestTags tags,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
    
    static member BuildTestWithTestNameTagsDataTestBody (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
        
        let tests =
            testFeature.Ignore (
                testName,
                TestTags tags,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    ////test name, tags, data, test function
    //static member BuildTestWithTestNameTagsDataTestFunctionNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            testName,
    //            TestTags tags,
    //            Data data,
    //            testBody,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
    //    
    //static member BuildTestWithTestNameTagsDataTestFunction (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            testName,
    //            TestTags tags,
    //            Data data,
    //            testBody,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
        
    //test name, tags, test body indicator, teardown
    static member BuildTestWithTestNameTagsTestBodyTeardown (testFeature: IFeature<string>) =
        let monitor, (testName, tags, _), (path, fileName, fullPath, lineNumber) =
            getTestParts ()
    
        let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
        let teardown = monitor.FunctionTeardownPassThrough
        
        let test =
            testFeature.Ignore (
                testName,
                TestTags tags,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (tags, testName), (path, fileName, lineNumber)
        
    //test name, tags, test body indicator
    static member BuildTestWithTestNameTagsTestBody (testFeature: IFeature<string>) =
        let monitor, (testName, tags, _), (path, fileName, fullPath, lineNumber) =
            getTestParts ()
    
        let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
        
        let test =
            testFeature.Ignore (
                testName,
                TestTags tags,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (tags, testName), (path, fileName, lineNumber)
    
    ////test name, tags, test function
    //static member BuildTestWithTestNameTagsTestFunction (testFeature: IFeature<string>) =
    //    let monitor, (testName, tags, _), (path, fileName, fullPath, lineNumber) =
    //        getTestParts ()
    //
    //    let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
    //    
    //    let test =
    //        testFeature.Ignore (
    //            testName,
    //            TestTags tags,
    //            testBody,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, test), (tags, testName), (path, fileName, lineNumber)
    
    //test name, setup, data, test body indicator, teardown
    static member BuildTestWithTestNameSetupDataTestBodyTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (_, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith  testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let tests =
            testFeature.Ignore (
                testName,
                Setup setup,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (testSetupValue, data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameSetupDataTestBodyTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (_, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith  testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let tests =
            testFeature.Ignore (
                testName,
                Setup setup,
                Data data,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (testSetupValue, data, testName), (path, fileName, lineNumber)
    
    //test name, setup, data, test body indicator
    static member BuildTestWithTestNameSetupDataTestBodyNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testNameRoot, testName), (_, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith  testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        
        let tests =
            testFeature.Ignore (
                testName,
                Setup setup,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testNameRoot), (path, fileName, lineNumber)
        
    static member BuildTestWithTestNameSetupDataTestBody (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
        let monitor, (testName, _), (_, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
            getDataTestParts repeatDataValue
    
        let setup = monitor.FunctionSetupFeatureWith  testSetupValue
        let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
        
        let tests =
            testFeature.Ignore (
                testName,
                Setup setup,
                Data data,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, tests), (data, testName), (path, fileName, lineNumber)
    
    //test name, setup, test body indicator, teardown
    static member BuildTestWithTestNameSetupTestBodyTeardown (testFeature: IFeature<string>) =
        let monitor, (testName, _, testSetupValue), (path, fileName, fullPath, lineNumber) =
            getTestParts ()
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureTwoParametersSuccess
        let teardown = monitor.FunctionTeardownFeatureFromSetup
        
        let test =
            testFeature.Ignore (
                testName,
                Setup setup,
                TestBody testBody,
                Teardown teardown,
                fullPath,
                lineNumber
            )
    
        (monitor, test), testName, (path, fileName, lineNumber)
        
    //test name, setup, test body indicator
    static member BuildTestWithTestNameSetupTestBody (testFeature: IFeature<string>) =
        let monitor, (testName, _, testSetupValue), (path, fileName, fullPath, lineNumber) =
            getTestParts ()
    
        let setup = monitor.FunctionSetupFeatureWith testSetupValue
        let testBody = monitor.FunctionTestFeatureTwoParametersSuccess
        
        let test =
            testFeature.Ignore (
                testName,
                Setup setup,
                TestBody testBody,
                fullPath,
                lineNumber
            )
    
        (monitor, test), (testName), (path, fileName, lineNumber)
    
    ////test name, data, test body indicator, teardown
    //static member BuildTestWithTestNameDataTestBodyTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testNameRoot, testName), (_, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    let teardown = monitor.FunctionTeardownPassThrough
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            testName,
    //            Data data,
    //            TestBody testBody,
    //            Teardown teardown,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (data, testNameRoot), (path, fileName, lineNumber)
    //    
    //static member BuildTestWithTestNameDataTestBodyTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testName, _), (_, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    let teardown = monitor.FunctionTeardownPassThrough
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            testName,
    //            Data data,
    //            TestBody testBody,
    //            Teardown teardown,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (data, testName), (path, fileName, lineNumber)
    //    
    ////test name, data, test body indicator
    //static member BuildTestWithTestNameDataTestBodyNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testNameRoot, testName), (_, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            testName,
    //            Data data,
    //            TestBody testBody,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (data, testNameRoot), (path, fileName, lineNumber)
    //
    //static member BuildTestWithTestNameDataTestBody (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testName, _), (_, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            testName,
    //            Data data,
    //            TestBody testBody,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (data, testName), (path, fileName, lineNumber)
    //    
    ////test name, data, test function
    //static member BuildTestWithTestNameDataTestFunctionNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testNameRoot, testName), (_, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            testName,
    //            Data data,
    //            testBody,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (data, testNameRoot), (path, fileName, lineNumber)
    //    
    //static member BuildTestWithTestNameDataTestFunction (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testName, _), (_, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            testName,
    //            Data data,
    //            testBody,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (data, testName), (path, fileName, lineNumber)
    //
    ////test name, test body indicator, teardown
    //static member BuildTestWithTestNameTestBodyTearDown (testFeature: IFeature<string>) =
    //    let monitor, (testName, _, _), (path, fileName, fullPath, lineNumber) =
    //        getTestParts ()
    //
    //    let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
    //    let teardown = monitor.FunctionTeardownPassThrough
    //    
    //    let test =
    //        testFeature.Ignore (
    //            testName,
    //            TestBody testBody,
    //            Teardown teardown,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, test), testName, (path, fileName, lineNumber)
    //
    ////test name, test body indicator
    //static member BuildTestWithTestNameTestBody (testFeature: IFeature<string>) =
    //    let monitor, (testName, _, _), (path, fileName, fullPath, lineNumber) =
    //        getTestParts ()
    //
    //    let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
    //    
    //    let test =
    //        testFeature.Ignore (
    //            testName,
    //            TestBody testBody,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, test), testName, (path, fileName, lineNumber)
    //
    ////test name, test function
    //static member BuildTestWithTestNameTestFunction (testFeature: IFeature<string>) =
    //    let monitor, (testName, _, _), (path, fileName, fullPath, lineNumber) =
    //        getTestParts ()
    //
    //    let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
    //    
    //    let test =
    //        testFeature.Ignore (
    //            testName,
    //            testBody,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, test), testName, (path, fileName, lineNumber)
    //    
    ////tags, setup, data, test body indicator, teardown
    //static member BuildTestWithTagsSetupDataTestBodyTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testNameRoot, testName), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let setup = monitor.FunctionSetupFeatureWith testSetupValue
    //    let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
    //    let teardown = monitor.FunctionTeardownFeatureFromSetup
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            TestTags tags,
    //            Setup setup,
    //            Data data,
    //            TestBody testBody,
    //            Teardown teardown,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (tags, testSetupValue, data, testNameRoot), (path, fileName, lineNumber)
    //    
    //static member BuildTestWithTagsSetupDataTestBodyTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testName, _), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts repeatDataValue
    //
    //    let setup = monitor.FunctionSetupFeatureWith testSetupValue
    //    let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
    //    let teardown = monitor.FunctionTeardownFeatureFromSetup
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            TestTags tags,
    //            Setup setup,
    //            Data data,
    //            TestBody testBody,
    //            Teardown teardown,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (tags, testSetupValue, data, testName), (path, fileName, lineNumber)
    //    
    ////tags, setup, data, test body indicator
    //static member BuildTestWithTagsSetupDataTestBodyNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testNameRoot, testName), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts  repeatDataValue
    //
    //    let setup = monitor.FunctionSetupFeatureWith testSetupValue
    //    let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            TestTags tags,
    //            Setup setup,
    //            Data data,
    //            TestBody testBody,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (tags, testSetupValue, data, testNameRoot), (path, fileName, lineNumber)
    //    
    //static member BuildTestWithTagsSetupDataTestBody (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testName, _), (tags, testSetupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts repeatDataValue
    //
    //    let setup = monitor.FunctionSetupFeatureWith testSetupValue
    //    let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            TestTags tags,
    //            Setup setup,
    //            Data data,
    //            TestBody testBody,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (tags, testSetupValue, data, testName), (path, fileName, lineNumber)
    //
    ////tags, setup, test body indicator, teardown
    //static member BuildTestWithTagsSetupTestBodyTeardown (testFeature: IFeature<string>) =
    //    let monitor, (testName, tags, testSetupValue), (path, fileName, fullPath, lineNumber) = getTestParts ()
    //
    //    let setup = monitor.FunctionSetupFeatureWith testSetupValue
    //    let testBody = monitor.FunctionTestFeatureTwoParametersSuccess
    //    let teardown = monitor.FunctionTeardownFeatureFromSetup
    //    
    //    let test =
    //        testFeature.Ignore (
    //            TestTags tags,
    //            Setup setup,
    //            TestBody testBody,
    //            Teardown teardown,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, test), (tags, testSetupValue, testName), (path, fileName, lineNumber)
    //    
    ////tags, setup, test body indicator
    //static member BuildTestWithTagsSetupTestBody (testFeature: IFeature<string>) =
    //    let monitor, (testName, tags, testSetupValue), (path, fileName, fullPath, lineNumber) = getTestParts ()
    //
    //    let setup = monitor.FunctionSetupFeatureWith testSetupValue
    //    let testBody = monitor.FunctionTestFeatureTwoParametersSuccess
    //    
    //    let test =
    //        testFeature.Ignore (
    //            TestTags tags,
    //            Setup setup,
    //            TestBody testBody,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, test), (tags, testSetupValue, testName), (path, fileName, lineNumber)
    //    
    ////tags, data, test body indicator, teardown
    //static member BuildTestWithTagsDataTestBodyTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    let teardown = monitor.FunctionTeardownPassThrough
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            TestTags tags,
    //            Data data,
    //            TestBody testBody,
    //            Teardown teardown,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
    //    
    //static member BuildTestWithTagsDataTestBodyTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    let teardown = monitor.FunctionTeardownPassThrough
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            TestTags tags,
    //            Data data,
    //            TestBody testBody,
    //            Teardown teardown,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
    //
    ////tags, data, test body indicator
    //static member BuildTestWithTagsDataTestBodyNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            TestTags tags,
    //            Data data,
    //            TestBody testBody,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
    //
    //static member BuildTestWithTagsDataTestBody (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            TestTags tags,
    //            Data data,
    //            TestBody testBody,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
    //    
    ////tags, data, test function
    //static member BuildTestWithTagsDataTestFunctionNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testNameRoot, testName), (tags, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            TestTags tags,
    //            Data data,
    //            testBody,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (tags, data, testNameRoot), (path, fileName, lineNumber)
    //    
    //static member BuildTestWithTagsDataTestFunction (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testName, _), (tags, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            TestTags tags,
    //            Data data,
    //            testBody,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (tags, data, testName), (path, fileName, lineNumber)
    //    
    ////tags, test body indicator, teardown
    //static member BuildTestWithTagsTestBodyTeardown (testFeature: IFeature<string>) =
    //    let monitor, (testName, tags, _), (path, fileName, fullPath, lineNumber) =
    //        getTestParts ()
    //
    //    let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
    //    let teardown = monitor.FunctionTeardownPassThrough
    //    
    //    let test =
    //        testFeature.Ignore (
    //            TestTags tags,
    //            TestBody testBody,
    //            Teardown teardown,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, test), (tags, testName), (path, fileName, lineNumber)
    //    
    ////tags, test body indicator
    //static member BuildTestWithTagsTestBody (testFeature: IFeature<string>) =
    //    let monitor, (testName, tags, _), (path, fileName, fullPath, lineNumber) =
    //        getTestParts ()
    //
    //    let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
    //    
    //    let test =
    //        testFeature.Ignore (
    //            TestTags tags,
    //            TestBody testBody,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, test), (tags, testName), (path, fileName, lineNumber)
    //    
    ////tags, test function
    //static member BuildTestWithTagsTestFunction (testFeature: IFeature<string>) =
    //    let monitor, (testName, tags, _), (path, fileName, fullPath, lineNumber) =
    //        getTestParts ()
    //
    //    let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
    //    
    //    let test =
    //        testFeature.Ignore (
    //            TestTags tags,
    //            testBody,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, test), (tags, testName), (path, fileName, lineNumber)
    //    
    ////setup, data, test body indicator, teardown
    //static member BuildTestWithSetupDataTestBodyTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testNameRoot, testName), (_, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let setup = monitor.FunctionSetupFeatureWith testSetupValue
    //    let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
    //    let teardown = monitor.FunctionTeardownFeatureFromSetup
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            Setup setup,
    //            Data data,
    //            TestBody testBody,
    //            Teardown teardown,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (testSetupValue, data, testNameRoot), (path, fileName, lineNumber)
    //
    //static member BuildTestWithSetupDataTestBodyTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testName, _), (_, testSetupValue, data), (path, fileName, fullPath, lineNumber) = getDataTestParts repeatDataValue
    //
    //    let setup = monitor.FunctionSetupFeatureWith testSetupValue
    //    let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
    //    let teardown = monitor.FunctionTeardownFeatureFromSetup
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            Setup setup,
    //            Data data,
    //            TestBody testBody,
    //            Teardown teardown,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (testSetupValue, data, testName), (path, fileName, lineNumber)
    //
    ////setup, data, test body indicator
    //static member BuildTestWithSetupDataTestBodyNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testNameRoot, testName), (_, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let setup = monitor.FunctionSetupFeatureWith  testSetupValue
    //    let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            Setup setup,
    //            Data data,
    //            TestBody testBody,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (testSetupValue, data, testNameRoot), (path, fileName, lineNumber)
    //    
    //static member BuildTestWithSetupDataTestBody (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testName, _), (_, testSetupValue, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let setup = monitor.FunctionSetupFeatureWith  testSetupValue
    //    let testBody = monitor.FunctionTestFeatureDataThreeParametersSuccess
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            Setup setup,
    //            Data data,
    //            TestBody testBody,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (testSetupValue, data, testName), (path, fileName, lineNumber)
    //    
    ////setup, test body indicator, teardown
    //static member BuildTestWithSetupTestBodyTeardown (testFeature: IFeature<string>) =
    //    let monitor, (testName, _, testSetupValue), (path, fileName, fullPath, lineNumber) =
    //        getTestParts ()
    //
    //    let setup = monitor.FunctionSetupFeatureWith testSetupValue
    //    let testBody = monitor.FunctionTestFeatureTwoParametersSuccess
    //    let teardown = monitor.FunctionTeardownFeatureFromSetup
    //    
    //    let test =
    //        testFeature.Ignore (
    //            Setup setup,
    //            TestBody testBody,
    //            Teardown teardown,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, test), (testSetupValue, testName), (path, fileName, lineNumber)
    //    
    ////setup, test body indicator
    //static member BuildTestWithSetupTestBody (testFeature: IFeature<string>) =
    //    let monitor, (testName, _, testSetupValue), (path, fileName, fullPath, lineNumber) = getTestParts ()
    //
    //    let setup = monitor.FunctionSetupFeatureWith testSetupValue
    //    let testBody = monitor.FunctionTestFeatureTwoParametersSuccess
    //    
    //    let test =
    //        testFeature.Ignore (
    //            Setup setup,
    //            TestBody testBody,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, test), (testSetupValue, testName), (path, fileName, lineNumber)
    //
    ////data, test body indicator, teardown
    //static member BuildTestWithDataTestBodyTeardownNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testNameRoot, testName), (_, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    let teardown = monitor.FunctionTeardownPassThrough
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            Data data,
    //            TestBody testBody,
    //            Teardown teardown,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (data, testNameRoot), (path, fileName, lineNumber)
    //    
    //static member BuildTestWithDataTestBodyTeardown (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testName, _), (_, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    let teardown = monitor.FunctionTeardownPassThrough
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            Data data,
    //            TestBody testBody,
    //            Teardown teardown,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (data, testName), (path, fileName, lineNumber)
    //
    ////data, test body indicator
    //static member BuildTestWithDataTestBodyNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testNameRoot, testName), (_, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            Data data,
    //            TestBody testBody,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (data, testNameRoot), (path, fileName, lineNumber)
    //
    //static member BuildTestWithDataTestBody (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testName, _), (_, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            Data data,
    //            TestBody testBody,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (data, testName), (path, fileName, lineNumber)
    //
    ////data, test function
    //static member BuildTestWithDataTestFunctionNameHints (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testNameRoot, testName), (_, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            Data data,
    //            testBody,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (data, testNameRoot), (path, fileName, lineNumber)
    //    
    //static member BuildTestWithDataTestFunction (testFeature: IFeature<string>, [<Optional; DefaultParameterValue(false)>] repeatDataValue: bool) =
    //    let monitor, (testName, _), (_, _, data), (path, fileName, fullPath, lineNumber) =
    //        getDataTestParts repeatDataValue
    //
    //    let testBody = monitor.FunctionTestPassThroughDataThreeParametersSuccess
    //    
    //    let tests =
    //        testFeature.Ignore (
    //            Data data,
    //            testBody,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, tests), (data, testName), (path, fileName, lineNumber)
    //    
    ////test body indicator, teardown
    //static member BuildTestWithTestBodyTearDown (testFeature: IFeature<string>) =
    //    let monitor, (testName, _, _), (path, fileName, fullPath, lineNumber) =
    //        getTestParts ()
    //
    //    let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
    //    let teardown = monitor.FunctionTeardownPassThrough
    //    
    //    let test =
    //        testFeature.Ignore (
    //            TestBody testBody,
    //            Teardown teardown,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, test), testName, (path, fileName, lineNumber)
    //
    ////test body indicator
    //static member BuildTestWithTestBody (testFeature: IFeature<string>) =
    //    let monitor, (testName, _, _), (path, fileName, fullPath, lineNumber) =
    //        getTestParts ()
    //
    //    let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
    //    
    //    let test =
    //        testFeature.Ignore (
    //            TestBody testBody,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, test), testName, (path, fileName, lineNumber)
    //
    ////test function
    //static member BuildTestWithTestFunction (testFeature: IFeature<string>) =
    //    let monitor, (testName, _, _), (path, fileName, fullPath, lineNumber) =
    //        getTestParts ()
    //
    //    let testBody = monitor.FunctionTestPassThroughTwoParametersSuccess
    //    
    //    let test =
    //        testFeature.Ignore (
    //            testBody,
    //            testName,
    //            fullPath,
    //            lineNumber
    //        )
    //
    //    (monitor, test), testName, (path, fileName, lineNumber)