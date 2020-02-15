# FlUnit

Prototype for a test framework where tests are defined using a fluent builder. Includes a skeleton VSTest adapter. 

```
using FlUnit;
using Shouldly;

pubic static class MyTests
{
  ..

  public static ITest WidgetCanProcessAThingy => TestThat
    .Given(new Widget("widget1"))
    .And(new Thingy("thingy1"))
    .When((wi, th) => wi.TryProcess(th))
    .Then((wi, th, t) => t.Result.ShouldBeTrue())
    .And((wi, th, t) => th.IsProcessed.ShouldBeTrue())
    .And((wi, th, t) => wi.HasProcessed.ShouldBeTrue());

  ..
}
```

More examples can be found in the [example test project](./src/Example.TestProject/ExampleTests.cs).

Pros
- Succinct, readable (sorta - better than MSpec, anyway..)
- Easy to do separate evaluation of each assertion if the runner supports it, can even have the option naming them automatically via ToString of expression bodies..
- Should be easy enough to extend to data driven (GivenEachOf..), even in combo (AndEachOf..)

Cons
- Pushes you towards using lambdas for stuff, BUT for the "When", if exceptions are thrown the stack trace isn't gonna be fun.
- delegate params aren't intuitive, and would get unwieldy for even a modest number of separate pre-reqs. Of course, can always do a single Given of, say, an anonymous object with a bunch of things in it..
  