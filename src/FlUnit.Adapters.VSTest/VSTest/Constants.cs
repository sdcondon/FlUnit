using System;

namespace FlUnit.Adapters.VSTest
{
    internal static class Constants
    {
        internal const string ExecutorUriString = "executor://FlUnitTestRunner/v0";

        internal static readonly Uri ExecutorUri = new Uri(ExecutorUriString);
    }
}
