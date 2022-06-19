# Advanced Fuctionality

This document details some more advanced areas of FlUnit's functionality, not covered in "getting started".

## Test Run Configuration

If VSTest is being used, FlUnit test configuration can be provided in a "FlUnit" element in the .runsettings file.
See the [annotated example](../../src/Example.TestProject/.runsettings) in the example test project for details.

Configuration that applies to individual tests can be overridden by individual tests through the use of the `UsingConfiguration` builder method.
Configuration overrides can be specified at any point up until the `When` clause is specified.

## Test Context (v1.1 and later)

Optionally, when specifying test prerequisites with `Given` and `GivenEachOf`, you can use an overload with a `Func<ITestContext, T>` parameter rather than a `Func<T>`.
That is, you can provide a delegate that accepts an `ITestContext` object - that will be provided by the test runner. This context object can be used to write test output and error messages, like this:

```
public static Test MyTest => TestThat
    .Given(ctx => new
    {
        Context = ctx,
        SUT = new MyClass(),
    })
    .When(g =>
    {
        g.Context.WriteOutputLine("Hello world!");
        g.SUT.Foo();
    })
    .ThenReturns();
```

If all you need as a prerequisite as the test context itself, you can use `GivenTestContext()` as a more readable alias of `Given(ctx => ctx)`;
