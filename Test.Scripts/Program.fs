open Archer.Arrow.Tests.RawTestObjects
open Archer.Bow
open Archer
open MicroLang.Lang

let framework = bow.Framework ()

framework
|> addManyTests [
    ``TestCase Should``.``Test Cases``
    ``TestCaseExecutor Execute Should``.``Test Cases``
]
|> runAndReport
