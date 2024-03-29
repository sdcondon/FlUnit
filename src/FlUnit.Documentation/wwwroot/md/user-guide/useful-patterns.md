# Useful Patterns

Here are a few patterns that may prove useful when using FlUnit.

## Test Cases as Records

Where you are testing the behaviour of some process with different inputs and expected outputs, consider creating a record encapsulating a test case and using `GivenEachOf`, like this:

```csharp
public static class MyTests
{
    private record TestCase(char Char, int Count, string Expected);
    
    public static Test CtorBehaviour => TestThat
        .GivenEachOf(() => new TestCase[]
        {
            new(Char: 'A', Count: 0, Expected: ""),
            new(Char: 'A', Count: 1, Expected: "A"),
            new(Char: 'A', Count: 2, Expected: "AA"),
        })
        .When(tc => new string(tc.Char, tc.Count))
        .ThenReturns((tc, retVal) => retVal.ShouldBe(tc.Expected));
}
```

Some notes on this approach:
* A real example of this can be found [here](https://github.com/sdcondon/SCGraphTheory.Search/blob/master/src/Search.Tests/Classic/AStarSearchTests.cs).
* Also note how this approach gives you a graceful way to get exactly the labels you want for your test cases - by overriding `ToString` in the record.

## Affected Object Graph as Prerequisite

It is entirely understandable when getting to grips with FlUnit to become stuck when trying to assert on something that is neither the return value of the `When` clause nor any of the objects referenced by it.
The key point to realise here is that your prerequisites (i.e. `Given` clause outputs) don't have to be only the objects that are referenced in the "When" clause.

Consider the following example from SCGraphTheory.AdjacencyList, where we are testing the behaviour when removing a node from a graph (a graph theory graph - a set of nodes and edges connecting them).
We only use the graph and the removed node in the "When" clause, but this operation should affect other objects too.
By assembling an anonymous object consisting of all potentially affected objects as our prerequisite, we can do this easily while still maintaining the clear distinction between the "Arrange", "Act" and "Assert" parts of our test.

Note how similar this looks to a test you might write with a method-based framework. The difference of course being that its split up into the various builder methods, requiring the addition of the "overhead" that is the creation of the anonymous object for prereqs, and the lambda parameters.
Whether what FlUnit offers (e.g. richer test result behaviour and ease of parameterised testing) outweighs this overhead is a key decision to make when deciding if FlUnit is right for your project.

```csharp
public static Test NodeRemoval => TestThat
    .Given(() =>
    {
        var graph = new Graph<Node, Edge>();

        Node node1, node2, node3;
        graph.Add(node1 = new Node());
        graph.Add(node2 = new Node());
        graph.Add(node3 = new Node());

        Edge edge1, edge2, edge3;
        graph.Add(edge1 = new Edge(node1, node2));
        graph.Add(edge2 = new Edge(node2, node3));
        graph.Add(edge3 = new Edge(node3, node1));

        return new { graph, node1, node2, node3, edge1, edge2, edge3 };
    })
    .When(given => given.graph.Remove(given.node3))
    .ThenReturns()
    .And((_, returnValue) => returnValue.Should().BeTrue())
    .And((given, _) => given.graph.Nodes.Should().BeEquivalentTo(new[] { given.node1, given.node2 }))
    .And((given, _) => given.graph.Edges.Should().BeEquivalentTo(new[] { given.edge1 }))
    .And((given, _) => given.node1.Edges.Should().BeEquivalentTo(new[] { given.edge1 }))
    .And((given, _) => given.node2.Edges.Should().BeEmpty());
```

## Prerequisite Builder Re-Use

Re-use of the builders that are returned at each step of building up the prerequisites of a test is one way to achieve succinct and readable test code re-use.
A simple example follows.

Note that once you start specifying assertions, the builders are mutable (each "And" modifies the existing builder..), so can't be re-used. Might revisit this decision at some point. Not sure why people would want to re-use a builder once the "When" clause has been specified, but there is something to be said for consistent behaviour..

```csharp
public static class MyTests
{
    private static readonly TestPrerequisitesBuilder<Widget, Collaborator> GivenAWidgetAndCollaborator = TestThat
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