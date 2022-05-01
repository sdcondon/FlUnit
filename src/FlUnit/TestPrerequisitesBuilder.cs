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
	public sealed class TestPrerequisitesBuilder
	{
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;

		internal TestPrerequisitesBuilder(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides)          
        {
            this.configurationOverrides = configurationOverrides;
        }

        /// <summary>
        /// Adds a configuration override for the test.
        /// </summary>
        /// <param name="configurationOverride">The configuration override to be added.</param>
        public TestPrerequisitesBuilder UsingConfiguration(Action<ITestConfiguration> configurationOverride)
        {
            // NB: We define a new enumerable and builder when overrides are added to facilitate builder re-use.
            // However, only create the new enumerable if an override is added (as opposed to in the ctor), to cut down on needless GC load.
            var overrides = new List<Action<ITestConfiguration>>(configurationOverrides);
            overrides.Add(configurationOverride);

            return new TestPrerequisitesBuilder(overrides);
        }

        /// <summary>
        /// Adds the first "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T1">The type of the pre-requisite.</typeparam>
        /// <param name="prereq1">A delegate for obtaining the pre-requisite.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1> Given<T1>(Func<T1> prereq1)
        {
            return new TestPrerequisitesBuilder<T1>(configurationOverrides, new SinglePrerequisiteClosure<T1>(tc => prereq1()).Arrange); // TODO - compiler generated closure bad for stack trace
        }

        /// <summary>
        /// Adds the first "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T1">The type of the pre-requisite.</typeparam>
        /// <param name="prereq1">
        /// A delegate for obtaining the pre-requisite.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1> Given<T1>(Func<ITestContext, T1> prereq1)
        {
            return new TestPrerequisitesBuilder<T1>(configurationOverrides, new SinglePrerequisiteClosure<T1>(prereq1).Arrange);
        }
                
        /// <summary>
        /// Adds the first "Given" clause for the test - that defines the test context as the prerequisite. This is just a more readable alias of:
        /// <code>
        /// Given(ctx => ctx)
        /// </code>
        /// </summary>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<ITestContext> GivenTestContext()
        {
            return new TestPrerequisitesBuilder<ITestContext>(configurationOverrides, new SinglePrerequisiteClosure<ITestContext>(ctx => ctx).Arrange); // TODO - compiler generated closure bad for stack trace
        }

        /// <summary>
        /// Adds the first "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T1">The type of the pre-requisite.</typeparam>
        /// <param name="prereqs1">A delegate for obtaining the pre-requisites, one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1> GivenEachOf<T1>(Func<IEnumerable<T1>> prereqs1)
        {
            return new TestPrerequisitesBuilder<T1>(configurationOverrides, tc => prereqs1());
        }

        /// <summary>
        /// Adds the first "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T1">The type of the pre-requisite.</typeparam>
        /// <param name="prereqs1">
        /// A delegate for obtaining the pre-requisites, one for each test case.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1> GivenEachOf<T1>(Func<ITestContext, IEnumerable<T1>> prereqs1)
        {
            return new TestPrerequisitesBuilder<T1>(configurationOverrides, prereqs1);
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="testAction">The action that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public ActionTestBuilder When(Action testAction)
        {
            return new ActionTestBuilder(configurationOverrides, testAction);
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public FunctionTestBuilder<TResult> When<TResult>(Func<TResult> testFunction)
        {
            return new FunctionTestBuilder<TResult>(configurationOverrides, testFunction);
        }
	}

	/// <summary>
    /// Test builder for which the "When" clause has not yet been provided - and as such can still be given more pre-requisites
    /// (and configuration overrides).
    /// </summary>
    /// <typeparam name="T1">The type of the 1st pre-requisite already defined.</typeparam>
	public sealed class TestPrerequisitesBuilder<T1>
	{
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<ITestContext, IEnumerable<T1>> arrange;

		internal TestPrerequisitesBuilder(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,          
            Func<ITestContext, IEnumerable<T1>> arrange)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
        }

        /// <summary>
        /// Adds a configuration override for the test.
        /// </summary>
        /// <param name="configurationOverride">The configuration override to be added.</param>
        public TestPrerequisitesBuilder<T1> UsingConfiguration(Action<ITestConfiguration> configurationOverride)
        {
            // NB: We define a new enumerable and builder when overrides are added to facilitate builder re-use.
            // However, only create the new enumerable if an override is added (as opposed to in the ctor), to cut down on needless GC load.
            var overrides = new List<Action<ITestConfiguration>>(configurationOverrides);
            overrides.Add(configurationOverride);

            return new TestPrerequisitesBuilder<T1>(overrides, arrange);
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T2">The type of the pre-requisite.</typeparam>
        /// <param name="prereq2">A delegate for obtaining the pre-requisite.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2> And<T2>(Func<T2> prereq2)
        {
            return new TestPrerequisitesBuilder<T1, T2>(configurationOverrides, (arrange, new SinglePrerequisiteClosure<T2>(tc => prereq2()).Arrange));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T2">The type of the pre-requisite.</typeparam>
        /// <param name="prereq2">
        /// A delegate for obtaining the pre-requisite.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2> And<T2>(Func<ITestContext, T2> prereq2)
        {
            return new TestPrerequisitesBuilder<T1, T2>(configurationOverrides, (arrange, new SinglePrerequisiteClosure<T2>(prereq2).Arrange));
        }
        
        /// <summary>
        /// Adds a "Given" clause for the test that defines the test context as a pre-requisite. This is just a more readable alias of:
        /// <code>
        /// And(ctx => ctx)
        /// </code>
        /// </summary>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, ITestContext> AndTestContext()
        {
            return new TestPrerequisitesBuilder<T1, ITestContext>(configurationOverrides, (arrange, new SinglePrerequisiteClosure<ITestContext>(ctx => ctx).Arrange));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T2">The type of the pre-requisite.</typeparam>
        /// <param name="prereqs2">A delegate for obtaining the pre-requisites, one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2> AndEachOf<T2>(Func<IEnumerable<T2>> prereqs2)
        {
            return new TestPrerequisitesBuilder<T1, T2>(configurationOverrides, (arrange, tc => prereqs2()));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T2">The type of the pre-requisite.</typeparam>
        /// <param name="prereqs2">
        /// A delegate for obtaining the pre-requisites, one for each test case.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2> AndEachOf<T2>(Func<ITestContext, IEnumerable<T2>> prereqs2)
        {
            return new TestPrerequisitesBuilder<T1, T2>(configurationOverrides, (arrange, prereqs2));
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="testAction">The action that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public ActionTestBuilder<T1> When(Action<T1> testAction)
        {
            return new ActionTestBuilder<T1>(configurationOverrides, arrange, testAction);
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public FunctionTestBuilder<T1, TResult> When<TResult>(Func<T1, TResult> testFunction)
        {
            return new FunctionTestBuilder<T1, TResult>(configurationOverrides, arrange, testFunction);
        }
	}

	/// <summary>
    /// Test builder for which the "When" clause has not yet been provided - and as such can still be given more pre-requisites
    /// (and configuration overrides).
    /// </summary>
    /// <typeparam name="T1">The type of the 1st pre-requisite already defined.</typeparam>
    /// <typeparam name="T2">The type of the 2nd pre-requisite already defined.</typeparam>
	public sealed class TestPrerequisitesBuilder<T1, T2>
	{
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>) arrange;

		internal TestPrerequisitesBuilder(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,          
            (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>) arrange)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
        }

        /// <summary>
        /// Adds a configuration override for the test.
        /// </summary>
        /// <param name="configurationOverride">The configuration override to be added.</param>
        public TestPrerequisitesBuilder<T1, T2> UsingConfiguration(Action<ITestConfiguration> configurationOverride)
        {
            // NB: We define a new enumerable and builder when overrides are added to facilitate builder re-use.
            // However, only create the new enumerable if an override is added (as opposed to in the ctor), to cut down on needless GC load.
            var overrides = new List<Action<ITestConfiguration>>(configurationOverrides);
            overrides.Add(configurationOverride);

            return new TestPrerequisitesBuilder<T1, T2>(overrides, (arrange.Item1, arrange.Item2));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T3">The type of the pre-requisite.</typeparam>
        /// <param name="prereq3">A delegate for obtaining the pre-requisite.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3> And<T3>(Func<T3> prereq3)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3>(configurationOverrides, (arrange.Item1, arrange.Item2, new SinglePrerequisiteClosure<T3>(tc => prereq3()).Arrange));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T3">The type of the pre-requisite.</typeparam>
        /// <param name="prereq3">
        /// A delegate for obtaining the pre-requisite.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3> And<T3>(Func<ITestContext, T3> prereq3)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3>(configurationOverrides, (arrange.Item1, arrange.Item2, new SinglePrerequisiteClosure<T3>(prereq3).Arrange));
        }
        
        /// <summary>
        /// Adds a "Given" clause for the test that defines the test context as a pre-requisite. This is just a more readable alias of:
        /// <code>
        /// And(ctx => ctx)
        /// </code>
        /// </summary>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, ITestContext> AndTestContext()
        {
            return new TestPrerequisitesBuilder<T1, T2, ITestContext>(configurationOverrides, (arrange.Item1, arrange.Item2, new SinglePrerequisiteClosure<ITestContext>(ctx => ctx).Arrange));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T3">The type of the pre-requisite.</typeparam>
        /// <param name="prereqs3">A delegate for obtaining the pre-requisites, one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3> AndEachOf<T3>(Func<IEnumerable<T3>> prereqs3)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3>(configurationOverrides, (arrange.Item1, arrange.Item2, tc => prereqs3()));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T3">The type of the pre-requisite.</typeparam>
        /// <param name="prereqs3">
        /// A delegate for obtaining the pre-requisites, one for each test case.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3> AndEachOf<T3>(Func<ITestContext, IEnumerable<T3>> prereqs3)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3>(configurationOverrides, (arrange.Item1, arrange.Item2, prereqs3));
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="testAction">The action that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public ActionTestBuilder<T1, T2> When(Action<T1, T2> testAction)
        {
            return new ActionTestBuilder<T1, T2>(configurationOverrides, arrange, testAction);
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public FunctionTestBuilder<T1, T2, TResult> When<TResult>(Func<T1, T2, TResult> testFunction)
        {
            return new FunctionTestBuilder<T1, T2, TResult>(configurationOverrides, arrange, testFunction);
        }
	}

	/// <summary>
    /// Test builder for which the "When" clause has not yet been provided - and as such can still be given more pre-requisites
    /// (and configuration overrides).
    /// </summary>
    /// <typeparam name="T1">The type of the 1st pre-requisite already defined.</typeparam>
    /// <typeparam name="T2">The type of the 2nd pre-requisite already defined.</typeparam>
    /// <typeparam name="T3">The type of the 3rd pre-requisite already defined.</typeparam>
	public sealed class TestPrerequisitesBuilder<T1, T2, T3>
	{
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>, Func<ITestContext, IEnumerable<T3>>) arrange;

		internal TestPrerequisitesBuilder(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,          
            (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>, Func<ITestContext, IEnumerable<T3>>) arrange)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
        }

        /// <summary>
        /// Adds a configuration override for the test.
        /// </summary>
        /// <param name="configurationOverride">The configuration override to be added.</param>
        public TestPrerequisitesBuilder<T1, T2, T3> UsingConfiguration(Action<ITestConfiguration> configurationOverride)
        {
            // NB: We define a new enumerable and builder when overrides are added to facilitate builder re-use.
            // However, only create the new enumerable if an override is added (as opposed to in the ctor), to cut down on needless GC load.
            var overrides = new List<Action<ITestConfiguration>>(configurationOverrides);
            overrides.Add(configurationOverride);

            return new TestPrerequisitesBuilder<T1, T2, T3>(overrides, (arrange.Item1, arrange.Item2, arrange.Item3));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T4">The type of the pre-requisite.</typeparam>
        /// <param name="prereq4">A delegate for obtaining the pre-requisite.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4> And<T4>(Func<T4> prereq4)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, new SinglePrerequisiteClosure<T4>(tc => prereq4()).Arrange));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T4">The type of the pre-requisite.</typeparam>
        /// <param name="prereq4">
        /// A delegate for obtaining the pre-requisite.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4> And<T4>(Func<ITestContext, T4> prereq4)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, new SinglePrerequisiteClosure<T4>(prereq4).Arrange));
        }
        
        /// <summary>
        /// Adds a "Given" clause for the test that defines the test context as a pre-requisite. This is just a more readable alias of:
        /// <code>
        /// And(ctx => ctx)
        /// </code>
        /// </summary>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, ITestContext> AndTestContext()
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, ITestContext>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, new SinglePrerequisiteClosure<ITestContext>(ctx => ctx).Arrange));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T4">The type of the pre-requisite.</typeparam>
        /// <param name="prereqs4">A delegate for obtaining the pre-requisites, one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4> AndEachOf<T4>(Func<IEnumerable<T4>> prereqs4)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, tc => prereqs4()));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T4">The type of the pre-requisite.</typeparam>
        /// <param name="prereqs4">
        /// A delegate for obtaining the pre-requisites, one for each test case.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4> AndEachOf<T4>(Func<ITestContext, IEnumerable<T4>> prereqs4)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, prereqs4));
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="testAction">The action that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public ActionTestBuilder<T1, T2, T3> When(Action<T1, T2, T3> testAction)
        {
            return new ActionTestBuilder<T1, T2, T3>(configurationOverrides, arrange, testAction);
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public FunctionTestBuilder<T1, T2, T3, TResult> When<TResult>(Func<T1, T2, T3, TResult> testFunction)
        {
            return new FunctionTestBuilder<T1, T2, T3, TResult>(configurationOverrides, arrange, testFunction);
        }
	}

	/// <summary>
    /// Test builder for which the "When" clause has not yet been provided - and as such can still be given more pre-requisites
    /// (and configuration overrides).
    /// </summary>
    /// <typeparam name="T1">The type of the 1st pre-requisite already defined.</typeparam>
    /// <typeparam name="T2">The type of the 2nd pre-requisite already defined.</typeparam>
    /// <typeparam name="T3">The type of the 3rd pre-requisite already defined.</typeparam>
    /// <typeparam name="T4">The type of the 4th pre-requisite already defined.</typeparam>
	public sealed class TestPrerequisitesBuilder<T1, T2, T3, T4>
	{
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>, Func<ITestContext, IEnumerable<T3>>, Func<ITestContext, IEnumerable<T4>>) arrange;

		internal TestPrerequisitesBuilder(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,          
            (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>, Func<ITestContext, IEnumerable<T3>>, Func<ITestContext, IEnumerable<T4>>) arrange)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
        }

        /// <summary>
        /// Adds a configuration override for the test.
        /// </summary>
        /// <param name="configurationOverride">The configuration override to be added.</param>
        public TestPrerequisitesBuilder<T1, T2, T3, T4> UsingConfiguration(Action<ITestConfiguration> configurationOverride)
        {
            // NB: We define a new enumerable and builder when overrides are added to facilitate builder re-use.
            // However, only create the new enumerable if an override is added (as opposed to in the ctor), to cut down on needless GC load.
            var overrides = new List<Action<ITestConfiguration>>(configurationOverrides);
            overrides.Add(configurationOverride);

            return new TestPrerequisitesBuilder<T1, T2, T3, T4>(overrides, (arrange.Item1, arrange.Item2, arrange.Item3, arrange.Item4));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T5">The type of the pre-requisite.</typeparam>
        /// <param name="prereq5">A delegate for obtaining the pre-requisite.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4, T5> And<T5>(Func<T5> prereq5)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4, T5>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, arrange.Item4, new SinglePrerequisiteClosure<T5>(tc => prereq5()).Arrange));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T5">The type of the pre-requisite.</typeparam>
        /// <param name="prereq5">
        /// A delegate for obtaining the pre-requisite.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4, T5> And<T5>(Func<ITestContext, T5> prereq5)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4, T5>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, arrange.Item4, new SinglePrerequisiteClosure<T5>(prereq5).Arrange));
        }
        
        /// <summary>
        /// Adds a "Given" clause for the test that defines the test context as a pre-requisite. This is just a more readable alias of:
        /// <code>
        /// And(ctx => ctx)
        /// </code>
        /// </summary>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4, ITestContext> AndTestContext()
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4, ITestContext>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, arrange.Item4, new SinglePrerequisiteClosure<ITestContext>(ctx => ctx).Arrange));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T5">The type of the pre-requisite.</typeparam>
        /// <param name="prereqs5">A delegate for obtaining the pre-requisites, one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4, T5> AndEachOf<T5>(Func<IEnumerable<T5>> prereqs5)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4, T5>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, arrange.Item4, tc => prereqs5()));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T5">The type of the pre-requisite.</typeparam>
        /// <param name="prereqs5">
        /// A delegate for obtaining the pre-requisites, one for each test case.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4, T5> AndEachOf<T5>(Func<ITestContext, IEnumerable<T5>> prereqs5)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4, T5>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, arrange.Item4, prereqs5));
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="testAction">The action that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public ActionTestBuilder<T1, T2, T3, T4> When(Action<T1, T2, T3, T4> testAction)
        {
            return new ActionTestBuilder<T1, T2, T3, T4>(configurationOverrides, arrange, testAction);
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public FunctionTestBuilder<T1, T2, T3, T4, TResult> When<TResult>(Func<T1, T2, T3, T4, TResult> testFunction)
        {
            return new FunctionTestBuilder<T1, T2, T3, T4, TResult>(configurationOverrides, arrange, testFunction);
        }
	}

	/// <summary>
    /// Test builder for which the "When" clause has not yet been provided - and as such can still be given more pre-requisites
    /// (and configuration overrides).
    /// </summary>
    /// <typeparam name="T1">The type of the 1st pre-requisite already defined.</typeparam>
    /// <typeparam name="T2">The type of the 2nd pre-requisite already defined.</typeparam>
    /// <typeparam name="T3">The type of the 3rd pre-requisite already defined.</typeparam>
    /// <typeparam name="T4">The type of the 4th pre-requisite already defined.</typeparam>
    /// <typeparam name="T5">The type of the 5th pre-requisite already defined.</typeparam>
	public sealed class TestPrerequisitesBuilder<T1, T2, T3, T4, T5>
	{
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>, Func<ITestContext, IEnumerable<T3>>, Func<ITestContext, IEnumerable<T4>>, Func<ITestContext, IEnumerable<T5>>) arrange;

		internal TestPrerequisitesBuilder(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,          
            (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>, Func<ITestContext, IEnumerable<T3>>, Func<ITestContext, IEnumerable<T4>>, Func<ITestContext, IEnumerable<T5>>) arrange)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
        }

        /// <summary>
        /// Adds a configuration override for the test.
        /// </summary>
        /// <param name="configurationOverride">The configuration override to be added.</param>
        public TestPrerequisitesBuilder<T1, T2, T3, T4, T5> UsingConfiguration(Action<ITestConfiguration> configurationOverride)
        {
            // NB: We define a new enumerable and builder when overrides are added to facilitate builder re-use.
            // However, only create the new enumerable if an override is added (as opposed to in the ctor), to cut down on needless GC load.
            var overrides = new List<Action<ITestConfiguration>>(configurationOverrides);
            overrides.Add(configurationOverride);

            return new TestPrerequisitesBuilder<T1, T2, T3, T4, T5>(overrides, (arrange.Item1, arrange.Item2, arrange.Item3, arrange.Item4, arrange.Item5));
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="testAction">The action that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public ActionTestBuilder<T1, T2, T3, T4, T5> When(Action<T1, T2, T3, T4, T5> testAction)
        {
            return new ActionTestBuilder<T1, T2, T3, T4, T5>(configurationOverrides, arrange, testAction);
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public FunctionTestBuilder<T1, T2, T3, T4, T5, TResult> When<TResult>(Func<T1, T2, T3, T4, T5, TResult> testFunction)
        {
            return new FunctionTestBuilder<T1, T2, T3, T4, T5, TResult>(configurationOverrides, arrange, testFunction);
        }
	}
}