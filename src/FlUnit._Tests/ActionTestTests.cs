using FluentAssertions;
using FlUnit.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlUnit._Tests
{
    //// NB: These tests cover the builders, too..
    [TestClass]
    public class ActionTestTests
    {
        [TestMethod]
        public void MinimalTest()
        {
            // Arrange
            Test test = TestThat
                .When(() => { })
                .ThenReturns();

            // Act & Assert
            ((Action)(() => test.Arrange(new TestContext()))).Should().NotThrow();
            test.Cases.Count.Should().Be(1);

            ((Action)test.Cases.Single().Act).Should().NotThrow();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("Test action should return successfully");
            ((Action)assertion.Assert).Should().NotThrow();
        }

        [TestMethod]
        public void SimpleTest()
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
            ((Action)(() => test.Arrange(new TestContext()))).Should().NotThrow();
            test.Cases.Count.Should().Be(1);

            ((Action)test.Cases.Single().Act).Should().NotThrow();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("sb.Length.Should().Be(1)");
            ((Action)assertion.Assert).Should().NotThrow();
        }

        [TestMethod]
        public void ComplexTest()
        {
            // Arrange: multiple prerequisites and assertions; also explicit assertion labels
            Test test = TestThat
                .Given(() => new StringBuilder())
                .And(() => "A")
                .When((sb, str) => { sb.Append(str); })
                .ThenReturns((sb, str) => sb.Length.Should().Be(str.Length), "Length should be correct")
                .And((sb, str) => sb.Capacity.Should().BeGreaterThanOrEqualTo(sb.Length), "Capacity should be consistent");

            // Act & Assert
            ((Action)(() => test.Arrange(new TestContext()))).Should().NotThrow();
            test.Cases.Count.Should().Be(1);

            ((Action)test.Cases.Single().Act).Should().NotThrow();
            test.Cases.Single().Assertions.Count.Should().Be(2);

            var assertion1 = test.Cases.Single().Assertions.First();
            assertion1.ToString().Should().Be("Length should be correct");
            ((Action)assertion1.Assert).Should().NotThrow();

            var assertion2 = test.Cases.Single().Assertions.Skip(1).First();
            assertion2.ToString().Should().Be("Capacity should be consistent");
            ((Action)assertion2.Assert).Should().NotThrow();
        }

        [TestMethod]
        public void MultipleCases_SomeExpectedToThrow()
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
            ((Action)(() => test.Arrange(new TestContext()))).Should().NotThrow();
            test.Cases.Count.Should().Be(3);

            var case1 = test.Cases.First();
            ((Action)case1.Act).Should().NotThrow();
            case1.Assertions.Count.Should().Be(1);
            case1.Assertions.Single().ToString().Should().Be("Outcome should be as expected");
            ((Action)case1.Assertions.Single().Assert).Should().NotThrow();

            var case2 = test.Cases.Skip(1).First();
            ((Action)case2.Act).Should().NotThrow();
            case2.Assertions.Count.Should().Be(1);
            case2.Assertions.Single().ToString().Should().Be("Outcome should be as expected");
            ((Action)case2.Assertions.Single().Assert).Should().NotThrow();

            var case3 = test.Cases.Skip(2).First();
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
                .Given(() => new StringBuilder(capacity: 0, maxCapacity: 1))
                .When(sb => { sb.Append("AB"); })
#if NET6_0
                .ThenThrows((_, exception) => exception.Should().BeOfType<ArgumentOutOfRangeException>());
#else
                .ThenThrows((_, exception) => exception.Should().BeOfType<ArgumentOutOfRangeException>(), "exception.Should().BeOfType<ArgumentOutOfRangeException>()"); // An example of LINQ limitations..
#endif

            // Act & Assert
            ((Action)(() => test.Arrange(new TestContext()))).Should().NotThrow();
            test.Cases.Count.Should().Be(1);

            ((Action)test.Cases.Single().Act).Should().NotThrow();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("exception.Should().BeOfType<ArgumentOutOfRangeException>()");

            ((Action)assertion.Assert).Should().NotThrow();
        }

        [TestMethod]
        public void ExpectedExceptionThrown_Shorthand()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new StringBuilder(capacity: 0, maxCapacity: 1))
                .When(sb => { sb.Append("AB"); })
                .ThenThrows();

            // Act & Assert
            ((Action)(() => test.Arrange(new TestContext()))).Should().NotThrow();
            test.Cases.Count.Should().Be(1);

            ((Action)test.Cases.Single().Act).Should().NotThrow();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("Test action should throw an exception");
            ((Action)assertion.Assert).Should().NotThrow();
        }

        [TestMethod]
        public void FailingAssertion()
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
            ((Action)(() => test.Arrange(new TestContext()))).Should().NotThrow();
            test.Cases.Count.Should().Be(1);

            ((Action)test.Cases.Single().Act).Should().NotThrow();
            test.Cases.Single().Assertions.Count.Should().Be(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.ToString().Should().Be("sb.Length.Should().Be(2)");
            ((Action)assertion.Assert).Should().Throw<TestFailureException>();
        }

        [TestMethod]
        public void MultipleInvocations()
        {
            Test test = TestThat
                .When(() => { })
                .ThenReturns(() => { }, "Empty assertion");

            ((Action)(() => test.Arrange(new TestContext()))).Should().NotThrow();
            test.Cases.Count.Should().Be(1);

            test.Cases.Single().Act();
            Assert.ThrowsException<InvalidOperationException>(test.Cases.Single().Act);
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
        public void TestOutput()
        {
            Test test = TestThat
                .GivenTestContext()
                .When(ctx => ctx.WriteOutput("Hello world"))
                .ThenReturns();

            var testContext = new TestContext();

            ((Action)(() => test.Arrange(testContext))).Should().NotThrow();
            test.Cases.Count.Should().Be(1);

            ((Action)test.Cases.Single().Act).Should().NotThrow();
            testContext.OutputMessages.Should().BeEquivalentTo(new[] { "Hello world" });
        }

#if !NET6_0
        [TestMethod]
        public void LinqAssertions()
        {
            // Arrange
            Test test = TestThat
                .When(() => { })
                .Then(o => Assert.Fail());

            // Act & Assert
            ((Action)(() => test.Arrange(new TestContext()))).Should().NotThrow();
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

        private class TestContext : ITestContext
        {
            public List<string> OutputMessages { get; } = new List<string>();

            public void WriteOutput(string output) => OutputMessages.Add(output);

            public void WriteOutputLine(string output) => throw new NotImplementedException();
        }
    }
}
