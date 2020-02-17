using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FlUnit
{
    //// TODO: T4 template me!!

    public class TestFunctionAndAssertions<TResult> : ITest
    {
        private readonly Func<TResult> testFunction;
        private readonly List<TestAssertion> assertions = new List<TestAssertion>();
        private TestFunctionResult<TResult> invocationResult;

        internal TestFunctionAndAssertions(Func<TResult> testFunction, Expression<Action<TestFunctionResult<TResult>>> assertion)
            : this(testFunction, assertion.Compile(), assertion.Body.ToString())
        {
        }

        internal TestFunctionAndAssertions(Func<TResult> testFunction, Action<TestFunctionResult<TResult>> assertion, string assertionDescription)
        {
            this.testFunction = testFunction;
            AddAssertion(assertion, assertionDescription);
        }

        public IEnumerable<TestAssertion> Assertions => assertions;

        public TestFunctionAndAssertions<TResult> And(Expression<Action<TestFunctionResult<TResult>>> assertion)
        {
            return And(assertion.Compile(), assertion.Body.ToString());
        }

        public TestFunctionAndAssertions<TResult> And(Action<TestFunctionResult<TResult>> assertion, string description)
        {
            AddAssertion(assertion, description);
            return this;
        }

        public void Act()
        {
            if (invocationResult != null)
            {
                throw new InvalidOperationException("Test action already invoked");
            }

            try
            {
                invocationResult = new TestFunctionResult<TResult>(testFunction());
            }
            catch (Exception e)
            {
                invocationResult = new TestFunctionResult<TResult>(e);
            }
        }

        private void AddAssertion(Action<TestFunctionResult<TResult>> assertion, string description)
        {
            assertions.Add(new TestAssertion(() => assertion(invocationResult), description));
        }
    }

    public class TestFunctionAndAssertions<T1, TResult> : ITest
    {
        private readonly T1 prereq;
        private readonly Func<T1, TResult> testFunction;
        private readonly List<TestAssertion> assertions = new List<TestAssertion>();
        private TestFunctionResult<TResult> invocationResult;

        internal TestFunctionAndAssertions(T1 prereq, Func<T1, TResult> testFunction, Expression<Action<T1, TestFunctionResult<TResult>>> assertion)
            : this(prereq, testFunction, assertion.Compile(), assertion.Body.ToString())
        {
        }

        internal TestFunctionAndAssertions(T1 prereq, Func<T1, TResult> testFunction, Action<T1, TestFunctionResult<TResult>> assertion, string description)
        {
            this.prereq = prereq;
            this.testFunction = testFunction;
            AddAssertion(assertion, description);
        }

        public IEnumerable<TestAssertion> Assertions => assertions;

        public TestFunctionAndAssertions<T1, TResult> And(Expression<Action<T1, TestFunctionResult<TResult>>> assertion)
        {
            return And(assertion.Compile(), assertion.Body.ToString());
        }

        public TestFunctionAndAssertions<T1, TResult> And(Action<T1, TestFunctionResult<TResult>> assertion, string description)
        {
            AddAssertion(assertion, description);
            return this;
        }

        public void Act()
        {
            if (invocationResult != null)
            {
                throw new InvalidOperationException("Test action already invoked");
            }

            try
            {
                invocationResult = new TestFunctionResult<TResult>(testFunction(prereq));
            }
            catch (Exception e)
            {
                invocationResult = new TestFunctionResult<TResult>(e);
            }
        }

        private void AddAssertion(Action<T1, TestFunctionResult<TResult>> assertion, string description)
        {
            assertions.Add(new TestAssertion(() => assertion(prereq, invocationResult), description));
        }
    }

    public class TestFunctionAndAssertions<T1, T2, TResult> : ITest
    {
        private readonly (T1, T2) prereqs;
        private readonly Func<T1, T2, TResult> testFunction;
        private readonly List<TestAssertion> assertions = new List<TestAssertion>();
        private TestFunctionResult<TResult> invocationResult;

        internal TestFunctionAndAssertions((T1, T2) prereqs, Func<T1, T2, TResult> testFunction, Expression<Action<T1, T2, TestFunctionResult<TResult>>> assertion)
            : this(prereqs, testFunction, assertion.Compile(), assertion.Body.ToString())
        {
        }

        internal TestFunctionAndAssertions((T1, T2) prereqs, Func<T1, T2, TResult> testFunction, Action<T1, T2, TestFunctionResult<TResult>> assertion, string description)
        {
            this.prereqs = prereqs;
            this.testFunction = testFunction;
            AddAssertion(assertion, description);
        }

        public IEnumerable<TestAssertion> Assertions => assertions;

        public TestFunctionAndAssertions<T1, T2, TResult> And(Expression<Action<T1, T2, TestFunctionResult<TResult>>> assertion)
        {
            return And(assertion.Compile(), assertion.Body.ToString());
        }

        public TestFunctionAndAssertions<T1, T2, TResult> And(Action<T1, T2, TestFunctionResult<TResult>> assertion, string description)
        {
            AddAssertion(assertion, description);
            return this;
        }

        public void Act()
        {
            if (invocationResult != null)
            {
                throw new InvalidOperationException("Test action already invoked");
            }

            try
            {
                invocationResult = new TestFunctionResult<TResult>(testFunction(prereqs.Item1, prereqs.Item2));
            }
            catch (Exception e)
            {
                invocationResult = new TestFunctionResult<TResult>(e);
            }
        }

        private void AddAssertion(Action<T1, T2, TestFunctionResult<TResult>> assertion, string description)
        {
            assertions.Add(new TestAssertion(() => assertion(prereqs.Item1, prereqs.Item2, invocationResult), description));
        }
    }

    public class TestFunctionAndAssertions<T1, T2, T3, TResult> : ITest
    {
        private readonly (T1, T2, T3) prereqs;
        private readonly Func<T1, T2, T3, TResult> testFunction;
        private readonly List<TestAssertion> assertions = new List<TestAssertion>();
        private TestFunctionResult<TResult> invocationResult;

        internal TestFunctionAndAssertions((T1, T2, T3) prereqs, Func<T1, T2, T3, TResult> testFunction, Expression<Action<T1, T2, T3, TestFunctionResult<TResult>>> assertion)
            : this(prereqs, testFunction, assertion.Compile(), assertion.Body.ToString())
        {
        }

        internal TestFunctionAndAssertions((T1, T2, T3) prereqs, Func<T1, T2, T3, TResult> testFunction, Action<T1, T2, T3, TestFunctionResult<TResult>> assertion, string description)
        {
            this.prereqs = prereqs;
            this.testFunction = testFunction;
            AddAssertion(assertion, description);
        }

        public IEnumerable<TestAssertion> Assertions => assertions;

        public TestFunctionAndAssertions<T1, T2, T3, TResult> And(Expression<Action<T1, T2, T3, TestFunctionResult<TResult>>> assertion)
        {
            return And(assertion.Compile(), assertion.Body.ToString());
        }

        public TestFunctionAndAssertions<T1, T2, T3, TResult> And(Action<T1, T2, T3, TestFunctionResult<TResult>> assertion, string description)
        {
            AddAssertion(assertion, description);
            return this;
        }

        public void Act()
        {
            if (invocationResult != null)
            {
                throw new InvalidOperationException("Test action already invoked");
            }

            try
            {
                invocationResult = new TestFunctionResult<TResult>(testFunction(prereqs.Item1, prereqs.Item2, prereqs.Item3));
            }
            catch (Exception e)
            {
                invocationResult = new TestFunctionResult<TResult>(e);
            }
        }

        private void AddAssertion(Action<T1, T2, T3, TestFunctionResult<TResult>> assertion, string description)
        {
            assertions.Add(new TestAssertion(() => assertion(prereqs.Item1, prereqs.Item2, prereqs.Item3, invocationResult), description));
        }
    }
}
