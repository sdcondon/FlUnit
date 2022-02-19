using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlUnit.Adapters.VSTest._Tests
{
    [TestClass]
    public class TestRunConfigurationTests
    {
        private static readonly object ExpectedDefaultSettings = new
        {
            Parallelise = true,
            TestConfiguration = new
            {
                ArrangementFailureCountsAsFailed = false
            }
        };

        [TestMethod]
        public void DefaultSettings()
        {
            TestRunConfiguration.ReadFromXml(null, "flunit").Should().BeEquivalentTo(ExpectedDefaultSettings);
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
                        <ArrangementFailureCountsAsFailed>true</ArrangementFailureCountsAsFailed>
                        <junksetting/>
                      </testconfiguration>
                    </flunit>
                    <parallelise>im not in the right place</parallelise>
                  </runsettings>";

            TestRunConfiguration.ReadFromXml(xml, "flunit").Should().BeEquivalentTo(new
            {
                Parallelise = false,
                TestConfiguration = new
                {
                    ArrangementFailureCountsAsFailed = true
                }
            });
        }

        [TestMethod]
        public void DoesntContainRightSection()
        {
            var xml =
                @"<runsettings>
                    <someothersection>hello</someothersection>
                    <notflunit>
                      <parallelise>false</parallelise>
                    </notflunit>
                    <parallelise>im not in the right place</parallelise>
                  </runsettings>";

            TestRunConfiguration.ReadFromXml(xml, "flunit").Should().BeEquivalentTo(ExpectedDefaultSettings);
        }
    }
}
