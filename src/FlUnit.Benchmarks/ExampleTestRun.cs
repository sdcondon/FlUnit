using BenchmarkDotNet.Attributes;
using Example.TestProject;
using FlUnit.Adapters.VSTest;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace FlUnit.Benchmarks
{
    [MemoryDiagnoser]
    [InProcess]
    public class ExampleTestRun
    {
        [Benchmark(Baseline = true)]
        public void Baseline()
        {
            new TestExecutor().RunTests(
                sources: new[] { typeof(ExampleTests).Assembly.Location },
                new FakeRunContext(),
                new FakeFrameworkHandle());
        }

        private class FakeRunContext : IRunContext
        {
            public bool KeepAlive => throw new NotImplementedException();

            public bool InIsolation => throw new NotImplementedException();

            public bool IsDataCollectionEnabled => throw new NotImplementedException();

            public bool IsBeingDebugged => throw new NotImplementedException();

            public string TestRunDirectory => throw new NotImplementedException();

            public string SolutionDirectory => throw new NotImplementedException();

            public IRunSettings RunSettings => null!;

            public ITestCaseFilterExpression GetTestCaseFilter(IEnumerable<string> supportedProperties, Func<string, TestProperty> propertyProvider)
            {
                throw new NotImplementedException();
            }
        }

        private class FakeFrameworkHandle : IFrameworkHandle
        {
            public bool EnableShutdownAfterTestRun
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public int LaunchProcessWithDebuggerAttached(string filePath, string workingDirectory, string arguments, IDictionary<string, string> environmentVariables)
            {
                throw new NotImplementedException();
            }

            public void RecordAttachments(IList<AttachmentSet> attachmentSets)
            {
            }

            public void RecordEnd(TestCase testCase, TestOutcome outcome)
            {
            }

            public void RecordResult(TestResult testResult)
            {
            }

            public void RecordStart(TestCase testCase)
            {
            }

            public void SendMessage(TestMessageLevel testMessageLevel, string message)
            {
            }
        }
    }
}
