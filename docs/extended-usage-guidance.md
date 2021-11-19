# Extended Usage Guidance

No in-depth docs just yet, but there follow some general notes that go beyond the introductory guidance.

## Patterns

Here are a few patterns that may prove useful when using FlUnit.

### Test Cases as Records

Where you are testing the behaviour of some process with different inputs and expected outputs, consider creating a record encapsulating a test case and using `GivenEachOf`, like this:

```
public static class MyTests
{
    private record TestCase(char Char, int Count, string Expected);
    
    public static Test RepeatBehaviour => TestThat
        .GivenEachOf(() => new[]
        {
            new TestCase(Char: 'A', Count: 0, Expected: ""),
            new TestCase(Char: 'A', Count: 1, Expected: "A"),
            new TestCase(Char: 'A', Count: 2, Expected: "AA"),
        })
        .When(tc => new string(tc.Char, tc.Count))
        .ThenReturns((tc, retVal) => retVal.ShouldBe(tc.Expected));
}
```

### Pre-Requisite Builder Re-Use

Re-use of the builders that are returned at each step of building up the pre-requisites of a test is one way to achieve succinct and readable test code re-use.
A simple example follows - which doesn't actually re-use a builder instance, but would work just as well if it did.

Note that once you start specifying assertions, the builders are mutable (each "And" modifies the existing builder..), so can't be re-used (the getter approach below would work though). Might revisit this decision at some point. Not sure why people would want to re-use a builder once the "When" clause has been specified, but there is something to be said for consistent behaviour..

```
public static class MyTests
{
    private static TestBuilderWithPrerequisites<Widget, Collaborator> GivenAWidgetAndCollaborator => TestThat
        .Given(() => new Widget())
        .And(() => new Collaborator());
    
    public static Test StandardProcessBehaviour => GivenAWidgetAndCollaborator
        .When((w, c) => w.Process(collaborator))
        .Then(...);
    
    public static Test SomeOtherBehaviour => GivenAWidgetAndCollaborator
        .When((w, c) => w.SomeOtherMethod(collaborator))
        .Then(...);
    
    public static Test YetAnotherBehaviour => GivenAWidgetAndCollaborator
        .And(() => new OptionalCollaborator())
        .When((w, c, oc) => w.YetAnotherMethod(c, oc))
        .Then(...);
}
```

## Other Notes

Other notes in no particular order.

### TDD Considerations

If you're writing some tests before implementation (and using a lambda for your assertion), you'll likely need to specify the types of your parameters for your assertion to let your IDE help you write it. Appreciate this is perhaps an area where method-based test frameworks have an edge - this is a price we pay for a richer test model..

```
public static class MyTests
{   
    public static Test StandardProcessBehaviour => GivenAWidgetAndCollaborator
        .When((w, c) => w.NonExisting(collaborator))
        .ThenReturns((Widget w, Collaborator c, int retVal) => ...);
}
```

### Test Lifetime - Test Property Getter vs Initializer

Test object lifetime is something you don't really need to worry about when writing tests (since its the test adapter that manages this).
However, you may find of interest if you are wondering if the examples are `public static Test MyTest => ...` rather than `public static Test MyTest { get; } = ...` for succinctness only.
While the abstraction doesn't require this, FlUnits `Test` implementations actually serve as containers for the pre-reqs and test action/function outcome - and are thus intended to be short-lived (to the extent that an invalidoperationexception is thrown if an adapter tries to run a Test instance twice).
It is thus very intentional that the examples use a getter rather than an auto-initializer..

### On the Reduced Responsibility of Assertion Libraries

Note how, by allowing for individual named assertions as part of the test model and explicit test results for them, we remove some of the responsibility adopted by the richer assertion libraries out there (e.g. custom error messages as part of assertions if used to just describe the failed assertion, Shouldly's awesome PDB-reading stuff thats unfortunately limited to Full PDBs..).

I view this as a positive - because being able to see at a glance what I'm asserting without that assertion failing is a powerful thing. Others may disagree..
