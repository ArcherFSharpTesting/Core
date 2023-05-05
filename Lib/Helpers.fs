module Archer.Arrows.Helpers

open System
open System.Diagnostics
open Archer
open Archer.Arrows.Internals
open Archer.CoreTypes.InternalTypes

type WrappedTeardownExecutor<'outerInputType, 'outerOutputType> (outerSetup: 'outerInputType -> Result<'outerOutputType, SetupTeardownFailure>, outerTeardown: Result<'outerOutputType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>, inner: ISetupTeardownExecutor<'outerOutputType>) as this =
    inherit SetupTeardownExecutor<'outerInputType, 'outerOutputType> (outerSetup, outerTeardown, inner.Execute)
    
    do
        let executor = inner
        executor.LifecycleEvent.AddHandler (fun sender args ->
            this.Trigger (sender, args)
        )
    
    override this.Trigger (sender: obj, args: SetupTeardownExecutorLifecycleEventArgs) =
        match args with
        | ExecuteSetupStart _ ->
            if sender = this then
                base.Trigger (this, args)
        | ExecuteSetupEnd _ ->
            if sender <> this then
                base.Trigger (this, args)
        | ExecuteStartTeardown ->
            if sender = this then
                base.Trigger (this, args)
        | ExecuteRunner _ ->
            if sender <> this then
                base.Trigger (this, args)
        | ExecuteRunnerEnd _ ->
            if sender = this then
                base.Trigger (this, args)
                
    override _.Clone () =
        WrappedTeardownExecutor<'outerInputType, 'outerOutputType> (outerSetup, outerTeardown, inner) :> ISetupTeardownExecutor<'outerInputType>

    member this.AsSetupTeardownExecutor with get () = this :> ISetupTeardownExecutor<'outerInputType>

let baseTransformer<'featureType, 'a> (featureSetup: SetupIndicator<unit, 'featureType>) (featureTeardown: TeardownIndicator<'featureType>) (internals: TestInternals, inner: ISetupTeardownExecutor<'featureType>) =
    let (Setup setup) = featureSetup
    let (Teardown teardown) = featureTeardown
        
    let executor = WrappedTeardownExecutor<unit,'featureType> (setup, teardown, inner)
    TestCase (internals.ContainerPath, internals.ContainerName, internals.TestName, executor, internals.Tags, internals.FilePath, internals.FileName, internals.LineNumber) :> ITest

let getNamesAt frame = 
    let trace = StackTrace ()
    let method = trace.GetFrame(frame).GetMethod ()
    let containerName = method.ReflectedType.Name
    let containerPath = method.ReflectedType.Namespace |> fun s -> s.Split ([|"$"|], StringSplitOptions.RemoveEmptyEntries) |> Array.last
            
    containerName, containerPath

let getNames () =
    getNamesAt 3