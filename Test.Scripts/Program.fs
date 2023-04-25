open Archer.Arrows.Tests.RawTestObjects
open Archer.Bow
open Archer
open MicroLang.Lang

let framework = bow.Framework ()

//(*
framework
|> addManyTests [
    ``Arrow Should``.``Test Cases``
    ``Feature Should``.``Test Cases``
    ``TestCase Should``.``Test Cases``
    ``TestCaseExecutor Should``.``Test Cases``
    ``TestCaseExecutor Execute Should``.``Test Cases``
    ``TestCaseExecutor Events Should``.``Test Cases``
    ``TestCaseExecutor Event Cancellation Should``.``Test Cases``
]
|> runAndReport
//*)
(*
framework
|> addTests [
    ``TestCaseExecutor Events Should``.``Not throw when TestStartTeardown throws``
]
|> runAndReport
//*)