namespace Archer.Arrows.Internal.Types

open Archer.Arrows.Internals

type IBuilder<'a, 'b> =
    abstract member Add: internals: TestInternals * executor: ISetupTeardownExecutor<'a> -> 'b
    
