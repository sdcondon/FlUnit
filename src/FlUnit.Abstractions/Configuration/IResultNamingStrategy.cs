﻿namespace FlUnit.Configuration
{
    /// <summary>
    /// Interface for types that provide a strategy for labelling individual FlUnit test results;
    /// </summary>
    public interface IResultNamingStrategy
    {
        /// <inheritdoc />
        string GetResultName(Test test, ITestCase testCase, ITestAssertion assertion);
    }
}
