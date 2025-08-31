<!-- (dl
(section-meta
    (title Using `FeatureBuilder.NewFeature` in Archer.Core)
)
) -->

FeatureBuilder.NewFeature is the primary entry point for defining features in Archer.FeatureBuilder. It lets you organize tests into logical groups, apply setup/teardown logic, and add tags for filtering. This flexible function supports a variety of overloads, allowing you to specify feature names, paths, setup and teardown logic, and tags.

<!-- (dl (# Basic Usage)) -->

Create a feature with just a name:

```fsharp
let feature = FeatureBuilder.NewFeature "My Feature Name"
```

<!-- (dl (# With Path and Name)) -->

Specify both a path and a name for the feature:

```fsharp
let feature = FeatureBuilder.NewFeature ("MyFeaturePath", "My Feature Name")
```

<!-- (dl (# With Setup and/or Teardown)) -->

You can provide setup and teardown functions to run before and after your tests:

```fsharp
let feature = FeatureBuilder.NewFeature (
    "MyFeaturePath",
    "My Feature Name",
    Setup (fun () -> Ok ()),
    Teardown (fun _ -> Ok ())
)
```

Or just setup:

```fsharp
let feature = FeatureBuilder.NewFeature (
    "MyFeaturePath",
    "My Feature Name",
    Setup (fun () -> Ok ())
)
```

Or just teardown:

```fsharp
let feature = FeatureBuilder.NewFeature (
    "MyFeaturePath",
    "My Feature Name",
    Teardown (fun _ -> Ok ())
)
```

<!-- (dl (# With Tags)) -->

You can add tags to your feature for filtering and organization:

```fsharp
let feature = FeatureBuilder.NewFeature (
    "MyFeaturePath",
    "My Feature Name",
    TestTags [ Category "Integration"; Category "Slow" ]
)
```

<!-- (dl (# Minimal Example)) -->

```fsharp
let feature = FeatureBuilder.NewFeature "Simple Feature"

feature.Test (fun _ ->
    // Your test code here
    TestSuccess
)
```

<!-- (dl (# Advanced Example)) -->

```fsharp
let feature = FeatureBuilder.NewFeature (
    "Path",
    "Advanced Feature",
    TestTags [ Category "Unit" ],
    Setup (fun () -> Ok ()),
    Teardown (fun _ -> Ok ())
)

feature.Test (fun _ ->
    // Your test code here
    TestSuccess
)
```

<!-- (dl (# Notes)) -->
- `Setup` and `Teardown` are helpers for resource management.
- `TestTags` and `Category` help organize and filter tests.
- You can use the feature's `Test` method to define individual tests.

For more details, see the test scripts in `Test.Scripts/Arrow NewFeature.fs`, `Arrow NewFeature With Setup.fs`, and `Arrow NewFeature With Teardown.fs`.
