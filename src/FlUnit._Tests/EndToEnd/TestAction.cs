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
        public void ComplexTest()
        {
            // Arrange: multiple prerequisites and assertions; also explicit assertion labels
            Test test = TestThat
                .Given(() => new StringBuilder())
                .And(() => "A")
                .When((sb, str) => { sb.Append(str); })
                .ThenReturns((sb, str) => sb.Length.ShouldBe(str.Length), "Length should be correct")
                .And((sb, str) => sb.Capacity.ShouldBeGreaterThanOrEqualTo(sb.Length), "Capacity should be consistent");

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count.ShouldBe(1);

            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count.ShouldBe(2);

            var assertion1 = test.Cases.Single().Assertions.First();
            assertion1.Description.ShouldBe("Length should be correct");
            ((Action)assertion1.Invoke).ShouldNotThrow();

            var assertion2 = test.Cases.Single().Assertions.Skip(1).First();
            assertion2.Description.ShouldBe("Capacity should be consistent");
            ((Action)assertion2.Invoke).ShouldNotThrow();
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
                        outcome.Exception.ShouldBeNull();
                    }
                    else
                    {
                        outcome.Exception.ShouldBeOfType(typeof(ArgumentOutOfRangeException));
                    }
                }, "Outcome should be as expected");

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count.ShouldBe(3);

            var case1 = test.Cases.First();
            ((Action)case1.Act).ShouldNotThrow();
            case1.Assertions.Count.ShouldBe(1);
            case1.Assertions.Single().Description.ShouldBe("Outcome should be as expected");
            ((Action)case1.Assertions.Single().Invoke).ShouldNotThrow();

            var case2 = test.Cases.Skip(1).First();
            ((Action)case2.Act).ShouldNotThrow();
            case2.Assertions.Count.ShouldBe(1);
            case2.Assertions.Single().Description.ShouldBe("Outcome should be as expected");
            ((Action)case2.Assertions.Single().Invoke).ShouldNotThrow();

            var case3 = test.Cases.Skip(2).First();
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
                .Given(() => new StringBuilder(capacity: 0, maxCapacity: 1))
                .When(sb => { sb.Append("AB"); })
                .ThenThrows((_, exception) => exception.ShouldBeOfType(typeof(ArgumentOutOfRangeException)));

            // Act & Assert
            ((Action)test.Arrange).ShouldNotThrow();
            test.Cases.Count.ShouldBe(1);

            ((Action)test.Cases.Single().Act).ShouldNotThrow();
            test.Cases.Single().Assertions.Count.ShouldBe(1);

            var assertion = test.Cases.Single().Assertions.Single();
#if NET6_0
            assertion.Description.ShouldBe("exception.ShouldBeOfType<DivideByZeroException>()");
#else // Example of LINQ not being a great solution - round trip..
            assertion.Description.ShouldBe("exception.ShouldBeOfType()");
#endif
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
