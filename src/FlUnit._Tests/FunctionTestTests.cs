using FluentAssertions;
using FlUnit.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace FlUnit._Tests
{
    //// NB: These tests conver the builders, too..
    [TestClass]
    public class FunctionTestTests
    {
        [TestMethod]
        public void MinimalTest()
        {
            // Arrange
            Test test = TestThat
                .When(() => 1)
                .ThenReturns();

            // Act & Assert
            ((Action)test.Arrange).Should().NotThrow();
            test.Cases.Count.Should().Be(1);

            ((Action)test.Cases.Single().Act).Should().NotThrow();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("Test function should return successfully");
            ((Action)assertion.Assert).Should().NotThrow();
        }

        [TestMethod]
        public void SimpleTest()
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
            ((Action)test.Arrange).Should().NotThrow();
            test.Cases.Count.Should().Be(1);

            ((Action)test.Cases.Single().Act).Should().NotThrow();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("sum.Should().Be(2)");
            ((Action)assertion.Assert).Should().NotThrow();
        }

        [TestMethod]
        public void ComplexTest()
        {
            // Arrange: multiple prerequisites and assertions; also explicit assertion labels
            Test test = TestThat
                .Given(() => 1)
                .And(() => 1)
                .When((x, y) => x + y)
                .ThenReturns((x, _, sum) => sum.Should().BeGreaterThan(x), "Sum should be greater than x")
                .And((_, y, sum) => sum.Should().BeGreaterThan(y), "Sum should be greater than y");

            // Act & Assert
            ((Action)test.Arrange).Should().NotThrow();
            test.Cases.Count.Should().Be(1);

            ((Action)test.Cases.Single().Act).Should().NotThrow();
            test.Cases.Single().Assertions.Count.Should().Be(2);

            var assertion1 = test.Cases.Single().Assertions.First();
            assertion1.ToString().Should().Be("Sum should be greater than x");
            ((Action)assertion1.Assert).Should().NotThrow();

            var assertion2 = test.Cases.Single().Assertions.Skip(1).First();
            assertion2.ToString().Should().Be("Sum should be greater than y");
            ((Action)assertion2.Assert).Should().NotThrow();
        }

        [TestMethod]
        public void MultipleCases_SomeExpectedToThrow()
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
            ((Action)test.Arrange).Should().NotThrow();
            test.Cases.Count.Should().Be(3);

            var case1 = test.Cases.First();
            case1.ToString().Should().Be("2");
            ((Action)case1.Act).Should().NotThrow();
            case1.Assertions.Count.Should().Be(1);
            case1.Assertions.Single().ToString().Should().Be("Outcome should be as expected");
            ((Action)case1.Assertions.Single().Assert).Should().NotThrow();

            var case2 = test.Cases.Skip(1).First();
            case2.ToString().Should().Be("1");
            ((Action)case2.Act).Should().NotThrow();
            case2.Assertions.Count.Should().Be(1);
            case2.Assertions.Single().ToString().Should().Be("Outcome should be as expected");
            ((Action)case2.Assertions.Single().Assert).Should().NotThrow();

            var case3 = test.Cases.Skip(2).First();
            case3.ToString().Should().Be("0");
            ((Action)case3.Act).Should().NotThrow();
            case3.Assertions.Count.Should().Be(1);
            case3.Assertions.Single().ToString().Should().Be("Outcome should be as expected");
            ((Action)case3.Assertions.Single().Assert).Should().NotThrow();
        }

        [TestMethod]
        public void ExpectedExceptionThrown()
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
            ((Action)test.Arrange).Should().NotThrow();
            test.Cases.Count.Should().Be(1);

            ((Action)test.Cases.Single().Act).Should().NotThrow();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("exception.Should().BeOfType<DivideByZeroException>()");

            ((Action)assertion.Assert).Should().NotThrow();
        }

        [TestMethod]
        public void ExpectedExceptionThrown_Shorthand()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new { x = 1, y = 0 })
                .When(given => given.x / given.y)
                .ThenThrows();

            // Act & Assert
            ((Action)test.Arrange).Should().NotThrow();
            test.Cases.Count.Should().Be(1);

            ((Action)test.Cases.Single().Act).Should().NotThrow();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("Test function should throw an exception");
            ((Action)assertion.Assert).Should().NotThrow();
        }

        [TestMethod]
        public void FailingAssertion()
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
            ((Action)test.Arrange).Should().NotThrow();
            test.Cases.Count.Should().Be(1);

            ((Action)test.Cases.Single().Act).Should().NotThrow();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("sum.Should().Be(3)");
            ((Action)assertion.Assert).Should().Throw<TestFailureException>();
        }

        [TestMethod]
        public void MultipleInvocations()
        {
            Test test = TestThat
                .When(() => 1)
                .ThenReturns(retVal => { }, "Empty assertion");

            ((Action)test.Arrange).Should().NotThrow();
            test.Cases.Count.Should().Be(1);

            test.Cases.Single().Act();
            Assert.ThrowsException<InvalidOperationException>(test.Cases.Single().Act);
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

#if !NET6_0
        [TestMethod]
        public void LinqAssertions()
        {
            // Arrange
            Test test = TestThat
                .When(() => 0)
                .Then(o => Assert.Fail());

            // Act & Assert
            ((Action)test.Arrange).Should().NotThrow();
            test.Cases.Count.Should().Be(1);

            ((Action)test.Cases.Single().Act).Should().NotThrow();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("Fail()");
            ((Action)assertion.Assert).Should().Throw<TestFailureException>();
        }
#endif

        private class Configuration : ITestConfiguration
        {
            public bool ArrangementFailureCountsAsFailed { get; set; }
            public IResultNamingStrategy ResultNamingStrategy { get; set; }
        }
    }
}
