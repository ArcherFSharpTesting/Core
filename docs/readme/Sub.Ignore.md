
<!-- (dl
(section-meta
    (title Using `Sub.Ignore` in Archer.Core)
)
) -->

`Sub.Ignore` allows you to define ignored (skipped) sub-features under a root feature in the Archer F# testing framework. This is useful for temporarily disabling specific sub-features or marking them as not yet implemented, while keeping them organized under their parent feature.

<!-- (dl (# Basic Usage)) -->

You use `Sub.Ignore` by piping a root feature into it:

```fsharp
let rootFeature = FeatureBuilder.NewFeature "Root Feature"

let ignoredSubFeature =
    rootFeature
    |> Sub.Ignore "Sub Feature To Ignore"
```

<!-- (dl (# With Path and Name)) -->

You can specify both a sub-path and a name. The sub-feature's path will be `{parentPath}.{subPath}`:

```fsharp
let ignoredSubFeature =
    rootFeature
    |> Sub.Ignore ("SubFeaturePath", "Sub Feature To Ignore")
// The resulting path will be: "RootFeaturePath.SubFeaturePath"
```

<!-- (dl (# With Setup and/or Teardown)) -->

You can provide setup and teardown logic for ignored sub-features as well:

```fsharp
let ignoredSubFeature =
    rootFeature
    |> Sub.Ignore (
        "SubFeaturePath",
        "Sub Feature To Ignore",
        Setup (fun () -> Ok ()),
        Teardown (fun _ -> Ok ())
    )
```

<!-- (dl (# With Tags)) -->

Add tags to ignored sub-features for filtering and organization:

```fsharp
let ignoredSubFeature =
    rootFeature
    |> Sub.Ignore (
        "SubFeaturePath",
        "Sub Feature To Ignore",
        TestTags [ Category "WIP" ]
    )
```

<!-- (dl (# Minimal Example)) -->

```fsharp
let ignoredSubFeature =
    rootFeature
    |> Sub.Ignore "Temporarily Disabled Sub-Feature"
```

<!-- (dl (# Notes)) -->
- The call pattern is always `rootFeature |> Sub.Ignore ...`.
- Sub.Ignore shares the same call structure and overloads as `Sub.Feature`, so you can easily swap between them as needed.
- When you provide a sub-feature setup, the call order is: root setup runs first, then sub-feature setup. For teardown, the order is reversed: sub-feature teardown runs first, then root teardown.
- Ignored sub-features will not execute their tests.
- Useful for marking sub-features as pending, under development, or temporarily disabled.

For more details, see the implementation in `Lib/Sub.fs` and related test scripts.
