# FlUnit

[![NuGet version (FlUnit)](https://img.shields.io/nuget/v/FlUnit.svg?style=flat-square)](https://www.nuget.org/packages/FlUnit/)

Prototype for a test framework where tests are defined using a fluent builder. Includes a skeleton VSTest adapter which records each assertion in a separate result of the test. 

```csharp
using FlUnit;
// NB: They went and added optional parameters in Shouldly v4 - which aren't supported by LINQ,
// meaning that the auto-naming of assertion clauses below only works with Shouldly v3-..
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

  // or..

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

As shown above, tests are defined as public static gettable properties of public static classes, with the help of a fluent builder to construct them. More examples can be found in the [example test project](./src/Example.TestProject/ExampleTests.cs).

Pros
- Succinct, readable
- Each assertion can be recorded as a separate result of the test. LINQ Expression-valued assertions are named automatically via ToString of expression bodies. This should make it easy to write tests of which the results are easy to understand. Like so:  
  ![Visual Studio Test Result Example](docs/VSTestResultExample.png)

Cons
- LINQ expression-valued assertion clauses do come with a performance cost. So does all of the passing of test objects (arranged prerequisites, test outcome objects, ..) between the provided delegates (as opposed to having a single test method). This will not be a particularly speedy framework - though I've not run any explicit tests to validate the degree to which this is true. The fact that the VSTest adapter is little more than a skeleton likely counteracts it to some degree at the moment.
- Inflexible in some ways, in that it requires you to be rather formal in the separation of the clauses of your tests. Sometimes a freer-flowing test structure is useful.
- Delegate params get unwieldy for even a modest number of separate "Given" clauses. Of course, can always do a single Given of, say, an anonymous object with a bunch of things in it - as shown above. Using C# 9's lambda discard parameters can also make things a little clearer.

## Next Steps

- Take some cues from the vstest adapter for mstest - what am I missing re debugging, parallelisation, test attachments, instrumentation, filtering etc?
- While separate result per assertion works well in VS itself, its not so clear on the command line. Work on the VSTest adapter to make it better from a test result perspective.
- Quality of life ideas:
  - Support custom test case labelling - `ToString()` of the prereqs only helpful when this yields something other than the type name..
  - Perhaps `ThenOfOutcome(o => o.Result.ShouldBe..)` and `ThenOfGiven1(g => g.Prop.ShouldBe..)` for succinctness? Though lambda discards work pretty well (to my eyes at least)..
  - We might have test cases where the prereqs aren't independent. E.g. allowing for: 
    ```
    public static Test SumOfOddAndAdjacentEven => TestThat
      .GivenEachOf(() => new[] { 1, 3, 5 })
      .AndEachOf(x => new[] { x - 1, x + 1 })
      ...  
    ```
    ..of course, people can generate these themselves in a single `GivenEachOf`, but supporting it as separate clauses might be handy. Maybe.

