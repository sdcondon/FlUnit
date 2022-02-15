using FlUnit.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlUnit
{
    /// <summary>
    /// Represents a test with 0 "Given" clauses and a "When" clause that does not return a value.
    /// </summary>
    public sealed class ActionTest : Test
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Action act;
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
            Action act,
            Func<Case, IEnumerable<Assertion>> makeAssertions)
        {
            this.configurationOverrides = configurationOverrides;
            this.act = act;
            this.makeAssertions = makeAssertions;
        }

        /// <summary>
        /// A collection of test cases that should be populated once <see cref="Arrange"/> is called.
        /// </summary>
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
        public override void Arrange()
        {
            try
            {
                cases = new[] { new Case(act, makeAssertions) };
            }
            catch (Exception e)
            {
                throw new TestFailureException(string.Format(Messages.ArrangementFailureMessageFormat, e.Message), e.StackTrace, e);
            }
        }

        /// <summary>
        /// Implementation of <see cref="ITestCase"/> used by <see cref="ActionTest"/>s.
        /// </summary>
        public class Case : ITestCase
        {
            private readonly Action act;
            internal TestActionOutcome invocationOutcome;

            internal Case(
                Action act,
                Func<Case, IEnumerable<Assertion>> makeAssertions)
            {
                this.act = act;
                this.Assertions = makeAssertions(this).ToArray();
            }

            /// <inheritdoc />
            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            /// <inheritdoc />
            public void Act()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    act();
                    invocationOutcome = new TestActionOutcome();
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestActionOutcome(e);
                }
            }

            /// <inheritdoc />
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
            private readonly Action<TestActionOutcome> assert;
            private readonly string description;

            internal Assertion(
                Case testCase,
                Action<TestActionOutcome> assert,
                string description)
            {
                this.testCase = testCase;
                this.assert = assert;
                this.description = description;
            }

            /// <inheritdoc />
            public void Assert()
            {
                if (testCase.invocationOutcome == null)
                {
                    throw new InvalidOperationException("Test action has not yet been invoked");
                }

                try
                {
                    assert(testCase.invocationOutcome);
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
        private readonly Func<IEnumerable<T1>> arrange;
        private readonly Action<T1> act;
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
            Func<IEnumerable<T1>> arrange,
            Action<T1> act,
            Func<Case, IEnumerable<Assertion>> makeAssertions)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.act = act;
            this.makeAssertions = makeAssertions;
        }

        /// <summary>
        /// A collection of test cases that should be populated once <see cref="Arrange"/> is called.
        /// </summary>
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
        public override void Arrange()
        {
            try
            {
                cases = arrange().Select(p => new Case(p, act, makeAssertions)).ToArray();
            }
            catch (Exception e)
            {
                throw new TestFailureException(string.Format(Messages.ArrangementFailureMessageFormat, e.Message), e.StackTrace, e);
            }
        }

        /// <summary>
        /// Implementation of <see cref="ITestCase"/> used by <see cref="ActionTest{T1}"/>s.
        /// </summary>
        public class Case : ITestCase
        {
            private readonly Action<T1> act;
            internal readonly T1 prereqs;
            internal TestActionOutcome invocationOutcome;

            internal Case(
                T1 prereqs,
                Action<T1> act,
                Func<Case, IEnumerable<Assertion>> makeAssertions)
            {
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = makeAssertions(this).ToArray();
            }

            /// <inheritdoc />
            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            /// <inheritdoc />
            public void Act()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    act(prereqs);
                    invocationOutcome = new TestActionOutcome();
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestActionOutcome(e);
                }
            }

            /// <inheritdoc />
            public string ToString(string format, IFormatProvider formatProvider)
            {
                return prereqs.ToString();
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
            private readonly Action<T1, TestActionOutcome> assert;
            private readonly string description;

            internal Assertion(
                Case testCase,
                Action<T1, TestActionOutcome> assert,
                string description)
            {
                this.testCase = testCase;
                this.assert = assert;
                this.description = description;
            }

            /// <inheritdoc />
            public void Assert()
            {
                if (testCase.invocationOutcome == null)
                {
                    throw new InvalidOperationException("Test action has not yet been invoked");
                }

                try
                {
                    assert(testCase.prereqs, testCase.invocationOutcome);
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
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange;
        private readonly Action<T1, T2> act;
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
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange,
            Action<T1, T2> act,
            Func<Case, IEnumerable<Assertion>> makeAssertions)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.act = act;
            this.makeAssertions = makeAssertions;
        }

        /// <summary>
        /// A collection of test cases that should be populated once <see cref="Arrange"/> is called.
        /// </summary>
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
        public override void Arrange()
        {
            try
            {
                cases = (
                    from p1 in arrange.Item1()
                    from p2 in arrange.Item2()
                    select new Case((p1, p2), act, makeAssertions)).ToArray();
            }
            catch (Exception e)
            {
                throw new TestFailureException(string.Format(Messages.ArrangementFailureMessageFormat, e.Message), e.StackTrace, e);
            }
        }

        /// <summary>
        /// Implementation of <see cref="ITestCase"/> used by <see cref="ActionTest{T1, T2}"/>s.
        /// </summary>
        public class Case : ITestCase
        {
            private readonly Action<T1, T2> act;
            internal readonly (T1, T2) prereqs;
            internal TestActionOutcome invocationOutcome;

            internal Case(
                (T1, T2) prereqs,
                Action<T1, T2> act,
                Func<Case, IEnumerable<Assertion>> makeAssertions)
            {
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = makeAssertions(this).ToArray();
            }

            /// <inheritdoc />
            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            /// <inheritdoc />
            public void Act()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    act(prereqs.Item1, prereqs.Item2);
                    invocationOutcome = new TestActionOutcome();
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestActionOutcome(e);
                }
            }

            /// <inheritdoc />
            public string ToString(string format, IFormatProvider formatProvider)
            {
                return prereqs.ToString();
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
            private readonly Action<T1, T2, TestActionOutcome> assert;
            private readonly string description;

            internal Assertion(
                Case testCase,
                Action<T1, T2, TestActionOutcome> assert,
                string description)
            {
                this.testCase = testCase;
                this.assert = assert;
                this.description = description;
            }

            /// <inheritdoc />
            public void Assert()
            {
                if (testCase.invocationOutcome == null)
                {
                    throw new InvalidOperationException("Test action has not yet been invoked");
                }

                try
                {
                    assert(testCase.prereqs.Item1, testCase.prereqs.Item2, testCase.invocationOutcome);
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
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange;
        private readonly Action<T1, T2, T3> act;
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
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange,
            Action<T1, T2, T3> act,
            Func<Case, IEnumerable<Assertion>> makeAssertions)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.act = act;
            this.makeAssertions = makeAssertions;
        }

        /// <summary>
        /// A collection of test cases that should be populated once <see cref="Arrange"/> is called.
        /// </summary>
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
        public override void Arrange()
        {
            try
            {
                cases = (
                    from p1 in arrange.Item1()
                    from p2 in arrange.Item2()
                    from p3 in arrange.Item3()
                    select new Case((p1, p2, p3), act, makeAssertions)).ToArray();
            }
            catch (Exception e)
            {
                throw new TestFailureException(string.Format(Messages.ArrangementFailureMessageFormat, e.Message), e.StackTrace, e);
            }
        }

        /// <summary>
        /// Implementation of <see cref="ITestCase"/> used by <see cref="ActionTest{T1, T2, T3}"/>s.
        /// </summary>
        public class Case : ITestCase
        {
            private readonly Action<T1, T2, T3> act;
            internal readonly (T1, T2, T3) prereqs;
            internal TestActionOutcome invocationOutcome;

            internal Case(
                (T1, T2, T3) prereqs,
                Action<T1, T2, T3> act,
                Func<Case, IEnumerable<Assertion>> makeAssertions)
            {
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = makeAssertions(this).ToArray();
            }

            /// <inheritdoc />
            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            /// <inheritdoc />
            public void Act()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    act(prereqs.Item1, prereqs.Item2, prereqs.Item3);
                    invocationOutcome = new TestActionOutcome();
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestActionOutcome(e);
                }
            }

            /// <inheritdoc />
            public string ToString(string format, IFormatProvider formatProvider)
            {
                return prereqs.ToString();
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
            private readonly Action<T1, T2, T3, TestActionOutcome> assert;
            private readonly string description;

            internal Assertion(
                Case testCase,
                Action<T1, T2, T3, TestActionOutcome> assert,
                string description)
            {
                this.testCase = testCase;
                this.assert = assert;
                this.description = description;
            }

            /// <inheritdoc />
            public void Assert()
            {
                if (testCase.invocationOutcome == null)
                {
                    throw new InvalidOperationException("Test action has not yet been invoked");
                }

                try
                {
                    assert(testCase.prereqs.Item1, testCase.prereqs.Item2, testCase.prereqs.Item3, testCase.invocationOutcome);
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