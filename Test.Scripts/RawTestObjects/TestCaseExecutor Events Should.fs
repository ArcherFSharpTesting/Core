module Archer.Arrow.Tests.RawTestObjects.``TestCaseExecutor Events Should``

open Archer
open Archer.Arrow
open Archer.Arrow.Tests
open Archer.CoreTypes.InternalTypes
open Archer.MicroLang

let private container = suite.Container ()

let ``Trigger all the events in order`` =
    container.Test (
        SetupPart setupExecutor,
        
        fun executor _ ->
            let mutable cnt = 0
            let mutable result = TestSuccess
            
            let checkResult newResult =
                if result = TestSuccess then
                    result <- newResult
            
            executor.TestLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | TestStartExecution _ ->
                    cnt
                    |> expects.ToBe 0
                    |> withMessage "TestStartExecution"
                    |> checkResult
                | TestStartSetup _ ->
                    cnt
                    |> expects.ToBe 1
                    |> withMessage "TestStartSetup"
                    |> checkResult
                | TestEndSetup _ ->
                    cnt
                    |> expects.ToBe 2
                    |> withMessage "TestEndSetup"
                    |> checkResult
                | TestStart _ ->
                    cnt
                    |> expects.ToBe 3
                    |> withMessage "TestStart"
                    |> checkResult
                | TestEnd _ ->
                    cnt
                    |> expects.ToBe 4
                    |> withMessage "TestEnd"
                    |> checkResult
                | TestStartTeardown ->
                    cnt
                    |> expects.ToBe 5
                    |> withMessage "TestStartTeardown"
                    |> checkResult
                | TestEndExecution _ ->
                    cnt
                    |> expects.ToBe 6
                    |> withMessage "TestEndExecution"
                    |> checkResult
                
                cnt <- cnt + 1
            )
            
            executor.Execute (getFakeEnvironment ())
            |> ignore
            
            result
            |> andResult (cnt |> expects.ToBe 7 |> withMessage "Did not trigger all events")
    )
    
let ``Trigger events with parent`` =
    container.Test (
        fun _ ->
            let feature = arrow.NewFeature ("Events", "ThatTrigger")
            let test = feature.Test (successfulTest, "From a test")
            let executor = test.GetExecutor ()
            
            let mutable result = TestSuccess
            let mutable hasRun = false
            
            let checkResult getMessage newResult =
                if result = TestSuccess && newResult = TestSuccess then ()
                elif result = TestSuccess then
                    result <-
                        newResult
                        |> withMessage (getMessage ())
                    
            
            executor.TestLifecycleEvent.AddHandler (fun sender args ->
                hasRun <- true
                sender
                |> expects.ToBe executor.Parent
                |> checkResult (fun () -> $"%A{args}")
            )
            
            executor.Execute (getFakeEnvironment ())
            |> ignore
            
            hasRun
            |> expects.ToBeTrue
            |> withMessage "Events did not trigger"
            |> andResult result
    )

let ``Test Cases`` = container.Tests