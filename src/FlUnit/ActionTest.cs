using FlUnit.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FlUnit
{
    /// <summary>
    /// Represents a test with 0 "Given" clauses and a "When" clause that does not return a value.
    /// </summary>
    public sealed class ActionTest : Test
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<ValueTask> act;
        private readonly Func<Case, IEnumerable<Assertion>> makeAssertions;
        private IReadOnlyCollection<ITestCase> cases;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionTest"/> class.
        /// </summary>
        /// <param name="configurationOverrides">Configuration overrides that the test should apply when run.</param>
        /// <param name="act">The callback to run the "When" clause of the test.</param>
        /// <param name="makeAssertions">The callback to create all of the assertions for a particular test case.</param>
        internal ActionTest(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<ValueTask> act,
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

        /// <inheritdoc />
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
        /// Implementation of <see cref="ITestCase"/> used by <see cref="ActionTest"/>s.
        /// </summary>
        public class Case : ITestCase
        {
            private readonly Test test;
            private readonly Func<ValueTask> act;
            internal TestActionOutcome invocationOutcome;

            internal Case(
                Test test,
                Func<ValueTask> act,
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
                    await act();
                    invocationOutcome = new TestActionOutcome();
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestActionOutcome(e);
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
        /// Implementation of <see cref="ITestAssertion"/> used by <see cref="ActionTest"/>s.
        /// </summary>
        public class Assertion : ITestAssertion
        {
            private readonly Case testCase;
            private readonly Func<TestActionOutcome, ValueTask> assert;
            private readonly string description;

            internal Assertion(
                Case testCase,
                Func<TestActionOutcome, ValueTask> assert,
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
                catch (Exception e) when (!(e is ITestFailureDetails))
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
    /// Represents a test with 1 "Given" clause and a "When" clause that does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    public sealed class ActionTest<T1> : Test
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<ITestContext, ValueTask<IEnumerable<T1>>> arrange;
        private readonly Func<T1, ValueTask> act;
        private readonly Func<Case, IEnumerable<Assertion>> makeAssertions;
        private IReadOnlyCollection<ITestCase> cases;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionTest{T1}"/> class.
        /// </summary>
        /// <param name="configurationOverrides">Configuration overrides that the test should apply when run.</param>
        /// <param name="arrange">The callback to run the "Given" clauses of the test, returning the test cases.</param>
        /// <param name="act">The callback to run the "When" clause of the test.</param>
        /// <param name="makeAssertions">The callback to create all of the assertions for a particular test case.</param>
        internal ActionTest(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<ITestContext, ValueTask<IEnumerable<T1>>> arrange,
            Func<T1, ValueTask> act,
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

        /// <inheritdoc />
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
        /// Implementation of <see cref="ITestCase"/> used by <see cref="ActionTest{T1}"/>s.
        /// </summary>
        public class Case : ITestCase
        {
            private readonly Test test;
            internal readonly T1 prereqs;
            private readonly Func<T1, ValueTask> act;
            internal TestActionOutcome invocationOutcome;

            internal Case(
                Test test,
                T1 prereqs,
                Func<T1, ValueTask> act,
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
                    await act(prereqs);
                    invocationOutcome = new TestActionOutcome();
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestActionOutcome(e);
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
        /// Implementation of <see cref="ITestAssertion"/> used by <see cref="ActionTest{T1}"/>s.
        /// </summary>
        public class Assertion : ITestAssertion
        {
            private readonly Case testCase;
            private readonly Func<T1, TestActionOutcome, ValueTask> assert;
            private readonly string description;

            internal Assertion(
                Case testCase,
                Func<T1, TestActionOutcome, ValueTask> assert,
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
                catch (Exception e) when (!(e is ITestFailureDetails))
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
    /// Represents a test with 2 "Given" clauses and a "When" clause that does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    public sealed class ActionTest<T1, T2> : Test
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>) arrange;
        private readonly Func<T1, T2, ValueTask> act;
        private readonly Func<Case, IEnumerable<Assertion>> makeAssertions;
        private IReadOnlyCollection<ITestCase> cases;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionTest{T1, T2}"/> class.
        /// </summary>
        /// <param name="configurationOverrides">Configuration overrides that the test should apply when run.</param>
        /// <param name="arrange">The callback to run the "Given" clauses of the test, returning the test cases.</param>
        /// <param name="act">The callback to run the "When" clause of the test.</param>
        /// <param name="makeAssertions">The callback to create all of the assertions for a particular test case.</param>
        internal ActionTest(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>) arrange,
            Func<T1, T2, ValueTask> act,
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

        /// <inheritdoc />
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
        /// Implementation of <see cref="ITestCase"/> used by <see cref="ActionTest{T1, T2}"/>s.
        /// </summary>
        public class Case : ITestCase
        {
            private readonly Test test;
            internal readonly (T1, T2) prereqs;
            private readonly Func<T1, T2, ValueTask> act;
            internal TestActionOutcome invocationOutcome;

            internal Case(
                Test test,
                (T1, T2) prereqs,
                Func<T1, T2, ValueTask> act,
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
                    await act(prereqs.Item1, prereqs.Item2);
                    invocationOutcome = new TestActionOutcome();
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestActionOutcome(e);
                }
            }

            /// <inheritdoc />
            // TODO: Ultimately I'd like to offer some light support for format strings - perhaps something like (empty/"g"/"G" for the current behaviour, )
            // i for "test case #", and (when there are multiple prereqs) an integer for the prequisite of that index.
            public string ToString(string format, IFormatProvider formatProvider)
            {
                List<string> nonTypeNames = new List<string>();

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
        /// Implementation of <see cref="ITestAssertion"/> used by <see cref="ActionTest{T1, T2}"/>s.
        /// </summary>
        public class Assertion : ITestAssertion
        {
            private readonly Case testCase;
            private readonly Func<T1, T2, TestActionOutcome, ValueTask> assert;
            private readonly string description;

            internal Assertion(
                Case testCase,
                Func<T1, T2, TestActionOutcome, ValueTask> assert,
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
                catch (Exception e) when (!(e is ITestFailureDetails))
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
    /// Represents a test with 3 "Given" clauses and a "When" clause that does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    public sealed class ActionTest<T1, T2, T3> : Test
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>) arrange;
        private readonly Func<T1, T2, T3, ValueTask> act;
        private readonly Func<Case, IEnumerable<Assertion>> makeAssertions;
        private IReadOnlyCollection<ITestCase> cases;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionTest{T1, T2, T3}"/> class.
        /// </summary>
        /// <param name="configurationOverrides">Configuration overrides that the test should apply when run.</param>
        /// <param name="arrange">The callback to run the "Given" clauses of the test, returning the test cases.</param>
        /// <param name="act">The callback to run the "When" clause of the test.</param>
        /// <param name="makeAssertions">The callback to create all of the assertions for a particular test case.</param>
        internal ActionTest(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>) arrange,
            Func<T1, T2, T3, ValueTask> act,
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

        /// <inheritdoc />
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
        /// Implementation of <see cref="ITestCase"/> used by <see cref="ActionTest{T1, T2, T3}"/>s.
        /// </summary>
        public class Case : ITestCase
        {
            private readonly Test test;
            internal readonly (T1, T2, T3) prereqs;
            private readonly Func<T1, T2, T3, ValueTask> act;
            internal TestActionOutcome invocationOutcome;

            internal Case(
                Test test,
                (T1, T2, T3) prereqs,
                Func<T1, T2, T3, ValueTask> act,
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
                    await act(prereqs.Item1, prereqs.Item2, prereqs.Item3);
                    invocationOutcome = new TestActionOutcome();
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestActionOutcome(e);
                }
            }

            /// <inheritdoc />
            // TODO: Ultimately I'd like to offer some light support for format strings - perhaps something like (empty/"g"/"G" for the current behaviour, )
            // i for "test case #", and (when there are multiple prereqs) an integer for the prequisite of that index.
            public string ToString(string format, IFormatProvider formatProvider)
            {
                List<string> nonTypeNames = new List<string>();

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
        /// Implementation of <see cref="ITestAssertion"/> used by <see cref="ActionTest{T1, T2, T3}"/>s.
        /// </summary>
        public class Assertion : ITestAssertion
        {
            private readonly Case testCase;
            private readonly Func<T1, T2, T3, TestActionOutcome, ValueTask> assert;
            private readonly string description;

            internal Assertion(
                Case testCase,
                Func<T1, T2, T3, TestActionOutcome, ValueTask> assert,
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
                catch (Exception e) when (!(e is ITestFailureDetails))
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
    /// Represents a test with 4 "Given" clauses and a "When" clause that does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="T4">The type of the 4th "Given" clause of the test.</typeparam>
    public sealed class ActionTest<T1, T2, T3, T4> : Test
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>) arrange;
        private readonly Func<T1, T2, T3, T4, ValueTask> act;
        private readonly Func<Case, IEnumerable<Assertion>> makeAssertions;
        private IReadOnlyCollection<ITestCase> cases;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionTest{T1, T2, T3, T4}"/> class.
        /// </summary>
        /// <param name="configurationOverrides">Configuration overrides that the test should apply when run.</param>
        /// <param name="arrange">The callback to run the "Given" clauses of the test, returning the test cases.</param>
        /// <param name="act">The callback to run the "When" clause of the test.</param>
        /// <param name="makeAssertions">The callback to create all of the assertions for a particular test case.</param>
        internal ActionTest(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>) arrange,
            Func<T1, T2, T3, T4, ValueTask> act,
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

        /// <inheritdoc />
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
        /// Implementation of <see cref="ITestCase"/> used by <see cref="ActionTest{T1, T2, T3, T4}"/>s.
        /// </summary>
        public class Case : ITestCase
        {
            private readonly Test test;
            internal readonly (T1, T2, T3, T4) prereqs;
            private readonly Func<T1, T2, T3, T4, ValueTask> act;
            internal TestActionOutcome invocationOutcome;

            internal Case(
                Test test,
                (T1, T2, T3, T4) prereqs,
                Func<T1, T2, T3, T4, ValueTask> act,
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
                    await act(prereqs.Item1, prereqs.Item2, prereqs.Item3, prereqs.Item4);
                    invocationOutcome = new TestActionOutcome();
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestActionOutcome(e);
                }
            }

            /// <inheritdoc />
            // TODO: Ultimately I'd like to offer some light support for format strings - perhaps something like (empty/"g"/"G" for the current behaviour, )
            // i for "test case #", and (when there are multiple prereqs) an integer for the prequisite of that index.
            public string ToString(string format, IFormatProvider formatProvider)
            {
                List<string> nonTypeNames = new List<string>();

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
        /// Implementation of <see cref="ITestAssertion"/> used by <see cref="ActionTest{T1, T2, T3, T4}"/>s.
        /// </summary>
        public class Assertion : ITestAssertion
        {
            private readonly Case testCase;
            private readonly Func<T1, T2, T3, T4, TestActionOutcome, ValueTask> assert;
            private readonly string description;

            internal Assertion(
                Case testCase,
                Func<T1, T2, T3, T4, TestActionOutcome, ValueTask> assert,
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
                catch (Exception e) when (!(e is ITestFailureDetails))
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
    /// Represents a test with 5 "Given" clauses and a "When" clause that does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="T4">The type of the 4th "Given" clause of the test.</typeparam>
    /// <typeparam name="T5">The type of the 5th "Given" clause of the test.</typeparam>
    public sealed class ActionTest<T1, T2, T3, T4, T5> : Test
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>, Func<ITestContext, ValueTask<IEnumerable<T5>>>) arrange;
        private readonly Func<T1, T2, T3, T4, T5, ValueTask> act;
        private readonly Func<Case, IEnumerable<Assertion>> makeAssertions;
        private IReadOnlyCollection<ITestCase> cases;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionTest{T1, T2, T3, T4, T5}"/> class.
        /// </summary>
        /// <param name="configurationOverrides">Configuration overrides that the test should apply when run.</param>
        /// <param name="arrange">The callback to run the "Given" clauses of the test, returning the test cases.</param>
        /// <param name="act">The callback to run the "When" clause of the test.</param>
        /// <param name="makeAssertions">The callback to create all of the assertions for a particular test case.</param>
        internal ActionTest(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>, Func<ITestContext, ValueTask<IEnumerable<T5>>>) arrange,
            Func<T1, T2, T3, T4, T5, ValueTask> act,
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

        /// <inheritdoc />
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
        /// Implementation of <see cref="ITestCase"/> used by <see cref="ActionTest{T1, T2, T3, T4, T5}"/>s.
        /// </summary>
        public class Case : ITestCase
        {
            private readonly Test test;
            internal readonly (T1, T2, T3, T4, T5) prereqs;
            private readonly Func<T1, T2, T3, T4, T5, ValueTask> act;
            internal TestActionOutcome invocationOutcome;

            internal Case(
                Test test,
                (T1, T2, T3, T4, T5) prereqs,
                Func<T1, T2, T3, T4, T5, ValueTask> act,
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
                    await act(prereqs.Item1, prereqs.Item2, prereqs.Item3, prereqs.Item4, prereqs.Item5);
                    invocationOutcome = new TestActionOutcome();
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestActionOutcome(e);
                }
            }

            /// <inheritdoc />
            // TODO: Ultimately I'd like to offer some light support for format strings - perhaps something like (empty/"g"/"G" for the current behaviour, )
            // i for "test case #", and (when there are multiple prereqs) an integer for the prequisite of that index.
            public string ToString(string format, IFormatProvider formatProvider)
            {
                List<string> nonTypeNames = new List<string>();

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
        /// Implementation of <see cref="ITestAssertion"/> used by <see cref="ActionTest{T1, T2, T3, T4, T5}"/>s.
        /// </summary>
        public class Assertion : ITestAssertion
        {
            private readonly Case testCase;
            private readonly Func<T1, T2, T3, T4, T5, TestActionOutcome, ValueTask> assert;
            private readonly string description;

            internal Assertion(
                Case testCase,
                Func<T1, T2, T3, T4, T5, TestActionOutcome, ValueTask> assert,
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
                catch (Exception e) when (!(e is ITestFailureDetails))
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