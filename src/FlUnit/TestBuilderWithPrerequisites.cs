using FlUnit.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlUnit
{
	/// <summary>
    /// Test builder for which the "When" clause has not yet been provided - and as such can still be given more pre-requisites
    /// (and configuration overrides).
    /// </summary>
	public sealed class TestBuilderWithPrerequisites
	{
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;

		internal TestBuilderWithPrerequisites(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides)          
        {
            this.configurationOverrides = configurationOverrides;
        }

        /// <summary>
        /// Adds a configuration override for the test.
        /// </summary>
        /// <param name="configurationOverride">The configuration override to be added.</param>
        public TestBuilderWithPrerequisites UsingConfiguration(Action<ITestConfiguration> configurationOverride)
        {
            // NB: We define a new enumerable and builder when overrides are added to facilitate builder re-use.
            // However, only create the new enumerable if an override is added (as opposed to in the ctor), to cut down on needless GC load.
            var overrides = new List<Action<ITestConfiguration>>(configurationOverrides);
            overrides.Add(configurationOverride);

            return new TestBuilderWithPrerequisites(overrides);
        }

        /// <summary>
        /// Adds the first "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T1">The type of the pre-requisite.</typeparam>
        /// <param name="prereq1">The pre-requisite.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestBuilderWithPrerequisites<T1> Given<T1>(Func<T1> prereq1)
        {
            return new TestBuilderWithPrerequisites<T1>(configurationOverrides, () => new[] { prereq1() });
        }

        /// <summary>
        /// Adds the first "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T1">The type of the pre-requisite.</typeparam>
        /// <param name="prereqs1">The pre-requisites, one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestBuilderWithPrerequisites<T1> GivenEachOf<T1>(Func<IEnumerable<T1>> prereqs1)
        {
            return new TestBuilderWithPrerequisites<T1>(configurationOverrides, prereqs1);
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="testAction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithAction When(Action testAction)
        {
            return new TestBuilderWithAction(configurationOverrides, testAction);
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithFunction<TResult> When<TResult>(Func<TResult> testFunction)
        {
            return new TestBuilderWithFunction<TResult>(configurationOverrides, testFunction);
        }
	}

	/// <summary>
    /// Test builder for which the "When" clause has not yet been provided - and as such can still be given more pre-requisites
    /// (and configuration overrides).
    /// </summary>
    /// <typeparam name="T1">The type of the 1st pre-requisite already defined.</typeparam>
	public sealed class TestBuilderWithPrerequisites<T1>
	{
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<IEnumerable<T1>> arrange;

		internal TestBuilderWithPrerequisites(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,          
            Func<IEnumerable<T1>> arrange)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
        }

        /// <summary>
        /// Adds a configuration override for the test.
        /// </summary>
        /// <param name="configurationOverride">The configuration override to be added.</param>
        public TestBuilderWithPrerequisites<T1> UsingConfiguration(Action<ITestConfiguration> configurationOverride)
        {
            // NB: We define a new enumerable and builder when overrides are added to facilitate builder re-use.
            // However, only create the new enumerable if an override is added (as opposed to in the ctor), to cut down on needless GC load.
            var overrides = new List<Action<ITestConfiguration>>(configurationOverrides);
            overrides.Add(configurationOverride);

            return new TestBuilderWithPrerequisites<T1>(overrides, arrange);
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T2">The type of the pre-requisite.</typeparam>
        /// <param name="prereq2">The pre-requisite.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestBuilderWithPrerequisites<T1, T2> And<T2>(Func<T2> prereq2)
        {
            return new TestBuilderWithPrerequisites<T1, T2>(configurationOverrides, (arrange, () => new[] { prereq2() }));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T2">The type of the pre-requisite.</typeparam>
        /// <param name="prereqs2">The pre-requisites, one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestBuilderWithPrerequisites<T1, T2> AndEachOf<T2>(Func<IEnumerable<T2>> prereqs2)
        {
            return new TestBuilderWithPrerequisites<T1, T2>(configurationOverrides, (arrange, prereqs2));
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="testAction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithAction<T1> When(Action<T1> testAction)
        {
            return new TestBuilderWithAction<T1>(configurationOverrides, arrange, testAction);
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithFunction<T1, TResult> When<TResult>(Func<T1, TResult> testFunction)
        {
            return new TestBuilderWithFunction<T1, TResult>(configurationOverrides, arrange, testFunction);
        }
	}

	/// <summary>
    /// Test builder for which the "When" clause has not yet been provided - and as such can still be given more pre-requisites
    /// (and configuration overrides).
    /// </summary>
    /// <typeparam name="T1">The type of the 1st pre-requisite already defined.</typeparam>
    /// <typeparam name="T2">The type of the 2nd pre-requisite already defined.</typeparam>
	public sealed class TestBuilderWithPrerequisites<T1, T2>
	{
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange;

		internal TestBuilderWithPrerequisites(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,          
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
        }

        /// <summary>
        /// Adds a configuration override for the test.
        /// </summary>
        /// <param name="configurationOverride">The configuration override to be added.</param>
        public TestBuilderWithPrerequisites<T1, T2> UsingConfiguration(Action<ITestConfiguration> configurationOverride)
        {
            // NB: We define a new enumerable and builder when overrides are added to facilitate builder re-use.
            // However, only create the new enumerable if an override is added (as opposed to in the ctor), to cut down on needless GC load.
            var overrides = new List<Action<ITestConfiguration>>(configurationOverrides);
            overrides.Add(configurationOverride);

            return new TestBuilderWithPrerequisites<T1, T2>(overrides, (arrange.Item1, arrange.Item2));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T3">The type of the pre-requisite.</typeparam>
        /// <param name="prereq3">The pre-requisite.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestBuilderWithPrerequisites<T1, T2, T3> And<T3>(Func<T3> prereq3)
        {
            return new TestBuilderWithPrerequisites<T1, T2, T3>(configurationOverrides, (arrange.Item1, arrange.Item2, () => new[] { prereq3() }));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T3">The type of the pre-requisite.</typeparam>
        /// <param name="prereqs3">The pre-requisites, one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestBuilderWithPrerequisites<T1, T2, T3> AndEachOf<T3>(Func<IEnumerable<T3>> prereqs3)
        {
            return new TestBuilderWithPrerequisites<T1, T2, T3>(configurationOverrides, (arrange.Item1, arrange.Item2, prereqs3));
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="testAction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithAction<T1, T2> When(Action<T1, T2> testAction)
        {
            return new TestBuilderWithAction<T1, T2>(configurationOverrides, arrange, testAction);
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithFunction<T1, T2, TResult> When<TResult>(Func<T1, T2, TResult> testFunction)
        {
            return new TestBuilderWithFunction<T1, T2, TResult>(configurationOverrides, arrange, testFunction);
        }
	}

	/// <summary>
    /// Test builder for which the "When" clause has not yet been provided - and as such can still be given more pre-requisites
    /// (and configuration overrides).
    /// </summary>
    /// <typeparam name="T1">The type of the 1st pre-requisite already defined.</typeparam>
    /// <typeparam name="T2">The type of the 2nd pre-requisite already defined.</typeparam>
    /// <typeparam name="T3">The type of the 3rd pre-requisite already defined.</typeparam>
	public sealed class TestBuilderWithPrerequisites<T1, T2, T3>
	{
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange;

		internal TestBuilderWithPrerequisites(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,          
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
        }

        /// <summary>
        /// Adds a configuration override for the test.
        /// </summary>
        /// <param name="configurationOverride">The configuration override to be added.</param>
        public TestBuilderWithPrerequisites<T1, T2, T3> UsingConfiguration(Action<ITestConfiguration> configurationOverride)
        {
            // NB: We define a new enumerable and builder when overrides are added to facilitate builder re-use.
            // However, only create the new enumerable if an override is added (as opposed to in the ctor), to cut down on needless GC load.
            var overrides = new List<Action<ITestConfiguration>>(configurationOverrides);
            overrides.Add(configurationOverride);

            return new TestBuilderWithPrerequisites<T1, T2, T3>(overrides, (arrange.Item1, arrange.Item2, arrange.Item3));
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="testAction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithAction<T1, T2, T3> When(Action<T1, T2, T3> testAction)
        {
            return new TestBuilderWithAction<T1, T2, T3>(configurationOverrides, arrange, testAction);
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithFunction<T1, T2, T3, TResult> When<TResult>(Func<T1, T2, T3, TResult> testFunction)
        {
            return new TestBuilderWithFunction<T1, T2, T3, TResult>(configurationOverrides, arrange, testFunction);
        }
	}
}