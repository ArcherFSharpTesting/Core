open Archer.Arrows.Tests.RawTestObjects
open Archer.Bow
open Archer
open Archer.CoreTypes.InternalTypes
open Archer.CoreTypes.InternalTypes.FrameworkTypes
open MicroLang.Lang

let framework = bow.Framework ()

let testFilter (_test: ITest) =
    true
    
framework.FrameworkLifecycleEvent
|> Event.add (fun args ->
    match args with
    | FrameworkStartExecution _ ->
        printfn ""
    | FrameworkTestLifeCycle (test, testEventLifecycle, _) ->
        match testEventLifecycle with
        | TestEndExecution testExecutionResult ->
            let successMsg =
                match testExecutionResult with
                | TestExecutionResult TestSuccess -> "Success"
                | _ -> "Fail"
                
            let report = $"%A{test} : (%s{successMsg})"
            printfn $"%s{report}"
        | _ -> ()
    | FrameworkEndExecution ->
        printfn "\n"
)

framework
|> addManyTests [
    ``Arrow NewFeature``.``Test Cases``
    ``Feature Should``.``Test Cases``
    ``TestCase Should``.``Test Cases``
    ``TestCaseExecutor Should``.``Test Cases``
    ``TestCaseExecutor Execute Should``.``Test Cases``
    ``TestCaseExecutor Events Should``.``Test Cases``
    ``TestCaseExecutor Event Cancellation Should``.``Test Cases``
]
|> filterRunAndReport testFilter
