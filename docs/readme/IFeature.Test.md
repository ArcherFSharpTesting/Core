
<!-- (dl
    (section-meta
        (title Using `IFeature.Test` in Archer.Arrow)
    )
) -->

The `IFeature<'featureType>.Test` method is the primary way to define tests for a feature in the Archer F# testing framework. It provides a flexible API for specifying test names, tags, setup/teardown logic, test data, and test bodies.

<!-- (dl (# Automatic Test Naming)) -->

The most idiomatic way to use `Test` is to assign its result to an identifier. The name of the identifier (including spaces and punctuation) is automatically used as the test's name. This allows for highly readable and descriptive test names without repeating yourself:

```fsharp
let ``A test of basic functionality`` =
    feature.Test (fun _ ->
        // Your test logic here
        TestSuccess
    )
```

In this example, the test's name will be "A test of basic functionality".


<!-- (dl (# Basic Usage \(Explicit Name\))) -->

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

<!-- (dl (# Overloads)) -->

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

<!-- (dl (# Parameters)) -->
- **testName**: The name of the test (string).
- **tags**: Optional tags for filtering and organization (`TestTags [...]`).
- **setup**: Optional setup function to prepare resources before the test.
- **data**: Optional test data for data-driven tests.
- **testBody**: The function containing the test logic. Signature depends on overload.
- **teardown**: Optional teardown function to clean up after the test.

<!-- (dl (# Writing a Test)) -->
1. Create a feature (e.g., with `Arrow.NewFeature`).
2. Call `feature.Test` with the desired parameters.
3. Implement your test logic in the test body function.
4. Return `TestSuccess` or another result from the test body.

<!-- (dl (# Example)) -->
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

<!-- (dl (# Notes)) -->
- The test body can access setup values, test data, and the test environment depending on the overload.
- Use tags and setup/teardown as needed for your scenario.
- See the `Internal.Types.IFeature.fs` file for all available overloads and signatures.
