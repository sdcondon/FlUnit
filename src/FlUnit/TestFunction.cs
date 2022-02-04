﻿using FlUnit.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlUnit
{
    /// <summary>
    /// Represents a test with 0 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    public sealed class TestFunction<TResult> : Test
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<TResult> act;
        private readonly Func<Case, IEnumerable<Case.Assertion>> makeAssertions;
        private IReadOnlyCollection<ITestCase> cases;

        /// <summary>
        /// Initializes a new instance of the <see cref="Test"/> class.
        /// </summary>
        /// <param name="configurationOverrides">Configuration overrides that the test should apply when run.</param>
        /// <param name="act">The callback to run the "When" clause of the test.</param>
        /// <param name="makeAssertions">The callback to create all of the assertions for a particular test case.</param>
        internal TestFunction(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<TResult> act,
            Func<Case, IEnumerable<Case.Assertion>> makeAssertions)
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

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public override void Arrange()
        {
            cases = new[] { new Case(act, makeAssertions) };
        }

        internal class Case : ITestCase
        {
            private readonly Func<TResult> act;
            private TestFunctionOutcome<TResult> invocationOutcome;

            internal Case(
                Func<TResult> act,
                Func<Case, IEnumerable<Assertion>> makeAssertions)
            {
                this.act = act;
                this.Assertions = makeAssertions(this).ToArray();
            }

            public string Description => string.Empty;

            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            public void Act()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(act());
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(e);
                }
            }

            internal class Assertion : ITestAssertion
            {
                private readonly Case testCase;
                private readonly Action<TestFunctionOutcome<TResult>> action;

                public Assertion(Case testCase, Action<TestFunctionOutcome<TResult>> action, string description)
                {
                    this.testCase = testCase;
                    this.action = action;
                    this.Description = description;
                }

                public string Description { get; }

                public void Invoke() => action(testCase.invocationOutcome);
            }
        }
    }

    /// <summary>
    /// Represents a test with 1 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    public sealed class TestFunction<T1, TResult> : Test
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<IEnumerable<T1>> arrange;
        private readonly Func<T1, TResult> act;
        private readonly Func<Case, IEnumerable<Case.Assertion>> makeAssertions;
        private IReadOnlyCollection<ITestCase> cases;

        /// <summary>
        /// Initializes a new instance of the <see cref="Test"/> class.
        /// </summary>
        /// <param name="configurationOverrides">Configuration overrides that the test should apply when run.</param>
        /// <param name="arrange">The callback to run the "Given" clauses of the test, returning the test cases.</param>
        /// <param name="act">The callback to run the "When" clause of the test.</param>
        /// <param name="makeAssertions">The callback to create all of the assertions for a particular test case.</param>
        internal TestFunction(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<IEnumerable<T1>> arrange,
            Func<T1, TResult> act,
            Func<Case, IEnumerable<Case.Assertion>> makeAssertions)
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

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public override void Arrange()
        {
            cases = arrange().Select(p => new Case(p, act, makeAssertions)).ToArray();
        }

        internal class Case : ITestCase
        {
            private readonly Func<T1, TResult> act;
            private readonly T1 prereqs;
            private TestFunctionOutcome<TResult> invocationOutcome;

            internal Case(
                T1 prereqs,
                Func<T1, TResult> act,
                Func<Case, IEnumerable<Assertion>> makeAssertions)
            {
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = makeAssertions(this).ToArray();
            }

            public string Description => prereqs.ToString();

            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            public void Act()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(act(prereqs));
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(e);
                }
            }

            internal class Assertion : ITestAssertion
            {
                private readonly Case testCase;
                private readonly Action<T1, TestFunctionOutcome<TResult>> action;

                public Assertion(Case testCase, Action<T1, TestFunctionOutcome<TResult>> action, string description)
                {
                    this.testCase = testCase;
                    this.action = action;
                    this.Description = description;
                }

                public string Description { get; }

                public void Invoke() => action(testCase.prereqs, testCase.invocationOutcome);
            }
        }
    }

    /// <summary>
    /// Represents a test with 2 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    public sealed class TestFunction<T1, T2, TResult> : Test
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange;
        private readonly Func<T1, T2, TResult> act;
        private readonly Func<Case, IEnumerable<Case.Assertion>> makeAssertions;
        private IReadOnlyCollection<ITestCase> cases;

        /// <summary>
        /// Initializes a new instance of the <see cref="Test"/> class.
        /// </summary>
        /// <param name="configurationOverrides">Configuration overrides that the test should apply when run.</param>
        /// <param name="arrange">The callback to run the "Given" clauses of the test, returning the test cases.</param>
        /// <param name="act">The callback to run the "When" clause of the test.</param>
        /// <param name="makeAssertions">The callback to create all of the assertions for a particular test case.</param>
        internal TestFunction(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange,
            Func<T1, T2, TResult> act,
            Func<Case, IEnumerable<Case.Assertion>> makeAssertions)
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

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public override void Arrange()
        {
            cases = (
                from p1 in arrange.Item1()
                from p2 in arrange.Item2()
                select new Case((p1, p2), act, makeAssertions)).ToArray();
        }

        internal class Case : ITestCase
        {
            private readonly Func<T1, T2, TResult> act;
            private readonly (T1, T2) prereqs;
            private TestFunctionOutcome<TResult> invocationOutcome;

            internal Case(
                (T1, T2) prereqs,
                Func<T1, T2, TResult> act,
                Func<Case, IEnumerable<Assertion>> makeAssertions)
            {
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = makeAssertions(this).ToArray();
            }

            public string Description => prereqs.ToString();

            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            public void Act()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(act(prereqs.Item1, prereqs.Item2));
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(e);
                }
            }

            internal class Assertion : ITestAssertion
            {
                private readonly Case testCase;
                private readonly Action<T1, T2, TestFunctionOutcome<TResult>> action;

                public Assertion(Case testCase, Action<T1, T2, TestFunctionOutcome<TResult>> action, string description)
                {
                    this.testCase = testCase;
                    this.action = action;
                    this.Description = description;
                }

                public string Description { get; }

                public void Invoke() => action(testCase.prereqs.Item1, testCase.prereqs.Item2, testCase.invocationOutcome);
            }
        }
    }

    /// <summary>
    /// Represents a test with 3 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    public sealed class TestFunction<T1, T2, T3, TResult> : Test
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange;
        private readonly Func<T1, T2, T3, TResult> act;
        private readonly Func<Case, IEnumerable<Case.Assertion>> makeAssertions;
        private IReadOnlyCollection<ITestCase> cases;

        /// <summary>
        /// Initializes a new instance of the <see cref="Test"/> class.
        /// </summary>
        /// <param name="configurationOverrides">Configuration overrides that the test should apply when run.</param>
        /// <param name="arrange">The callback to run the "Given" clauses of the test, returning the test cases.</param>
        /// <param name="act">The callback to run the "When" clause of the test.</param>
        /// <param name="makeAssertions">The callback to create all of the assertions for a particular test case.</param>
        internal TestFunction(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange,
            Func<T1, T2, T3, TResult> act,
            Func<Case, IEnumerable<Case.Assertion>> makeAssertions)
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

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public override void Arrange()
        {
            cases = (
                from p1 in arrange.Item1()
                from p2 in arrange.Item2()
                from p3 in arrange.Item3()
                select new Case((p1, p2, p3), act, makeAssertions)).ToArray();
        }

        internal class Case : ITestCase
        {
            private readonly Func<T1, T2, T3, TResult> act;
            private readonly (T1, T2, T3) prereqs;
            private TestFunctionOutcome<TResult> invocationOutcome;

            internal Case(
                (T1, T2, T3) prereqs,
                Func<T1, T2, T3, TResult> act,
                Func<Case, IEnumerable<Assertion>> makeAssertions)
            {
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = makeAssertions(this).ToArray();
            }

            public string Description => prereqs.ToString();

            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            public void Act()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(act(prereqs.Item1, prereqs.Item2, prereqs.Item3));
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(e);
                }
            }

            internal class Assertion : ITestAssertion
            {
                private readonly Case testCase;
                private readonly Action<T1, T2, T3, TestFunctionOutcome<TResult>> action;

                public Assertion(Case testCase, Action<T1, T2, T3, TestFunctionOutcome<TResult>> action, string description)
                {
                    this.testCase = testCase;
                    this.action = action;
                    this.Description = description;
                }

                public string Description { get; }

                public void Invoke() => action(testCase.prereqs.Item1, testCase.prereqs.Item2, testCase.prereqs.Item3, testCase.invocationOutcome);
            }
        }
    }
}