using Example.TestProject;
using FlUnit.Adapters.VSTest._Tests.TestDoubles;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Linq;

namespace FlUnit.Adapters.VSTest._Tests
{
    [TestClass]
    public class RunExampleTestsByAssembly
    {
        [TestMethod]
        public void Smoke()
        {
            var runner = new TestRunner();
            var runContext = new Mock<IRunContext>();
            var frameworkHandle = new FakeFrameworkHandle();
            runner.RunTests(
                sources: new[] { typeof(ExampleTests).Assembly.Location },
                runContext.Object,
                frameworkHandle);

            AssertTestResult(frameworkHandle, "Example.TestProject.ExampleTests.ProcessHasSideEffects", TestOutcome.Passed, new[]
            {
                ("process.Result.ShouldBeTrue()", TestOutcome.Passed),
                ("sut.HasProcessed.ShouldBeTrue()", TestOutcome.Passed),
                ("collaborator.HasBeenProcessed.ShouldBeTrue()", TestOutcome.Passed)
            });

            AssertTestResult(frameworkHandle, "Example.TestProject.ExampleTests.ProcessHasSideEffects2", TestOutcome.Passed, new[]
            {
                ("process.Result.ShouldBeTrue()", TestOutcome.Passed),
                ("given.sut.HasProcessed.ShouldBeTrue()", TestOutcome.Passed),
                ("given.collaborator.HasBeenProcessed.ShouldBeTrue()", TestOutcome.Passed)
            });

            AssertTestResult(frameworkHandle, "Example.TestProject.ExampleTests.ProcessThrowsOnNullCollaborator", TestOutcome.Passed, new[]
            {
                ((string)null, TestOutcome.Passed)
            });

            AssertTestResult(frameworkHandle, "Example.TestProject.ExampleTests.ProcessDoesntThrowOnNullCollaborator", TestOutcome.Failed, new[]
            {
                ((string)null, TestOutcome.Failed)
            });

            AssertTestResult(frameworkHandle, "Example.TestProject.ExampleTests.ProcessDoesntThrowOnNullCollaborator2", TestOutcome.Skipped, new[]
            {
                ((string)null, TestOutcome.Skipped),
            });

            AssertTestResult(frameworkHandle, "Example.TestProject.ExampleTests.CtorDoesntThrow", TestOutcome.Passed, new[]
            {
                ((string)null, TestOutcome.Passed)
            });

            AssertTestResult(frameworkHandle, "Example.TestProject.ExampleTests.BlockBodies", TestOutcome.Passed, new[]
            {
                ((string)null, TestOutcome.Passed)
            });

            AssertTestResult(frameworkHandle, "Example.TestProject.ExampleTests.SumOfOddAndSixIsOdd", TestOutcome.Passed, new[]
            {
                ("1", TestOutcome.Passed),
                ("3", TestOutcome.Passed),
                ("5", TestOutcome.Passed),
            });

            AssertTestResult(frameworkHandle, "Example.TestProject.ExampleTests.SumOfEvenAndOdd", TestOutcome.Passed, new[]
            {
                ("(addition.Result % 2).ShouldBe(1) for test case (1, 2)", TestOutcome.Passed),
                ("addition.Result.ShouldBeGreaterThan(x) for test case (1, 2)", TestOutcome.Passed),
                ("(addition.Result % 2).ShouldBe(1) for test case (1, 4)", TestOutcome.Passed),
                ("addition.Result.ShouldBeGreaterThan(x) for test case (1, 4)", TestOutcome.Passed),
                ("(addition.Result % 2).ShouldBe(1) for test case (1, 6)", TestOutcome.Passed),
                ("addition.Result.ShouldBeGreaterThan(x) for test case (1, 6)", TestOutcome.Passed),
                ("(addition.Result % 2).ShouldBe(1) for test case (3, 2)", TestOutcome.Passed),
                ("addition.Result.ShouldBeGreaterThan(x) for test case (3, 2)", TestOutcome.Passed),
                ("(addition.Result % 2).ShouldBe(1) for test case (3, 4)", TestOutcome.Passed),
                ("addition.Result.ShouldBeGreaterThan(x) for test case (3, 4)", TestOutcome.Passed),
                ("(addition.Result % 2).ShouldBe(1) for test case (3, 6)", TestOutcome.Passed),
                ("addition.Result.ShouldBeGreaterThan(x) for test case (3, 6)", TestOutcome.Passed),
                ("(addition.Result % 2).ShouldBe(1) for test case (5, 2)", TestOutcome.Passed),
                ("addition.Result.ShouldBeGreaterThan(x) for test case (5, 2)", TestOutcome.Passed),
                ("(addition.Result % 2).ShouldBe(1) for test case (5, 4)", TestOutcome.Passed),
                ("addition.Result.ShouldBeGreaterThan(x) for test case (5, 4)", TestOutcome.Passed),
                ("(addition.Result % 2).ShouldBe(1) for test case (5, 6)", TestOutcome.Passed),
                ("addition.Result.ShouldBeGreaterThan(x) for test case (5, 6)", TestOutcome.Passed),
            });
        }

        private void AssertTestResult(FakeFrameworkHandle handle, string testName, TestOutcome expectedOutcome, IEnumerable<(string, TestOutcome)> expectedResults)
        {
            handle.TestCases.ContainsKey(testName).ShouldBeTrue(testName);
            handle.TestOutcomes[testName].ShouldBe(expectedOutcome);
            handle.TestResults[testName].Count.ShouldBe(expectedResults.Count());
            foreach (var result in handle.TestResults[testName].Zip(expectedResults))
            {
                result.First.DisplayName.ShouldBe(result.Second.Item1);
                result.First.Outcome.ShouldBe(result.Second.Item2);
            }
        }
    }
}
