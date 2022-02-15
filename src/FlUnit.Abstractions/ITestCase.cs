﻿using System;
using System.Collections.Generic;

namespace FlUnit
{
    /// <summary>
    /// Interface for types representing an individual test case.
    /// </summary>
    public interface ITestCase : IFormattable
    {
        /// <summary>
        /// Named assertions that should all succeed (that is, not throw) once <see cref="Act"/> has been invoked.
        /// </summary>
        IReadOnlyCollection<ITestAssertion> Assertions { get; }

        /// <summary>
        /// Invokes the test action.
        /// </summary>
        void Act();
    }
}
