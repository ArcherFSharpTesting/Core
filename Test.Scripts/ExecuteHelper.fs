[<AutoOpen>]
module Archer.Core.Tests.ExecuteHelper

open Archer.CoreTypes.InternalTypes

let executeFunction (executor: ITestExecutor) =
    let run () =
        executor.Execute (getFakeEnvironment ())
        
    run
    
let runIt f = f ()

let runExecutor (test: ITestExecutor) =
    test
    |> executeFunction
    |> runIt

let runTest (test: ITest) =
    test
    |> getTestExecutor
    |> runExecutor
    
let runAllTests (tests: ITest list) =
    tests
    |> List.map runTest
    
let silentlyRunExecutor (test: ITestExecutor) =
    test
    |> runExecutor
    |> ignore
    
let silentlyRunTest (test: ITest) =
    test
    |> runTest
    |> ignore
    
let silentlyRunAllTests (tests: ITest list) =
    tests
    |> List.map runTest
    |> ignore