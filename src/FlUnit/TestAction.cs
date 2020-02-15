using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlUnit
{
    //// TODO: T4 template me!!

    public class TestAction
    {
        private readonly Action testAction;

        internal TestAction(Action testAction)
        {
            this.testAction = testAction;
        }

        public TestActionAndAssertions Then(Expression<Action<Task>> assertion)
        {
            return new TestActionAndAssertions(testAction, assertion);
        }

        public TestActionAndAssertions Then(Action<Task> assertion, string description)
        {
            return new TestActionAndAssertions(testAction, assertion, description);
        }
    }

    public class TestAction<T1>
    {
        private readonly T1 prereq;
        private readonly Action<T1> testAction;

        internal TestAction(T1 prereq, Action<T1> testAction)
        {
            this.prereq = prereq;
            this.testAction = testAction;
        }

        public TestActionAndAssertions<T1> Then(Expression<Action<T1, Task>> assertion)
        {
            return new TestActionAndAssertions<T1>(prereq, testAction, assertion);
        }

        public TestActionAndAssertions<T1> Then(Action<T1, Task> assertion, string description)
        {
            return new TestActionAndAssertions<T1>(prereq, testAction, assertion, description);
        }
    }

    public class TestAction<T1, T2>
    {
        private readonly (T1, T2) prereqs;
        private readonly Action<T1, T2> testAction;

        internal TestAction((T1, T2) prereqs, Action<T1, T2> testAction)
        {
            this.prereqs = prereqs;
            this.testAction = testAction;
        }

        public TestActionAndAssertions<T1, T2> Then(Expression<Action<T1, T2, Task>> assertion)
        {
            return new TestActionAndAssertions<T1, T2>(prereqs, testAction, assertion);
        }

        public TestActionAndAssertions<T1, T2> Then(Action<T1, T2, Task> assertion, string description)
        {
            return new TestActionAndAssertions<T1, T2>(prereqs, testAction, assertion, description);
        }
    }

    public class TestAction<T1, T2, T3>
    {
        private readonly (T1, T2, T3) prereqs;
        private readonly Action<T1, T2, T3> testAction;

        internal TestAction((T1, T2, T3) prereqs, Action<T1, T2, T3> testAction)
        {
            this.prereqs = prereqs;
            this.testAction = testAction;
        }

        public TestActionAndAssertions<T1, T2, T3> Then(Expression<Action<T1, T2, T3, Task>> assertion)
        {
            return new TestActionAndAssertions<T1, T2, T3>(prereqs, testAction, assertion);
        }

        public TestActionAndAssertions<T1, T2, T3> Then(Action<T1, T2, T3, Task> assertion, string description)
        {
            return new TestActionAndAssertions<T1, T2, T3>(prereqs, testAction, assertion, description);
        }
    }
}
