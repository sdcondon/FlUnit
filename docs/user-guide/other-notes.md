## Other Notes

Other notes in no particular order.



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
