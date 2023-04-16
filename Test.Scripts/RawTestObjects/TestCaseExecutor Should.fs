module Archer.Arrow.Tests.RawTestObjects.``TestCaseExecutor Should``

open Archer.Arrow
open Archer.Arrow.Internal
open Archer.Arrow.Tests
open Archer.MicroLang

let private container = suite.Container ()

let buildFeatureUnderTest _ = arrow.NewFeature (ignoreString (), ignoreString ()) 

let setupBuildExecutorWithSetupAction _ =
    let buildExecutor setupAction =
        let feature = buildFeatureUnderTest ()
        let test = feature.Test (Setup setupAction, TestBody successfulEnvironmentTest, ignoreString(), $"%s{ignoreString ()}.fs", ignoreInt ())
        test.GetExecutor ()
        
    buildExecutor |> Ok

let ``Run the setup action when execute is called`` =
    container.Test (
        SetupPart setupBuildExecutorWithSetupAction,
        
        fun testBuilder _ ->
            let mutable wasRun = false
            let setupAction _ =
                wasRun <- true
                Ok ()
                
            let executor = testBuilder setupAction
            
            executor.Execute (getEmptyFrameworkEnvironment ())
            |> ignore
                
            wasRun
            |> expects.ToBeTrue
            |> withMessage "Test did not run"
    )

let ``Test Cases`` = container.Tests