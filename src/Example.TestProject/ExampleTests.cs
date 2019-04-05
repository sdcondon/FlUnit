namespace Example.TestProject
{
    using FlUnit;
    using FluentAssertions;

    public class TestSubject
    {
        public bool Foo(Collaborator collaborator)
        {
            return true;
        }
    }

    public class Collaborator
    {
        public bool MyBool { get; set; }
        public string MyString { get; set; }
    }

    public class ExampleTests
    {
        ITest FooShouldWork => TestThat
            .Given(new TestSubject())
            .And(new Collaborator() { MyString = "blarble" })
            .When((sut, collaborator) => sut.Foo(collaborator))
            .Then((sut, collaborator, task) => task.Result.Should().BeTrue());
    }
}
