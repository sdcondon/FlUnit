# FlUnit Roadmap

Proper issue tracking would be overkill at this point, so just a bullet list to organise my thoughts:

- *(May / Jun - v1.1)* Possible post-v1 additions (after a break to work on other projects) - unlikely that ALL of this will make it into 1.1, but we'll see:
  - VSTest platform adapter internal improvements
    - Improvement of stack traces on test failure (eliminate FlUnit stack frames completely)
    - Get rid of some aspects of the core execution logic that are too influenced by VSTest
  - A little more configurability:
    - For specification of strategy for duration records (which currently makes a "sensible" decision which may not be appropriate in all situations).
    - Allow for control over parallel partitioning - likely to be trait based (e.g. allow specification of a trait name - all tests with same value won't run in parallel). Also want to allow for by class name and namespace - whether thats treated as a special case or if we hook this into trait system is TBD.
    - Support custom test case labelling. Could be via support in IAssertion for format strings specified in config?
  - Basic test tidy-up support
  - (most likely of these to be shunted into 1.2) Basic attachment & output support.
This is likely to require injecting some kind of test context object.
I really want to double-down on the convention-less/static nature of FlUnit - i.e. no convention-based ctor parameters, all discoverable via IDE method listings etc.
Plan A right now is to introduce some kind of ITestContext as a prerequisite if needed.
That is, `TestThat.GivenTestContext().And()...When((cxt, ) => ...)`.
This particular approach doesn't allow for test context to be placed inside an anonymous prereq object, though.
Which is perhaps a good thing? But is a mandate for users, rather than a choice.
More concerning is that it doesn't allow context to be used during prerequisite creation.
So, instead (or as well) could allow for cxt to be specified as a parameter of Given delegates (`Given(cxt => ...)`).
Then `GivenTestContext()` could still exist, simply as a more readable alias of `Given(cxt => cxt)`.
Hmm, maybe - this is more complex?
Still mulling this one over.
- *(At some point, maybe - v1.2 or later)* Other features:
  - Look at achieving greater accuracy in durations in the vstest adapter. Now that I realise you can record duration separately to start and end time. I could pause the the duration timing while doing framework-y things..
  - Support for async tests?

Not going to do, at least in the near future:
- QoL: Perhaps `Then/AndOfReturnValue(rv => rv.ShouldBe..)` and `Then/AndOfGiven1(g => g.Prop.ShouldBe..)` for succinctness? No - Lambda discards work pretty well (to my eyes at least), and `OfGiven1`, `OfGiven2` is better dealt with via complex prereq objects
- QoL: dependent assertions - some assertions only make sense if a prior assertion has succeeded (easy for method-based test frameworks, but not for us..). Such assertions should probably give an inconclusive result? Assertions that return a value (assert a value is of a particular type, cast and return it) also a possibility - though thats probably inviting unacceptable complexity. A basic version of this could be useful though - perhaps an `AndAlso` (echoing C# operator name) - which will make all following assertions inconclusive if any prior assertion failed? No - this is best left to assertion frameworks (e.g. FluentAssertions `Which`)
