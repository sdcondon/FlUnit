# FlUnit

[![NuGet version (FlUnit)](https://img.shields.io/nuget/v/FlUnit.svg?style=flat-square)](https://www.nuget.org/packages/FlUnit/)

Prototype for a test framework where tests are defined using a fluent builder. Includes a skeleton VSTest adapter which records each assertion in a separate result of the test. 

### Usage

Create a (.NET 5) class library and add some package references:
- `Microsoft.NET.Test.Sdk` - to identify this as a test project
- `FlUnit` - which contains the important stuff - the builder and test classes
- `FlUnit.Adapters.VSTest` - the VSTest adapter package, so that the VSTest platform knows how to find and run FlUnit tests.
- You'll also probably want to include an assertion library of your choice - the example code below uses `Shouldly`, for example.

As shown below, tests are defined as public static gettable properties of public static classes, with the help of a fluent builder to construct them. More examples can be found in the [example test project](./src/Example.TestProject/ExampleTests.cs).

```csharp
using FlUnit;
// NB: Shouldly v4 adds optional parameters to many of its assertion methods - which Linq expressions can't
// represent - meaning that the examples here (which attempt to make use of LINQ expression-valued assertions
// for automatic labelling - see below for more on this) only work with Shouldly v3-..
using Shouldly;

public static class MyTests
{
  public static Test WidgetCanProcessAThingy => TestThat
    .Given(() => new Widget("widget1"))
    .And(() => new Thingy("thingy1"))
    .When((wi, th) => wi.TryProcess(th))
    .Then((wi, th, t) => t.Result.ShouldBeTrue())
    .And((wi, th, t) => th.IsProcessed.ShouldBeTrue())
    .And((wi, th, t) => wi.HasProcessed.ShouldBeTrue());

  // or, you may find that a single 'given' clause returning an anonymous
  // object makes for more readable tests:

  public static Test WidgetCanProcessAThingy => TestThat
    .Given(() => new
    {
      widget = new Widget("widget1"),
      thingy = new Thingy("thingy1")
    })
    .When(given => given.widget.TryProcess(given.thingy))
    .Then((given, tryProcess) => tryProcess.Result.ShouldBeTrue())
    .And((given, tryProcess) => given.thingy.IsProcessed.ShouldBeTrue())
    .And((given, tryProcess) => given.widget.HasProcessed.ShouldBeTrue());

  // multiple test cases are also supported, for example..
  // (and note how using C# 9's lambda discard parameters can make assertion clauses a little clearer)

  public static Test SumOfEvenAndOdd => TestThat
    .GivenEachOf(() => new[] { 1, 3, 5 })
    .AndEachOf(() => new[] { 2, 4, 6 })
    .When((x, y) => x + y)
    .Then((_, _, addition) => (addition.Result % 2).ShouldBe(1))
    .And((x, _, addition) => addition.Result.ShouldBeGreaterThan(x));
}
```

### Pros
- Succinct, readable
- A richer model for tests than that found in many other test frameworks (without requiring the verbose code required by frameworks such as MSpec) makes a few things possible:
  - Parameterised tests are easy without requiring awkward attribute-based parameter retrieval.
  - The "arrange" clauses of a test don't have to be counted as part of the test proper, providing an easy way to distinguish inconclusive tests (because their arrangements failed) from failed ones - providing some assistance to the isolation of issues.
  - Similarly, each assertion can be recorded as a separate result of the test - providing an easy way to achieve the best practice of a single assertion per test result without requiring complex code factoring when a single action should have multiple consequences.
  - LINQ expression-valued assertions can be named automatically via ToString of expression bodies. This can make it easy to write tests of which the results are easy to understand. Like so:  
  ![Visual Studio Test Result Example](docs/VSTestResultExample.png)

### Cons
- All of the passing of test objects (arranged prerequisites, test outcome objects, ..) between the provided delegates (as opposed to having a single test method) comes at a performance cost - though I've not run any explicit tests to validate the extent of this. The fact that the VSTest adapter is little more than a skeleton likely counteracts it to some degree at the moment.
- LINQ expression-valued assertion clauses come with some drawbacks. Building an expression tree is relatively expensive, so there's an additional performance cost here. You also can't put breakpoints on them (though subjectively the desire to do this should be relatively rare - given that they're just assertions rather than the "meat" of the test).
- Delegate params get unwieldy for even a modest number of separate "Given" clauses. Of course, can always do a single Given of, say, an anonymous object with a bunch of things in it - as shown above. Using C# 9's lambda discard parameters can also make things a little clearer.

## Project Status

The plan is to continue to chip away at this, but I have now more or less reached the limit of what can be hacked together - and properly getting to grips with (in particular) the VSTest platform may make for slow going for a little while..

- Take some cues from the vstest adapter for mstest - what am I missing regarding debugging, parallelisation, test attachments, instrumentation, filtering etc?
- Quality of life ideas:
  - Support custom test case labelling - `ToString()` of the prereqs only helpful when this yields something other than the type name.. Perhaps `WithResultLabels`?
  - Perhaps `ThenOfOutcome(o => o.Result.ShouldBe..)` and `ThenOfGiven1(g => g.Prop.ShouldBe..)` for succinctness? Though lambda discards work pretty well (to my eyes at least)..
- Slightly zany ideas:
  - If the abstractions in the abstractions package can be properly maintained, completely free of any test info beyond the Test class itself (including attributes and whatnot), it may actually be worth renaming them (and any depending adapters) to something like PropUnit... - because any implementation of tests as gettable properties could use the same adapter. Interesting notion - not sure how useful it'd actually turn out to be..

