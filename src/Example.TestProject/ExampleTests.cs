using FluentAssertions;
using FlUnit;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Example.TestProject
{
    [Trait("ClassLevelTrait", nameof(ExampleTests))]
    public static class ExampleTests
    {
        // Basic example
        public static Test ProcessingOfCollaborator => TestThat
            .Given(() => new TestSubject())
            .And(() => new Collaborator())
            .When((sut, collaborator) => sut.Process(collaborator))
            .ThenReturns((sut, collaborator, retVal) => retVal.Should().BeTrue())
            .And((sut, collaborator, retVal) => sut.HasProcessed.Should().BeTrue())
            .And((sut, collaborator, retVal) => collaborator.HasBeenProcessed.Should().BeTrue());

        // Basic example with single anonymous object-valued 'Given' clause,
        // explicit separate assertion for verifying that the 'When' clause returned rather than throwing,
        // and discard params to make assertion clauses clearer
        public static Test ProcessingOfCollaborator_ButPrettier => TestThat
            .Given(() => new
            {
                sut = new TestSubject(),
                collaborator = new Collaborator()
            })
            .When(given => given.sut.Process(given.collaborator))
            .ThenReturns()
            .And((_, retVal) => retVal.Should().BeTrue())
            .And((given, _) => given.sut.HasProcessed.Should().BeTrue())
            .And((given, _) => given.collaborator.HasBeenProcessed.Should().BeTrue());

        // Negative test that passes
        public static Test ProcessThrowsOnNullCollaborator => TestThat
            .Given(() => new TestSubject())
            .When(sut => sut.Process(null))
            .ThenThrows((_, exception) => exception.Should().BeOfType(typeof(ArgumentNullException)));

        // Positive test with failing implicit assertion
        // (we expect it to return a particular value, but it actually throws)
        [Trait("ExampleOfAFailingTest")]
        public static Test ProcessReturnsTrueOnNullCollaborator => TestThat
            .Given(() => new TestSubject())
            .When(sut => sut.Process(null))
            .ThenReturns((_, retVal) => retVal.Should().BeTrue());

        // Negative test with failing implicit assertion
        // (we expect it to throw, but it actually returns)
        [Trait("ExampleOfAFailingTest")]
        public static Test ProcessThrowsOnNonNullCollaborator => TestThat
            .Given(() => new TestSubject())
            .When(sut => sut.Process(new Collaborator()))
            .ThenThrows((_, ex) => ex.Should().BeOfType(typeof(ArgumentNullException)));

        // Test with failing explicit assertion
        // (we expect it to return a different value)
        [Trait("ExampleOfAFailingTest")]
        public static Test ProcessReturnsFalseOnNonNullCollaborator => TestThat
            .Given(() => new TestSubject())
            .When(sut => sut.Process(new Collaborator()))
            .ThenReturns((_, retVal) => retVal.Should().BeFalse());

        // Test with failing arrangement
        [Trait("ExampleOfAFailingTest")]
        public static Test ProcessDoesntThrowOnNullCollaborator2 => TestThat
            .Given(() => new TestSubject(shouldThrow: true))
            .When(sut => sut.Process(null))
            .ThenReturns((_, retVal) => retVal.Should().BeTrue());

        // Async clauses
        public static Test AsyncTestClauses => TestThat
            .Given(() => new TestSubject())
            .AndAsync(() => Task.FromResult(new Collaborator()))
            .WhenAsync(async (ts, c) => await ts.ProcessAsync(c))
            .ThenReturns()
            .AndAsync(async (_, c, rv) => { await Task.Yield(); rv.Should().BeTrue(); });

        // Test with no prerequisites
        public static Test CtorDoesntThrow => TestThat
            .When(() => new TestSubject())
            .ThenReturns(retVal => retVal.Should().BeOfType<TestSubject>());

        // Pointless test (that nevertheless serves as an example of the simplest possible valid test)
        public static Test Nothing => TestThat
            .When(() => { })
            .ThenReturns();

        // Block bodies are fine (as would be using delegates pointing at non-anonymous methods).
        public static Test BlockBodies => TestThat
            .Given(() =>
            {
                return new
                {
                    sut = new TestSubject(),
                    collaborator = new Collaborator()
                };
            })
            .When(given =>
            {
                return given.sut.Process(given.collaborator);
            })
            .ThenReturns((_, retVal) =>
            {
                retVal.Should().BeTrue();
            });

        // Multiple test cases
        public static Test SumOfOddAndSixIsOdd => TestThat
            .GivenEachOf(() => new[] { 1, 3, 5 })
            .When(x => x + 6)
            .ThenReturns((_, sum) => (sum % 2).Should().Be(1));

        // Test cases in combination with multiple assertions
        public static Test SumOfEvenAndOdd => TestThat
            .GivenEachOf(() => new[] { 1, 3, 5 })
            .AndEachOf(() => new[] { 2, 4, 6 })
            .When((x, y) => x + y)
            .ThenReturns()
            .And((_, _, sum) => (sum % 2).Should().Be(1))
            .And((x, _, sum) => sum.Should().BeGreaterThan(x));

        // More features: per-test configuration overrides and output messages
        public static Test AdvancedFunctionality => TestThat
            .UsingConfiguration(c => c.ArrangementFailureCountsAsFailed = true)
            .GivenTestContext()
            .AndEachOf(() => new[] { 1, 3, 5 })
            .When((ctx, i) =>
            {
                ctx.WriteOutputLine($"About to calculate {i} + 1..");
                return i + 1;
            })
            .ThenReturns()
            .And((ctx, i, sum) =>
            {
                ctx.WriteOutputLine($"About to check the result of {i} + 1..");
                sum.Should().Be(i + 1);
            });

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

            public Task<bool> ProcessAsync(Collaborator collaborator)
            {
                if (collaborator == null) throw new ArgumentNullException(nameof(collaborator));
                HasProcessed = true;
                collaborator.HasBeenProcessed = true;
                return Task.FromResult(true);
            }
        }

        private class Collaborator
        {
            public bool HasBeenProcessed { get; set; }
        }
    }
}
 