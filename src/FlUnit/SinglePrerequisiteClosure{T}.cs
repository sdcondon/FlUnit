﻿using System;
using System.Collections.Generic;

namespace FlUnit
{
    /// <summary>
    /// Just to avoid ugly compiler-generated closures showing up in stack traces - while I figure out how best to
    /// tidy the relevant frames away from traces completely (and perhaps make debug behaviour a little better while at it - TPL?).
    /// </summary>
    /// <typeparam name="T">The type of the prerequisite.</typeparam>
    internal class SinglePrerequisiteClosure<T>
    {
        private readonly Func<ITestContext, T> prerequisite;

        public SinglePrerequisiteClosure(Func<ITestContext, T> prerequisite) => this.prerequisite = prerequisite;

        public IEnumerable<T> Arrange(ITestContext testContext) => new[] { prerequisite(testContext) };
    }
}
