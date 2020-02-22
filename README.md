# FlUnit

Prototype for a test framework where tests are defined using a fluent builder. Includes a skeleton VSTest adapter which records each assertion in a separate result of the test. 

```
using FlUnit;
using Shouldly;

pubic static class MyTests
{
  ..

  public static Test WidgetCanProcessAThingy => TestThat
    .Given(new Widget("widget1"))
    .And(new Thingy("thingy1"))
    .When((wi, th) => wi.TryProcess(th))
    .Then((wi, th, t) => t.Result.ShouldBeTrue())
    .And((wi, th, t) => th.IsProcessed.ShouldBeTrue())
    .And((wi, th, t) => wi.HasProcessed.ShouldBeTrue());

  // or..

  public static Test WidgetCanProcessAThingy => TestThat
    .Given(new
    {
      widget = new Widget("widget1"),
      thingy = new Thingy("thingy1")
    })
    .When(given => given.widget.TryProcess(given.thingy))
    .Then((given, tryProcess) => tryProcess.Result.ShouldBeTrue())
    .And((given, tryProcess) => given.thingy.IsProcessed.ShouldBeTrue())
    .And((given, tryProcess) => given.widget.HasProcessed.ShouldBeTrue());

  ..
}
```

As shown above, tests are defined as public static gettable properties of public static classes, with the help of a fluent builder to construct them. More examples can be found in the [example test project](./src/Example.TestProject/ExampleTests.cs).

Pros
- Succinct, readable
- Each assertion can be recorded as a separate result of the test. LINQ Expression-valued assertions are named automatically via ToString of expression bodies.
- Should be easy enough to extend to data driven (GivenEachOf..), even in combination (AndEachOf..)

Cons
- Delegate params get unwieldy for even a modest number of separate "Given" clauses. Of course, can always do a single Given of, say, an anonymous object with a bunch of things in it - as shown above.
  