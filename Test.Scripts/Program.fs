open Archer
open Archer.Arrows.Tests
open Archer.Arrows.Tests.Ignore.TestName.Tags.Setup
open Archer.Arrows.Tests.Ignore.TestName.Tags.Setup.Data
open Archer.Arrows.Tests.RawTestObjects
open Archer.Arrows.Tests.Feature.Test.Data.TestBodyIndicator
open Archer.Arrows.Tests.Feature.Test.Data.TestBodyIndicator.Teardown
open Archer.Arrows.Tests.Feature.Test.Data.TestFunction
open Archer.Arrows.Tests.Feature.Test.Setup.Data.TestBodyIndicator
open Archer.Arrows.Tests.Feature.Test.Setup.Data.TestBodyIndicator.Teardown
open Archer.Arrows.Tests.Feature.Test.Setup.TestBodyIndicator
open Archer.Arrows.Tests.Feature.Test.Setup.TestBodyIndicator.Teardown
open Archer.Arrows.Tests.Feature.Test.Tags.Data.TestBodyIndicator
open Archer.Arrows.Tests.Feature.Test.Tags.Data.TestBodyIndicator.Teardown
open Archer.Arrows.Tests.Feature.Test.Tags.Data.TestFunction
open Archer.Arrows.Tests.Feature.Test.Tags.Setup.Data.TestBodyIndicator
open Archer.Arrows.Tests.Feature.Test.Tags.Setup.Data.TestBodyIndicator.Teardown
open Archer.Arrows.Tests.Feature.Test.Tags.Setup.TestBodyIndicator
open Archer.Arrows.Tests.Feature.Test.Tags.Setup.TestBodyIndicator.Teardown
open Archer.Arrows.Tests.Feature.Test.Tags.TestBodyIndicator
open Archer.Arrows.Tests.Feature.Test.Tags.TestBodyIndicator.Teardown
open Archer.Arrows.Tests.Feature.Test.Tags.TestFunction
open Archer.Arrows.Tests.Feature.Test.TestBodyIndicator
open Archer.Arrows.Tests.Feature.Test.TestBodyIndicator.Teardown
open Archer.Arrows.Tests.Feature.Test.TestFunction
open Archer.Arrows.Tests.Feature.Test.TestName.Data.TestBodyIndicator
open Archer.Arrows.Tests.Feature.Test.TestName.Data.TestBodyIndicator.Teardown
open Archer.Arrows.Tests.Feature.Test.TestName.Data.TestFunction
open Archer.Arrows.Tests.Feature.Test.TestName.Setup.Data.TestBodyIndicator
open Archer.Arrows.Tests.Feature.Test.TestName.Setup.Data.TestBodyIndicator.Teardown
open Archer.Arrows.Tests.Feature.Test.TestName.Setup.TestBodyIndicator
open Archer.Arrows.Tests.Feature.Test.TestName.Setup.TestBodyIndicator.Teardown
open Archer.Arrows.Tests.Feature.Test.TestName.Tags.Data.TestBodyIndicator
open Archer.Arrows.Tests.Feature.Test.TestName.Tags.Data.TestBodyIndicator.Teardown
open Archer.Arrows.Tests.Feature.Test.TestName.Tags.Data.TestFunction
open Archer.Arrows.Tests.Feature.Test.TestName.Tags.Setup.Data.TestBodyIndicator
open Archer.Arrows.Tests.Feature.Test.TestName.Tags.Setup.Data.TestBodyIndicator.Teardown
open Archer.Arrows.Tests.Feature.Test.TestName.Tags.Setup.TestBodyIndicator
open Archer.Arrows.Tests.Feature.Test.TestName.Tags.Setup.TestBodyIndicator.Teardown
open Archer.Arrows.Tests.Feature.Test.TestName.Tags.TestBodyIndicator
open Archer.Arrows.Tests.Feature.Test.TestName.Tags.TestBodyIndicator.Teardown
open Archer.Arrows.Tests.Feature.Test.TestName.Tags.TestFunction
open Archer.Arrows.Tests.Feature.Test.TestName.TestBodyIndicator
open Archer.Arrows.Tests.Feature.Test.TestName.TestBodyIndicator.Teardown
open Archer.Arrows.Tests.Feature.Test.TestName.TestFunction
open Archer.Bow
open Archer.CoreTypes.InternalTypes
open Archer.CoreTypes.InternalTypes.RunnerTypes
open Archer.Logger.Summaries
open MicroLang.Lang

let private runner = bow.Runner ()

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
    
    // Feature.Test Tests
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
    ``017 - Feature Test with test name, tags, data, test function one parameter should``.``Test Cases``
    
    ``018 - Feature Test with test name, tags, test body indicator two parameters, teardown should``.``Test Cases``
    ``019 - Feature Test with test name, tags, test body indicator one parameter, teardown should``.``Test Cases``
    
    ``020 - Feature Test with test name, tags, test body indicator two parameters should``.``Test Cases``
    ``021 - Feature Test with test name, tags, test body indicator one parameter``.``Test Cases``
    
    ``022 - Feature Test with test name, tags, test function two parameters should``.``Test Cases``
    ``023 - Feature Test with test name, tags, test function one parameter should``.``Test Cases``
    
    ``024 - Feature Test with test name, setup, data, test body indicator three parameters, teardown should``.``Test Cases``
    ``025 - Feature Test with test name, setup, data, test body indicator two parameters, teardown should``.``Test Cases``
    
    ``026 - Feature Test with test name, setup, data, test body indicator three parameters``.``Test Cases``
    ``027 - Feature Test with test name, setup, data, test body indicator two parameters``.``Test Cases``
    
    ``028 - Feature Test with test name, setup, test body indicator two parameters, teardown should``.``Test Cases``
    ``029 - Feature Test with test name, setup, test body indicator one parameter, teardown should``.``Test Cases``
    
    ``030 - Feature Test with test name, setup, test body indicator two parameters should``.``Test Cases``
    ``031 - Feature Test with test name, setup, test body indicator one parameter should``.``Test Cases``
    
    ``032 - Feature Test with test name, data, test body indicator three parameters, teardown should``.``Test Cases``
    ``033 - Feature Test with test name, data, test body indicator two parameters, teardown should``.``Test Cases``
    ``034 - Feature Test with test name, data, test body indicator one parameter, teardown should``.``Test Cases``
    
    ``035 - Feature Test with test name, data, test body indicator three parameters should``.``Test Cases``
    ``036 - Feature Test with test name, data, test body indicator two parameters should``.``Test Cases``
    ``037 - Feature Test with test name, data, test body indicator one parameter should``.``Test Cases``
    
    ``038 - Feature Test with test name, data, test function three parameters should``.``Test Cases``
    ``039 - Feature Test with test name, data, test function two parameters should``.``Test Cases``
    ``040 - Feature Test with test name, data, test function one parameter should``.``Test Cases``
    
    ``041 - Feature Test with test name, test body indicator two parameters, teardown should``.``Test Cases``
    ``042 - Feature Test with test name, test body indicator one parameter, teardown should``.``Test Cases``
    
    ``043 - Feature Test with test name, test body indicator two parameters should``.``Test Cases``
    ``044 - Feature Test with test name, test body indicator one parameter should``.``Test Cases``
    
    ``045 - Feature Test with test name, test function two parameters should``.``Test Cases``
    ``046 - Feature Test with test name, test function one parameter should``.``Test Cases``
    
    ``047 - Feature Test with tags, setup, data, test body indicator three parameters, teardown should``.``Test Cases``
    ``048 - Feature Test with tags, setup, data, test body indicator two parameters, teardown should``.``Test Cases``
    
    ``049 - Feature Test with tags, setup, data, test body indicator three parameters should``.``Test Cases``
    ``050 - Feature Test with tags, setup, data, test body indicator two parameters should``.``Test Cases``
    
    ``051 - Feature Test with tags, setup, test body indicator two parameters, teardown should``.``Test Cases``
    ``052 - Feature Test with tags, setup, test body indicator one parameter, teardown should``.``Test Cases``
    
    ``053 - Feature Test with tags, setup, test body indicator two parameters should``.``Test Cases``
    ``054 - Feature Test with tags, setup, test body indicator one parameter should``.``Test Cases``
    
    ``055 - Feature Test with tags, data, test body indicator three parameters, teardown should``.``Test Cases``
    ``056 - Feature Test with tags, data, test body indicator two parameters, teardown should``.``Test Cases``
    ``057 - Feature Test with tags, data, test body indicator one parameter, teardown should``.``Test Cases``
    
    ``058 - Feature Test with tags, data, test body indicator two three parameters should``.``Test Cases``
    ``059 - Feature Test with tags, data, test body indicator two parameters should``.``Test Cases``
    ``060 - Feature Test with tags, data, test body indicator one parameter should``.``Test Cases``
    
    ``061 - Feature Test with tags, data, test function three parameters should``.``Test Cases``
    ``062 - Feature Test with tags, data, test function two parameters should``.``Test Cases``
    ``063 - Feature Test with tags, data, test function one parameter should``.``Test Cases``
    
    ``064 - Feature Test with tags, test body indicator two parameters, teardown should``.``Test Cases``
    ``065 - Feature Test with tags, test body indicator one parameter, teardown should``.``Test Cases``
    
    ``066 - Feature Test with tags, test body indicator two parameters should``.``Test Cases``
    ``067 - Feature Test with tags, test body indicator one parameter should``.``Test Cases``
    
    ``068 - Feature Test with tags, test function two parameters should``.``Test Cases``
    ``069 - Feature Test with tags, test function one parameter should``.``Test Cases``
    
    ``070 - Feature Test with setup, data, test body indicator three parameters, teardown``.``Test Cases``
    ``071 - Test Feature with setup, data, test body indicator two parameters, teardown should``.``Test Cases``
    
    ``072 - Feature Test with setup, data, test body indicator three parameters should``.``Test Cases``
    ``073 - Feature Test with setup, data, test body indicator two parameters should``.``Test Cases``
    
    ``074 - Feature Test with setup, test body indicator two parameters, teardown should``.``Test Cases``
    ``075 - Feature Test with setup, test body indicator one parameter, teardown should``.``Test Cases``
    
    ``076 - Feature Test with setup, test body indicator two parameters should``.``Test Cases``
    ``077 - Feature Test with setup, test body indicator one parameter should``.``Test Cases``
    
    ``078 - Feature Test with data, test body indicator three parameters, teardown should``.``Test Cases``
    ``079 - Feature Test with data, test body indicator two parameters, teardown should``.``Test Cases``
    ``080 - Feature Test with data, test body indicator one parameter, teardown should``.``Test Cases``
    
    ``081 - Feature Test with data, test body indicator three parameters should``.``Test Cases``
    ``082 - Feature Test with data, test body indicator two parameters should``.``Test Cases``
    ``083 - Feature Test with data, test body indicator one parameter should``.``Test Cases``
    
    ``084 - Feature Test with data, test function three parameters should``.``Test Cases``
    ``085 - Feature Test with data, test function two parameters should``.``Test Cases``
    ``086 - Feature Test with data, test function one parameter should``.``Test Cases``
    
    ``087 - Feature Test with test body indicator two parameters, teardown should``.``Test Cases``
    ``088 - Feature Test with test body indicator one parameter, teardown should``.``Test Cases``
    
    ``089 - Feature Test with test body indicator two parameters should``.``Test Cases``
    ``090 - Feature Test with test body indicator one parameter should``.``Test Cases``
    
    ``091 - Feature Test with test function two parameters should``.``Test Cases``
    ``092 - Feature Test with test function one parameter should``.``Test Cases``
    
    // Feature.Ignore Tests
    ``001 - Feature Ignore with test name, tags, setup, data, test body, teardown should``.``Test Cases``
    ``002 - Feature Ignore with test name, tags, setup, data, test body should``.``Test Cases``
    
    ``003 - Feature Ignore with test name, tags, setup, test body, teardown should``.``Test Cases``
    ``004 - Feature Ignore with test name, tags, setup, test body should``.``Test Cases``
    
    ``Arrow Ignore``.``Test Cases``
]
|> runAndReport
