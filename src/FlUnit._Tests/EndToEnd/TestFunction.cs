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
                        outcome.Result.ShouldBeGreaterThan(0);
                    }
                    else
                    {
                        outcome.Exception.ShouldBeOfType<DivideByZeroException>();
                    }
                }, "Outcome should be as expected");

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count.ShouldBe(3);

            var case1 = test.Cases.First();
            case1.Description.ShouldBe("2");
            ((Action)case1.Act).ShouldNotThrow();
            case1.Assertions.Count.ShouldBe(1);
            case1.Assertions.Single().Description.ShouldBe("Outcome should be as expected");
            ((Action)case1.Assertions.Single().Invoke).ShouldNotThrow();

            var case2 = test.Cases.Skip(1).First();
            case2.Description.ShouldBe("1");
            ((Action)case2.Act).ShouldNotThrow();
            case2.Assertions.Count.ShouldBe(1);
            case2.Assertions.Single().Description.ShouldBe("Outcome should be as expected");
            ((Action)case2.Assertions.Single().Invoke).ShouldNotThrow();

            var case3 = test.Cases.Skip(2).First();
            case3.Description.ShouldBe("0");
            ((Action)case3.Act).ShouldNotThrow();
            case3.Assertions.Count.ShouldBe(1);
            case3.Assertions.Single().Description.ShouldBe("Outcome should be as expected");
            ((Action)case3.Assertions.Single().Invoke).ShouldNotThrow();
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
