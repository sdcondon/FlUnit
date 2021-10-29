using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Linq;

namespace FlUnit._Tests
{
    [TestClass]
    public class TestFunction
    {
        [TestMethod]
        public void MinimalTest()
        {
            // Arrange
            Test test = TestThat
                .When(() => 1)
                .ThenReturns();

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count.ShouldBe(1);

            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count.ShouldBe(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.Description.ShouldBe("Test function should return successfully");
            ((Action)assertion.Invoke).ShouldNotThrow();
        }

        [TestMethod]
        public void SimpleTest()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new { x = 1, y = 1 })
                .When(given => given.x + given.y)
                .ThenReturns((_, sum) => sum.ShouldBe(2));

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count.ShouldBe(1);

            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count.ShouldBe(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.Description.ShouldBe("sum.ShouldBe(2)");
            ((Action)assertion.Invoke).ShouldNotThrow();
        }

        [TestMethod]
        public void MultiplePrereqsAndAssertions()
        {
            // Arrange
            Test test = TestThat
                .Given(() => 1)
                .And(() => 1)
                .When((x, y) => x + y)
                .ThenReturns((x, _, sum) => sum.ShouldBeGreaterThan(x))
                .And((_, y, sum) => sum.ShouldBeGreaterThan(y));

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count.ShouldBe(1);

            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count.ShouldBe(2);

            var assertion1 = test.Cases.Single().Assertions.First();
            assertion1.Description.ShouldBe("sum.ShouldBeGreaterThan(x)");
            ((Action)assertion1.Invoke).ShouldNotThrow();

            var assertion2 = test.Cases.Single().Assertions.Skip(1).First();
            assertion2.Description.ShouldBe("sum.ShouldBeGreaterThan(y)");
            ((Action)assertion2.Invoke).ShouldNotThrow();
        }

        [TestMethod]
        public void ExpectedExceptionThrown()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new { x = 1, y = 0 })
                .When(given => given.x / given.y)
                .ThenThrows((_, exception) => exception.ShouldBeOfType(typeof(DivideByZeroException)));

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count.ShouldBe(1);

            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count.ShouldBe(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.Description.ShouldBe("exception.ShouldBeOfType(System.DivideByZeroException)");
            ((Action)assertion.Invoke).ShouldNotThrow();
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
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count.ShouldBe(1);

            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count.ShouldBe(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.Description.ShouldBe("Test function should throw an exception");
            ((Action)assertion.Invoke).ShouldNotThrow();
        }

        [TestMethod]
        public void FailingAssertion()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new { x = 1, y = 1 })
                .When(given => given.x + given.y)
                .ThenReturns((_, sum) => sum.ShouldBe(3));

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count.ShouldBe(1);

            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count.ShouldBe(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.Description.ShouldBe("sum.ShouldBe(3)");
            ((Action)assertion.Invoke).ShouldThrow(typeof(ShouldAssertException));
        }

        [TestMethod]
        public void MultipleInvocations()
        {
            Test test = TestThat
                .When(() => 1)
                .ThenReturns(retVal => { }, "Empty assertion");

            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count.ShouldBe(1);

            test.Cases.Single().Act();
            Assert.ThrowsException<InvalidOperationException>(test.Cases.Single().Act);
        }
    }
}
