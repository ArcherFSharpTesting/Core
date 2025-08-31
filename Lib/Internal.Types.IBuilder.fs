namespace Archer.Core.Internal.Types

open Archer.Core.Internals

/// <summary>
/// Defines a builder interface for creating test-related objects with setup and teardown execution capabilities.
/// This interface provides a contract for building objects of type 'b using test internals and setup/teardown execution.
/// </summary>
/// <typeparam name="'a">The type parameter for the setup/teardown executor</typeparam>
/// <typeparam name="'b">The type of object that will be built and returned by the Add method</typeparam>
type IBuilder<'a, 'b> =
    /// <summary>
    /// Adds test internals and a setup/teardown executor to build an object of type 'b.
    /// This method combines the test configuration and execution context to produce the final result.
    /// </summary>
    /// <param name="internals">The test internals containing configuration and metadata for the test</param>
    /// <param name="executor">The setup and teardown executor responsible for managing test lifecycle operations</param>
    /// <returns>An object of type 'b representing the built test-related construct</returns>
    abstract member Add: internals: TestInternals * executor: ISetupTeardownExecutor<'a> -> 'b
    
