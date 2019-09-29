# FlUnit

Prototype for a test framework where tests are defined using a fluent builder. Includes a skeleton VSTest adapter. 

```
using FlUnit;
using FluentAssertions;

pubic static class MyTests
{
  ..

  public static ITest WidgetCanProcessAThingy => TestThat
    .Given(new Widget("widget1"))
    .And(new Thingy("thingy1"))
    .When((wi, th) => wi.Process(th))
    .Then((wi, th, t) => t.Result.ShouldBe(true))
    .And((wi, th, t) => th.IsProcessed.ShouldBe(true))
    .And((wi, th, t) => wi.HasProcessed.ShouldBe(true));

  ..
}
```