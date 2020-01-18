using System;
using System.Collections.Generic;

namespace FlUnit
{
    public interface ITest
    {
        void Run();
        IEnumerable<(Action, string)> Assertions { get; }
    }
}
