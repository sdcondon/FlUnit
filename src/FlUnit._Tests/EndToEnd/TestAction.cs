using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Linq;
using System.Text;

namespace FlUnit._Tests
{
    [TestClass]
    public class TestAction
    {
        [TestMethod]
        public void MinimalTest()
        {
            // Arrange
            Test test = TestThat
                .When(() => { })
                .ThenReturns();

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count.ShouldBe(1);

            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count.ShouldBe(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.Description.ShouldBe("Test action should return successfully");
            ((Action)assertion.Invoke).ShouldNotThrow();
        }

        [TestMethod]
        public void SimpleTest()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new StringBuilder())
                .When(sb => { sb.Append("A"); })
                .ThenReturns(sb => sb.Length.ShouldBe(1));

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count.ShouldBe(1);

            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count.ShouldBe(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.Description.ShouldBe("sb.Length.ShouldBe(1)");
            ((Action)assertion.Invoke).ShouldNotThrow();
        }

        [TestMethod]
        public void MultiplePrereqsAndAssertions()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new StringBuilder())
                .And(() => "A")
                .When((sb, str) => { sb.Append(str); })
                .ThenReturns((sb, str) => sb.Length.ShouldBe(str.Length))
                .And((sb, str) => sb.Capacity.ShouldBeGreaterThanOrEqualTo(sb.Length));

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count.ShouldBe(1);

            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count.ShouldBe(2);

            var assertion1 = test.Cases.Single().Assertions.First();
            assertion1.Description.ShouldBe("sb.Length.ShouldBe(str.Length)");
            ((Action)assertion1.Invoke).ShouldNotThrow();

            var assertion2 = test.Cases.Single().Assertions.Skip(1).First();
            assertion2.Description.ShouldBe("sb.Capacity.ShouldBeGreaterThanOrEqualTo(sb.Length)");
            ((Action)assertion2.Invoke).ShouldNotThrow();
        }

        [TestMethod]
        public void ExpectedExceptionThrown()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new StringBuilder(capacity: 0, maxCapacity: 1))
                .When(sb => { sb.Append("AB"); })
                .ThenThrows((_, exception) => exception.ShouldBeOfType(typeof(ArgumentOutOfRangeException)));

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count.ShouldBe(1);

            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count.ShouldBe(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.Description.ShouldBe("exception.ShouldBeOfType(System.ArgumentOutOfRangeException)");
            ((Action)assertion.Invoke).ShouldNotThrow();
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
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count.ShouldBe(1);

            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count.ShouldBe(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.Description.ShouldBe("Test action should throw an exception");
            ((Action)assertion.Invoke).ShouldNotThrow();
        }

        [TestMethod]
        public void FailingAssertion()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new StringBuilder())
                .When(sb => { sb.Append("A"); })
                .ThenReturns(sb => sb.Length.ShouldBe(2));

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count.ShouldBe(1);

            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count.ShouldBe(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.Description.ShouldBe("sb.Length.ShouldBe(2)");
            ((Action)assertion.Invoke).ShouldThrow(typeof(ShouldAssertException));
        }

        [TestMethod]
        public void MultipleInvocations()
        {
            Test test = TestThat
                .When(() => { })
                .ThenReturns(() => { }, "Empty assertion");

            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count.ShouldBe(1);

            test.Cases.Single().Act();
            Assert.ThrowsException<InvalidOperationException>(test.Cases.Single().Act);
        }
    }
}
