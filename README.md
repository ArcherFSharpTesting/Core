<!-- GENERATED DOCUMENT DO NOT EDIT! -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->

<!-- Compiled with doculisp https://www.npmjs.com/package/doculisp -->

# Archer.Arrow F# Testing Language #

1. Overview: [Archer Test Framework Overview](#archer-test-framework-overview)
2. Feature: [Using Arrow.NewFeature in Archer.Arrow](#using-arrownewfeature-in-archerarrow)
3. Feature: [Using `Arrow.Ignore` in Archer.Arrow](#using-arrowignore-in-archerarrow)

## Archer Test Framework Overview ##

Archer is a modern, idiomatic F# test framework designed for clarity, composability, and expressiveness. It enables you to write highly readable tests with minimal boilerplate, leveraging F#'s language features for a natural test authoring experience.

### Key Features ###

- **Descriptive Test Names**: Test names are automatically inferred from the identifier, allowing for natural language descriptions.
- **Composable Features**: Organize tests into features and sub-features for clear structure and reuse.
- **Flexible Setup/Teardown**: Support for setup and teardown logic at any feature or sub-feature level.
- **Data-Driven Testing**: Easily define tests that run over sequences of data.
- **Tagging and Filtering**: Use tags to categorize and filter tests.
- **Ignore Support**: Temporarily skip tests or features with a single keyword.

### Basic Example ###

```fsharp
let feature = Arrow.NewFeature "Math Feature"

let ``Addition should work`` =
    feature.Test (fun _ ->
        let result = 2 + 2
        if result = 4 then TestSuccess else TestFailure "Addition failed"
    )
```

### Organizing Tests ###

- Use `Arrow.NewFeature` to define a feature.
- Use `Sub.Feature` to create sub-features under a parent feature.
- Assign tests to identifiers for automatic naming.

### Setup and Teardown ###

Setup logic in Archer is run before each test. Instead of relying on global variables, the result of the setup function is passed directly to the test as a parameter. This makes tests more predictable and easier to reason about.

```fsharp
let feature = Arrow.NewFeature (
    Setup (fun () -> Ok (setupValue)),
    Teardown (fun setupValue -> Ok ())
)

let ``A test using setup`` =
    feature.Test (fun setupValue ->
        // Use setupValue directly in your test
        TestSuccess
    )
```

### Ignoring Tests ###

```fsharp
let ``A test to skip`` = feature.Ignore (fun _ -> ())
```

### Data-Driven Tests ###

```fsharp
let ``Test with data`` =
    feature.Test (
        Data [1; 2; 3],
        TestBody (fun data _ ->
            // Test logic using 'data'
            TestSuccess
        )
    )
```

### Tags and Filtering ###

```fsharp
let ``A tagged test`` =
    feature.Test (
        TestTags [ Category "Integration" ],
        fun _ -> TestSuccess
    )
```

### Getting Started ###

1. Define a feature with `Arrow.NewFeature`.
2. Add tests using the `Test` method, assigning them to identifiers.
3. Use setup, teardown, data, and tags as needed.
4. Run your tests with your preferred test runner.

For more details, see the API documentation and example files in the repository.

## Using Arrow.NewFeature in Archer.Arrow ##

`Arrow.NewFeature` is a flexible function for defining test features in the Archer F# testing framework. It supports a variety of overloads, allowing you to specify feature names, paths, setup and teardown logic, and tags. Below are the most common usage patterns.

### Basic Usage ###

Create a feature with just a name:

```fsharp
let feature = Arrow.NewFeature "My Feature Name"
```

### With Path and Name ###

Specify both a path and a name for the feature:

```fsharp
let feature = Arrow.NewFeature ("MyFeaturePath", "My Feature Name")
```

### With Setup and/or Teardown ###

You can provide setup and teardown functions to run before and after your tests:

```fsharp
let feature = Arrow.NewFeature (
    "MyFeaturePath",
    "My Feature Name",
    Setup (fun () -> Ok ()),
    Teardown (fun _ -> Ok ())
)
```

Or just setup:

```fsharp
let feature = Arrow.NewFeature (
    "MyFeaturePath",
    "My Feature Name",
    Setup (fun () -> Ok ())
)
```

Or just teardown:

```fsharp
let feature = Arrow.NewFeature (
    "MyFeaturePath",
    "My Feature Name",
    Teardown (fun _ -> Ok ())
)
```

### With Tags ###

You can add tags to your feature for filtering and organization:

```fsharp
let feature = Arrow.NewFeature (
    "MyFeaturePath",
    "My Feature Name",
    TestTags [ Category "Integration"; Category "Slow" ]
)
```

### Minimal Example ###

```fsharp
let feature = Arrow.NewFeature "Simple Feature"

feature.Test (fun _ ->
    // Your test code here
    TestSuccess
)
```

### Advanced Example ###

```fsharp
let feature = Arrow.NewFeature (
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

### Notes ###

- `Setup` and `Teardown` are helpers for resource management.
- `TestTags` and `Category` help organize and filter tests.
- You can use the feature's `Test` method to define individual tests.

For more details, see the test scripts in `Test.Scripts/Arrow NewFeature.fs`, `Arrow NewFeature With Setup.fs`, and `Arrow NewFeature With Teardown.fs`.

## Using `Arrow.Ignore` in Archer.Arrow ##

`Arrow.Ignore` allows you to define features or tests that are intentionally skipped or ignored during test execution. This is useful for temporarily disabling tests or marking features as not yet implemented.

### Basic Usage ###

Ignore a feature by name:

```fsharp
let ignoredFeature = Arrow.Ignore "Feature To Ignore"
```

### With Path and Name ###

You can specify both a path and a name:

```fsharp
let ignoredFeature = Arrow.Ignore ("FeaturePath", "Feature To Ignore")
```

### With Setup and/or Teardown ###

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

### With Tags ###

Add tags to ignored features for organization:

```fsharp
let ignoredFeature = Arrow.Ignore (
	"FeaturePath",
	"Feature To Ignore",
	TestTags [ Category "WIP"; Category "Integration" ]
)
```

### Minimal Example ###

```fsharp
let ignoredFeature = Arrow.Ignore "Temporarily Disabled Feature"
```

### Advanced Example ###

```fsharp
let ignoredFeature = Arrow.Ignore (
	"Path",
	"Ignored Feature",
	TestTags [ Category "Slow" ],
	Setup (fun () -> Ok ()),
	Teardown (fun _ -> Ok ())
)
```

### Interchangeability with `Arrow.NewFeature` ###

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

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->
<!-- GENERATED DOCUMENT DO NOT EDIT! -->