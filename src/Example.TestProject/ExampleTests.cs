using FlUnit;
using Shouldly;
using System;

namespace Example.TestProject
{
    public static class ExampleTests
    {
        // Basic example
        public static ITest ProcessHasSideEffects => TestThat
            .Given(new TestSubject())
            .And(new Collaborator())

            .When((sut, collaborator) => sut.Process(collaborator))

            .Then((sut, collaborator, task) => task.Result.ShouldBeTrue())
            .And((sut, collaborator, task) => sut.HasProcessed.ShouldBeTrue())
            .And((sut, collaborator, task) => collaborator.HasBeenProcessed.ShouldBeTrue());
         
        // Basic example with single anonymous object-valued 'Given' clause
        public static ITest ProcessHasSideEffects2 => TestThat
            .Given(new { sut = new TestSubject(), collaborator = new Collaborator() })
            .When(given => given.sut.Process(given.collaborator))
            .Then((given, when) => when.Result.ShouldBeTrue())
            .And((given, when) => given.sut.HasProcessed.ShouldBeTrue())
            .And((given, when) => given.collaborator.HasBeenProcessed.ShouldBeTrue());

        // Negative test
        public static ITest ProcessThrowsOnNullCollaborator => TestThat
            .Given(new TestSubject())
            .When(sut => sut.Process(null))
            .Then((sut, task) => task.Exception.ShouldBeOfType(typeof(ArgumentNullException)));

        // Test with no prereqs
        public static ITest CtorDoesntThrow => TestThat
            .When(() => new TestSubject())
            .Then(task => task.Exception.ShouldBeNull());

        // Block bodies
        public static ITest BlockBodies => TestThat
            .Given(new
            {
                sut = new TestSubject(),
                collaborator = new Collaborator()
            })
            .When(given =>
            {
                given.sut.Process(given.collaborator);
            })
            .Then((given, when) =>
            {
                when.Exception.ShouldBeNull();
            }, "Exception should be null");

        private class TestSubject
        {
            public bool HasProcessed { get; private set; }

            public bool Process(Collaborator collaborator)
            {
                if (collaborator == null) throw new ArgumentNullException();
                HasProcessed = true;
                collaborator.HasBeenProcessed = true;
                return true;
            }
        }

        private class Collaborator
        {
            public bool HasBeenProcessed { get; set; }
        }
    }
}
 