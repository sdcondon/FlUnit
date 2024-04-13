using FluentAssertions;
using FlUnit.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlUnit.Tests
{
    //// NB: These tests cover the builders, too..
    [TestClass]
    public class FunctionTestTests
    {
        [TestMethod]
        public async Task MinimalTest()
        {
            // Arrange
            Test test = TestThat
                .When(() => 1)
                .ThenReturns();

            // Act & Assert
            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(1);

            await new Func<Task>(async () => await test.Cases.Single().ActAsync()).Should().NotThrowAsync();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("Test function should return successfully");
            await new Func<Task>(async () => await assertion.AssertAsync()).Should().NotThrowAsync();
        }

        [TestMethod]
        public async Task SimpleTest()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new { x = 1, y = 1 })
                .When(given => given.x + given.y)
#if NET6_0
                .ThenReturns((_, sum) => sum.Should().Be(2));
#else
                .ThenReturns((_, sum) => sum.Should().Be(2), "sum.Should().Be(2)"); // An example of LINQ limitations..
#endif

            // Act & Assert
            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(1);

            await new Func<Task>(async () => await test.Cases.Single().ActAsync()).Should().NotThrowAsync();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("sum.Should().Be(2)");
            await new Func<Task>(async () => await assertion.AssertAsync()).Should().NotThrowAsync();
        }

        [TestMethod]
        public async Task SimpleTestWithAsyncClauses()
        {
            // Arrange
            Test test = TestThat
                .GivenAsync(async () => { await Task.Yield(); return new { x = 1, y = 1 }; })
                .WhenAsync(async given => { await Task.Yield(); return given.x + given.y; })
                .ThenReturns()
                .AndAsync(async (_, sum) => { await Task.Yield(); sum.Should().Be(2); }, "sum.Should().Be(2)");

            // Act & Assert
            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(1);

            await new Func<Task>(async () => await test.Cases.Single().ActAsync()).Should().NotThrowAsync();
            test.Cases.Single().Assertions.Count.Should().Be(2);

            var assertion1 = test.Cases.Single().Assertions.First();
            assertion1.ToString().Should().Be("Test function should return successfully");
            await new Func<Task>(async () => await assertion1.AssertAsync()).Should().NotThrowAsync();

            var assertion2 = test.Cases.Single().Assertions.Skip(1).First();
            assertion2.ToString().Should().Be("sum.Should().Be(2)");
            await new Func<Task>(async () => await assertion2.AssertAsync()).Should().NotThrowAsync();
        }

        [TestMethod]
        public async Task ComplexTest()
        {
            // Arrange: multiple prerequisites and assertions; also explicit assertion labels
            Test test = TestThat
                .Given(() => 1)
                .And(() => 1)
                .When((x, y) => x + y)
                .ThenReturns((x, _, sum) => sum.Should().BeGreaterThan(x), "Sum should be greater than x")
                .And((_, y, sum) => sum.Should().BeGreaterThan(y), "Sum should be greater than y");

            // Act & Assert
            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(1);

            await new Func<Task>(async () => await test.Cases.Single().ActAsync()).Should().NotThrowAsync();
            test.Cases.Single().Assertions.Count.Should().Be(2);

            var assertion1 = test.Cases.Single().Assertions.First();
            assertion1.ToString().Should().Be("Sum should be greater than x");
            await new Func<Task>(async () => await assertion1.AssertAsync()).Should().NotThrowAsync();

            var assertion2 = test.Cases.Single().Assertions.Skip(1).First();
            assertion2.ToString().Should().Be("Sum should be greater than y");
            await new Func<Task>(async () => await assertion2.AssertAsync()).Should().NotThrowAsync();
        }

        [TestMethod]
        public async Task MultipleCases_SomeExpectedToThrow()
        {
            // Arrange
            // This is a horrible test - and I suspect that all tests where 
            // some cases are expected to throw and others aren't would look terible to me.
            // Hence feeling very conflicted about adding Then(..) back in.
            // With a TestCase class containing some logic it might be better though, and libraries
            // should empower people, not constrain them..
            Test test = TestThat
                .GivenEachOf(() => new[] { 2, 1, 0 })
                .When(x => 2 / x)
                .Then((x, outcome) =>
                {
                    if (x > 0)
                    {
                        outcome.Result.Should().BeGreaterThan(0);
                    }
                    else
                    {
                        outcome.Exception.Should().BeOfType<DivideByZeroException>();
                    }
                }, "Outcome should be as expected");

            // Act & Assert
            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(3);

            var case1 = test.Cases.First();
            case1.ToString().Should().Be("2");
            await new Func<Task>(async () => await case1.ActAsync()).Should().NotThrowAsync();
            case1.Assertions.Count.Should().Be(1);
            case1.Assertions.Single().ToString().Should().Be("Outcome should be as expected");
            await new Func<Task>(async () => await case1.Assertions.Single().AssertAsync()).Should().NotThrowAsync();

            var case2 = test.Cases.Skip(1).First();
            case2.ToString().Should().Be("1");
            await new Func<Task>(async () => await case2.ActAsync()).Should().NotThrowAsync();
            case2.Assertions.Count.Should().Be(1);
            case2.Assertions.Single().ToString().Should().Be("Outcome should be as expected");
            await new Func<Task>(async () => await case2.Assertions.Single().AssertAsync()).Should().NotThrowAsync();

            var case3 = test.Cases.Skip(2).First();
            case3.ToString().Should().Be("0");
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
                .Given(() => new { x = 1, y = 0 })
                .When(given => given.x / given.y)
#if NET6_0
                .ThenThrows((_, exception) => exception.Should().BeOfType<DivideByZeroException>());
#else
                .ThenThrows((_, exception) => exception.Should().BeOfType<DivideByZeroException>(), "exception.Should().BeOfType<DivideByZeroException>()"); // An example of LINQ limitations..
#endif

            // Act & Assert
            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(1);

            await new Func<Task>(async () => await test.Cases.Single().ActAsync()).Should().NotThrowAsync();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("exception.Should().BeOfType<DivideByZeroException>()");

            await new Func<Task>(async () => await assertion.AssertAsync()).Should().NotThrowAsync();
        }

        [TestMethod]
        public async Task ExpectedExceptionThrown_Shorthand()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new { x = 1, y = 0 })
                .When(given => given.x / given.y)
                .ThenThrows();

            // Act & Assert
            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(1);

            await new Func<Task>(async () => await test.Cases.Single().ActAsync()).Should().NotThrowAsync();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("Test function should throw an exception");
            await new Func<Task>(async () => await assertion.AssertAsync()).Should().NotThrowAsync();
        }

        [TestMethod]
        public async Task FailingAssertion()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new { x = 1, y = 1 })
                .When(given => given.x + given.y)
#if NET6_0
                .ThenReturns((_, sum) => sum.Should().Be(3));
#else
                .ThenReturns((_, sum) => sum.Should().Be(3), "sum.Should().Be(3)"); // An example of LINQ limitations..
#endif

            // Act & Assert
            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(1);

            await new Func<Task>(async () => await test.Cases.Single().ActAsync()).Should().NotThrowAsync();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("sum.Should().Be(3)");
            await new Func<Task>(async () => await assertion.AssertAsync()).Should().ThrowAsync<TestFailureException>();
        }

        [TestMethod]
        public async Task MultipleInvocations()
        {
            Test test = TestThat
                .When(() => 1)
                .ThenReturns(retVal => { }, "Empty assertion");

            await new Func<Task>(async () => await test.ArrangeAsync(new TestContext())).Should().NotThrowAsync();
            test.Cases.Count.Should().Be(1);

            await test.Cases.Single().ActAsync();
            await new Func<Task>(async () => await test.Cases.Single().ActAsync()).Should().ThrowAsync<InvalidOperationException>();
        }

        [TestMethod]
        public void ConfigurationOverrides()
        {
            var sharedBuilder = TestThat
                .UsingConfiguration(c => c.ArrangementFailureCountsAsFailed = false);

            Test test1 = sharedBuilder
                .When(() => { })
                .ThenReturns();

            Test test2 = sharedBuilder
                .UsingConfiguration(c => c.ArrangementFailureCountsAsFailed = true)
                .When(() => { })
                .ThenReturns();

            Configuration test1Config = new();
            test1.ApplyConfigurationOverrides(test1Config);
            test1Config.ArrangementFailureCountsAsFailed = false;

            Configuration test2Config = new();
            test2.ApplyConfigurationOverrides(test2Config);
            test2Config.ArrangementFailureCountsAsFailed = true;
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
                    return 1;
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
                .When(() => 0)
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

        private class Configuration : ITestConfiguration
        {
            public bool ArrangementFailureCountsAsFailed { get; set; }
            public IResultNamingStrategy ResultNamingStrategy { get; set; }
        }

        private class TestContext : ITestContext
        {
            public List<string> ErrorMessages { get; } = new List<string>();

            public List<string> OutputMessages { get; } = new List<string>();

            public void WriteError(string error) => ErrorMessages.Add(error);

            public void WriteErrorLine(string error) => throw new NotImplementedException();

            public void WriteOutput(string output) => OutputMessages.Add(output);

            public void WriteOutputLine(string output) => throw new NotImplementedException();
        }
    }
}
