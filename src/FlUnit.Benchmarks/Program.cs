using BenchmarkDotNet.Running;
using System.Reflection;

namespace FlUnit.Benchmarks
{
    public class Program
    {
        /// <summary>
        /// Application entry point.
        /// </summary>
        public static void Main(string[] args)
        {
            // See https://benchmarkdotnet.org/articles/guides/console-args.html (or run app with --help)
            BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);
        }
    }
}