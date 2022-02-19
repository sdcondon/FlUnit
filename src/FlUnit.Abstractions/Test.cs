using FlUnit.Configuration;
using System;
using System.Collections.Generic;

namespace FlUnit
{
    /// <summary>
    /// Abstraction for types representing a runnable test.
    /// </summary>
    /// <remarks>
    /// NB: This is an abstract class rather than an interface to facilitate implicit conversions from builders.
    /// </remarks>
    public abstract class Test : IDisposable
    {
        /// <summary>
        /// Gets a collection of test cases that should be populated once <see cref="Arrange"/> is called.
        /// </summary>
        public abstract IReadOnlyCollection<ITestCase> Cases { get; }

        /// <summary>
        /// Gets a value indicating whether this test has any configuration overrides to apply.
        /// </summary>
        /// <remarks>
        /// NB: While we *could* support configuration overrides with a single method - it'd require either a configuration implementation
        /// in the FlUnit library (which isn't ideal - configuration is an execution concern) so that it can be cloned, or would require
        /// configuration as a struct (which definitely isn't ideal). So we have a separate property instead, so that the execution logic
        /// can determine whether it needs to clone the configuration or not..
        /// </remarks>
        public abstract bool HasConfigurationOverrides { get; }

        /// <summary>
        /// Applies any appropriate configuration overrides for this test.
        /// </summary>
        /// <param name="testConfiguration">The test configuration object to apply the overrides to.</param>
        public abstract void ApplyConfigurationOverrides(ITestConfiguration testConfiguration);

        /// <summary>
        /// Arranges the test - populating the <see cref="Cases"/> property.
        /// </summary>
        public abstract void Arrange();

        /// <inheritdoc/>
        public abstract void Dispose();
    }
}
