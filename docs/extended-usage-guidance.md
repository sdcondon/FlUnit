# Extended Usage Guidance

No in-depth docs just yet, but there follow some general notes that go beyond the introductory guidance.

## Patterns

Here's a few patterns that may prove useful when using FlUnit.

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
        .Then((tc, retVal) => retVal.ShouldBe(tc.Expected));
}
```

### Builder Re-Use

Re-use of the builders that are returned at each step of building a test is one way to achieve succinct and readable test code re-use. A simple example follows (though of course there a large number of possible variations on this):

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
        .Then((Widget w, Collaborator c, int retVal) => ...);
}
```

### Test Lifetime - Test Property Getter vs Initializer

Test object lifetime is something you don't really need to worry about when writing tests (since its the test adapter that manages this stuff).
However, you may find of interest if you are wondering if the examples are `public static Test MyTest => ...` rather than `public static Test MyTest { get; } = ...` for succinctness only.
While the abstraction doesn't require this*, FlUnits `Test` implementations actually serve as containers for the pre-reqs and test action/function outcome - and are thus intended to be short-lived (to the extent that an invalidoperationexception is thrown if an adapter tries to run a Test instance twice).
It is thus very intentional that the examples use a getter rather than an auto-initializer..

*\* this inconsistency is something that mildly concerns me about the design - and is the reason I figure its worth mentioning here.*

