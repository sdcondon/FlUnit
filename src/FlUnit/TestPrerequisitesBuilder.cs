using FlUnit.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlUnit
{
	/// <summary>
    /// Test builder for which the "When" clause has not yet been provided - and as such can still be given more prerequisites
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
        /// <typeparam name="T1">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter1">A delegate for obtaining the prerequisite.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1> Given<T1>(Func<T1> prereqGetter1)
        {
            return new TestPrerequisitesBuilder<T1>(configurationOverrides, new SinglePrerequisiteClosure<T1>(tc => prereqGetter1.ToAsyncWrapper()()).Arrange);
        }

        /// <summary>
        /// Adds the first "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T1">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter1">
        /// A delegate for obtaining the prerequisite.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1> Given<T1>(Func<ITestContext, T1> prereqGetter1)
        {
            return new TestPrerequisitesBuilder<T1>(configurationOverrides, new SinglePrerequisiteClosure<T1>(prereqGetter1.ToAsyncWrapper()).Arrange);
        }

        /// <summary>
        /// Adds the first "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T1">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter1">A delegate for obtaining the prerequisite asynchronously.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1> GivenAsync<T1>(Func<Task<T1>> prereqGetter1)
        {
            return new TestPrerequisitesBuilder<T1>(configurationOverrides, new SinglePrerequisiteClosure<T1>(tc => prereqGetter1.ToAsyncWrapper()()).Arrange);
        }

        /// <summary>
        /// Adds the first "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T1">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter1">
        /// A delegate for obtaining the prerequisite asynchronously.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to observe test cancellation, write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1> GivenAsync<T1>(Func<ITestContext, Task<T1>> prereqGetter1)
        {
            return new TestPrerequisitesBuilder<T1>(configurationOverrides, new SinglePrerequisiteClosure<T1>(prereqGetter1.ToAsyncWrapper()).Arrange);
        }

        /// <summary>
        /// Adds the first "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T1">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter1">A delegate for obtaining the prerequisites, one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1> GivenEachOf<T1>(Func<IEnumerable<T1>> prereqsGetter1)
        {
            return new TestPrerequisitesBuilder<T1>(configurationOverrides, tc => prereqsGetter1.ToAsyncWrapper()());
        }

        /// <summary>
        /// Adds the first "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T1">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter1">
        /// A delegate for obtaining the prerequisites, one for each test case.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1> GivenEachOf<T1>(Func<ITestContext, IEnumerable<T1>> prereqsGetter1)
        {
            return new TestPrerequisitesBuilder<T1>(configurationOverrides, prereqsGetter1.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds the first "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T1">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter1">A delegate for obtaining the prerequisites, one for each test case, asynchronously.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1> GivenEachOfAsync<T1>(Func<Task<IEnumerable<T1>>> prereqsGetter1)
        {
            return new TestPrerequisitesBuilder<T1>(configurationOverrides, tc => prereqsGetter1.ToAsyncWrapper()());
        }

        /// <summary>
        /// Adds the first "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T1">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter1">
        /// A delegate for obtaining the prerequisites, one for each test case, asynchronously.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to observe test cancellation, write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1> GivenEachOfAsync<T1>(Func<ITestContext, Task<IEnumerable<T1>>> prereqsGetter1)
        {
            return new TestPrerequisitesBuilder<T1>(configurationOverrides, prereqsGetter1.ToAsyncWrapper());
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
            return new TestPrerequisitesBuilder<ITestContext>(configurationOverrides, new SinglePrerequisiteClosure<ITestContext>(new Func<ITestContext, ITestContext>(ctx => ctx).ToAsyncWrapper()).Arrange);
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="testAction">The action that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public ActionTestBuilder When(Action testAction)
        {
            return new ActionTestBuilder(configurationOverrides, testAction.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public FunctionTestBuilder<TResult> When<TResult>(Func<TResult> testFunction)
        {
            return new FunctionTestBuilder<TResult>(configurationOverrides, testFunction.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="asyncTestAction">The action that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public ActionTestBuilder WhenAsync(Func<Task> asyncTestAction)
        {
            return new ActionTestBuilder(configurationOverrides, asyncTestAction.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="asyncTestFunction">The function that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public FunctionTestBuilder<TResult> WhenAsync<TResult>(Func<Task<TResult>> asyncTestFunction)
        {
            return new FunctionTestBuilder<TResult>(configurationOverrides, asyncTestFunction.ToAsyncWrapper());
        }
	}

	/// <summary>
    /// Test builder for which the "When" clause has not yet been provided - and as such can still be given more prerequisites
    /// (and configuration overrides).
    /// </summary>
    /// <typeparam name="T1">The type of the 1st prerequisite already defined.</typeparam>
	public sealed class TestPrerequisitesBuilder<T1>
	{
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<ITestContext, ValueTask<IEnumerable<T1>>> arrange;

		internal TestPrerequisitesBuilder(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<ITestContext, ValueTask<IEnumerable<T1>>> arrange)
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
        /// <typeparam name="T2">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter2">A delegate for obtaining the prerequisite.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2> And<T2>(Func<T2> prereqGetter2)
        {
            return new TestPrerequisitesBuilder<T1, T2>(configurationOverrides, (arrange, new SinglePrerequisiteClosure<T2>(tc => prereqGetter2.ToAsyncWrapper()()).Arrange));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T2">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter2">
        /// A delegate for obtaining the prerequisite.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2> And<T2>(Func<ITestContext, T2> prereqGetter2)
        {
            return new TestPrerequisitesBuilder<T1, T2>(configurationOverrides, (arrange, new SinglePrerequisiteClosure<T2>(prereqGetter2.ToAsyncWrapper()).Arrange));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T2">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter2">A delegate for obtaining the prerequisite asynchronously.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2> AndAsync<T2>(Func<Task<T2>> prereqGetter2)
        {
            return new TestPrerequisitesBuilder<T1, T2>(configurationOverrides, (arrange, new SinglePrerequisiteClosure<T2>(tc => prereqGetter2.ToAsyncWrapper()()).Arrange));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T2">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter2">
        /// A delegate for obtaining the prerequisite asynchronously.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to observe test cancellation, write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2> AndAsync<T2>(Func<ITestContext, Task<T2>> prereqGetter2)
        {
            return new TestPrerequisitesBuilder<T1, T2>(configurationOverrides, (arrange, new SinglePrerequisiteClosure<T2>(prereqGetter2.ToAsyncWrapper()).Arrange));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T2">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter2">A delegate for obtaining the prerequisites, one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2> AndEachOf<T2>(Func<IEnumerable<T2>> prereqsGetter2)
        {
            return new TestPrerequisitesBuilder<T1, T2>(configurationOverrides, (arrange, tc => prereqsGetter2.ToAsyncWrapper()()));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T2">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter2">
        /// A delegate for obtaining the prerequisites, one for each test case.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2> AndEachOf<T2>(Func<ITestContext, IEnumerable<T2>> prereqsGetter2)
        {
            return new TestPrerequisitesBuilder<T1, T2>(configurationOverrides, (arrange, prereqsGetter2.ToAsyncWrapper()));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T2">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter2">A delegate for obtaining the prerequisites, one for each test case, asynchronously.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2> AndEachOfAsync<T2>(Func<Task<IEnumerable<T2>>> prereqsGetter2)
        {
            return new TestPrerequisitesBuilder<T1, T2>(configurationOverrides, (arrange, tc => prereqsGetter2.ToAsyncWrapper()()));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T2">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter2">
        /// A delegate for obtaining the prerequisites, one for each test case, asynchronously.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to observe test cancellation, write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2> AndEachOfAsync<T2>(Func<ITestContext, Task<IEnumerable<T2>>> prereqsGetter2)
        {
            return new TestPrerequisitesBuilder<T1, T2>(configurationOverrides, (arrange, prereqsGetter2.ToAsyncWrapper()));
        }

        /// <summary>
        /// Adds a "Given" clause for the test that defines the test context as a prerequisite. This is just a more readable alias of:
        /// <code>
        /// And(ctx => ctx)
        /// </code>
        /// </summary>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, ITestContext> AndTestContext()
        {
            return new TestPrerequisitesBuilder<T1, ITestContext>(configurationOverrides, (arrange, new SinglePrerequisiteClosure<ITestContext>(new Func<ITestContext, ITestContext>(ctx => ctx).ToAsyncWrapper()).Arrange));
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="testAction">The action that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public ActionTestBuilder<T1> When(Action<T1> testAction)
        {
            return new ActionTestBuilder<T1>(configurationOverrides, arrange, testAction.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public FunctionTestBuilder<T1, TResult> When<TResult>(Func<T1, TResult> testFunction)
        {
            return new FunctionTestBuilder<T1, TResult>(configurationOverrides, arrange, testFunction.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="asyncTestAction">The action that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public ActionTestBuilder<T1> WhenAsync(Func<T1, Task> asyncTestAction)
        {
            return new ActionTestBuilder<T1>(configurationOverrides, arrange, asyncTestAction.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="asyncTestFunction">The function that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public FunctionTestBuilder<T1, TResult> WhenAsync<TResult>(Func<T1, Task<TResult>> asyncTestFunction)
        {
            return new FunctionTestBuilder<T1, TResult>(configurationOverrides, arrange, asyncTestFunction.ToAsyncWrapper());
        }
	}

	/// <summary>
    /// Test builder for which the "When" clause has not yet been provided - and as such can still be given more prerequisites
    /// (and configuration overrides).
    /// </summary>
    /// <typeparam name="T1">The type of the 1st prerequisite already defined.</typeparam>
    /// <typeparam name="T2">The type of the 2nd prerequisite already defined.</typeparam>
	public sealed class TestPrerequisitesBuilder<T1, T2>
	{
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>) arrange;

		internal TestPrerequisitesBuilder(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>) arrange)
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
        /// <typeparam name="T3">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter3">A delegate for obtaining the prerequisite.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3> And<T3>(Func<T3> prereqGetter3)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3>(configurationOverrides, (arrange.Item1, arrange.Item2, new SinglePrerequisiteClosure<T3>(tc => prereqGetter3.ToAsyncWrapper()()).Arrange));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T3">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter3">
        /// A delegate for obtaining the prerequisite.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3> And<T3>(Func<ITestContext, T3> prereqGetter3)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3>(configurationOverrides, (arrange.Item1, arrange.Item2, new SinglePrerequisiteClosure<T3>(prereqGetter3.ToAsyncWrapper()).Arrange));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T3">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter3">A delegate for obtaining the prerequisite asynchronously.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3> AndAsync<T3>(Func<Task<T3>> prereqGetter3)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3>(configurationOverrides, (arrange.Item1, arrange.Item2, new SinglePrerequisiteClosure<T3>(tc => prereqGetter3.ToAsyncWrapper()()).Arrange));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T3">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter3">
        /// A delegate for obtaining the prerequisite asynchronously.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to observe test cancellation, write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3> AndAsync<T3>(Func<ITestContext, Task<T3>> prereqGetter3)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3>(configurationOverrides, (arrange.Item1, arrange.Item2, new SinglePrerequisiteClosure<T3>(prereqGetter3.ToAsyncWrapper()).Arrange));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T3">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter3">A delegate for obtaining the prerequisites, one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3> AndEachOf<T3>(Func<IEnumerable<T3>> prereqsGetter3)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3>(configurationOverrides, (arrange.Item1, arrange.Item2, tc => prereqsGetter3.ToAsyncWrapper()()));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T3">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter3">
        /// A delegate for obtaining the prerequisites, one for each test case.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3> AndEachOf<T3>(Func<ITestContext, IEnumerable<T3>> prereqsGetter3)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3>(configurationOverrides, (arrange.Item1, arrange.Item2, prereqsGetter3.ToAsyncWrapper()));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T3">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter3">A delegate for obtaining the prerequisites, one for each test case, asynchronously.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3> AndEachOfAsync<T3>(Func<Task<IEnumerable<T3>>> prereqsGetter3)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3>(configurationOverrides, (arrange.Item1, arrange.Item2, tc => prereqsGetter3.ToAsyncWrapper()()));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T3">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter3">
        /// A delegate for obtaining the prerequisites, one for each test case, asynchronously.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to observe test cancellation, write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3> AndEachOfAsync<T3>(Func<ITestContext, Task<IEnumerable<T3>>> prereqsGetter3)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3>(configurationOverrides, (arrange.Item1, arrange.Item2, prereqsGetter3.ToAsyncWrapper()));
        }

        /// <summary>
        /// Adds a "Given" clause for the test that defines the test context as a prerequisite. This is just a more readable alias of:
        /// <code>
        /// And(ctx => ctx)
        /// </code>
        /// </summary>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, ITestContext> AndTestContext()
        {
            return new TestPrerequisitesBuilder<T1, T2, ITestContext>(configurationOverrides, (arrange.Item1, arrange.Item2, new SinglePrerequisiteClosure<ITestContext>(new Func<ITestContext, ITestContext>(ctx => ctx).ToAsyncWrapper()).Arrange));
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="testAction">The action that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public ActionTestBuilder<T1, T2> When(Action<T1, T2> testAction)
        {
            return new ActionTestBuilder<T1, T2>(configurationOverrides, arrange, testAction.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public FunctionTestBuilder<T1, T2, TResult> When<TResult>(Func<T1, T2, TResult> testFunction)
        {
            return new FunctionTestBuilder<T1, T2, TResult>(configurationOverrides, arrange, testFunction.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="asyncTestAction">The action that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public ActionTestBuilder<T1, T2> WhenAsync(Func<T1, T2, Task> asyncTestAction)
        {
            return new ActionTestBuilder<T1, T2>(configurationOverrides, arrange, asyncTestAction.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="asyncTestFunction">The function that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public FunctionTestBuilder<T1, T2, TResult> WhenAsync<TResult>(Func<T1, T2, Task<TResult>> asyncTestFunction)
        {
            return new FunctionTestBuilder<T1, T2, TResult>(configurationOverrides, arrange, asyncTestFunction.ToAsyncWrapper());
        }
	}

	/// <summary>
    /// Test builder for which the "When" clause has not yet been provided - and as such can still be given more prerequisites
    /// (and configuration overrides).
    /// </summary>
    /// <typeparam name="T1">The type of the 1st prerequisite already defined.</typeparam>
    /// <typeparam name="T2">The type of the 2nd prerequisite already defined.</typeparam>
    /// <typeparam name="T3">The type of the 3rd prerequisite already defined.</typeparam>
	public sealed class TestPrerequisitesBuilder<T1, T2, T3>
	{
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>) arrange;

		internal TestPrerequisitesBuilder(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>) arrange)
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
        /// <typeparam name="T4">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter4">A delegate for obtaining the prerequisite.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4> And<T4>(Func<T4> prereqGetter4)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, new SinglePrerequisiteClosure<T4>(tc => prereqGetter4.ToAsyncWrapper()()).Arrange));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T4">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter4">
        /// A delegate for obtaining the prerequisite.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4> And<T4>(Func<ITestContext, T4> prereqGetter4)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, new SinglePrerequisiteClosure<T4>(prereqGetter4.ToAsyncWrapper()).Arrange));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T4">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter4">A delegate for obtaining the prerequisite asynchronously.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4> AndAsync<T4>(Func<Task<T4>> prereqGetter4)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, new SinglePrerequisiteClosure<T4>(tc => prereqGetter4.ToAsyncWrapper()()).Arrange));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T4">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter4">
        /// A delegate for obtaining the prerequisite asynchronously.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to observe test cancellation, write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4> AndAsync<T4>(Func<ITestContext, Task<T4>> prereqGetter4)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, new SinglePrerequisiteClosure<T4>(prereqGetter4.ToAsyncWrapper()).Arrange));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T4">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter4">A delegate for obtaining the prerequisites, one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4> AndEachOf<T4>(Func<IEnumerable<T4>> prereqsGetter4)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, tc => prereqsGetter4.ToAsyncWrapper()()));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T4">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter4">
        /// A delegate for obtaining the prerequisites, one for each test case.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4> AndEachOf<T4>(Func<ITestContext, IEnumerable<T4>> prereqsGetter4)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, prereqsGetter4.ToAsyncWrapper()));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T4">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter4">A delegate for obtaining the prerequisites, one for each test case, asynchronously.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4> AndEachOfAsync<T4>(Func<Task<IEnumerable<T4>>> prereqsGetter4)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, tc => prereqsGetter4.ToAsyncWrapper()()));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T4">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter4">
        /// A delegate for obtaining the prerequisites, one for each test case, asynchronously.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to observe test cancellation, write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4> AndEachOfAsync<T4>(Func<ITestContext, Task<IEnumerable<T4>>> prereqsGetter4)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, prereqsGetter4.ToAsyncWrapper()));
        }

        /// <summary>
        /// Adds a "Given" clause for the test that defines the test context as a prerequisite. This is just a more readable alias of:
        /// <code>
        /// And(ctx => ctx)
        /// </code>
        /// </summary>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, ITestContext> AndTestContext()
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, ITestContext>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, new SinglePrerequisiteClosure<ITestContext>(new Func<ITestContext, ITestContext>(ctx => ctx).ToAsyncWrapper()).Arrange));
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="testAction">The action that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public ActionTestBuilder<T1, T2, T3> When(Action<T1, T2, T3> testAction)
        {
            return new ActionTestBuilder<T1, T2, T3>(configurationOverrides, arrange, testAction.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public FunctionTestBuilder<T1, T2, T3, TResult> When<TResult>(Func<T1, T2, T3, TResult> testFunction)
        {
            return new FunctionTestBuilder<T1, T2, T3, TResult>(configurationOverrides, arrange, testFunction.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="asyncTestAction">The action that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public ActionTestBuilder<T1, T2, T3> WhenAsync(Func<T1, T2, T3, Task> asyncTestAction)
        {
            return new ActionTestBuilder<T1, T2, T3>(configurationOverrides, arrange, asyncTestAction.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="asyncTestFunction">The function that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public FunctionTestBuilder<T1, T2, T3, TResult> WhenAsync<TResult>(Func<T1, T2, T3, Task<TResult>> asyncTestFunction)
        {
            return new FunctionTestBuilder<T1, T2, T3, TResult>(configurationOverrides, arrange, asyncTestFunction.ToAsyncWrapper());
        }
	}

	/// <summary>
    /// Test builder for which the "When" clause has not yet been provided - and as such can still be given more prerequisites
    /// (and configuration overrides).
    /// </summary>
    /// <typeparam name="T1">The type of the 1st prerequisite already defined.</typeparam>
    /// <typeparam name="T2">The type of the 2nd prerequisite already defined.</typeparam>
    /// <typeparam name="T3">The type of the 3rd prerequisite already defined.</typeparam>
    /// <typeparam name="T4">The type of the 4th prerequisite already defined.</typeparam>
	public sealed class TestPrerequisitesBuilder<T1, T2, T3, T4>
	{
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>) arrange;

		internal TestPrerequisitesBuilder(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>) arrange)
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
        /// <typeparam name="T5">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter5">A delegate for obtaining the prerequisite.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4, T5> And<T5>(Func<T5> prereqGetter5)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4, T5>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, arrange.Item4, new SinglePrerequisiteClosure<T5>(tc => prereqGetter5.ToAsyncWrapper()()).Arrange));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T5">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter5">
        /// A delegate for obtaining the prerequisite.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4, T5> And<T5>(Func<ITestContext, T5> prereqGetter5)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4, T5>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, arrange.Item4, new SinglePrerequisiteClosure<T5>(prereqGetter5.ToAsyncWrapper()).Arrange));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T5">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter5">A delegate for obtaining the prerequisite asynchronously.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4, T5> AndAsync<T5>(Func<Task<T5>> prereqGetter5)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4, T5>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, arrange.Item4, new SinglePrerequisiteClosure<T5>(tc => prereqGetter5.ToAsyncWrapper()()).Arrange));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T5">The type of the prerequisite.</typeparam>
        /// <param name="prereqGetter5">
        /// A delegate for obtaining the prerequisite asynchronously.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to observe test cancellation, write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4, T5> AndAsync<T5>(Func<ITestContext, Task<T5>> prereqGetter5)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4, T5>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, arrange.Item4, new SinglePrerequisiteClosure<T5>(prereqGetter5.ToAsyncWrapper()).Arrange));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T5">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter5">A delegate for obtaining the prerequisites, one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4, T5> AndEachOf<T5>(Func<IEnumerable<T5>> prereqsGetter5)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4, T5>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, arrange.Item4, tc => prereqsGetter5.ToAsyncWrapper()()));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T5">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter5">
        /// A delegate for obtaining the prerequisites, one for each test case.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4, T5> AndEachOf<T5>(Func<ITestContext, IEnumerable<T5>> prereqsGetter5)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4, T5>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, arrange.Item4, prereqsGetter5.ToAsyncWrapper()));
        }

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T5">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter5">A delegate for obtaining the prerequisites, one for each test case, asynchronously.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4, T5> AndEachOfAsync<T5>(Func<Task<IEnumerable<T5>>> prereqsGetter5)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4, T5>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, arrange.Item4, tc => prereqsGetter5.ToAsyncWrapper()()));
        }
        
        /// <summary>
        /// Adds another "Given" clause for the test, making use of the test context.
        /// </summary>
        /// <typeparam name="T5">The type of the prerequisite.</typeparam>
        /// <param name="prereqsGetter5">
        /// A delegate for obtaining the prerequisites, one for each test case, asynchronously.
        /// It will be passed an object representing the test context by the test runner.
        /// This can be used to observe test cancellation, write output, register test result attachments and so on.
        /// </param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4, T5> AndEachOfAsync<T5>(Func<ITestContext, Task<IEnumerable<T5>>> prereqsGetter5)
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4, T5>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, arrange.Item4, prereqsGetter5.ToAsyncWrapper()));
        }

        /// <summary>
        /// Adds a "Given" clause for the test that defines the test context as a prerequisite. This is just a more readable alias of:
        /// <code>
        /// And(ctx => ctx)
        /// </code>
        /// </summary>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestPrerequisitesBuilder<T1, T2, T3, T4, ITestContext> AndTestContext()
        {
            return new TestPrerequisitesBuilder<T1, T2, T3, T4, ITestContext>(configurationOverrides, (arrange.Item1, arrange.Item2, arrange.Item3, arrange.Item4, new SinglePrerequisiteClosure<ITestContext>(new Func<ITestContext, ITestContext>(ctx => ctx).ToAsyncWrapper()).Arrange));
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="testAction">The action that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public ActionTestBuilder<T1, T2, T3, T4> When(Action<T1, T2, T3, T4> testAction)
        {
            return new ActionTestBuilder<T1, T2, T3, T4>(configurationOverrides, arrange, testAction.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public FunctionTestBuilder<T1, T2, T3, T4, TResult> When<TResult>(Func<T1, T2, T3, T4, TResult> testFunction)
        {
            return new FunctionTestBuilder<T1, T2, T3, T4, TResult>(configurationOverrides, arrange, testFunction.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="asyncTestAction">The action that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public ActionTestBuilder<T1, T2, T3, T4> WhenAsync(Func<T1, T2, T3, T4, Task> asyncTestAction)
        {
            return new ActionTestBuilder<T1, T2, T3, T4>(configurationOverrides, arrange, asyncTestAction.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="asyncTestFunction">The function that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public FunctionTestBuilder<T1, T2, T3, T4, TResult> WhenAsync<TResult>(Func<T1, T2, T3, T4, Task<TResult>> asyncTestFunction)
        {
            return new FunctionTestBuilder<T1, T2, T3, T4, TResult>(configurationOverrides, arrange, asyncTestFunction.ToAsyncWrapper());
        }
	}

	/// <summary>
    /// Test builder for which the "When" clause has not yet been provided - and as such can still be given more prerequisites
    /// (and configuration overrides).
    /// </summary>
    /// <typeparam name="T1">The type of the 1st prerequisite already defined.</typeparam>
    /// <typeparam name="T2">The type of the 2nd prerequisite already defined.</typeparam>
    /// <typeparam name="T3">The type of the 3rd prerequisite already defined.</typeparam>
    /// <typeparam name="T4">The type of the 4th prerequisite already defined.</typeparam>
    /// <typeparam name="T5">The type of the 5th prerequisite already defined.</typeparam>
	public sealed class TestPrerequisitesBuilder<T1, T2, T3, T4, T5>
	{
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>, Func<ITestContext, ValueTask<IEnumerable<T5>>>) arrange;

		internal TestPrerequisitesBuilder(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>, Func<ITestContext, ValueTask<IEnumerable<T5>>>) arrange)
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
            return new ActionTestBuilder<T1, T2, T3, T4, T5>(configurationOverrides, arrange, testAction.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public FunctionTestBuilder<T1, T2, T3, T4, T5, TResult> When<TResult>(Func<T1, T2, T3, T4, T5, TResult> testFunction)
        {
            return new FunctionTestBuilder<T1, T2, T3, T4, T5, TResult>(configurationOverrides, arrange, testFunction.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds a "When" clause that does not return a value.
        /// </summary>
        /// <param name="asyncTestAction">The action that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public ActionTestBuilder<T1, T2, T3, T4, T5> WhenAsync(Func<T1, T2, T3, T4, T5, Task> asyncTestAction)
        {
            return new ActionTestBuilder<T1, T2, T3, T4, T5>(configurationOverrides, arrange, asyncTestAction.ToAsyncWrapper());
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="asyncTestFunction">The function that is the "When" clause of the test. It should have one parameter for each defined "Given" clause (if any).</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public FunctionTestBuilder<T1, T2, T3, T4, T5, TResult> WhenAsync<TResult>(Func<T1, T2, T3, T4, T5, Task<TResult>> asyncTestFunction)
        {
            return new FunctionTestBuilder<T1, T2, T3, T4, T5, TResult>(configurationOverrides, arrange, asyncTestFunction.ToAsyncWrapper());
        }
	}
}