using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Linq;

namespace FlUnit._Tests
{
    [TestClass]
    public class EndToEndTests
    {
        [TestMethod]
        public void ValidInvocationOf_MinimalTest()
        {
            // Arrange
            Test test = TestThat
                .When(() => { })
                .Then(a => { }, "Empty assertion");

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count().ShouldBe(1);
            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count().ShouldBe(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.Description.ShouldBe("Empty assertion");
            ((Action)assertion.Invoke).ShouldNotThrow();
        }

        [TestMethod]
        public void ValidInvocationOf_SimpleTest()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new { x = 1, y = 1 })
                .When(given => given.x + given.y)
                .Then((given, sum) => sum.Result.ShouldBe(2));

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count().ShouldBe(1);
            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count().ShouldBe(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.Description.ShouldBe("sum.Result.ShouldBe(2)");
            ((Action)assertion.Invoke).ShouldNotThrow();
        }

        [TestMethod]
        public void ValidInvocationOf_TestWithMultiplePrereqsAndAssertions()
        {
            // Arrange
            Test test = TestThat
                .Given(() => 1)
                .And(() => 1)
                .When((x, y) => x + y)
                .Then((x, y, sum) => sum.Result.ShouldBeGreaterThan(x))
                .And((x, y, sum) => sum.Result.ShouldBeGreaterThan(y));

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count().ShouldBe(1);
            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count().ShouldBe(2);

            var assertion1 = test.Cases.Single().Assertions.First();
            assertion1.Description.ShouldBe("sum.Result.ShouldBeGreaterThan(x)");
            ((Action)assertion1.Invoke).ShouldNotThrow();

            var assertion2 = test.Cases.Single().Assertions.Skip(1).First();
            assertion2.Description.ShouldBe("sum.Result.ShouldBeGreaterThan(y)");
            ((Action)assertion2.Invoke).ShouldNotThrow();
        }

        [TestMethod]
        public void ValidInvocationOf_TestWithExceptionExpected()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new { x = 1, y = 0 })
                .When(given => given.x / given.y)
                .Then((given, division) => division.Exception.ShouldBeOfType(typeof(DivideByZeroException)));

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count().ShouldBe(1);
            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count().ShouldBe(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.Description.ShouldBe("division.Exception.ShouldBeOfType(System.DivideByZeroException)");
            ((Action)assertion.Invoke).ShouldNotThrow();
        }

        [TestMethod]
        public void ValidInvocationOf_TestWithFailingAssertion()
        {
            // Arrange
            Test test = TestThat
                .Given(() => new { x = 1, y = 1 })
                .When(given => given.x + given.y)
                .Then((given, sum) => sum.Result.ShouldBe(3));

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count().ShouldBe(1);
            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count().ShouldBe(1);

            var assertion = test.Cases.Single().Assertions.Single();
            assertion.Description.ShouldBe("sum.Result.ShouldBe(3)");
            ((Action)assertion.Invoke).ShouldThrow(typeof(ShouldAssertException));
        }

        [TestMethod]
        public void InvalidInvocation_MultipleTimes()
        {
            Test test = TestThat
                .When(() => { })
                .Then(a => { }, "Empty assertion");

            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count().ShouldBe(1);
            test.Cases.Single().Act();
            Assert.ThrowsException<InvalidOperationException>(test.Cases.Single().Act);
        }
    }
}
