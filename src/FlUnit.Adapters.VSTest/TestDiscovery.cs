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
        /// Finds all of the properties that represent tests in a given loaded assembly - along with the trait providers that should be applied to each.
        /// </summary>
        /// <param name="assembly">The assembly to look in.</param>
        /// <param name="runSettings">Unused for the moment. Sue me.</param>
        /// <returns>An enumerable of test metadata.</returns>
        public static IEnumerable<TestMetadata> FindTests(Assembly assembly, TestRunSettings runSettings)
        {
            (T member, IEnumerable<ITraitProvider> traitProviders) ConcatTraitProviders<T>(T memberInfo, IEnumerable<ITraitProvider> traitProviders)
                where T : MemberInfo
            {
                return (memberInfo, traitProviders: traitProviders.Concat(memberInfo.GetCustomAttributes().OfType<ITraitProvider>()));
            }

            var assemblyTraitProviders = assembly.GetCustomAttributes().OfType<ITraitProvider>();

            // TODO-PERFORMANCE: parallelisation? plinq?
            // TODO-MAINTAINABILITY: this is hideous
            return assembly
                .ExportedTypes.Select(t => ConcatTraitProviders(t, assemblyTraitProviders))
                .SelectMany(t => t.member.GetProperties().Where(IsTestProperty).Select(p =>
                {
                    var tuple = ConcatTraitProviders(p, t.traitProviders);
                    return new TestMetadata(tuple.member, tuple.traitProviders);
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
