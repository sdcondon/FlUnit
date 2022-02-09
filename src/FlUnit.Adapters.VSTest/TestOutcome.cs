namespace FlUnit.Adapters
{
    /// <summary>
    /// FlUnit's very own enumeration for test outcomes.
    /// </summary>
    public enum TestOutcome
    {
        /// <summary>
        /// The test passed.
        /// </summary>
        Passed,

        /// <summary>
        /// The test failed - either because an assertion failed, OR because one of the "Given" clauses threw
        /// and test configuration specifies that this should be recorded simply as a failure.
        /// </summary>
        Failed,

        /// <summary>
        /// The "When" clause of the test could not be invoked because one of the "Given" clauses threw,
        /// and test configuration specifies that this shouldn't be marked simply as a failure.
        /// </summary>
        ArrangementFailed
    }
}
