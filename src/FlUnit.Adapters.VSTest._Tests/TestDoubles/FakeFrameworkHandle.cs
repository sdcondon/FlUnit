using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;

namespace FlUnit.Adapters.VSTest._Tests.TestDoubles
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using System.Collections.Concurrent;

    class FakeFrameworkHandle : IFrameworkHandle
    {
        public ConcurrentDictionary<string, TestCase> TestCases { get; } = new ConcurrentDictionary<string, TestCase>();
        public ConcurrentDictionary<string, TestOutcome> TestOutcomes { get; } = new ConcurrentDictionary<string, TestOutcome>();
        public ConcurrentDictionary<string, IList<TestResult>> TestResults { get; } = new ConcurrentDictionary<string, IList<TestResult>>();

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
            throw new NotImplementedException();
        }

        public void RecordEnd(TestCase testCase, TestOutcome outcome)
        {
            TestOutcomes[testCase.DisplayName] = outcome;
        }

        public void RecordResult(TestResult testResult)
        {
            TestResults[testResult.TestCase.DisplayName].Add(testResult);
        }

        public void RecordStart(TestCase testCase)
        {
            TestCases.TryAdd(testCase.DisplayName, testCase);
            TestResults.TryAdd(testCase.DisplayName, new List<TestResult>());
        }

        public void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
        }
    }
}
