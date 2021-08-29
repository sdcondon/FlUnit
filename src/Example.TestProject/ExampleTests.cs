using FlUnit;
using Shouldly;
using System;

namespace Example.TestProject
{
    [Trait("ClassLevelTrait", "ExampleTests")]
    public static class ExampleTests
    {
        // Basic example
        [Trait("PropertyLevelTrait", "ProcessHasSideEffects")]
        public static Test ProcessHasSideEffects => TestThat
            .Given(() => new TestSubject())
            .And(() => new Collaborator())

            .When((sut, collaborator) => sut.Process(collaborator))

            .Then((sut, collaborator, process) => process.Result.ShouldBeTrue())
            .And((sut, collaborator, process) => sut.HasProcessed.ShouldBeTrue())
            .And((sut, collaborator, process) => collaborator.HasBeenProcessed.ShouldBeTrue());
         
        // Basic example with single anonymous object-valued 'Given' clause
        public static Test ProcessHasSideEffects2 => TestThat
            .Given(() => new
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
            .Given(() => new TestSubject())
            .When(sut => sut.Process(null))
            .Then((sut, process) => process.Exception.ShouldBeOfType(typeof(ArgumentNullException)));

        // Test with failing assertion
        public static Test ProcessDoesntThrowOnNullCollaborator => TestThat
            .Given(() => new TestSubject())
            .When(sut => sut.Process(null))
            .Then((sut, process) => process.Exception.ShouldBeNull());

        // Test with failing arrangement
        public static Test ProcessDoesntThrowOnNullCollaborator2 => TestThat
            .Given(() => new TestSubject(shouldThrow: true))
            .When(sut => sut.Process(null))
            .Then((sut, process) => process.Exception.ShouldBeNull());

        // Test with no prereqs
        public static Test CtorDoesntThrow => TestThat
            .When(() => new TestSubject())
            .Then(ctor => ctor.Exception.ShouldBeNull());

        // Block bodies
        public static Test BlockBodies => TestThat
            .Given(() => new
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

        // Multiple test cases
        public static Test SumOfOddAndSixIsOdd => TestThat
            .GivenEachOf(() => new[] { 1, 3, 5 })
            .When(x => x + 6)
            .Then((x, addition) => (addition.Result % 2).ShouldBe(1));

        // Test cases in combination with multiple assertions,
        // with discard params to make assertion clauses clearer
        public static Test SumOfEvenAndOdd => TestThat
            .GivenEachOf(() => new[] { 1, 3, 5 })
            .AndEachOf(() => new[] { 2, 4, 6 })
            .When((x, y) => x + y)
            .Then((_, _, addition) => (addition.Result % 2).ShouldBe(1))
            .And((x, _, addition) => addition.Result.ShouldBeGreaterThan(x));

        private class TestSubject
        {
            public TestSubject(bool shouldThrow = false)
            {
                if (shouldThrow)
                {
                    throw new ArgumentException("shouldThrow must be false.", nameof(shouldThrow));
                }
            }

            public bool HasProcessed { get; private set; }

            public bool Process(Collaborator collaborator)
            {
                if (collaborator == null) throw new ArgumentNullException(nameof(collaborator));
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
 