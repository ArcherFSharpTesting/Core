
<!-- (dl
	(section-meta
	(title Using `Arrow.Ignore` in Archer.Arrow)
	)
) -->


`Arrow.Ignore` allows you to define features or tests that are intentionally skipped or ignored during test execution. This is useful for temporarily disabling tests or marking features as not yet implemented.

<!-- (dl (# Basic Usage)) -->

Ignore a feature by name:

```fsharp
let ignoredFeature = Arrow.Ignore "Feature To Ignore"
```

<!-- (dl (# With Path and Name)) -->

You can specify both a path and a name:

```fsharp
let ignoredFeature = Arrow.Ignore ("FeaturePath", "Feature To Ignore")
```

<!-- (dl (# With Setup and/or Teardown)) -->

You can provide setup and teardown functions, even for ignored features:

```fsharp
let ignoredFeature = Arrow.Ignore (
	"FeaturePath",
	"Feature To Ignore",
	Setup (fun () -> Ok ()),
	Teardown (fun _ -> Ok ())
)
```

Or just setup:

```fsharp
let ignoredFeature = Arrow.Ignore (
	"FeaturePath",
	"Feature To Ignore",
	Setup (fun () -> Ok ())
)
```

Or just teardown:

```fsharp
let ignoredFeature = Arrow.Ignore (
	"FeaturePath",
	"Feature To Ignore",
	Teardown (fun _ -> Ok ())
)
```

<!-- (dl (# With Tags)) -->

Add tags to ignored features for organization:

```fsharp
let ignoredFeature = Arrow.Ignore (
	"FeaturePath",
	"Feature To Ignore",
	TestTags [ Category "WIP"; Category "Integration" ]
)
```

<!-- (dl (# Minimal Example)) -->

```fsharp
let ignoredFeature = Arrow.Ignore "Temporarily Disabled Feature"
```

<!-- (dl (# Advanced Example)) -->

```fsharp
let ignoredFeature = Arrow.Ignore (
	"Path",
	"Ignored Feature",
	TestTags [ Category "Slow" ],
	Setup (fun () -> Ok ()),
	Teardown (fun _ -> Ok ())
)
```


<!-- (dl (# Interchangeability with `Arrow.NewFeature`)) -->

`Arrow.Ignore` and `Arrow.NewFeature` share the same call structure and overloads. This means you can swap one for the other with minimal code changes. For example, if you want to temporarily disable a feature, simply replace `Arrow.NewFeature` with `Arrow.Ignore` using the same arguments:

```fsharp
// Normal feature
let feature = Arrow.NewFeature ("Path", "Feature Name", Setup (fun () -> Ok ()), Teardown (fun _ -> Ok ()))

// Temporarily ignored feature
let feature = Arrow.Ignore ("Path", "Feature Name", Setup (fun () -> Ok ()), Teardown (fun _ -> Ok ()))
```

This design makes it easy to enable or disable features as needed, without changing the structure of your test code.

---

- Ignored features will not execute their tests.
- You can use `Arrow.Ignore` with the same overloads as `Arrow.NewFeature` (name, path, setup, teardown, tags).
- Useful for marking features as pending, under development, or temporarily disabled.

For more details, see the test scripts in `Test.Scripts/Arrow Ignore.fs` and related files.
