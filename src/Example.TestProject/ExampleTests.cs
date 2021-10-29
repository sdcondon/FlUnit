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

            .ThenReturns((sut, collaborator, retVal) => retVal.ShouldBeTrue())
            .And((sut, collaborator, retVal) => sut.HasProcessed.ShouldBeTrue())
            .And((sut, collaborator, retVal) => collaborator.HasBeenProcessed.ShouldBeTrue());
         
        // Basic example with single anonymous object-valued 'Given' clause
        public static Test ProcessHasSideEffects2 => TestThat
            .Given(() => new
            {
                sut = new TestSubject(),
                collaborator = new Collaborator()
            })
            .When(given => given.sut.Process(given.collaborator))
            .ThenReturns((given, retVal) => retVal.ShouldBeTrue())
            .And((given, retVal) => given.sut.HasProcessed.ShouldBeTrue())
            .And((given, retVal) => given.collaborator.HasBeenProcessed.ShouldBeTrue());

        // Negative test
        public static Test ProcessThrowsOnNullCollaborator => TestThat
            .Given(() => new TestSubject())
            .When(sut => sut.Process(null))
            .ThenThrows((sut, exception) => exception.ShouldBeOfType(typeof(ArgumentNullException)));

        // Test with failing assertion
        public static Test ProcessDoesntThrowOnNullCollaborator => TestThat
            .Given(() => new TestSubject())
            .When(sut => sut.Process(null))
            .ThenReturns((sut, retVal) => retVal.ShouldBeTrue());

        // Test with failing arrangement
        public static Test ProcessDoesntThrowOnNullCollaborator2 => TestThat
            .Given(() => new TestSubject(shouldThrow: true))
            .When(sut => sut.Process(null))
            .ThenReturns((sut, retVal) => retVal.ShouldBeTrue());

        // Test with no prereqs
        public static Test CtorDoesntThrow => TestThat
            .When(() => new TestSubject())
            .ThenReturns(retVal => retVal.ShouldBeOfType<TestSubject>());

        // Block bodies
        public static Test BlockBodies => TestThat
            .Given(() => new
            {
                sut = new TestSubject(),
                collaborator = new Collaborator()
            })
            .When(given =>
            {
                return given.sut.Process(given.collaborator);
            })
            .ThenReturns((given, retVal) =>
            {
                retVal.ShouldBeTrue();
            }, "Return value should be true");

        // Multiple test cases
        public static Test SumOfOddAndSixIsOdd => TestThat
            .GivenEachOf(() => new[] { 1, 3, 5 })
            .When(x => x + 6)
            .ThenReturns((x, sum) => (sum % 2).ShouldBe(1));

        // Test cases in combination with multiple assertions,
        // with discard params to make assertion clauses clearer
        public static Test SumOfEvenAndOdd => TestThat
            .GivenEachOf(() => new[] { 1, 3, 5 })
            .AndEachOf(() => new[] { 2, 4, 6 })
            .When((x, y) => x + y)
            .ThenReturns((_, _, sum) => (sum % 2).ShouldBe(1))
            .And((x, _, sum) => sum.ShouldBeGreaterThan(x));

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
 