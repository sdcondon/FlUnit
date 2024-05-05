using System;
using System.Threading.Tasks;

namespace FlUnit
{
    internal static class FunctionExtensions
    {
        public static Func<ValueTask<TOut>> ToAsyncWrapper<TOut>(this Func<TOut> func)
        {
            return () => ValueTask.FromResult(func.Invoke());
        }

        public static Func<ValueTask<TOut>> ToAsyncWrapper<TOut>(this Func<Task<TOut>> func)
        {
            return () => new ValueTask<TOut>(func.Invoke());
        }

        public static Func<ValueTask> ToAsyncWrapper(this Func<Task> func)
        {
            return () => new ValueTask(func.Invoke());
        }

        public static Func<T1, ValueTask<TOut>> ToAsyncWrapper<T1, TOut>(this Func<T1, TOut> func)
        {
            return (a1) => ValueTask.FromResult(func.Invoke(a1));
        }

        public static Func<T1, ValueTask<TOut>> ToAsyncWrapper<T1, TOut>(this Func<T1, Task<TOut>> func)
        {
            return (a1) => new ValueTask<TOut>(func.Invoke(a1));
        }

        public static Func<T1, ValueTask> ToAsyncWrapper<T1>(this Func<T1, Task> func)
        {
            return (a1) => new ValueTask(func.Invoke(a1));
        }

        public static Func<T1, T2, ValueTask<TOut>> ToAsyncWrapper<T1, T2, TOut>(this Func<T1, T2, TOut> func)
        {
            return (a1, a2) => ValueTask.FromResult(func.Invoke(a1, a2));
        }

        public static Func<T1, T2, ValueTask<TOut>> ToAsyncWrapper<T1, T2, TOut>(this Func<T1, T2, Task<TOut>> func)
        {
            return (a1, a2) => new ValueTask<TOut>(func.Invoke(a1, a2));
        }

        public static Func<T1, T2, ValueTask> ToAsyncWrapper<T1, T2>(this Func<T1, T2, Task> func)
        {
            return (a1, a2) => new ValueTask(func.Invoke(a1, a2));
        }

        public static Func<T1, T2, T3, ValueTask<TOut>> ToAsyncWrapper<T1, T2, T3, TOut>(this Func<T1, T2, T3, TOut> func)
        {
            return (a1, a2, a3) => ValueTask.FromResult(func.Invoke(a1, a2, a3));
        }

        public static Func<T1, T2, T3, ValueTask<TOut>> ToAsyncWrapper<T1, T2, T3, TOut>(this Func<T1, T2, T3, Task<TOut>> func)
        {
            return (a1, a2, a3) => new ValueTask<TOut>(func.Invoke(a1, a2, a3));
        }

        public static Func<T1, T2, T3, ValueTask> ToAsyncWrapper<T1, T2, T3>(this Func<T1, T2, T3, Task> func)
        {
            return (a1, a2, a3) => new ValueTask(func.Invoke(a1, a2, a3));
        }

        public static Func<T1, T2, T3, T4, ValueTask<TOut>> ToAsyncWrapper<T1, T2, T3, T4, TOut>(this Func<T1, T2, T3, T4, TOut> func)
        {
            return (a1, a2, a3, a4) => ValueTask.FromResult(func.Invoke(a1, a2, a3, a4));
        }

        public static Func<T1, T2, T3, T4, ValueTask<TOut>> ToAsyncWrapper<T1, T2, T3, T4, TOut>(this Func<T1, T2, T3, T4, Task<TOut>> func)
        {
            return (a1, a2, a3, a4) => new ValueTask<TOut>(func.Invoke(a1, a2, a3, a4));
        }

        public static Func<T1, T2, T3, T4, ValueTask> ToAsyncWrapper<T1, T2, T3, T4>(this Func<T1, T2, T3, T4, Task> func)
        {
            return (a1, a2, a3, a4) => new ValueTask(func.Invoke(a1, a2, a3, a4));
        }

        public static Func<T1, T2, T3, T4, T5, ValueTask<TOut>> ToAsyncWrapper<T1, T2, T3, T4, T5, TOut>(this Func<T1, T2, T3, T4, T5, TOut> func)
        {
            return (a1, a2, a3, a4, a5) => ValueTask.FromResult(func.Invoke(a1, a2, a3, a4, a5));
        }

        public static Func<T1, T2, T3, T4, T5, ValueTask<TOut>> ToAsyncWrapper<T1, T2, T3, T4, T5, TOut>(this Func<T1, T2, T3, T4, T5, Task<TOut>> func)
        {
            return (a1, a2, a3, a4, a5) => new ValueTask<TOut>(func.Invoke(a1, a2, a3, a4, a5));
        }

        public static Func<T1, T2, T3, T4, T5, ValueTask> ToAsyncWrapper<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5, Task> func)
        {
            return (a1, a2, a3, a4, a5) => new ValueTask(func.Invoke(a1, a2, a3, a4, a5));
        }

        public static Func<T1, T2, T3, T4, T5, T6, ValueTask<TOut>> ToAsyncWrapper<T1, T2, T3, T4, T5, T6, TOut>(this Func<T1, T2, T3, T4, T5, T6, TOut> func)
        {
            return (a1, a2, a3, a4, a5, a6) => ValueTask.FromResult(func.Invoke(a1, a2, a3, a4, a5, a6));
        }

        public static Func<T1, T2, T3, T4, T5, T6, ValueTask<TOut>> ToAsyncWrapper<T1, T2, T3, T4, T5, T6, TOut>(this Func<T1, T2, T3, T4, T5, T6, Task<TOut>> func)
        {
            return (a1, a2, a3, a4, a5, a6) => new ValueTask<TOut>(func.Invoke(a1, a2, a3, a4, a5, a6));
        }

        public static Func<T1, T2, T3, T4, T5, T6, ValueTask> ToAsyncWrapper<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6, Task> func)
        {
            return (a1, a2, a3, a4, a5, a6) => new ValueTask(func.Invoke(a1, a2, a3, a4, a5, a6));
        }
   }
}