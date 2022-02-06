using System;
using System.Collections.Generic;

namespace FlUnit
{
    // Just to avoid ugly compiler-generated closures showing up in stack traces - while I figure out how to
    // tidy them away from traces completely (and perhaps make debug behaviour a little better while at it - TPL?).
    internal class SinglePrerequisiteClosure<T>
    {
        private Func<T> prerequisite;

        public SinglePrerequisiteClosure(Func<T> prerequisite) => this.prerequisite = prerequisite;

        public IEnumerable<T> Arrange() => new[] { prerequisite() };
    }
}
