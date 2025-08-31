
<!-- (dl
	(section-meta
	(title Using `FeatureBuilder.Ignore` in Archer.Core)
	)
) -->

FeatureBuilder.Ignore lets you temporarily disable features or tests in Archer.Core, keeping them visible and organized in your test suite. Use it to mark work-in-progress, pending, or intentionally skipped tests without deleting them.

<!-- (dl (# Basic Usage)) -->

Ignore a feature by name:

```fsharp
let ignoredFeature = FeatureBuilder.Ignore "Feature To Ignore"
```

<!-- (dl (# With Path and Name)) -->

You can specify both a path and a name:

```fsharp
let ignoredFeature = FeatureBuilder.Ignore ("FeaturePath", "Feature To Ignore")
```

<!-- (dl (# With Setup and/or Teardown)) -->

You can provide setup and teardown functions, even for ignored features:

```fsharp
let ignoredFeature = FeatureBuilder.Ignore (
	"FeaturePath",
	"Feature To Ignore",
	Setup (fun () -> Ok ()),
	Teardown (fun _ -> Ok ())
)
```

Or just setup:

```fsharp
let ignoredFeature = FeatureBuilder.Ignore (
	"FeaturePath",
	"Feature To Ignore",
	Setup (fun () -> Ok ())
)
```

Or just teardown:

```fsharp
let ignoredFeature = FeatureBuilder.Ignore (
	"FeaturePath",
	"Feature To Ignore",
	Teardown (fun _ -> Ok ())
)
```

<!-- (dl (# With Tags)) -->

Add tags to ignored features for organization:

```fsharp
let ignoredFeature = FeatureBuilder.Ignore (
	"FeaturePath",
	"Feature To Ignore",
	TestTags [ Category "WIP"; Category "Integration" ]
)
```

<!-- (dl (# Minimal Example)) -->

```fsharp
let ignoredFeature = FeatureBuilder.Ignore "Temporarily Disabled Feature"
```

<!-- (dl (# Advanced Example)) -->

```fsharp
let ignoredFeature = FeatureBuilder.Ignore (
	"Path",
	"Ignored Feature",
	TestTags [ Category "Slow" ],
	Setup (fun () -> Ok ()),
	Teardown (fun _ -> Ok ())
)
```


<!-- (dl (# Interchangeability with `FeatureBuilder.NewFeature`)) -->

`FeatureBuilder.Ignore` and `FeatureBuilder.NewFeature` share the same call structure and overloads. This means you can swap one for the other with minimal code changes. For example, if you want to temporarily disable a feature, simply replace `FeatureBuilder.NewFeature` with `FeatureBuilder.Ignore` using the same arguments:

```fsharp
// Normal feature
let feature = FeatureBuilder.NewFeature ("Path", "Feature Name", Setup (fun () -> Ok ()), Teardown (fun _ -> Ok ()))

// Temporarily ignored feature
let feature = FeatureBuilder.Ignore ("Path", "Feature Name", Setup (fun () -> Ok ()), Teardown (fun _ -> Ok ()))
```

This design makes it easy to enable or disable features as needed, without changing the structure of your test code.

---

- Ignored features will not execute their tests.
- You can use `FeatureBuilder.Ignore` with the same overloads as `FeatureBuilder.NewFeature` (name, path, setup, teardown, tags).
- Useful for marking features as pending, under development, or temporarily disabled.

For more details, see the test scripts in `Test.Scripts/Arrow Ignore.fs` and related files.
