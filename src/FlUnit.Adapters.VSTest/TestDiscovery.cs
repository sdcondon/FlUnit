using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FlUnit.Adapters
{
    /// <summary>
    /// Core logic for finding FlUnit tests.
    /// </summary>
    internal static class TestDiscovery
    {
        /// <summary>
        /// Finds all of the properties that represent tests in a given loaded assembly - along with the traits that are associated with each.
        /// </summary>
        /// <param name="assembly">The assembly to look in.</param>
        /// <param name="runConfiguration">Unused for the moment. Sue me.</param>
        /// <returns>An enumerable of <see cref="TestMetadata"/>, one for each discovered test.</returns>
        public static IEnumerable<TestMetadata> FindTests(Assembly assembly, TestRunConfiguration runConfiguration)
        {
            (T member, IEnumerable<ITraitProvider> traitProviders) ConcatTraitProviders<T>(T memberInfo, IEnumerable<ITraitProvider> traitProviders)
                where T : MemberInfo
            {
                return (memberInfo, traitProviders: traitProviders.Concat(memberInfo.GetCustomAttributes().OfType<ITraitProvider>()));
            }

            var assemblyTraitProviders = assembly.GetCustomAttributes().OfType<ITraitProvider>();

            // NB: Possible performance concerns here. Benchmarks proj shows that an AsParallel
            // here slows example test proj run down, though. That may just be due to its small
            // size, but then most test projects could probably be expected to be small?
            // More testing needed before doing anything differently here.
            return assembly.ExportedTypes
                .Select(t => ConcatTraitProviders(t, assemblyTraitProviders))
                .SelectMany(t => t.member.GetProperties().Where(IsTestProperty).Select(p =>
                {
                    var (testProperty, traitProviders) = ConcatTraitProviders(p, t.traitProviders);
                    return new TestMetadata(testProperty, traitProviders.Select(tp => tp.GetTrait(testProperty)));
                }));
        }

        private static bool IsTestProperty(PropertyInfo p)
        {
            return p.CanRead
                && p.GetMethod.IsPublic
                && p.GetMethod.IsStatic
                && typeof(Test).IsAssignableFrom(p.PropertyType);
        }
    }
}
