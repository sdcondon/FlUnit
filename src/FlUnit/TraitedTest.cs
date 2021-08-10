using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FlUnit
{
    /// <summary>
    /// Decorator class that adds traits to the underlying test.
    /// </summary>
    public class TraitedTest : Test
    {
        private readonly Test innerTest;
        private readonly CompositeDictionary traits;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraitedTest"/> class.
        /// </summary>
        /// <param name="innerTest"></param>
        /// <param name="traitName"></param>
        /// <param name="traitValue"></param>
        public TraitedTest(Test innerTest, string traitName, string traitValue)
        {
            this.innerTest = innerTest;
            this.traits = new CompositeDictionary(innerTest.Traits);
            traits.Add(traitName, traitValue);
        }

        /// <summary>
        /// Gets the traits of the test.
        /// </summary>
        public override IReadOnlyDictionary<string, string> Traits => traits;

        /// <inheritdoc />
        public override IReadOnlyCollection<ITestCase> Cases => innerTest.Cases;

        /// <inheritdoc />
        public override void Arrange() => innerTest.Arrange();
        
        /// <summary>
        /// Adds a trait to the test.
        /// </summary>
        /// <param name="name">The name of the trait.</param>
        /// <param name="value">the value of the treait.</param>
        internal void AddTrait(string name, string value) => traits.Add(name, value);

        private class CompositeDictionary : IDictionary<string, string>, IReadOnlyDictionary<string, string>
        {
            private readonly IReadOnlyDictionary<string, string> innerDictionary;
            private readonly Dictionary<string, string> selfDictionary = new Dictionary<string, string>();

            public CompositeDictionary(IReadOnlyDictionary<string, string> innerDictionary) => this.innerDictionary = innerDictionary;

            public string this[string key]
            {
                get
                {
                    if (selfDictionary.TryGetValue(key, out var value) || innerDictionary.TryGetValue(key, out value))
                    {
                        return value;
                    }

                    throw new KeyNotFoundException();
                }

                set
                {
                    selfDictionary[key] = value;
                }
            }

            public ICollection<string> Keys => throw new NotSupportedException(); // Not needed internally and not part of IReadOnlyDictionary<string, string>

            public ICollection<string> Values => throw new NotSupportedException(); // Not needed internally and not part of IReadOnlyDictionary<string, string>

            IEnumerable<string> IReadOnlyDictionary<string, string>.Keys => innerDictionary.Keys.Union(selfDictionary.Keys); // NB: Not concat because duplicates are possible (see indexer implementation).

            IEnumerable<string> IReadOnlyDictionary<string, string>.Values => innerDictionary.Values.Concat(selfDictionary.Values); // TODO: dedupe

            public int Count => innerDictionary.Count + selfDictionary.Count;

            public bool IsReadOnly => throw new NotSupportedException(); // Not needed internally and not part of IReadOnlyDictionary<string, string>

            public void Add(string key, string value)
            {
                if (innerDictionary.ContainsKey(key) || selfDictionary.ContainsKey(key))
                {
                    throw new ArgumentException();
                }

                selfDictionary.Add(key, value);
            }

            public void Add(KeyValuePair<string, string> item) => throw new NotSupportedException(); // Not needed internally and not part of IReadOnlyDictionary<string, string>

            public void Clear() => throw new NotSupportedException(); // Not needed internally and not part of IReadOnlyDictionary<string, string>

            public bool Contains(KeyValuePair<string, string> item) => throw new NotSupportedException(); // Not needed internally and not part of IReadOnlyDictionary<string, string>

            public bool ContainsKey(string key) => innerDictionary.ContainsKey(key) || selfDictionary.ContainsKey(key);

            public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) => throw new NotSupportedException(); // Not needed internally and not part of IReadOnlyDictionary<string, string>

            public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => innerDictionary.Concat(selfDictionary).GetEnumerator();

            public bool Remove(string key) => throw new NotSupportedException(); // Not needed internally and not part of IReadOnlyDictionary<string, string>

            public bool Remove(KeyValuePair<string, string> item) => throw new NotSupportedException(); // Not needed internally and not part of IReadOnlyDictionary<string, string>

            public bool TryGetValue(string key, out string value) => selfDictionary.TryGetValue(key, out value) || innerDictionary.TryGetValue(key, out value);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
