open Archer
open Archer.Arrows.Tests
open Archer.Arrows.Tests.Test
open Archer.Arrows.Tests.RawTestObjects
open Archer.Bow
open Archer.CoreTypes.InternalTypes
open Archer.CoreTypes.InternalTypes.RunnerTypes
open Archer.Logger.Summaries
open MicroLang.Lang

let runner = bow.Runner ()

runner.RunnerLifecycleEvent
|> Event.add (fun args ->
    match args with
    | RunnerStartExecution _ ->
        printfn ""
    | RunnerTestLifeCycle (test, testEventLifecycle, _) ->
        match testEventLifecycle with
        | TestEndExecution testExecutionResult ->
            match testExecutionResult with
            | TestExecutionResult TestSuccess -> ()
            | result ->
                let transformedResult = defaultTestExecutionResultSummaryTransformer result test
                printfn $"%s{transformedResult}"
            
        | _ -> ()
    | RunnerEndExecution ->
        printfn "\n"
)

runner
|> addMany [
    ``Feature Should``.``Test Cases``
    ``TestCase Should``.``Test Cases``
    ``TestCaseExecutor Should``.``Test Cases``
    ``TestCaseExecutor Execute Should``.``Test Cases``
    ``TestCaseExecutor Events Should``.``Test Cases``
    ``TestCaseExecutor Event Cancellation Should``.``Test Cases``
    ``Arrow NewFeature``.``Test Cases``
    ``Arrow NewFeature With Setup``.``Test Cases``
    ``Arrow NewFeature With Teardown``.``Test Cases``
    
    ``001 - Feature Test with test name, tags, setup, data, test body indicator three parameters, teardown should``.``Test Cases``
    ``002 - Feature Test with test name, tags, setup, data, test body indicator two parameters, teardown should``.``Test Cases``
    
    ``003 - Feature Test with test name, tags, setup, data, test body indicator three parameters should``.``Test Cases``
    ``004 - Feature Test with test name, tags, setup, data, test body indicator two parameters should``.``Test Cases``
    
    ``005 - Feature Test with test name, tags, setup, test body indicator two parameters, teardown should``.``Test Cases``
    ``006 - Feature Test with test name, tags, setup, test body indicator one parameter, teardown should``.``Test Cases``
    
    ``007 - Feature Test with test name, tags, setup, test body indicator two parameters should``.``Test Cases``
    ``008 - Feature Test with test name, tags, setup, test body indicator one parameter should``.``Test Cases``
    
    ``009 - Feature Test with test name, tags, data, test body indicator three parameters, teardown should``.``Test Cases``
    ``010 - Feature Test with test name, tags, data, test body indicator two parameters, teardown should``.``Test Cases``
    ``011 - Feature Test with test name, tags, data, test body indicator one parameter, teardown should``.``Test Cases``
    
    ``012 - Feature Test with test name, tags, data, test body indicator three parameters should``.``Test Cases``
    ``013 - Feature Test with test name, tags, data, test body indicator two parameters should``.``Test Cases``
    ``014 - Feature Test with test name, tags, data, test body indicator one parameter should``.``Test Cases``
    
    ``015 - Feature Test with test name, tags, data, test function three parameters should``.``Test Cases``
    ``016 - Feature Test with test name, tags, data, test function two parameters should``.``Test Cases``
    
    ``Arrow Ignore``.``Test Cases``
]
|> runAndReport
