using Example.TestProject;
using FluentAssertions;
using FlUnit.Adapters.VSTest._Tests.TestDoubles;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace FlUnit.Adapters.VSTest._Tests
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

    [TestClass]
    public class RunExampleTestsByAssembly
    {
        [TestMethod]
        public void Smoke()
        {
            var runner = new TestExecutor();
            var runContext = new Mock<IRunContext>();
            var frameworkHandle = new FakeFrameworkHandle();

            void AssertTestResult(string testName, IEnumerable<Trait> expectedTraits, TestOutcome expectedOutcome, IEnumerable<object> expectedResults)
            {
                frameworkHandle.TestCases.ContainsKey(testName).Should().BeTrue();
                frameworkHandle.TestCases[testName].Traits.Should().BeEquivalentTo(expectedTraits);
                frameworkHandle.TestOutcomes[testName].Should().Be(expectedOutcome);
                frameworkHandle.TestResults[testName].Should().BeEquivalentTo(expectedResults);
            }

            // Act
            runner.RunTests(
                sources: new[] { typeof(ExampleTests).Assembly.Location },
                runContext.Object,
                frameworkHandle);

            // Assert
            AssertTestResult(
                "Example.TestProject.ExampleTests.ProcessingOfCollaborator",
                new[]
                {
                    new Trait("ClassLevelTrait", "ExampleTests")
                },
                TestOutcome.Passed,
                new[]
                {
                    new { DisplayName = "retVal.Should().BeTrue()", Outcome = TestOutcome.Passed },
                    new { DisplayName = "sut.HasProcessed.Should().BeTrue()", Outcome = TestOutcome.Passed },
                    new { DisplayName = "collaborator.HasBeenProcessed.Should().BeTrue()", Outcome = TestOutcome.Passed },
                });

            AssertTestResult(
                "Example.TestProject.ExampleTests.ProcessingOfCollaborator_ButPrettier",
                new[]
                {
                    new Trait("ClassLevelTrait", "ExampleTests")
                },
                TestOutcome.Passed,
                new[]
                {
                    new { DisplayName = "Test function should return successfully", Outcome = TestOutcome.Passed },
                    new { DisplayName = "retVal.Should().BeTrue()", Outcome = TestOutcome.Passed },
                    new { DisplayName = "given.sut.HasProcessed.Should().BeTrue()", Outcome = TestOutcome.Passed },
                    new { DisplayName = "given.collaborator.HasBeenProcessed.Should().BeTrue()", Outcome = TestOutcome.Passed },
                });

            AssertTestResult(
                "Example.TestProject.ExampleTests.ProcessThrowsOnNullCollaborator",
                new[]
                {
                    new Trait("ClassLevelTrait", "ExampleTests")
                },
                TestOutcome.Passed,
                new[]
                {
                    new { DisplayName = (string)null, Outcome = TestOutcome.Passed },
                });

            AssertTestResult(
                "Example.TestProject.ExampleTests.ProcessReturnsTrueOnNullCollaborator",
                new[]
                {
                    new Trait("ClassLevelTrait", "ExampleTests"),
                    new Trait("ExampleOfAFailingTest", null)
                },
                TestOutcome.Failed,
                new[]
                {
                    new { DisplayName = (string)null, Outcome = TestOutcome.Failed },
                });

            AssertTestResult(
                "Example.TestProject.ExampleTests.ProcessThrowsOnNonNullCollaborator",
                new[]
                {
                    new Trait("ClassLevelTrait", "ExampleTests"),
                    new Trait("ExampleOfAFailingTest", null)
                },
                TestOutcome.Failed,
                new[]
                {
                    new { DisplayName = (string)null, Outcome = TestOutcome.Failed },
                });


            AssertTestResult(
                "Example.TestProject.ExampleTests.ProcessReturnsFalseOnNonNullCollaborator",
                new[]
                {
                    new Trait("ClassLevelTrait", "ExampleTests"),
                    new Trait("ExampleOfAFailingTest", null)
                },
                TestOutcome.Failed,
                new[]
                {
                    new { DisplayName = (string)null, Outcome = TestOutcome.Failed },
                });

            AssertTestResult(
                "Example.TestProject.ExampleTests.ProcessDoesntThrowOnNullCollaborator2",
                new[]
                {
                    new Trait("ClassLevelTrait", "ExampleTests"),
                    new Trait("ExampleOfAFailingTest", null)
                },
                TestOutcome.Skipped,
                new[]
                {
                    new { DisplayName = (string)null, Outcome = TestOutcome.Skipped },
                });

            AssertTestResult(
                "Example.TestProject.ExampleTests.CtorDoesntThrow",
                new[]
                {
                    new Trait("ClassLevelTrait", "ExampleTests")
                },
                TestOutcome.Passed,
                new[]
                {
                    new { DisplayName = (string)null, Outcome = TestOutcome.Passed },
                });

            AssertTestResult(
                "Example.TestProject.ExampleTests.Nothing",
                new[]
                {
                    new Trait("ClassLevelTrait", "ExampleTests")
                },
                TestOutcome.Passed,
                new[]
                {
                    new { DisplayName = (string)null, Outcome = TestOutcome.Passed },
                });

            AssertTestResult(
                "Example.TestProject.ExampleTests.BlockBodies",
                new[]
                {
                    new Trait("ClassLevelTrait", "ExampleTests")
                },
                TestOutcome.Passed,
                new[]
                {
                    new { DisplayName = (string)null, Outcome = TestOutcome.Passed },
                });

            AssertTestResult(
                "Example.TestProject.ExampleTests.SumOfOddAndSixIsOdd",
                new[]
                {
                    new Trait("ClassLevelTrait", "ExampleTests")
                },
                TestOutcome.Passed,
                new[]
                {
                    new { DisplayName = "1", Outcome = TestOutcome.Passed },
                    new { DisplayName = "3", Outcome = TestOutcome.Passed },
                    new { DisplayName = "5", Outcome = TestOutcome.Passed },
                });

            AssertTestResult(
                "Example.TestProject.ExampleTests.SumOfEvenAndOdd",
                new[]
                {
                    new Trait("ClassLevelTrait", "ExampleTests")
                },
                TestOutcome.Passed,
                new[]
                {
                    new { DisplayName = "Test function should return successfully for test case (1, 2)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "(sum % 2).Should().Be(1) for test case (1, 2)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "sum.Should().BeGreaterThan(x) for test case (1, 2)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "Test function should return successfully for test case (1, 4)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "(sum % 2).Should().Be(1) for test case (1, 4)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "sum.Should().BeGreaterThan(x) for test case (1, 4)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "Test function should return successfully for test case (1, 6)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "(sum % 2).Should().Be(1) for test case (1, 6)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "sum.Should().BeGreaterThan(x) for test case (1, 6)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "Test function should return successfully for test case (3, 2)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "(sum % 2).Should().Be(1) for test case (3, 2)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "sum.Should().BeGreaterThan(x) for test case (3, 2)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "Test function should return successfully for test case (3, 4)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "(sum % 2).Should().Be(1) for test case (3, 4)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "sum.Should().BeGreaterThan(x) for test case (3, 4)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "Test function should return successfully for test case (3, 6)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "(sum % 2).Should().Be(1) for test case (3, 6)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "sum.Should().BeGreaterThan(x) for test case (3, 6)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "Test function should return successfully for test case (5, 2)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "(sum % 2).Should().Be(1) for test case (5, 2)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "sum.Should().BeGreaterThan(x) for test case (5, 2)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "Test function should return successfully for test case (5, 4)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "(sum % 2).Should().Be(1) for test case (5, 4)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "sum.Should().BeGreaterThan(x) for test case (5, 4)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "Test function should return successfully for test case (5, 6)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "(sum % 2).Should().Be(1) for test case (5, 6)", Outcome = TestOutcome.Passed },
                    new { DisplayName = "sum.Should().BeGreaterThan(x) for test case (5, 6)", Outcome = TestOutcome.Passed },
                });
        }
    }
}
