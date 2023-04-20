[<AutoOpen>]
module Archer.Arrows.Tests.ExecuteHelper

open Archer.CoreTypes.InternalTypes

let executeFunction (executor: ITestExecutor) =
    let run () =
        executor.Execute (getFakeEnvironment ())
        
    run
    
let runIt f = f ()