![FlUnit Logo](src/FlUnitIcon.png)

# FlUnit

[![NuGet version (FlUnit)](https://img.shields.io/nuget/v/FlUnit.svg?style=flat-square)](https://www.nuget.org/packages/FlUnit/) [![NuGet downloads (FlUnit)](https://img.shields.io/nuget/dt/FlUnit.svg?style=flat-square)](https://www.nuget.org/packages/FlUnit/)

A test framework in which tests are defined using a fluent builder. Includes a VSTest adapter for running tests in Visual Studio or with `dotnet test`, which records each assertion in a separate result of the test. 

## Usage Guidance

### Getting Started

Create a .NET 6 class library and add some package references:
- [`Microsoft.NET.Test.Sdk`](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/) - to identify this as a test project
- [`FlUnit`](https://www.nuget.org/packages/FlUnit/) - which contains the important stuff - the builder and test classes
- [`FlUnit.VS.TestAdapter`](https://www.nuget.org/packages/FlUnit.VS.TestAdapter/) - the VSTest adapter package, so that the VSTest platform knows how to find and run FlUnit tests.
- You'll also need to include an assertion library of your choice - the example code below uses [`FluentAssertions`](https://www.nuget.org/packages/FluentAssertions/), for example.
- [`coverlet.collector`](https://www.nuget.org/packages/coverlet.collector/) does work with FlUnit tests - so feel free to add that, too.

NB: a .NET Standard 2.0 version of the framework does exist, and targeting earlier versions of the framework does work, but there are some caveats. Details can be found [here](docs/user-guide/other-notes.md#caveats-when-targeting-net-5-or-earlier). All the examples and documentation below assumes .NET 6.

As shown below, tests are defined as public static gettable properties of public static classes, with the help of a fluent builder to construct them. More examples can be found in the [example test project](./src/Example.TestProject/ExampleTests.cs).

```csharp
using FlUnit;
using FluentAssertions;

public static class MyTests
{
  // First, a heavily annotated example. Start by calling a method on the "TestThat" static class,
  // each of which return a builder to continue with.
  public static Test WidgetCanProcessAThingy => TestThat
    // Arrange: Use the "Given" and "And" methods to provide delegates for obtaining each
    // pre-requisite of the test. Specifying pre-requisites is optional. Starting your test
    // with "When" is equally valid.
    .Given(() => new Widget("widget1"))
    .And(() => new Thingy("thingy1"))
    // Act: Once all pre-requisites are specified, call "When" to specify the "Act" part of the test.
    // Provide a delegate that accepts one parameter for each pre-requisite. The delegate can return
    // a value or be void.
    .When((wi, th) => wi.TryProcess(th))
    // Assert: assertions can be provided with the "ThenReturns" and "And" methods, or the "ThenThrows"
    // and "And" methods. You provide a delegate for the assertion itself and (optionally) a string
    // description for the associated test result. If you do not provide an explicit description, the text
    // of the assertion argument will be used - trimmed down to just its body if it is a lambda. For
    // "ThenReturns", the delegate should accept one parameter for each pre-requisite, and one for the
    // return value of the When clause (if it returns one). For "ThenThrows", see the third example,
    // below. Assertion failure should be indicated by a thrown exception.
    .ThenReturns((wi, th, retVal) => retVal.Should().BeTrue())
    .And((wi, th, retVal) => th.IsProcessed.Should().BeTrue())
    .And((wi, th, retVal) => wi.HasProcessed.Should().BeTrue());
    // NB: No call required to build a Test from a builder - builders with at least one declared assertion
    // are implicitly convertible to Test instances.

  // You may find that a single 'given' clause returning an anonymous
  // object makes for more readable tests (separate given clauses is more useful when
  // when you have multiple test cases - see below). Also note how C# 9's lambda discard
  // parameters can make assertion clauses clearer:
  public static Test WidgetCanProcessAThingy => TestThat
    .Given(() => new
    {
      widget = new Widget("widget1"),
      thingy = new Thingy("thingy1")
    })
    .When(given => given.widget.TryProcess(given.thingy))
    .ThenReturns((_, retVal) => retVal.Should().BeTrue())
    .And((given, _) => given.thingy.IsProcessed.Should().BeTrue())
    .And((given, _) => given.widget.HasProcessed.Should().BeTrue());

  // Expecting exceptions is easy, and test traits are supported
  // (at the test, class or assembly level):
  [Trait("Category", "Negative Tests")]
  public static Test WidgetThrowsOnNullArg => TestThat
    .Given(() => new Widget("widget1"))
    .When(widget => widget.TryProcess(null))
    // Obviously, the difference between this and 'ThenReturns' is that the
    // final parameter of the delegate is the thrown exception, not the return value.
    .ThenThrows((_, ex) => ex.Should().BeOfType<ArgumentNullException>())
    .And((widget, _) => widget.HasProcessed.Should().BeFalse());

  // Parameterised tests are supported without awkward attribute-based
  // argument retrieval. This is my favourite aspect of FlUnit - and I suspect
  // that anyone whose mind easily goes to test cases and data-driven testing
  // will enjoy this. Also in this example, note that there is a parameterless
  // version of ThenReturns, that adds the assertion that just verifies that the
  // when clause returned successfully. An equivalent ThenThrows overload also exists.
  public static Test SumOfEvenAndOdd => TestThat
    .GivenEachOf(() => new[] { 1, 3, 5 })
    .AndEachOf(() => new[] { 2, 4, 6 })
    .When((x, y) => x + y)
    .ThenReturns()
    .And((_, _, sum) => (sum % 2).Should().Be(1))
    .And((x, _, sum) => sum.Should().BeGreaterThan(x));
}
```

### Test Execution & Results

If you've included a reference to the VSTest adapter, the Visual Studio IDE and `dotnet test` should both be able to find and run the tests.

With the VSTest adapter:
* Tests are named for the name of the property.
* Tests with multiple cases or multiple assertions give one result per test case per assertion. It is possible to override this logic with a configuration setting, but by default the label of each result depends on the multiplicity of cases and assertions, as follows:
  * With a single case and multiple assertions, the result label is the description of the assertion.
  * With multiple cases each with a single assertion, the result label is the ToString of the case (which when there are multiple given clauses, is a value tuple of each)
  * With multiple cases each with a multiple assertions, the result label is "\{assertion description\} for test case \{case ToString\}", like this:  
    ![Visual Studio Test Result Example](docs/VSTestResultExample.png)
* The duration reported for each result is the time taken for the `When` clause (and only the `When` clause) to execute. Configurability of this behaviour is on the list for v1.1.

### Where Next?

For more guidance, please see the [User Guide](docs/user-guide/README.md).

## Roadmap

Proper issue tracking would be overkill at this point, so just a bullet list to organise my thoughts:

General ongoing work:
- Take some cues from other frameworks - what am I missing regarding debugging, parallelisation, test attachments, instrumentation, filtering etc?

Specific work, highest priority first:
- *(Feb - v1.0)* V1 diligence & release
  - Get some performance benchmarks in place
  - Review abstractions - for flexibility / stability
  - Any required "doing it properly" stuff in the test adapter.
  - Resolve most or all TODOs
  - Split into separate repos
  - README in packages
- *(May / Jun - v1.1)* Possible post-v1 additions (after a break to work on other projects):
  - A little more configurability:
    - For specification of strategy for duration records (which currently makes a "sensible" decision which may not be appropriate in all situations).
    - Allow for control over parallel partitioning - likely to be trait based (e.g. allow specification of a trait name - all tests with same value won't run in parallel). Also want to allow for by class name and namespace - whether thats treated as a special case or if we hook this into trait system is TBD.
  - QoL: Support custom test case labelling - `ToString()` of the prereqs only helpful when this yields something other than the type name. Perhaps `WithResultLabels`? Perhaps somehow support IFormatProviders for test cases (thus making it easy to specify with test settings)? Needs careful thought..
  - Basic attachment & output support.
This is likely to require injecting some kind of test context object.
I really want to double-down on the convention-less/static nature of FlUnit - i.e. no convention-based ctor parameters, all discoverable via IDE method listings etc.
Plan A right now is to introduce some kind of ITestContext as a pre-requisite if needed.
That is, `TestThat.GivenTestContext().And()...When((cxt, ) => ...)`.
This particular approach doesn't allow for test context to be placed inside an anonymous prereq object, though.
Which is perhaps a good thing? But is a mandate for users, rather than a choice.
More concerning is that it doesn't allow context to be used during pre-req creation.
So, instead (or as well) could allow for cxt to be specified as a parameter of Given delegates (`Given(cxt => ...)`).
Then `GivenTestContext()` could still exist, simply as a more readable alias of `Given(cxt => cxt)`.
Hmm, maybe - this is more complex?
Still mulling this one over.
- *(At some point - v1.2 or later)* Other features:
  - Support for async tests?

Not going to do, at least in the near future:
- QoL: Perhaps `ThenOfReturnValue(rv => rv.ShouldBe..)` and `ThenOfGiven1(g => g.Prop.ShouldBe..)` for succinctness? No - Lambda discards work pretty well (to my eyes at least), and `OfGiven1`, `OfGiven2` is better dealt with via complex prereq objects
- QoL: dependent assertions - some assertions only make sense if a prior assertion has succeeded (easy for method-based test frameworks, but not for us..). Such assertions should probably give an inconclusive result? Assertions that return a value (assert a value is of a particular type, cast and return it) also a possibility - though thats probably inviting unacceptable complexity. A basic version of this could be useful though - perhaps an `AndAlso` (echoing C# operator name) - which will make all following assertions inconclusive if any prior assertion failed? No - this is best left to assertion frameworks (e.g. FluentAssertions `Which`)


