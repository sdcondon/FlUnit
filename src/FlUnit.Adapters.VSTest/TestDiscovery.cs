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

            // TODO-PERFORMANCE: parallelisation? plinq?
            return assembly.ExportedTypes
                .Select(t => ConcatTraitProviders(t, assemblyTraitProviders))
                .SelectMany(t => t.member.GetProperties().Where(IsTestProperty).Select(p =>
                {
                    var (member, traitProviders) = ConcatTraitProviders(p, t.traitProviders);
                    return new TestMetadata(member, traitProviders.Select(tp => tp.Trait));
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
