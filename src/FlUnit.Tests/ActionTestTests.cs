using FluentAssertions;
using FlUnit.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlUnit.Tests
{
    //// NB: These tests cover the builders, too..
    [TestClass]
    public class ActionTestTests
    {
        [TestMethod]
        public async Task MinimalTest()
        {
            // Arrange
            Test test = TestThat
                .When(() => { })
                .ThenReturns();

            // Act & Assert
            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(1);

            await new Func<Task>(async () => await test.Cases.Single().ActAsync()).Should().NotThrowAsync();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("Test action should return successfully");
            await new Func<Task>(async () => await assertion.AssertAsync()).Should().NotThrowAsync();
        }

        [TestMethod]
        public async Task SimpleTest()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new StringBuilder())
                .When(sb => { sb.Append('A'); })
#if NET6_0
                .ThenReturns(sb => sb.Length.Should().Be(1));
#else
                .ThenReturns(sb => sb.Length.Should().Be(1), "sb.Length.Should().Be(1)"); // An example of LINQ limitations..
#endif

            // Act & Assert
            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(1);

            await new Func<Task>(async () => await test.Cases.Single().ActAsync()).Should().NotThrowAsync();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("sb.Length.Should().Be(1)");
            await new Func<Task>(async () => await assertion.AssertAsync()).Should().NotThrowAsync();
        }

        [TestMethod]
        public async Task SimpleTestWithAsyncClauses()
        {
            // Arrange
            Test test = TestThat
                .GivenAsync(async () => { await Task.Yield(); return new StringBuilder(); })
                .WhenAsync(async sb => { await Task.Yield(); sb.Append('A'); })
                .ThenReturns()
                .AndAsync(async sb => { await Task.Yield(); sb.Length.Should().Be(1); }, "sb.Length.Should().Be(1)");

            // Act & Assert
            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(1);

            await new Func<Task>(async () => await test.Cases.Single().ActAsync()).Should().NotThrowAsync();
            test.Cases.Single().Assertions.Count.Should().Be(2);

            var assertion1 = test.Cases.Single().Assertions.First();
            assertion1.ToString().Should().Be("Test action should return successfully");
            await new Func<Task>(async () => await assertion1.AssertAsync()).Should().NotThrowAsync();

            var assertion2 = test.Cases.Single().Assertions.Skip(1).First();
            assertion2.ToString().Should().Be("sb.Length.Should().Be(1)");
            await new Func<Task>(async () => await assertion2.AssertAsync()).Should().NotThrowAsync();
        }

        [TestMethod]
        public async Task ComplexTest()
        {
            // Arrange: multiple prerequisites and assertions; also explicit assertion labels
            Test test = TestThat
                .Given(() => new StringBuilder())
                .And(() => "A")
                .When((sb, str) => { sb.Append(str); })
                .ThenReturns((sb, str) => sb.Length.Should().Be(str.Length), "Length should be correct")
                .And((sb, str) => sb.Capacity.Should().BeGreaterThanOrEqualTo(sb.Length), "Capacity should be consistent");

            // Act & Assert
            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(1);

            await new Func<Task>(async () => await test.Cases.Single().ActAsync()).Should().NotThrowAsync();
            test.Cases.Single().Assertions.Count.Should().Be(2);

            var assertion1 = test.Cases.Single().Assertions.First();
            assertion1.ToString().Should().Be("Length should be correct");
            await new Func<Task>(async () => await assertion1.AssertAsync()).Should().NotThrowAsync();

            var assertion2 = test.Cases.Single().Assertions.Skip(1).First();
            assertion2.ToString().Should().Be("Capacity should be consistent");
            await new Func<Task>(async () => await assertion2.AssertAsync()).Should().NotThrowAsync();
        }

        [TestMethod]
        public async Task MultipleCases_SomeExpectedToThrow()
        {
            // Arrange
            // This is a horrible test - and I suspect that all tests where 
            // some cases are expected to throw and others aren't would look terible to me.
            // In truth, I'm feeling very conflicted about adding Then(..) back in.
            // With a TestCase class containing some logic it might be better though, and libraries
            // should empower people, not constrain them..
            Test test = TestThat
                .Given(() => new StringBuilder(capacity: 0, maxCapacity: 1))
                .AndEachOf(() => new[] { "", "A", "AB" })
                .When((sb, str) => { sb.Append(str); })
                .Then((sb, str, outcome) =>
                {
                    if (str.Length <= 1)
                    {
                        outcome.Exception.Should().BeNull();
                    }
                    else
                    {
                        outcome.Exception.Should().BeOfType(typeof(ArgumentOutOfRangeException));
                    }
                }, "Outcome should be as expected");

            // Act & Assert
            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(3);

            var case1 = test.Cases.First();
            await new Func<Task>(async () => await case1.ActAsync()).Should().NotThrowAsync();
            case1.Assertions.Count.Should().Be(1);
            case1.Assertions.Single().ToString().Should().Be("Outcome should be as expected");
            await new Func<Task>(async () => await case1.Assertions.Single().AssertAsync()).Should().NotThrowAsync();

            var case2 = test.Cases.Skip(1).First();
            await new Func<Task>(async () => await case2.ActAsync()).Should().NotThrowAsync();
            case2.Assertions.Count.Should().Be(1);
            case2.Assertions.Single().ToString().Should().Be("Outcome should be as expected");
            await new Func<Task>(async () => await case2.Assertions.Single().AssertAsync()).Should().NotThrowAsync();

            var case3 = test.Cases.Skip(2).First();
            await new Func<Task>(async () => await case3.ActAsync()).Should().NotThrowAsync();
            case3.Assertions.Count.Should().Be(1);
            case3.Assertions.Single().ToString().Should().Be("Outcome should be as expected");
            await new Func<Task>(async () => await case3.Assertions.Single().AssertAsync()).Should().NotThrowAsync();
        }

        [TestMethod]
        public async Task ExpectedExceptionThrown()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new StringBuilder(capacity: 0, maxCapacity: 1))
                .When(sb => { sb.Append("AB"); })
#if NET6_0
                .ThenThrows((_, exception) => exception.Should().BeOfType<ArgumentOutOfRangeException>());
#else
                .ThenThrows((_, exception) => exception.Should().BeOfType<ArgumentOutOfRangeException>(), "exception.Should().BeOfType<ArgumentOutOfRangeException>()"); // An example of LINQ limitations..
#endif

            // Act & Assert
            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(1);

            await new Func<Task>(async () => await test.Cases.Single().ActAsync()).Should().NotThrowAsync();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("exception.Should().BeOfType<ArgumentOutOfRangeException>()");

            await new Func<Task>(async () => await assertion.AssertAsync()).Should().NotThrowAsync();
        }

        [TestMethod]
        public async Task ExpectedExceptionThrown_Shorthand()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new StringBuilder(capacity: 0, maxCapacity: 1))
                .When(sb => { sb.Append("AB"); })
                .ThenThrows();

            // Act & Assert
            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(1);

            await new Func<Task>(async () => await test.Cases.Single().ActAsync()).Should().NotThrowAsync();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("Test action should throw an exception");
            await new Func<Task>(async () => await assertion.AssertAsync()).Should().NotThrowAsync();
        }

        [TestMethod]
        public async Task FailingAssertion()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new StringBuilder())
                .When(sb => { sb.Append('A'); })
#if NET6_0
                .ThenReturns(sb => sb.Length.Should().Be(2));
#else
                .ThenReturns(sb => sb.Length.Should().Be(2), "sb.Length.Should().Be(2)");
#endif

            // Act & Assert
            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(1);

            await new Func<Task>(async () => await test.Cases.Single().ActAsync()).Should().NotThrowAsync();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("sb.Length.Should().Be(2)");
            await new Func<Task>(async () => await assertion.AssertAsync()).Should().ThrowAsync<TestFailureException>();
        }

        [TestMethod]
        public async Task MultipleInvocations()
        {
            Test test = TestThat
                .When(() => { })
                .ThenReturns(() => { }, "Empty assertion");

            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(1);

            await test.Cases.Single().ActAsync();
            await new Func<Task>(async () => await test.Cases.Single().ActAsync()).Should().ThrowAsync<InvalidOperationException>();
        }

        [TestMethod]
        public void ConfigurationOverrides()
        {
            var sharedBuilder = TestThat
                .UsingConfiguration(c => c.ArrangementFailureCountsAsFailed = true);

            Test test1 = sharedBuilder
                .When(() => { })
                .ThenReturns();

            Test test2 = sharedBuilder
                .UsingConfiguration(c => c.ArrangementFailureCountsAsFailed = false)
                .When(() => { })
                .ThenReturns();

            Configuration test1Config = new();
            test1.ApplyConfigurationOverrides(test1Config);
            test1Config.ArrangementFailureCountsAsFailed = true;

            Configuration test2Config = new();
            test2.ApplyConfigurationOverrides(test2Config);
            test2Config.ArrangementFailureCountsAsFailed = false;
        }

        [TestMethod]
        public async Task TestOutput()
        {
            Test test = TestThat
                .GivenTestContext()
                .When(ctx =>
                {
                    ctx.WriteOutput("Hello world");
                    ctx.WriteError("Argh!");
                })
                .ThenReturns();

            var testContext = new TestContext();

            await new Func<Task>(async () => await test.ArrangeAsync(testContext)).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(1);

            await new Func<Task>(async () => await test.Cases.Single().ActAsync()).Should().NotThrowAsync();
            testContext.OutputMessages.Should().BeEquivalentTo(new[] { "Hello world" });
            testContext.ErrorMessages.Should().BeEquivalentTo(new[] { "Argh!" });
        }

#if !NET6_0
        [TestMethod]
        public async Task LinqAssertions()
        {
            // Arrange
            Test test = TestThat
                .When(() => { })
                .Then(o => Assert.Fail());

            // Act & Assert
            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(1);

            await new Func<Task>(async () => await test.Cases.Single().ActAsync()).Should().NotThrowAsync();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("Fail()");
            await new Func<Task>(async () => await assertion.AssertAsync()).Should().ThrowAsync<TestFailureException>();
        }
#endif

        private async Task ArrangeAsyncShouldNotThrow(Test test)
        {
            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
        }

        private class Configuration : ITestConfiguration
        {
            public bool ArrangementFailureCountsAsFailed { get; set; }
            public IResultNamingStrategy ResultNamingStrategy { get; set; }
        }

        private class TestContext : ITestContext
        {
            public List<string> ErrorMessages { get; } = new List<string>();

            public List<string> OutputMessages { get; } = new List<string>();

            public CancellationToken TestCancellation => throw new NotImplementedException();

            public void WriteError(string error) => ErrorMessages.Add(error);

            public void WriteErrorLine(string error) => throw new NotImplementedException();

            public void WriteOutput(string output) => OutputMessages.Add(output);

            public void WriteOutputLine(string output) => throw new NotImplementedException();
        }
    }
}
