namespace Example.TestProject
{
    using FlUnit;
    using FluentAssertions;

    public class TestSubject
    {
        public bool HasFooed { get; private set; }

        public bool Foo(Collaborator collaborator)
        {
            HasFooed = true;
            collaborator.HasBeenFooed = true;
            return true;
        }
    }

    public class Collaborator
    {
        public bool HasBeenFooed { get; set; }
    }

    public class ExampleTests
    {
        public static ITest FooShouldWork => TestThat
            .Given(new TestSubject())
            .And(new Collaborator())
            .When((sut, collaborator) => sut.Foo(collaborator))
            .Then((sut, collaborator, task) => task.Result.Should().BeTrue())
            .And((sut, collaborator, task) => sut.HasFooed.Should().BeTrue())
            .And((sut, collaborator, task) => collaborator.HasBeenFooed.Should().BeTrue());
    }
}
