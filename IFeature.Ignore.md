# Using `IFeature.Ignore` in Archer.Arrow

The `IFeature<'featureType>.Ignore` method allows you to define tests that are intentionally skipped or ignored. This is useful for marking tests as pending, under development, or temporarily disabled, while keeping them visible in your test suite.

## Automatic Test Naming

Just like `Test`, the idiomatic way to use `Ignore` is to assign its result to an identifier. The name of the identifier (including spaces and punctuation) is automatically used as the test's name:

```fsharp
let ``A test that is not ready yet`` =
    feature.Ignore (fun _ ->
        // This test will be skipped
        ()
    )
```

In this example, the test's name will be "A test that is not ready yet".

## Basic Usage (Explicit Name)

You can also specify the test name explicitly as the first argument:

```fsharp
feature.Ignore (
    "Test To Ignore",
    fun _ -> ()
)
```

## Overloads

`IFeature.Ignore` supports the same overloads as `Test`, including:
- With or without tags
- With or without setup/teardown
- With or without test data
- With different test body signatures

## Example
```fsharp
let ``Should ignore this test for now`` =
    feature.Ignore (
        TestTags [ Category "WIP" ],
        fun _ -> ()
    )
```

## Compatibility with `Test`

`Ignore` has a compatible signature with the `Test` function, so you can ignore a test simply by changing the method call from `Test` to `Ignore`:

```fsharp
let ``A test that is not ready yet`` =
    feature.Ignore (fun _ -> ())

// To enable the test, just change to:
let ``A test that is not ready yet`` =
    feature.Test (fun _ -> TestSuccess)
```

However, `Ignore` is less restrictive than `Test`. This means that a test written directly as an ignored test may require some adjustments to be unignored, especially if the test body or parameters do not match the stricter requirements of `Test`.

## Notes
- Ignored tests will appear in your test results as skipped or ignored.
- Use this to document work-in-progress or temporarily disabled tests.
- See the `Internal.Types.IFeature.fs` file for all available overloads and signatures.
