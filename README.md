<!-- GENERATED DOCUMENT DO NOT EDIT! -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->

<!-- Compiled with doculisp https://www.npmjs.com/package/doculisp -->

# Archer.Arrow F# Testing Language #

1. Overview: [Archer Test Framework Overview](#archer-test-framework-overview)
2. Feature: [Using Arrow.NewFeature in Archer.Arrow](#using-arrownewfeature-in-archerarrow)
3. Feature: [Using `Arrow.Ignore` in Archer.Arrow](#using-arrowignore-in-archerarrow)
4. Feature: [Using `Sub.Feature` in Archer.Arrow](#using-subfeature-in-archerarrow)
5. Feature: [Using `Sub.Ignore` in Archer.Arrow](#using-subignore-in-archerarrow)
6. Feature: [Using `IFeature.Test` in Archer.Arrow](#using-ifeaturetest-in-archerarrow)
7. Feature: [Using `IFeature.Ignore` in Archer.Arrow](#using-ifeatureignore-in-archerarrow)

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

Arrow.NewFeature is the primary entry point for defining features in Archer.Arrow. It lets you organize tests into logical groups, apply setup/teardown logic, and add tags for filtering. This flexible function supports a variety of overloads, allowing you to specify feature names, paths, setup and teardown logic, and tags.

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

Arrow.Ignore lets you temporarily disable features or tests in Archer.Arrow, keeping them visible and organized in your test suite. Use it to mark work-in-progress, pending, or intentionally skipped tests without deleting them.

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

## Using `Sub.Feature` in Archer.Arrow ##

Sub.Feature lets you create nested or child features under a root feature, enabling you to organize your Archer.Arrow tests into a clear hierarchy. Use it to group related scenarios, manage complex test suites, and apply setup, teardown, or tags at different levels of your test structure.

### Basic Usage ###

You use `Sub.Feature` by piping a root feature into it:

```fsharp
let rootFeature = Arrow.NewFeature "Root Feature"

let subFeature =
    rootFeature
    |> Sub.Feature "Sub Feature Name"
```

### With Path and Name ###

You can specify both a path and a name for the sub-feature. When you provide a sub-path, the sub-feature's path becomes `{parentPath}.{subPath}` where `parentPath` is the path of the root feature:

```fsharp
let subFeature =
    rootFeature
    |> Sub.Feature ("SubFeaturePath", "Sub Feature Name")
// The resulting path will be: "RootFeaturePath.SubFeaturePath"
```

### With Setup and/or Teardown ###

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

### With Tags ###

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

### Minimal Example ###

```fsharp
let subFeature =
    rootFeature
    |> Sub.Feature "Child Feature"
```

### Notes ###

- The call pattern is always `rootFeature |> Sub.Feature ...`.
- Sub-features inherit context from their parent/root feature.
- You can use the same overloads as with `Arrow.NewFeature` (name, path, setup, teardown, tags).
- Useful for organizing large test suites into logical groups.

For more details, see the test scripts and the implementation in `Lib/Sub.fs`.

## Using `Sub.Ignore` in Archer.Arrow ##

`Sub.Ignore` allows you to define ignored (skipped) sub-features under a root feature in the Archer F# testing framework. This is useful for temporarily disabling specific sub-features or marking them as not yet implemented, while keeping them organized under their parent feature.

### Basic Usage ###

You use `Sub.Ignore` by piping a root feature into it:

```fsharp
let rootFeature = Arrow.NewFeature "Root Feature"

let ignoredSubFeature =
    rootFeature
    |> Sub.Ignore "Sub Feature To Ignore"
```

### With Path and Name ###

You can specify both a sub-path and a name. The sub-feature's path will be `{parentPath}.{subPath}`:

```fsharp
let ignoredSubFeature =
    rootFeature
    |> Sub.Ignore ("SubFeaturePath", "Sub Feature To Ignore")
// The resulting path will be: "RootFeaturePath.SubFeaturePath"
```

### With Setup and/or Teardown ###

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

### With Tags ###

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

### Minimal Example ###

```fsharp
let ignoredSubFeature =
    rootFeature
    |> Sub.Ignore "Temporarily Disabled Sub-Feature"
```

### Notes ###

- The call pattern is always `rootFeature |> Sub.Ignore ...`.
- Sub.Ignore shares the same call structure and overloads as `Sub.Feature`, so you can easily swap between them as needed.
- When you provide a sub-feature setup, the call order is: root setup runs first, then sub-feature setup. For teardown, the order is reversed: sub-feature teardown runs first, then root teardown.
- Ignored sub-features will not execute their tests.
- Useful for marking sub-features as pending, under development, or temporarily disabled.

For more details, see the implementation in `Lib/Sub.fs` and related test scripts.

## Using `IFeature.Test` in Archer.Arrow ##

The `IFeature<'featureType>.Test` method is the primary way to define tests for a feature in the Archer F# testing framework. It provides a flexible API for specifying test names, tags, setup/teardown logic, test data, and test bodies.

### Automatic Test Naming ###

The most idiomatic way to use `Test` is to assign its result to an identifier. The name of the identifier (including spaces and punctuation) is automatically used as the test's name. This allows for highly readable and descriptive test names without repeating yourself:

```fsharp
let ``A test of basic functionality`` =
    feature.Test (fun _ ->
        // Your test logic here
        TestSuccess
    )
```

In this example, the test's name will be "A test of basic functionality".

### Basic Usage (Explicit Name) ###

You can also specify the test name explicitly as the first argument:

```fsharp
feature.Test (
    "Test Name",
    TestTags [ Category "Unit" ],
    Setup (fun () -> Ok ()),
    TestBody (fun setup env ->
        // Your test logic here
        TestSuccess
    ),
    Teardown (fun setup -> Ok ())
)
```

### Overloads ###

`IFeature.Test` is highly overloaded to support many combinations:
- With or without tags
- With or without setup/teardown
- With or without test data
- With different test body signatures (accepting setup, environment, data, etc.)

For example, you can write a minimal test:

```fsharp
feature.Test (
    "Simple Test",
    TestBody (fun _ _ -> TestSuccess)
)
```

Or a data-driven test:

```fsharp
feature.Test (
    "Data Test",
    TestTags [ Category "Property" ],
    Setup (fun () -> Ok ()),
    Data [1; 2; 3],
    TestBody (fun data _setup _env ->
        // Test logic using 'data'
        TestSuccess
    ),
    Teardown (fun _ -> Ok ())
)
```

### Parameters ###

- **testName**: The name of the test (string).
- **tags**: Optional tags for filtering and organization (`TestTags [...]`).
- **setup**: Optional setup function to prepare resources before the test.
- **data**: Optional test data for data-driven tests.
- **testBody**: The function containing the test logic. Signature depends on overload.
- **teardown**: Optional teardown function to clean up after the test.

### Writing a Test ###

1. Create a feature (e.g., with `Arrow.NewFeature`).
2. Call `feature.Test` with the desired parameters.
3. Implement your test logic in the test body function.
4. Return `TestSuccess` or another result from the test body.

### Example ###

```fsharp
let feature = Arrow.NewFeature "Math Feature"

feature.Test (
    "Addition should work",
    TestTags [ Category "Math" ],
    Setup (fun () -> Ok ()),
    TestBody (fun _ _ ->
        let result = 2 + 2
        if result = 4 then TestSuccess else TestFailure "Addition failed"
    ),
    Teardown (fun _ -> Ok ())
)
```

### Notes ###

- The test body can access setup values, test data, and the test environment depending on the overload.
- Use tags and setup/teardown as needed for your scenario.
- See the `Internal.Types.IFeature.fs` file for all available overloads and signatures.

## Using `IFeature.Ignore` in Archer.Arrow ##

The `IFeature<'featureType>.Ignore` method allows you to define tests that are intentionally skipped or ignored. This is useful for marking tests as pending, under development, or temporarily disabled, while keeping them visible in your test suite.

### Automatic Test Naming ###

Just like `Test`, the idiomatic way to use `Ignore` is to assign its result to an identifier. The name of the identifier (including spaces and punctuation) is automatically used as the test's name:

```fsharp
let ``A test that is not ready yet`` =
    feature.Ignore (fun _ ->
        // This test will be skipped
        ()
    )
```

In this example, the test's name will be "A test that is not ready yet".

### Basic Usage (Explicit Name) ###

You can also specify the test name explicitly as the first argument:

```fsharp
feature.Ignore (
    "Test To Ignore",
    fun _ -> ()
)
```

### Overloads ###

`IFeature.Ignore` supports the same overloads as `Test`, including:
- With or without tags
- With or without setup/teardown
- With or without test data
- With different test body signatures

### Example ###

```fsharp
let ``Should ignore this test for now`` =
    feature.Ignore (
        TestTags [ Category "WIP" ],
        fun _ -> ()
    )
```

### Compatibility with `Test` ###

`Ignore` has a compatible signature with the `Test` function, so you can ignore a test simply by changing the method call from `Test` to `Ignore`:

```fsharp
let ``A test that is not ready yet`` =
    feature.Ignore (fun _ -> ())

// To enable the test, just change to:
let ``A test that is not ready yet`` =
    feature.Test (fun _ -> TestSuccess)
```

However, `Ignore` is less restrictive than `Test`. This means that a test written directly as an ignored test may require some adjustments to be unignored, especially if the test body or parameters do not match the stricter requirements of `Test`.

### Notes ###

- Ignored tests will appear in your test results as skipped or ignored.
- Use this to document work-in-progress or temporarily disabled tests.
- See the `Internal.Types.IFeature.fs` file for all available overloads and signatures.

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->
<!-- GENERATED DOCUMENT DO NOT EDIT! -->