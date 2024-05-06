using FlUnit.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FlUnit
{
    /// <summary>
    /// Represents a test with 0 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTest<TResult> : Test
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<ValueTask<TResult>> act;
        private readonly Func<Case, IEnumerable<Assertion>> makeAssertions;
        private IReadOnlyCollection<ITestCase> cases;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionTest{TResult}"/> class.
        /// </summary>
        /// <param name="configurationOverrides">Configuration overrides that the test should apply when run.</param>
        /// <param name="act">The callback to run the "When" clause of the test.</param>
        /// <param name="makeAssertions">The callback to create all of the assertions for a particular test case.</param>
        internal FunctionTest(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<ValueTask<TResult>> act,
            Func<Case, IEnumerable<Assertion>> makeAssertions)
        {
            this.configurationOverrides = configurationOverrides;
            this.act = act;
            this.makeAssertions = makeAssertions;
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<ITestCase> Cases => cases ?? throw new InvalidOperationException("Test not yet arranged");

        /// <inheritdoc />
        public override bool HasConfigurationOverrides => configurationOverrides.Any();

        /// <inheritdoc />
        public override void ApplyConfigurationOverrides(ITestConfiguration testConfiguration)
        {
            foreach (var configurationOverride in configurationOverrides)
            {
                configurationOverride(testConfiguration);
            }
        }

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public override async ValueTask ArrangeAsync(ITestContext testContext)
        {
            try
            {
                cases = new[] { new Case(this, act, makeAssertions) };
            }
            catch (Exception e)
            {
                throw new TestFailureException(string.Format(Messages.ArrangementFailureMessageFormat, e.Message), e.StackTrace, e);
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            // TODO: allow for tidy-up. perhaps dispose IDisposable prereqs (on the assumption that we
            // own them) or invoke custom dispose method..
        }

        /// <summary>
        /// Implementation of <see cref="ITestCase"/> used by <see cref="FunctionTest{TResult}"/>s.
        /// </summary>
        public class Case : ITestCase
        {
            private readonly Test test;
            private readonly Func<ValueTask<TResult>> act;
            internal TestFunctionOutcome<TResult> invocationOutcome;

            internal Case(
                Test test,
                Func<ValueTask<TResult>> act,
                Func<Case, IEnumerable<Assertion>> makeAssertions)
            {
                this.test = test;
                this.act = act;
                this.Assertions = makeAssertions(this).ToArray();
            }

            /// <inheritdoc />
            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            /// <inheritdoc />
            public async ValueTask ActAsync()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(await act());
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(e);
                }
            }

            /// <inheritdoc />
			// TODO: Ultimately I'd like to offer some light support for format strings - perhaps something like (empty/"g"/"G" for the current behaviour, )
            // i for "test case #", and (when there are multiple prereqs) an integer for the prequisite of that index.
            public string ToString(string format, IFormatProvider formatProvider)
            {
                return "[implicit test case]";
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return ToString(null, null);
            }
        }

        /// <summary>
        /// Implementation of <see cref="ITestAssertion"/> used by <see cref="FunctionTest{TResult}"/>s.
        /// </summary>
        public class Assertion : ITestAssertion
        {
            private readonly Case testCase;
            private readonly Func<TestFunctionOutcome<TResult>, ValueTask> assert;
            private readonly string description;

            internal Assertion(
                Case testCase,
                Func<TestFunctionOutcome<TResult>, ValueTask> assert,
                string description)
            {
                this.testCase = testCase;
                this.assert = assert;
                this.description = description;
            }

            /// <inheritdoc />
            public async ValueTask AssertAsync()
            {
                if (testCase.invocationOutcome == null)
                {
                    throw new InvalidOperationException("Test action has not yet been invoked");
                }

                try
                {
                    await assert(testCase.invocationOutcome);
                }
                catch (Exception e) when (e is not ITestFailureDetails)
                {
                    throw new TestFailureException(e.Message, e.StackTrace, e);
                }
            }

            /// <inheritdoc />
            public string ToString(string format, IFormatProvider formatProvider)
            {
                return description;
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return ToString(null, null);
            }
        }
    }

    /// <summary>
    /// Represents a test with 1 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTest<T1, TResult> : Test
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<ITestContext, ValueTask<IEnumerable<T1>>> arrange;
        private readonly Func<T1, ValueTask<TResult>> act;
        private readonly Func<Case, IEnumerable<Assertion>> makeAssertions;
        private IReadOnlyCollection<ITestCase> cases;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionTest{T1, TResult}"/> class.
        /// </summary>
        /// <param name="configurationOverrides">Configuration overrides that the test should apply when run.</param>
        /// <param name="arrange">The callback to run the "Given" clauses of the test, returning the test cases.</param>
        /// <param name="act">The callback to run the "When" clause of the test.</param>
        /// <param name="makeAssertions">The callback to create all of the assertions for a particular test case.</param>
        internal FunctionTest(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<ITestContext, ValueTask<IEnumerable<T1>>> arrange,
            Func<T1, ValueTask<TResult>> act,
            Func<Case, IEnumerable<Assertion>> makeAssertions)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.act = act;
            this.makeAssertions = makeAssertions;
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<ITestCase> Cases => cases ?? throw new InvalidOperationException("Test not yet arranged");

        /// <inheritdoc />
        public override bool HasConfigurationOverrides => configurationOverrides.Any();

        /// <inheritdoc />
        public override void ApplyConfigurationOverrides(ITestConfiguration testConfiguration)
        {
            foreach (var configurationOverride in configurationOverrides)
            {
                configurationOverride(testConfiguration);
            }
        }

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public override async ValueTask ArrangeAsync(ITestContext testContext)
        {
            try
            {
                cases = (await arrange(testContext)).Select(p => new Case(this, p, act, makeAssertions)).ToArray();
            }
            catch (Exception e)
            {
                throw new TestFailureException(string.Format(Messages.ArrangementFailureMessageFormat, e.Message), e.StackTrace, e);
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            // TODO: allow for tidy-up. perhaps dispose IDisposable prereqs (on the assumption that we
            // own them) or invoke custom dispose method..
        }

        /// <summary>
        /// Implementation of <see cref="ITestCase"/> used by <see cref="FunctionTest{T1, TResult}"/>s.
        /// </summary>
        public class Case : ITestCase
        {
            private readonly Test test;
            internal readonly T1 prereqs;
            private readonly Func<T1, ValueTask<TResult>> act;
            internal TestFunctionOutcome<TResult> invocationOutcome;

            internal Case(
                Test test,
                T1 prereqs,
                Func<T1, ValueTask<TResult>> act,
                Func<Case, IEnumerable<Assertion>> makeAssertions)
            {
                this.test = test;
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = makeAssertions(this).ToArray();
            }

            /// <inheritdoc />
            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            /// <inheritdoc />
            public async ValueTask ActAsync()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(await act(prereqs));
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(e);
                }
            }

            /// <inheritdoc />
			// TODO: Ultimately I'd like to offer some light support for format strings - perhaps something like (empty/"g"/"G" for the current behaviour, )
            // i for "test case #", and (when there are multiple prereqs) an integer for the prequisite of that index.
            public string ToString(string format, IFormatProvider formatProvider)
            {
                string prereqToString = prereqs.ToString();

                if (prereqToString.Equals(prereqs.GetType().ToString()))
                {
                    return $"test case #{Array.IndexOf(test.Cases.ToArray(), this) + 1}";
                }
                else
                {
                    return prereqToString;
                }
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return ToString(null, null);
            }
        }

        /// <summary>
        /// Implementation of <see cref="ITestAssertion"/> used by <see cref="FunctionTest{T1, TResult}"/>s.
        /// </summary>
        public class Assertion : ITestAssertion
        {
            private readonly Case testCase;
            private readonly Func<T1, TestFunctionOutcome<TResult>, ValueTask> assert;
            private readonly string description;

            internal Assertion(
                Case testCase,
                Func<T1, TestFunctionOutcome<TResult>, ValueTask> assert,
                string description)
            {
                this.testCase = testCase;
                this.assert = assert;
                this.description = description;
            }

            /// <inheritdoc />
            public async ValueTask AssertAsync()
            {
                if (testCase.invocationOutcome == null)
                {
                    throw new InvalidOperationException("Test action has not yet been invoked");
                }

                try
                {
                    await assert(testCase.prereqs, testCase.invocationOutcome);
                }
                catch (Exception e) when (e is not ITestFailureDetails)
                {
                    throw new TestFailureException(e.Message, e.StackTrace, e);
                }
            }

            /// <inheritdoc />
            public string ToString(string format, IFormatProvider formatProvider)
            {
                return description;
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return ToString(null, null);
            }
        }
    }

    /// <summary>
    /// Represents a test with 2 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTest<T1, T2, TResult> : Test
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>) arrange;
        private readonly Func<T1, T2, ValueTask<TResult>> act;
        private readonly Func<Case, IEnumerable<Assertion>> makeAssertions;
        private IReadOnlyCollection<ITestCase> cases;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionTest{T1, T2, TResult}"/> class.
        /// </summary>
        /// <param name="configurationOverrides">Configuration overrides that the test should apply when run.</param>
        /// <param name="arrange">The callback to run the "Given" clauses of the test, returning the test cases.</param>
        /// <param name="act">The callback to run the "When" clause of the test.</param>
        /// <param name="makeAssertions">The callback to create all of the assertions for a particular test case.</param>
        internal FunctionTest(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>) arrange,
            Func<T1, T2, ValueTask<TResult>> act,
            Func<Case, IEnumerable<Assertion>> makeAssertions)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.act = act;
            this.makeAssertions = makeAssertions;
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<ITestCase> Cases => cases ?? throw new InvalidOperationException("Test not yet arranged");

        /// <inheritdoc />
        public override bool HasConfigurationOverrides => configurationOverrides.Any();

        /// <inheritdoc />
        public override void ApplyConfigurationOverrides(ITestConfiguration testConfiguration)
        {
            foreach (var configurationOverride in configurationOverrides)
            {
                configurationOverride(testConfiguration);
            }
        }

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public override async ValueTask ArrangeAsync(ITestContext testContext)
        {
            try
            {
                // TODO: can probably assume these are safe to execute in parallel?
                var p1s = await arrange.Item1(testContext);
                var p2s = await arrange.Item2(testContext);
                cases = (
                    from p1 in p1s
                    from p2 in p2s
                    select new Case(this, (p1, p2), act, makeAssertions)).ToArray();
            }
            catch (Exception e)
            {
                throw new TestFailureException(string.Format(Messages.ArrangementFailureMessageFormat, e.Message), e.StackTrace, e);
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            // TODO: allow for tidy-up. perhaps dispose IDisposable prereqs (on the assumption that we
            // own them) or invoke custom dispose method..
        }

        /// <summary>
        /// Implementation of <see cref="ITestCase"/> used by <see cref="FunctionTest{T1, T2, TResult}"/>s.
        /// </summary>
        public class Case : ITestCase
        {
            private readonly Test test;
            internal readonly (T1, T2) prereqs;
            private readonly Func<T1, T2, ValueTask<TResult>> act;
            internal TestFunctionOutcome<TResult> invocationOutcome;

            internal Case(
                Test test,
                (T1, T2) prereqs,
                Func<T1, T2, ValueTask<TResult>> act,
                Func<Case, IEnumerable<Assertion>> makeAssertions)
            {
                this.test = test;
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = makeAssertions(this).ToArray();
            }

            /// <inheritdoc />
            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            /// <inheritdoc />
            public async ValueTask ActAsync()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(await act(prereqs.Item1, prereqs.Item2));
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(e);
                }
            }

            /// <inheritdoc />
			// TODO: Ultimately I'd like to offer some light support for format strings - perhaps something like (empty/"g"/"G" for the current behaviour, )
            // i for "test case #", and (when there are multiple prereqs) an integer for the prequisite of that index.
            public string ToString(string format, IFormatProvider formatProvider)
            {
                List<string> nonTypeNames = new();

                var tuple = prereqs as ITuple;
                for (var i = 0; i < tuple.Length; i++)
                {
                    var item = tuple[i];
                    var itemToString = item.ToString();
                    if (!itemToString.Equals(item.GetType().ToString()))
                    {
                        nonTypeNames.Add(itemToString);
                    }
                }

                if (nonTypeNames.Count == 0)
                {
                    return $"test case #{Array.IndexOf(test.Cases.ToArray(), this) + 1}";
                }
                else if (nonTypeNames.Count == 1)
                {
                    return nonTypeNames[0];
                }
                else
                {
                    return $"({string.Join(", ", nonTypeNames)})";
                }
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return ToString(null, null);
            }
        }

        /// <summary>
        /// Implementation of <see cref="ITestAssertion"/> used by <see cref="FunctionTest{T1, T2, TResult}"/>s.
        /// </summary>
        public class Assertion : ITestAssertion
        {
            private readonly Case testCase;
            private readonly Func<T1, T2, TestFunctionOutcome<TResult>, ValueTask> assert;
            private readonly string description;

            internal Assertion(
                Case testCase,
                Func<T1, T2, TestFunctionOutcome<TResult>, ValueTask> assert,
                string description)
            {
                this.testCase = testCase;
                this.assert = assert;
                this.description = description;
            }

            /// <inheritdoc />
            public async ValueTask AssertAsync()
            {
                if (testCase.invocationOutcome == null)
                {
                    throw new InvalidOperationException("Test action has not yet been invoked");
                }

                try
                {
                    await assert(testCase.prereqs.Item1, testCase.prereqs.Item2, testCase.invocationOutcome);
                }
                catch (Exception e) when (e is not ITestFailureDetails)
                {
                    throw new TestFailureException(e.Message, e.StackTrace, e);
                }
            }

            /// <inheritdoc />
            public string ToString(string format, IFormatProvider formatProvider)
            {
                return description;
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return ToString(null, null);
            }
        }
    }

    /// <summary>
    /// Represents a test with 3 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTest<T1, T2, T3, TResult> : Test
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>) arrange;
        private readonly Func<T1, T2, T3, ValueTask<TResult>> act;
        private readonly Func<Case, IEnumerable<Assertion>> makeAssertions;
        private IReadOnlyCollection<ITestCase> cases;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionTest{T1, T2, T3, TResult}"/> class.
        /// </summary>
        /// <param name="configurationOverrides">Configuration overrides that the test should apply when run.</param>
        /// <param name="arrange">The callback to run the "Given" clauses of the test, returning the test cases.</param>
        /// <param name="act">The callback to run the "When" clause of the test.</param>
        /// <param name="makeAssertions">The callback to create all of the assertions for a particular test case.</param>
        internal FunctionTest(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>) arrange,
            Func<T1, T2, T3, ValueTask<TResult>> act,
            Func<Case, IEnumerable<Assertion>> makeAssertions)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.act = act;
            this.makeAssertions = makeAssertions;
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<ITestCase> Cases => cases ?? throw new InvalidOperationException("Test not yet arranged");

        /// <inheritdoc />
        public override bool HasConfigurationOverrides => configurationOverrides.Any();

        /// <inheritdoc />
        public override void ApplyConfigurationOverrides(ITestConfiguration testConfiguration)
        {
            foreach (var configurationOverride in configurationOverrides)
            {
                configurationOverride(testConfiguration);
            }
        }

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public override async ValueTask ArrangeAsync(ITestContext testContext)
        {
            try
            {
                // TODO: can probably assume these are safe to execute in parallel?
                var p1s = await arrange.Item1(testContext);
                var p2s = await arrange.Item2(testContext);
                var p3s = await arrange.Item3(testContext);
                cases = (
                    from p1 in p1s
                    from p2 in p2s
                    from p3 in p3s
                    select new Case(this, (p1, p2, p3), act, makeAssertions)).ToArray();
            }
            catch (Exception e)
            {
                throw new TestFailureException(string.Format(Messages.ArrangementFailureMessageFormat, e.Message), e.StackTrace, e);
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            // TODO: allow for tidy-up. perhaps dispose IDisposable prereqs (on the assumption that we
            // own them) or invoke custom dispose method..
        }

        /// <summary>
        /// Implementation of <see cref="ITestCase"/> used by <see cref="FunctionTest{T1, T2, T3, TResult}"/>s.
        /// </summary>
        public class Case : ITestCase
        {
            private readonly Test test;
            internal readonly (T1, T2, T3) prereqs;
            private readonly Func<T1, T2, T3, ValueTask<TResult>> act;
            internal TestFunctionOutcome<TResult> invocationOutcome;

            internal Case(
                Test test,
                (T1, T2, T3) prereqs,
                Func<T1, T2, T3, ValueTask<TResult>> act,
                Func<Case, IEnumerable<Assertion>> makeAssertions)
            {
                this.test = test;
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = makeAssertions(this).ToArray();
            }

            /// <inheritdoc />
            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            /// <inheritdoc />
            public async ValueTask ActAsync()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(await act(prereqs.Item1, prereqs.Item2, prereqs.Item3));
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(e);
                }
            }

            /// <inheritdoc />
			// TODO: Ultimately I'd like to offer some light support for format strings - perhaps something like (empty/"g"/"G" for the current behaviour, )
            // i for "test case #", and (when there are multiple prereqs) an integer for the prequisite of that index.
            public string ToString(string format, IFormatProvider formatProvider)
            {
                List<string> nonTypeNames = new();

                var tuple = prereqs as ITuple;
                for (var i = 0; i < tuple.Length; i++)
                {
                    var item = tuple[i];
                    var itemToString = item.ToString();
                    if (!itemToString.Equals(item.GetType().ToString()))
                    {
                        nonTypeNames.Add(itemToString);
                    }
                }

                if (nonTypeNames.Count == 0)
                {
                    return $"test case #{Array.IndexOf(test.Cases.ToArray(), this) + 1}";
                }
                else if (nonTypeNames.Count == 1)
                {
                    return nonTypeNames[0];
                }
                else
                {
                    return $"({string.Join(", ", nonTypeNames)})";
                }
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return ToString(null, null);
            }
        }

        /// <summary>
        /// Implementation of <see cref="ITestAssertion"/> used by <see cref="FunctionTest{T1, T2, T3, TResult}"/>s.
        /// </summary>
        public class Assertion : ITestAssertion
        {
            private readonly Case testCase;
            private readonly Func<T1, T2, T3, TestFunctionOutcome<TResult>, ValueTask> assert;
            private readonly string description;

            internal Assertion(
                Case testCase,
                Func<T1, T2, T3, TestFunctionOutcome<TResult>, ValueTask> assert,
                string description)
            {
                this.testCase = testCase;
                this.assert = assert;
                this.description = description;
            }

            /// <inheritdoc />
            public async ValueTask AssertAsync()
            {
                if (testCase.invocationOutcome == null)
                {
                    throw new InvalidOperationException("Test action has not yet been invoked");
                }

                try
                {
                    await assert(testCase.prereqs.Item1, testCase.prereqs.Item2, testCase.prereqs.Item3, testCase.invocationOutcome);
                }
                catch (Exception e) when (e is not ITestFailureDetails)
                {
                    throw new TestFailureException(e.Message, e.StackTrace, e);
                }
            }

            /// <inheritdoc />
            public string ToString(string format, IFormatProvider formatProvider)
            {
                return description;
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return ToString(null, null);
            }
        }
    }

    /// <summary>
    /// Represents a test with 4 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="T4">The type of the 4th "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTest<T1, T2, T3, T4, TResult> : Test
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>) arrange;
        private readonly Func<T1, T2, T3, T4, ValueTask<TResult>> act;
        private readonly Func<Case, IEnumerable<Assertion>> makeAssertions;
        private IReadOnlyCollection<ITestCase> cases;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionTest{T1, T2, T3, T4, TResult}"/> class.
        /// </summary>
        /// <param name="configurationOverrides">Configuration overrides that the test should apply when run.</param>
        /// <param name="arrange">The callback to run the "Given" clauses of the test, returning the test cases.</param>
        /// <param name="act">The callback to run the "When" clause of the test.</param>
        /// <param name="makeAssertions">The callback to create all of the assertions for a particular test case.</param>
        internal FunctionTest(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>) arrange,
            Func<T1, T2, T3, T4, ValueTask<TResult>> act,
            Func<Case, IEnumerable<Assertion>> makeAssertions)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.act = act;
            this.makeAssertions = makeAssertions;
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<ITestCase> Cases => cases ?? throw new InvalidOperationException("Test not yet arranged");

        /// <inheritdoc />
        public override bool HasConfigurationOverrides => configurationOverrides.Any();

        /// <inheritdoc />
        public override void ApplyConfigurationOverrides(ITestConfiguration testConfiguration)
        {
            foreach (var configurationOverride in configurationOverrides)
            {
                configurationOverride(testConfiguration);
            }
        }

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public override async ValueTask ArrangeAsync(ITestContext testContext)
        {
            try
            {
                // TODO: can probably assume these are safe to execute in parallel?
                var p1s = await arrange.Item1(testContext);
                var p2s = await arrange.Item2(testContext);
                var p3s = await arrange.Item3(testContext);
                var p4s = await arrange.Item4(testContext);
                cases = (
                    from p1 in p1s
                    from p2 in p2s
                    from p3 in p3s
                    from p4 in p4s
                    select new Case(this, (p1, p2, p3, p4), act, makeAssertions)).ToArray();
            }
            catch (Exception e)
            {
                throw new TestFailureException(string.Format(Messages.ArrangementFailureMessageFormat, e.Message), e.StackTrace, e);
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            // TODO: allow for tidy-up. perhaps dispose IDisposable prereqs (on the assumption that we
            // own them) or invoke custom dispose method..
        }

        /// <summary>
        /// Implementation of <see cref="ITestCase"/> used by <see cref="FunctionTest{T1, T2, T3, T4, TResult}"/>s.
        /// </summary>
        public class Case : ITestCase
        {
            private readonly Test test;
            internal readonly (T1, T2, T3, T4) prereqs;
            private readonly Func<T1, T2, T3, T4, ValueTask<TResult>> act;
            internal TestFunctionOutcome<TResult> invocationOutcome;

            internal Case(
                Test test,
                (T1, T2, T3, T4) prereqs,
                Func<T1, T2, T3, T4, ValueTask<TResult>> act,
                Func<Case, IEnumerable<Assertion>> makeAssertions)
            {
                this.test = test;
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = makeAssertions(this).ToArray();
            }

            /// <inheritdoc />
            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            /// <inheritdoc />
            public async ValueTask ActAsync()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(await act(prereqs.Item1, prereqs.Item2, prereqs.Item3, prereqs.Item4));
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(e);
                }
            }

            /// <inheritdoc />
			// TODO: Ultimately I'd like to offer some light support for format strings - perhaps something like (empty/"g"/"G" for the current behaviour, )
            // i for "test case #", and (when there are multiple prereqs) an integer for the prequisite of that index.
            public string ToString(string format, IFormatProvider formatProvider)
            {
                List<string> nonTypeNames = new();

                var tuple = prereqs as ITuple;
                for (var i = 0; i < tuple.Length; i++)
                {
                    var item = tuple[i];
                    var itemToString = item.ToString();
                    if (!itemToString.Equals(item.GetType().ToString()))
                    {
                        nonTypeNames.Add(itemToString);
                    }
                }

                if (nonTypeNames.Count == 0)
                {
                    return $"test case #{Array.IndexOf(test.Cases.ToArray(), this) + 1}";
                }
                else if (nonTypeNames.Count == 1)
                {
                    return nonTypeNames[0];
                }
                else
                {
                    return $"({string.Join(", ", nonTypeNames)})";
                }
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return ToString(null, null);
            }
        }

        /// <summary>
        /// Implementation of <see cref="ITestAssertion"/> used by <see cref="FunctionTest{T1, T2, T3, T4, TResult}"/>s.
        /// </summary>
        public class Assertion : ITestAssertion
        {
            private readonly Case testCase;
            private readonly Func<T1, T2, T3, T4, TestFunctionOutcome<TResult>, ValueTask> assert;
            private readonly string description;

            internal Assertion(
                Case testCase,
                Func<T1, T2, T3, T4, TestFunctionOutcome<TResult>, ValueTask> assert,
                string description)
            {
                this.testCase = testCase;
                this.assert = assert;
                this.description = description;
            }

            /// <inheritdoc />
            public async ValueTask AssertAsync()
            {
                if (testCase.invocationOutcome == null)
                {
                    throw new InvalidOperationException("Test action has not yet been invoked");
                }

                try
                {
                    await assert(testCase.prereqs.Item1, testCase.prereqs.Item2, testCase.prereqs.Item3, testCase.prereqs.Item4, testCase.invocationOutcome);
                }
                catch (Exception e) when (e is not ITestFailureDetails)
                {
                    throw new TestFailureException(e.Message, e.StackTrace, e);
                }
            }

            /// <inheritdoc />
            public string ToString(string format, IFormatProvider formatProvider)
            {
                return description;
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return ToString(null, null);
            }
        }
    }

    /// <summary>
    /// Represents a test with 5 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="T4">The type of the 4th "Given" clause of the test.</typeparam>
    /// <typeparam name="T5">The type of the 5th "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTest<T1, T2, T3, T4, T5, TResult> : Test
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>, Func<ITestContext, ValueTask<IEnumerable<T5>>>) arrange;
        private readonly Func<T1, T2, T3, T4, T5, ValueTask<TResult>> act;
        private readonly Func<Case, IEnumerable<Assertion>> makeAssertions;
        private IReadOnlyCollection<ITestCase> cases;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionTest{T1, T2, T3, T4, T5, TResult}"/> class.
        /// </summary>
        /// <param name="configurationOverrides">Configuration overrides that the test should apply when run.</param>
        /// <param name="arrange">The callback to run the "Given" clauses of the test, returning the test cases.</param>
        /// <param name="act">The callback to run the "When" clause of the test.</param>
        /// <param name="makeAssertions">The callback to create all of the assertions for a particular test case.</param>
        internal FunctionTest(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>, Func<ITestContext, ValueTask<IEnumerable<T5>>>) arrange,
            Func<T1, T2, T3, T4, T5, ValueTask<TResult>> act,
            Func<Case, IEnumerable<Assertion>> makeAssertions)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.act = act;
            this.makeAssertions = makeAssertions;
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<ITestCase> Cases => cases ?? throw new InvalidOperationException("Test not yet arranged");

        /// <inheritdoc />
        public override bool HasConfigurationOverrides => configurationOverrides.Any();

        /// <inheritdoc />
        public override void ApplyConfigurationOverrides(ITestConfiguration testConfiguration)
        {
            foreach (var configurationOverride in configurationOverrides)
            {
                configurationOverride(testConfiguration);
            }
        }

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public override async ValueTask ArrangeAsync(ITestContext testContext)
        {
            try
            {
                // TODO: can probably assume these are safe to execute in parallel?
                var p1s = await arrange.Item1(testContext);
                var p2s = await arrange.Item2(testContext);
                var p3s = await arrange.Item3(testContext);
                var p4s = await arrange.Item4(testContext);
                var p5s = await arrange.Item5(testContext);
                cases = (
                    from p1 in p1s
                    from p2 in p2s
                    from p3 in p3s
                    from p4 in p4s
                    from p5 in p5s
                    select new Case(this, (p1, p2, p3, p4, p5), act, makeAssertions)).ToArray();
            }
            catch (Exception e)
            {
                throw new TestFailureException(string.Format(Messages.ArrangementFailureMessageFormat, e.Message), e.StackTrace, e);
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            // TODO: allow for tidy-up. perhaps dispose IDisposable prereqs (on the assumption that we
            // own them) or invoke custom dispose method..
        }

        /// <summary>
        /// Implementation of <see cref="ITestCase"/> used by <see cref="FunctionTest{T1, T2, T3, T4, T5, TResult}"/>s.
        /// </summary>
        public class Case : ITestCase
        {
            private readonly Test test;
            internal readonly (T1, T2, T3, T4, T5) prereqs;
            private readonly Func<T1, T2, T3, T4, T5, ValueTask<TResult>> act;
            internal TestFunctionOutcome<TResult> invocationOutcome;

            internal Case(
                Test test,
                (T1, T2, T3, T4, T5) prereqs,
                Func<T1, T2, T3, T4, T5, ValueTask<TResult>> act,
                Func<Case, IEnumerable<Assertion>> makeAssertions)
            {
                this.test = test;
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = makeAssertions(this).ToArray();
            }

            /// <inheritdoc />
            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            /// <inheritdoc />
            public async ValueTask ActAsync()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(await act(prereqs.Item1, prereqs.Item2, prereqs.Item3, prereqs.Item4, prereqs.Item5));
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(e);
                }
            }

            /// <inheritdoc />
			// TODO: Ultimately I'd like to offer some light support for format strings - perhaps something like (empty/"g"/"G" for the current behaviour, )
            // i for "test case #", and (when there are multiple prereqs) an integer for the prequisite of that index.
            public string ToString(string format, IFormatProvider formatProvider)
            {
                List<string> nonTypeNames = new();

                var tuple = prereqs as ITuple;
                for (var i = 0; i < tuple.Length; i++)
                {
                    var item = tuple[i];
                    var itemToString = item.ToString();
                    if (!itemToString.Equals(item.GetType().ToString()))
                    {
                        nonTypeNames.Add(itemToString);
                    }
                }

                if (nonTypeNames.Count == 0)
                {
                    return $"test case #{Array.IndexOf(test.Cases.ToArray(), this) + 1}";
                }
                else if (nonTypeNames.Count == 1)
                {
                    return nonTypeNames[0];
                }
                else
                {
                    return $"({string.Join(", ", nonTypeNames)})";
                }
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return ToString(null, null);
            }
        }

        /// <summary>
        /// Implementation of <see cref="ITestAssertion"/> used by <see cref="FunctionTest{T1, T2, T3, T4, T5, TResult}"/>s.
        /// </summary>
        public class Assertion : ITestAssertion
        {
            private readonly Case testCase;
            private readonly Func<T1, T2, T3, T4, T5, TestFunctionOutcome<TResult>, ValueTask> assert;
            private readonly string description;

            internal Assertion(
                Case testCase,
                Func<T1, T2, T3, T4, T5, TestFunctionOutcome<TResult>, ValueTask> assert,
                string description)
            {
                this.testCase = testCase;
                this.assert = assert;
                this.description = description;
            }

            /// <inheritdoc />
            public async ValueTask AssertAsync()
            {
                if (testCase.invocationOutcome == null)
                {
                    throw new InvalidOperationException("Test action has not yet been invoked");
                }

                try
                {
                    await assert(testCase.prereqs.Item1, testCase.prereqs.Item2, testCase.prereqs.Item3, testCase.prereqs.Item4, testCase.prereqs.Item5, testCase.invocationOutcome);
                }
                catch (Exception e) when (e is not ITestFailureDetails)
                {
                    throw new TestFailureException(e.Message, e.StackTrace, e);
                }
            }

            /// <inheritdoc />
            public string ToString(string format, IFormatProvider formatProvider)
            {
                return description;
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return ToString(null, null);
            }
        }
    }
}