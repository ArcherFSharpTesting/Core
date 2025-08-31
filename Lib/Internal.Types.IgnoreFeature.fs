namespace Archer.Core.Internal.Types

open Archer
open Archer.Core
open Archer.Core.Internals
open Archer.CoreTypes.InternalTypes
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
        
type IgnoreFeature<'featureType> (featurePath, featureName, featureTags: TestTag list, transformer: TestInternals * ISetupTeardownExecutor<'featureType> -> ITest, featureLocation: CodeLocation) =
    inherit Feature<'featureType> (featurePath, featureName, featureTags, transformer)
    override this.Test (tags: TagsIndicator, _setup: SetupIndicator<'featureType, 'setupType>, _testBody: TestBodyIndicator<TestFunctionTwoParameters<'setupType, TestEnvironment>>, _teardown: TeardownIndicator<'setupType>, [<CallerMemberName; Optional; DefaultParameterValue("")>] testName: string, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let location = getLocation fileFullName lineNumber
        
        let testBody _ _ = TestIgnored (None, featureLocation) |> TestFailure
        
        let test =
            let (TestTags tags) = tags
            let inner = SetupTeardownExecutor (Ok, (fun _ _ -> Ok ()), fun value env -> env |> testBody value |> TestExecutionResult) :> ISetupTeardownExecutor<'featureType>
            transformer ({ ContainerPath = featurePath; ContainerName = featureName; TestName = testName; Tags = [tags; featureTags] |> List.concat; FilePath = location.FilePath; FileName = location.FileName; LineNumber = lineNumber }, inner)
        
        this.AddTest test