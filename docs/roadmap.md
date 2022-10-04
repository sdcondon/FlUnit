# FlUnit Roadmap

Proper issue tracking would be overkill, so just a bullet list to organise my thoughts:

- I'm starting to think about **v1.2** now, driven purely by things that annoy me in my own usage of this framework. v1.2 is likely to include:
  - Configurability:
    - Take a look at configurability of test execution strategy in general (should different cases be different "Tests" and so on)
    - Test case labelling is annoying at the mo. Support custom test case labelling. Could be via support in IAssertion for format strings specified in config? But honestly thinking that was a mistake. Second param to "When" builder methods maybe.. The goal is to make it easy to produce really good result labels.
- On the to-do list for later:
  - Configurability:
    - Of strategy for duration records (which currently makes a "sensible" decision which may not be appropriate in all situations). Look at achieving greater accuracy in durations in the vstest adapter. Now that I realise you can record duration separately to start and end time. I could pause the the duration timing while doing framework-y things..
    - Expand on parallel partitioning control by allowing for by class name and namespace - whether thats treated as a special case or if we hook this into trait system is TBD.
  - Basic test tidy-up support. Open questions here about if/when we should consider objects (pre-requisites, test function return values) to be "owned" by the test, and thus its responsibility to dispose of. What is the ideal default behaviour, and by what mechanisms should we support deviation from that.
  - Support for async tests?
  - Test attachment support
  - VSTest platform adapter internal improvements
    - Improvement of stack traces on test failure (eliminate FlUnit stack frames completely)
    - Get rid of some aspects of the core execution logic that are too influenced by VSTest


Not going to do, at least in the near future:
- QoL: Perhaps `Then/AndOfReturnValue(rv => rv.ShouldBe..)` and `Then/AndOfGiven1(g => g.Prop.ShouldBe..)` for succinctness? No - Lambda discards work pretty well (to my eyes at least), and `OfGiven1`, `OfGiven2` is better dealt with via complex prereq objects
- QoL: dependent assertions - some assertions only make sense if a prior assertion has succeeded (easy for method-based test frameworks, but not for us..). Such assertions should probably give an inconclusive result? Assertions that return a value (assert a value is of a particular type, cast and return it) also a possibility - though thats probably inviting unacceptable complexity. A basic version of this could be useful though - perhaps an `AndAlso` (echoing C# operator name) - which will make all following assertions inconclusive if any prior assertion failed? No - this is best left to assertion frameworks (e.g. FluentAssertions `Which`)
