using System;
using System.Threading.Tasks;

namespace FlUnit
{
    internal static class ActionExtensions
    {
        public static Func<ValueTask> ToAsyncWrapper(this Action action)
        {
            return () =>
            {
                action.Invoke();
                return ValueTask.CompletedTask;
            };
        }

        public static Func<T1, ValueTask> ToAsyncWrapper<T1>(this Action<T1> action)
        {
            return (a1) =>
            {
                action.Invoke(a1);
                return ValueTask.CompletedTask;
            };
        }

        public static Func<T1, T2, ValueTask> ToAsyncWrapper<T1, T2>(this Action<T1, T2> action)
        {
            return (a1, a2) =>
            {
                action.Invoke(a1, a2);
                return ValueTask.CompletedTask;
            };
        }

        public static Func<T1, T2, T3, ValueTask> ToAsyncWrapper<T1, T2, T3>(this Action<T1, T2, T3> action)
        {
            return (a1, a2, a3) =>
            {
                action.Invoke(a1, a2, a3);
                return ValueTask.CompletedTask;
            };
        }

        public static Func<T1, T2, T3, T4, ValueTask> ToAsyncWrapper<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action)
        {
            return (a1, a2, a3, a4) =>
            {
                action.Invoke(a1, a2, a3, a4);
                return ValueTask.CompletedTask;
            };
        }

        public static Func<T1, T2, T3, T4, T5, ValueTask> ToAsyncWrapper<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action)
        {
            return (a1, a2, a3, a4, a5) =>
            {
                action.Invoke(a1, a2, a3, a4, a5);
                return ValueTask.CompletedTask;
            };
        }

        public static Func<T1, T2, T3, T4, T5, T6, ValueTask> ToAsyncWrapper<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action)
        {
            return (a1, a2, a3, a4, a5, a6) =>
            {
                action.Invoke(a1, a2, a3, a4, a5, a6);
                return ValueTask.CompletedTask;
            };
        }
   }
}