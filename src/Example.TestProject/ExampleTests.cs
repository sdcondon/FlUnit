using FlUnit;
using Shouldly;
using System;

namespace Example.TestProject
{
    public static class ExampleTests
    {
        // Basic example
        public static Test ProcessHasSideEffects => TestThat
            .Given(new TestSubject())
            .And(new Collaborator())

            .When((sut, collaborator) => sut.Process(collaborator))

            .Then((sut, collaborator, process) => process.Result.ShouldBeTrue())
            .And((sut, collaborator, process) => sut.HasProcessed.ShouldBeTrue())
            .And((sut, collaborator, process) => collaborator.HasBeenProcessed.ShouldBeTrue());
         
        // Basic example with single anonymous object-valued 'Given' clause
        public static Test ProcessHasSideEffects2 => TestThat
            .Given(new
            {
                sut = new TestSubject(),
                collaborator = new Collaborator()
            })
            .When(given => given.sut.Process(given.collaborator))
            .Then((given, process) => process.Result.ShouldBeTrue())
            .And((given, process) => given.sut.HasProcessed.ShouldBeTrue())
            .And((given, process) => given.collaborator.HasBeenProcessed.ShouldBeTrue());

        // Negative test
        public static Test ProcessThrowsOnNullCollaborator => TestThat
            .Given(new TestSubject())
            .When(sut => sut.Process(null))
            .Then((sut, process) => process.Exception.ShouldBeOfType(typeof(ArgumentNullException)));

        // Failing test
        public static Test ProcessDoesntThrowOnNullCollaborator => TestThat
            .Given(new TestSubject())
            .When(sut => sut.Process(null))
            .Then((sut, process) => process.Exception.ShouldBeNull());

        // Test with no prereqs
        public static Test CtorDoesntThrow => TestThat
            .When(() => new TestSubject())
            .Then(ctor => ctor.Exception.ShouldBeNull());

        // Block bodies
        public static Test BlockBodies => TestThat
            .Given(new
            {
                sut = new TestSubject(),
                collaborator = new Collaborator()
            })
            .When(given =>
            {
                given.sut.Process(given.collaborator);
            })
            .Then((given, process) =>
            {
                process.Exception.ShouldBeNull();
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
 