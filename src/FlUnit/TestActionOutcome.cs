﻿using System;

namespace FlUnit
{
    /// <summary>
    /// Container for the outcome of the invoking the "When" clause of a test.
    /// </summary>
    // TODO-BREAKING-V2: is this still needed now that When clauses are async by default (so have a ValueTask/Task)?
    // ThrowIfEx/NoEx become static methods on TestFailureException? Changes the args of Then(..) - breaking change.
    public sealed class TestActionOutcome
    {
        internal TestActionOutcome() { }

        internal TestActionOutcome(Exception exception) => Exception = exception;

        /// <summary>
        /// Gets the exception that was thrown, or null.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Throws a <see cref="TestFailureException"/> if the Exception property is populated.
        /// </summary>
        public void ThrowIfException()
        {
            if (Exception != null)
            {
                throw new TestFailureException(string.Format(Messages.TestOutcomeExceptionNotExpectedButThrownFormat, Exception.Message), Exception.StackTrace, Exception);
            }
        }

        /// <summary>
        /// Throws a <see cref="TestFailureException"/> if the outcome indicates that the test action returned successfully.
        /// </summary>
        public void ThrowIfNoException()
        {
            if (Exception == null)
            {
                throw new TestFailureException(Messages.TestOutcomeExceptionExpectedButNotThrown);
            }
        }
    }
}