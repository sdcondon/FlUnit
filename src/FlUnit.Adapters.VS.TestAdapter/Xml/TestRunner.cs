using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Xml;

namespace FlUnit.Adapters.VSTest.Xml
{   
    [FileExtension(".xml")]
    [DefaultExecutorUri(ExecutorUriString)]
    [ExtensionUri(ExecutorUriString)]
    public class TestRunner : ITestDiscoverer, ITestExecutor
    {
        public const string ExecutorUriString = "executor://XmlTestExecutor";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);

        private bool isCancelled = false;

        public void DiscoverTests(
            IEnumerable<string> sources,
            IDiscoveryContext discoveryContext,
            IMessageLogger logger,
            ITestCaseDiscoverySink discoverySink)
        {
            GetTests(sources, discoveryContext).ForEach(tc => discoverySink.SendTestCase(tc));
        }

        public void RunTests(
            IEnumerable<string> sources,
            IRunContext runContext,
            IFrameworkHandle frameworkHandle)
        {
            RunTests(GetTests(sources, runContext), runContext, frameworkHandle);
        }

        public void RunTests(
            IEnumerable<TestCase> tests,
            IRunContext runContext,
            IFrameworkHandle frameworkHandle)
        {
            isCancelled = false;

            foreach (TestCase test in tests)
            {
                if (isCancelled)
                {
                    break;
                }

                var testResult = new TestResult(test);

                testResult.Outcome = (TestOutcome)test.GetPropertyValue(TestResultProperties.Outcome);
                frameworkHandle.RecordResult(testResult);
            }
        }

        public void Cancel()
        {
            isCancelled = true;
        }

        private static List<TestCase> GetTests(
            IEnumerable<string> sources,
            IDiscoveryContext discoveryContext)
        {
            List<TestCase> tests = new List<TestCase>();
            foreach (string source in sources)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(source);

                var nodes = doc.SelectNodes("//Tests/Test");
                foreach (XmlNode node in nodes)
                {
                    XmlAttribute nameAttribute = node.Attributes["name"];
                    if (nameAttribute != null && !string.IsNullOrWhiteSpace(nameAttribute.Value))
                    {
                        var testCase = new TestCase(nameAttribute.Value, ExecutorUri, source)
                        {
                            CodeFilePath = source,
                        };

                        Enum.TryParse<TestOutcome>(node.Attributes["outcome"].Value, out var outcome);
                        testCase.SetPropertyValue(TestResultProperties.Outcome, outcome);

                        tests.Add(testCase);
                    }
                }
            }

            return tests;
        }
    }
}
