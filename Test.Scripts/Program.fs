open Archer.Arrow.Tests.TestCase
open Archer.Bow
open Archer
open MicroLang.Lang

let framework = bow.Framework ()

framework
|> addManyTests [
    Should.``Test Cases``
]
|> runAndReport
