/// <summary>
/// Helper functions and types for the Archer testing framework.
/// Provides utilities for test execution, setup/teardown management, and automatic test discovery.
/// </summary>
module Archer.Arrows.Helpers

open System
open System.Diagnostics
open Archer
open Archer.Arrows.Internals
open Archer.CoreTypes.InternalTypes

/// <summary>
/// A wrapper executor that manages nested setup/teardown operations and forwards lifecycle events.
/// This type enables composition of multiple setup/teardown layers while maintaining proper event propagation.
/// </summary>
/// <typeparam name="'outerInputType">The input type for the outer setup operation</typeparam>
/// <typeparam name="'outerOutputType">The output type from the outer setup that becomes input to the inner executor</typeparam>
type WrappedTeardownExecutor<'outerInputType, 'outerOutputType> (outerSetup: 'outerInputType -> Result<'outerOutputType, SetupTeardownFailure>, outerTeardown: Result<'outerOutputType, SetupTeardownFailure> -> TestResult option -> Result<unit, SetupTeardownFailure>, inner: ISetupTeardownExecutor<'outerOutputType>) as this =
    inherit SetupTeardownExecutor<'outerInputType, 'outerOutputType> (outerSetup, outerTeardown, inner.Execute)
    
    /// <summary>
    /// Initialize the wrapper by subscribing to the inner executor's lifecycle events
    /// </summary>
    do
        let executor = inner
        executor.LifecycleEvent.AddHandler (fun sender args ->
            this.Trigger (sender, args)
        )
    
    /// <summary>
    /// Override the trigger method to control which lifecycle events are propagated based on sender.
    /// This ensures proper event ordering and prevents duplicate events during nested execution.
    /// </summary>
    /// <param name="sender">The object that triggered the event</param>
    /// <param name="args">The lifecycle event arguments</param>
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
                
    /// <summary>
    /// Create a clone of this wrapped executor with the same configuration
    /// </summary>
    /// <returns>A new instance of the wrapped executor</returns>
    override _.Clone () =
        WrappedTeardownExecutor<'outerInputType, 'outerOutputType> (outerSetup, outerTeardown, inner) :> ISetupTeardownExecutor<'outerInputType>

    /// <summary>
    /// Provides access to this instance as an ISetupTeardownExecutor interface
    /// </summary>
    member this.AsSetupTeardownExecutor with get () = this :> ISetupTeardownExecutor<'outerInputType>

/// <summary>
/// Transforms feature setup and teardown indicators into a complete test case.
/// This function creates the bridge between high-level test definitions and low-level execution.
/// </summary>
/// <typeparam name="'featureType">The type used for feature setup/teardown context</typeparam>
/// <typeparam name="'a">Additional type parameter for flexibility</typeparam>
/// <param name="featureSetup">The setup operation for the feature</param>
/// <param name="featureTeardown">The teardown operation for the feature</param>
/// <param name="internals">Test metadata including names, tags, and location information</param>
/// <param name="inner">The inner executor that will run the actual test</param>
/// <returns>A configured test case ready for execution</returns>
let baseTransformer<'featureType, 'a> (featureSetup: SetupIndicator<unit, 'featureType>) (featureTeardown: TeardownIndicator<'featureType>) (internals: TestInternals, inner: ISetupTeardownExecutor<'featureType>) =
    let (Setup setup) = featureSetup
    let (Teardown teardown) = featureTeardown
        
    let executor = WrappedTeardownExecutor<unit,'featureType> (setup, teardown, inner)
    TestCase (internals.ContainerPath, internals.ContainerName, internals.TestName, executor, internals.Tags, internals.FilePath, internals.FileName, internals.LineNumber) :> ITest
    
/// <summary>
/// Removes escape sequences from strings, specifically handling escaped commas.
/// This is used when processing test names and paths that may contain special characters.
/// </summary>
/// <param name="value">The string to process</param>
/// <returns>The string with escape sequences removed</returns>
let removeEscapes (value: string) =
    value.Replace ("\\,", ",")

/// <summary>
/// Extracts the container name and path from a specific stack frame.
/// This function uses reflection to automatically determine test organization from the call stack.
/// </summary>
/// <param name="frame">The stack frame number to examine (0 is current method, 1 is caller, etc.)</param>
/// <returns>A tuple containing (containerName, containerPath)</returns>
let getNamesAt frame = 
    let trace = StackTrace ()
    let method = trace.GetFrame(frame).GetMethod ()
    let containerName = method.ReflectedType.Name |> removeEscapes
    let containerPath = method.ReflectedType.Namespace |> fun s -> s.Split ([|"$"|], StringSplitOptions.RemoveEmptyEntries) |> Array.last |> removeEscapes
            
    containerName, containerPath

/// <summary>
/// Gets the container name and path from the calling context.
/// Uses a fixed stack frame depth (3) to reach the actual test definition method.
/// Frame 0: getNames, Frame 1: getNamesAt, Frame 2: calling helper, Frame 3: test definition
/// </summary>
/// <returns>A tuple containing (containerName, containerPath) for the test</returns>
let getNames () =
    getNamesAt 3