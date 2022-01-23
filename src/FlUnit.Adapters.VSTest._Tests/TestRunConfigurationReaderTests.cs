using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlUnit.Adapters.VSTest._Tests
{
    [TestClass]
    public class TestRunConfigurationReaderTests
    {
        [TestMethod]
        public void DefaultSettings()
        {
            TestRunConfigurationReader.ReadXml(null, "flunit").Should().BeEquivalentTo(new
            {
                Parallelise = true,
                TestConfiguration = new
                {
                    ArrangementFailureOutcome = TestOutcome.Skipped
                }
            });
        }

        [TestMethod]
        public void Smoke()
        {
            var xml =
                @"<runsettings>
                    <someothersection>hello</someothersection>
                    <flunit>
                      <!-- commenty comment -->
                      <junksetting>i am junk</junksetting>
                      <parallelise>false</parallelise>
                      <testconfiguration>
                        <arrangementfailureoutcome>Failed</arrangementfailureoutcome>
                        <junksetting/>
                      </testconfiguration>
                    </flunit>
                    <parallelise>im not in the right place</parallelise>
                  </runsettings>";

            TestRunConfigurationReader.ReadXml(xml, "flunit").Should().BeEquivalentTo(new
            {
                Parallelise = false,
                TestConfiguration = new
                {
                    ArrangementFailureOutcome = TestOutcome.Failed
                }
            });
        }
    }
}
