﻿using System.Collections.Generic;

namespace FlUnit
{
    /// <summary>
    /// Abstraction for types representing a runnable test.
    /// </summary>
    /// <remarks>
    /// NB: This is an abstract class rather than an interface to facilitate implicit conversions from builders.
    /// </remarks>
    public abstract class Test
    {
        /// <summary>
        /// A collection of test cases that should be populated once <see cref="Arrange"/> is called.
        /// </summary>
        public abstract IReadOnlyCollection<ITestCase> Cases { get; protected set; }

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public abstract void Arrange();
    }
}
