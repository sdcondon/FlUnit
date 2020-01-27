using FlUnit;
using FluentAssertions;
using System;

namespace Example.TestProject
{
    public class TestSubject
    {
        public bool HasFooed { get; private set; }

        public bool Foo(Collaborator collaborator)
        {
            if (collaborator == null) throw new ArgumentNullException();
            HasFooed = true;
            collaborator.HasBeenFooed = true;
            return true;
        }
    }

    public class Collaborator
    {
        public bool HasBeenFooed { get; set; }
    }

    public static class ExampleTests
    {
        public static ITest ExtendedBodies => TestThat
            .Given(new
            {
                sut = new TestSubject(),
                c = new Collaborator()
            })

            .When(arr =>
            {
                arr.sut.Foo(arr.c);
            })

            .Then((arr, t) =>
            {
                t.Exception.Should().BeNull();
            }, "Exception should be null");

        public static ITest TestWithNoPrerequisites => TestThat
            .When(() => new TestSubject())
            .Then(task => task.Exception.Should().BeNull());

        public static ITest FooThrowsOnNullCollaborator => TestThat
            .Given(new TestSubject())
            .When(sut => sut.Foo(null))
            .Then((sut, task) => task.Exception.Should().BeOfType(typeof(ArgumentNullException)));

        public static ITest FooHasSideEffects => TestThat
            .Given(new TestSubject())
            .And(new Collaborator())

            .When((sut, collaborator) => sut.Foo(collaborator))

            .Then((sut, collaborator, task) => task.Result.Should().BeTrue())
            .And((sut, collaborator, task) => sut.HasFooed.Should().BeTrue())
            .And((sut, collaborator, task) => collaborator.HasBeenFooed.Should().BeTrue());
         
        public static ITest AltFooHasAppropriateSideEffects => TestThat
            .Given(new { sut = new TestSubject(), collaborator = new Collaborator() })
            .When(test => test.sut.Foo(test.collaborator))
            .Then((test, task) => task.Result.Should().BeTrue())
            .And((test, task) => test.sut.HasFooed.Should().BeTrue())
            .And((test, task) => test.collaborator.HasBeenFooed.Should().BeTrue());
    }
}
 