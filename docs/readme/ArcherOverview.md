<!-- (dl
(section-meta
    (title Archer Test Framework Overview)
)
) -->

Archer is a modern, idiomatic F# test framework designed for clarity, composability, and expressiveness. It enables you to write highly readable tests with minimal boilerplate, leveraging F#'s language features for a natural test authoring experience.

<!-- (dl (# Key Features)) -->

- **Descriptive Test Names**: Test names are automatically inferred from the identifier, allowing for natural language descriptions.
- **Composable Features**: Organize tests into features and sub-features for clear structure and reuse.
- **Flexible Setup/Teardown**: Support for setup and teardown logic at any feature or sub-feature level.
- **Data-Driven Testing**: Easily define tests that run over sequences of data.
- **Tagging and Filtering**: Use tags to categorize and filter tests.
- **Ignore Support**: Temporarily skip tests or features with a single keyword.

<!-- (dl (# Basic Example)) -->
```fsharp
let feature = Arrow.NewFeature "Math Feature"

let ``Addition should work`` =
    feature.Test (fun _ ->
        let result = 2 + 2
        if result = 4 then TestSuccess else TestFailure "Addition failed"
    )
```

<!-- (dl (# Organizing Tests)) -->
- Use `Arrow.NewFeature` to define a feature.
- Use `Sub.Feature` to create sub-features under a parent feature.
- Assign tests to identifiers for automatic naming.


<!-- (dl (# Setup and Teardown)) -->

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

<!-- (dl (# Ignoring Tests)) -->
```fsharp
let ``A test to skip`` = feature.Ignore (fun _ -> ())
```

<!-- (dl (# Data-Driven Tests)) -->
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

<!-- (dl (# Tags and Filtering)) -->
```fsharp
let ``A tagged test`` =
    feature.Test (
        TestTags [ Category "Integration" ],
        fun _ -> TestSuccess
    )
```

<!-- (dl (# Getting Started)) -->
1. Define a feature with `Arrow.NewFeature`.
2. Add tests using the `Test` method, assigning them to identifiers.
3. Use setup, teardown, data, and tags as needed.
4. Run your tests with your preferred test runner.

For more details, see the API documentation and example files in the repository.
