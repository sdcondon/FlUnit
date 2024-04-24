# Getting Started

Create a .NET 6 (or above) class library and add some package references:
- [`Microsoft.NET.Test.Sdk`](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/) - to identify this as a test project
- [`FlUnit`](https://www.nuget.org/packages/FlUnit/) - which contains the important stuff - the builder and test classes
- [`FlUnit.VS.TestAdapter`](https://www.nuget.org/packages/FlUnit.VS.TestAdapter/) - the VSTest adapter package, so that the VSTest platform knows how to find and run FlUnit tests.
- You'll also need to include an assertion library of your choice - the example code below uses [`FluentAssertions`](https://www.nuget.org/packages/FluentAssertions/), for example.
- [`coverlet.collector`](https://www.nuget.org/packages/coverlet.collector/) does work with FlUnit tests - so feel free to add that, too.

NB: a .NET Standard 2.0 version of the framework does exist, and targeting earlier versions of the framework does work, but there are some caveats.
Details can be found [here](user-guide/other-notes.md#caveats-when-targeting-net-5-or-earlier). All the examples and documentation below assumes .NET 6+.

As shown below, tests are defined as public static gettable properties of public static classes, with the help of a fluent builder to construct them.
More examples can be found in the [example test project](https://github.com/sdcondon/FlUnit/blob/main/src/Example.TestProject/ExampleTests.cs).

```csharp
using FlUnit;
using FluentAssertions;

public static class MyTests
{
  // First, a heavily annotated example. Start by calling a method on the "TestThat" static class,
  // each of which return a builder to continue with.
  public static Test ProcessingOfCollaborator => TestThat
    // Arrange: Use the "Given" and "And" methods to provide delegates for obtaining each
    // prerequisite of the test. Specifying prerequisites is optional. Starting your test
    // with "When" is equally valid.
    .Given(() => new TestSubject())
    .And(() => new Collaborator())
    // Act: Once all prerequisites are specified, call "When" to specify the "Act" part of the test.
    // Provide a delegate that has one parameter for each prerequisite. The delegate can return
    // a value or be void.
    .When((sut, collaborator) => sut.Process(collaborator))
    // Assert: assertions can be provided with the "ThenReturns" and "And" methods, or the "ThenThrows"
    // and "And" methods. You provide a delegate for the assertion itself and (optionally) a string
    // description for the associated test result. If you do not provide an explicit description, the text
    // of the assertion argument will be used - trimmed down to just its body if it is a lambda. For
    // "ThenReturns", the delegate should have one parameter for each prerequisite, and one for the
    // return value of the When clause (if it returns one). For "ThenThrows", see the third example,
    // below. Assertion failure should be indicated by a thrown exception.
    .ThenReturns((sut, collaborator, retVal) => retVal.Should().BeTrue())
    .And((sut, collaborator, retVal) => sut.HasProcessed.Should().BeTrue())
    .And((sut, collaborator, retVal) => collaborator.HasBeenProcessed.Should().BeTrue());
    // NB: No call is required to build a Test from a builder, because builders with at least one declared
    // assertion are implicitly convertible to Test instances.

  // You may find that a single 'given' clause returning an anonymous
  // object makes for more readable tests (separate given clauses is more useful when
  // when you have multiple test cases - see below). Also note that C# 9's lambda discard
  // parameters can make assertion clauses clearer. Finally, note that there is a parameterless
  // version of ThenReturns, that adds an assertion that just verifies that the
  // 'when' clause returned successfully. Here's a copy of the test above that makes use of all
  // of those to prettify things a bit:
  public static Test ProcessingOfCollaborator_ButPrettier => TestThat
    .Given(() => new
    {
        sut = new TestSubject(),
        collaborator = new Collaborator()
    })
    .When(given => given.sut.Process(given.collaborator))
    .ThenReturns()
    .And((_, retVal) => retVal.Should().BeTrue())
    .And((given, _) => given.sut.HasProcessed.Should().BeTrue())
    .And((given, _) => given.collaborator.HasBeenProcessed.Should().BeTrue());

  // Expecting exceptions is easy, and test traits are supported
  // (at the test, class or assembly level):
  [Trait("Category", "Negative Tests")]
  public static Test ProcessThrowsOnNullCollaborator => TestThat
    .Given(() => new TestSubject())
    .When(sut => sut.Process(null))
    // Obviously, the difference between this and 'ThenReturns' is that the
    // final parameter of the delegate is the thrown exception, not the return value.
    // A parameterless overload of ThenThrows also exists, which adds an assertion
    // that just verifies that an exception was thrown:
    .ThenThrows((_, exception) => exception.Should().BeOfType(typeof(ArgumentNullException)));

  // Parameterised tests are supported without awkward attribute-based
  // argument retrieval. This is my favourite aspect of FlUnit - and I suspect
  // that anyone who tends toward parameterised tests will enjoy this.
  public static Test SumOfEvenAndOdd => TestThat
    .GivenEachOf(() => new[] { 1, 3, 5 })
    .AndEachOf(() => new[] { 2, 4, 6 })
    .When((x, y) => x + y)
    .ThenReturns()
    .And((_, _, sum) => (sum % 2).Should().Be(1))
    .And((x, _, sum) => sum.Should().BeGreaterThan(x));

  // Test clauses can be asynchronous - the test runner will await the result.
  // Note that ITestContext (see "advanced functionality" for more about test context) includes
  // a CancellationToken property for communicating test cancellation to the test.
  public static Test AsyncTestClauses => TestThat
     .GivenTestContext()
     .AndEachOfAsync(cxt => MyRemoteTestCaseSource.GetTestCasesAsync(cxt.TestCancellation))
	 .WhenAsync((cxt, tc) => tc.DoAsyncThing(cxt.TestCancellation))
	 .ThenReturns()
	 .AndAsync((cxt, tc) => tc.AssertSomethingAsync(cxt.TestCancellation))
}
```

## Test Execution & Results

If you've included a reference to the VSTest adapter, the Visual Studio IDE and `dotnet test` should both be able to find and run the tests.

With the VSTest adapter (as of v1.0.1):
* Tests are named for the name of the property.
* Tests with multiple cases or multiple assertions give one result per test case per assertion.
  Labelling logic is specifiable with a configuration setting, but by default the label of each result depends on the multiplicity of cases and assertions, as follows:
  * With a single case and multiple assertions, the result label is the description of the assertion.
  * With multiple cases each with a single assertion, the result labels are formulated as follows.
  We look at each prerequisite's ToString. If any override ToString (i.e. don't just return the type name), the label is the ToString of each such prerequisite.
  If none of the prerequisites override ToString, the case label is just "test case #X".
  * With multiple cases each with a multiple assertions, the result label is "\{assertion description\} for \{case label, as above\}", like this:  
    ![Visual Studio Test Result Example](img/VSTestResultExample.png)
* The duration reported for the first assertion of each test case is the time taken for the `When` clause and the assertion to execute.
  All assertions after the first report only their own duration.
  Note that this means that using the parameterless `ThenReturns()` or `ThenThrows()` builder methods you're going to get an assertion that tells you (very roughly, of course) how long the `When` clause took.
  Configurability of this behaviour will be revisited at some point.

## Why FlUnit?

Introduction complete, here we talk about why you might want to use FlUnit (and why not).

FlUnit's notable strengths include:

- **FlUnit tests are succinct & readable.**
The resultant reduced thinking time & confusion risk can be massively powerful, especially in "inner loop" (i.e. unit-level) tests.
In particular, the enforced structure for tests (notably, no interlacing of action and assertion) pushes you to write easily understandable tests.
- **It's conventionless and discoverable.**
The fluent builders enable you to discover functionality by your IDE showing available builder methods. No more having to look up what attributes you need to do parameterised tests, how to inject context into a test class constructor, and what static types to use to do test output.
- **A richer model for tests than that found in many other test frameworks** (without requiring the verbose code required by frameworks such as MSpec) makes a few things possible, some of which are demonstrated in the "getting started" guidance, above.
  - **Parameterised tests** are easy without requiring awkward attribute-based parameter retrieval. Note that this is essentially because the prerequisites are passed to the "When" delegate - meaning that *all* tests are parameterised.
  - The "arrange" clauses of a test don't have to be counted as part of the test proper, providing an easy way to distinguish inconclusive tests (because their arrangements failed) from failed ones - providing some assistance to the isolation of issues.
  - Specifying each assertion separately means we can record them as a separate results of the test - providing an easy way to achieve the best practice of a single assertion per test result without requiring complex code factoring when a single action should have multiple consequences. Further, we can use language & framework features to name said results automatically (CallerExpressionArgument for .NET 6+, LINQ expressions for earlier versions). This makes it easy to write tests that show what is being asserted at a glance, without requiring test failure (or unhelpfully long test names). This in turn makes your tests more discoverable, and ultimately plays a small part in making your production code easier to maintain.

As with any design, there are downsides. FlUnit's notable weaknesses include:

- The **baseline complexity** of FlUnit tests is greater than that of tests using a method-based framework.
- The **enforced test structure** can make certain scenarios a little awkward.
  - Primarily, people have become stuck when getting to grips with FlUnit and trying to assert on objects that are neither the return value of the `When` clause nor any of the prerequisites referenced by it. There is a [simple pattern of usage](user-guide/useful-patterns.md#affected-object-graph-as-prerequisite) that can get you over this hurdle - but this is likely to be the main reason not to use FlUnit if you consider it too awkward.
  - Also consider what is needed to check the value of an out parameter. Ugly code..
- **Delegate params can get unwieldy** for even a modest number of separate "Given" clauses. Of course, can always do a single Given of, say, an anonymous object with a bunch of things in it - as shown above. Using C# 9's lambda discard parameters can also make things a little clearer.
- All of the passing of test objects (arranged prerequisites, return values ..) between the provided delegates (as opposed to having a single test method) comes at a **performance cost** - though I've not run any explicit tests to validate the extent of this. The fact that the VSTest adapter is minimal so far likely counteracts it to some degree at the moment.
