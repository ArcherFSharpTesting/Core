namespace Archer.Arrows.Internal.Types

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer
open Archer.Arrows
open Archer.Arrows.Internals
open Archer.CoreTypes.InternalTypes
        
type IgnoreFeature<'featureType> (featurePath, featureName, transformer: TestInternals * ISetupTeardownExecutor<'featureType> -> ITest, featureLocation: CodeLocation) =
    inherit Feature<'featureType> (featurePath, featureName, transformer)
    override this.Test (tags: TagsIndicator, _setup: SetupIndicator<'featureType, 'setupType>, _testBody: TestBodyWithEnvironmentIndicator<'setupType>, _teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let location = getLocation fileFullName lineNumber
        
        let testBody _ _ = TestIgnored (None, featureLocation) |> TestFailure
        
        let test =
            let (TestTags tags) = tags
            let inner = SetupTeardownExecutor (Ok, (fun _ _ -> Ok ()), fun value env -> env |> testBody value |> TestExecutionResult) :> ISetupTeardownExecutor<'featureType>
            transformer ({ ContainerPath = featurePath; ContainerName = featureName; TestName = testName; Tags = tags; FilePath = location.FilePath; FileName = location.FileName; LineNumber = lineNumber }, inner)
        
        this.AddTest test