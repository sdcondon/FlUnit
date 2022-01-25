## Other Notes

Other notes in no particular order.

### Notable Strengths & Weaknesses

FlUnits notable strengths include:
- Succinct & readable.
  - I would argue that the resultant reduced thinking time & confusion risk significantly mitigates any performance shortfalls (which I should stress I don't necessarily know are there, end-to-end - but see the "cons" section for some suspicions).
  - In particular, the enforced structure for tests (notably, no interlacing of action and assertion) pushes you to write easily understandable tests.
- A richer model for tests than that found in many other test frameworks (without requiring the verbose code required by frameworks such as MSpec) makes a few things possible, some of which are demonstrated in the "getting started" guidance, above.
  - Parameterised tests are easy without requiring awkward attribute-based parameter retrieval. Note that this is essentially because the pre-requisites are passed to the "When" delegate - meaning that *all* tests are parameterised.
  - The "arrange" clauses of a test don't have to be counted as part of the test proper, providing an easy way to distinguish inconclusive tests (because their arrangements failed) from failed ones - providing some assistance to the isolation of issues.
  - Specifying each assertion separately means we can record them as a separate results of the test - providing an easy way to achieve the best practice of a single assertion per test result without requiring complex code factoring when a single action should have multiple consequences. Further, we can use language & framework features to name said results automatically (CallerExpressionArgument for .NET 6+, LINQ expressions for earlier versions). This makes it easy to write tests that show what is being asserted at a glance, without requiring test failure (or unhelpfully long test names). This in turn makes your tests more discoverable, and ultimately plays a small part in making your production code easier to maintain.

FlUnit's notable weaknesses include:
- The enforced test structure can make certain scenarios mildly awkward. Consider for example what is needed to check the value of an out parameter.
- All of the passing of test objects (arranged prerequisites, return values ..) between the provided delegates (as opposed to having a single test method) comes at a performance cost - though I've not run any explicit tests to validate the extent of this. The fact that the VSTest adapter is little more than a skeleton likely counteracts it to some degree at the moment.
- Delegate params get unwieldy for even a modest number of separate "Given" clauses. Of course, can always do a single Given of, say, an anonymous object with a bunch of things in it - as shown above. Using C# 9's lambda discard parameters can also make things a little clearer.


### TDD Considerations

If you're writing some tests before implementation (and using a lambda for your assertion), you'll likely need to specify the types of your parameters for your assertion to let your IDE help you write it. Appreciate this is perhaps an area where method-based test frameworks have an edge - this is a price we pay for a richer test model..

```csharp
public static class MyTests
{   
    public static Test StandardProcessBehaviour => GivenAWidgetAndCollaborator
        .When((w, c) => w.NonExisting(collaborator))
        .ThenReturns((Widget w, Collaborator c, int retVal) => ...);
}
```

### On the Reduced Responsibility of Assertion Libraries

Note how, by allowing for individual named assertions as part of the test model and explicit test results for them, we remove some of the responsibility adopted by the richer assertion libraries out there (e.g. custom error messages as part of assertions if used to just describe the failed assertion, Shouldly's awesome PDB-reading stuff).

I view this as a positive - because being able to see at a glance what I'm asserting without that assertion failing is a powerful thing. Others may disagree..

### Caveats When Targeting .NET 5 or Earlier

While a version of the framework that targets .NET Standard 2.0 exists, and should work properly, there is one thing in particular to note:

**The .NET Standard version uses LINQ for automatic assertion naming, which has some limitations:** In .NET 6, the automatic naming of assertion results is achieved via the [CallerArgumentExpression](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-10#callerargumentexpression-attribute-diagnostics) attribute and the C# 10 compiler's knowledge thereof.
This of course isn't available in earlier versions, so in the .NET standard version of the framework, automatic naming is facilitated by allowing you to specify assertions as LINQ expressions. These have some limitations and caveats.
For example, LINQ expressions can't represent method calls with optional arguments (which are pretty common in assertion frameworks). 
Also, the round trip from expression tree back to string representation may result in some unexpected output. 
Finally, building expression trees comes at a performance cost.

### Test Lifetime - Test Property Getter vs Initializer

Test object lifetime is something you don't really need to worry about when writing tests (since its the test adapter that manages this).
However, you may find of interest if you are wondering if the examples are `public static Test MyTest => ...` rather than `public static Test MyTest { get; } = ...` for succinctness only.
While the abstraction doesn't require this, FlUnits `Test` implementations actually serve as containers for the pre-reqs and test action/function outcome - and are thus intended to be short-lived (to the extent that an invalidoperationexception is thrown if an adapter tries to run a Test instance twice).
It is thus very intentional that the examples use a getter rather than an auto-initializer..
