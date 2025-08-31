<!-- (dl
(section-meta
    (title Using `Sub.Feature` in Archer.Core)
)
) -->

Sub.Feature lets you create nested or child features under a root feature, enabling you to organize your Archer.Core tests into a clear hierarchy. Use it to group related scenarios, manage complex test suites, and apply setup, teardown, or tags at different levels of your test structure.

<!-- (dl (# Basic Usage)) -->

You use `Sub.Feature` by piping a root feature into it:

```fsharp
let rootFeature = FeatureBuilder.NewFeature "Root Feature"

let subFeature =
    rootFeature
    |> Sub.Feature "Sub Feature Name"
```


<!-- (dl (# With Path and Name)) -->

You can specify both a path and a name for the sub-feature. When you provide a sub-path, the sub-feature's path becomes `{parentPath}.{subPath}` where `parentPath` is the path of the root feature:

```fsharp
let subFeature =
    rootFeature
    |> Sub.Feature ("SubFeaturePath", "Sub Feature Name")
// The resulting path will be: "RootFeaturePath.SubFeaturePath"
```

<!-- (dl (# With Setup and/or Teardown)) -->

You can provide setup and teardown logic for sub-features as well:

```fsharp
let subFeature =
    rootFeature
    |> Sub.Feature (
        "SubFeaturePath",
        "Sub Feature Name",
        Setup (fun () -> Ok ()),
        Teardown (fun _ -> Ok ())
    )
```

When you provide setup logic to the sub feature, the parent's setup logic will be called first and then the sub feature's setup logic will be called.

When you provide teardown logic to the sub feature, the sub feature's teardown logic will be called first followed by the parent's.

<!-- (dl (# With Tags)) -->

Add tags to sub-features for filtering and organization:

```fsharp
let subFeature =
    rootFeature
    |> Sub.Feature (
        "SubFeaturePath",
        "Sub Feature Name",
        TestTags [ Category "Integration" ]
    )
```

<!-- (dl (# Minimal Example)) -->

```fsharp
let subFeature =
    rootFeature
    |> Sub.Feature "Child Feature"
```

<!-- (dl (# Notes)) -->
- The call pattern is always `rootFeature |> Sub.Feature ...`.
- Sub-features inherit context from their parent/root feature.
- You can use the same overloads as with `FeatureBuilder.NewFeature` (name, path, setup, teardown, tags).
- Useful for organizing large test suites into logical groups.

For more details, see the test scripts and the implementation in `Lib/Sub.fs`.
