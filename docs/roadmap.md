# FlUnit Roadmap

Proper issue tracking would be overkill, so just a bullet list to organise my thoughts:

- *(v1.1 - Jun)* Possible post-v1 additions (after a break to work on other projects) - unlikely that ALL of this will make it into 1.1, but we'll see:
  - (COMPLETE) Basic output support, via a context object that can be specified as a pre-requisite (or part of a pre-requisite) using `Given` methods.
  - A little more configurability:
    - (MOSTLY COMPLETE) Allow for control over parallel partitioning - likely to be trait based (e.g. allow specification of a trait name - all tests with same value won't run in parallel).

- *(v1.2 or later - when new functionality would be useful for me, or if FlUnit hits somewhere around the ~10k download mark)* Other features:
  - Configurability:
    - Of strategy for duration records (which currently makes a "sensible" decision which may not be appropriate in all situations). Look at achieving greater accuracy in durations in the vstest adapter. Now that I realise you can record duration separately to start and end time. I could pause the the duration timing while doing framework-y things.. May creep to configurability of test execution strategy in general (should different cases be different "Tests" and so on)
    - Support custom test case labelling. Could be via support in IAssertion for format strings specified in config?
    - Expand on parallel partitioning control by allowing for by class name and namespace - whether thats treated as a special case or if we hook this into trait system is TBD.
  - Support for async tests?
  - Test attachment support
  - VSTest platform adapter internal improvements
    - Improvement of stack traces on test failure (eliminate FlUnit stack frames completely)
    - Get rid of some aspects of the core execution logic that are too influenced by VSTest
  - Basic test tidy-up support

Not going to do, at least in the near future:
- QoL: Perhaps `Then/AndOfReturnValue(rv => rv.ShouldBe..)` and `Then/AndOfGiven1(g => g.Prop.ShouldBe..)` for succinctness? No - Lambda discards work pretty well (to my eyes at least), and `OfGiven1`, `OfGiven2` is better dealt with via complex prereq objects
- QoL: dependent assertions - some assertions only make sense if a prior assertion has succeeded (easy for method-based test frameworks, but not for us..). Such assertions should probably give an inconclusive result? Assertions that return a value (assert a value is of a particular type, cast and return it) also a possibility - though thats probably inviting unacceptable complexity. A basic version of this could be useful though - perhaps an `AndAlso` (echoing C# operator name) - which will make all following assertions inconclusive if any prior assertion failed? No - this is best left to assertion frameworks (e.g. FluentAssertions `Which`)
