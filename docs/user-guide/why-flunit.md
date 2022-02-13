# Why FlUnit?

FlUnits notable strengths include:

- **(Well-written) FlUnit tests are succinct & readable.**
The resultant reduced thinking time & confusion risk can be massively powerful, especially in "inner loop" (i.e. unit-level) tests.
In particular, the enforced structure for tests (notably, no interlacing of action and assertion) pushes you to write easily understandable tests.
- **It's conventionless and discoverable.**
The fluent builders enable you to discover functionality by your IDE showing available builder methods. No more having to look up what attributes you need to do parameterised tests, and how to inject context into a test class constructor.
- **A richer model for tests than that found in many other test frameworks** (without requiring the verbose code required by frameworks such as MSpec) makes a few things possible, some of which are demonstrated in the "getting started" guidance, above.
  - Parameterised tests are easy without requiring awkward attribute-based parameter retrieval. Note that this is essentially because the pre-requisites are passed to the "When" delegate - meaning that *all* tests are parameterised.
  - The "arrange" clauses of a test don't have to be counted as part of the test proper, providing an easy way to distinguish inconclusive tests (because their arrangements failed) from failed ones - providing some assistance to the isolation of issues.
  - Specifying each assertion separately means we can record them as a separate results of the test - providing an easy way to achieve the best practice of a single assertion per test result without requiring complex code factoring when a single action should have multiple consequences. Further, we can use language & framework features to name said results automatically (CallerExpressionArgument for .NET 6+, LINQ expressions for earlier versions). This makes it easy to write tests that show what is being asserted at a glance, without requiring test failure (or unhelpfully long test names). This in turn makes your tests more discoverable, and ultimately plays a small part in making your production code easier to maintain.

As with any design, there are downsides. If any of these FlUnit's notable weaknesses include:
- The enforced test structure can make certain scenarios a little awkward.
  - Primarily, people have become stuck when getting to grips with FlUnit and trying to assert on objects that are neither the return value of the `When` clause nor any of the pre-requisites referenced by it. There is a [simple pattern of usage](https://github.com/sdcondon/FlUnit/blob/main/docs/user-guide/useful-patterns.md#affected-object-graph-as-prerequisite) that can get you over this hurdle - but this is likely to main reason not to use FlUnit if you consider it too awkward.
  - Also consider what is needed to check the value of an out parameter. Ugly code..
- Delegate params get unwieldy for even a modest number of separate "Given" clauses. Of course, can always do a single Given of, say, an anonymous object with a bunch of things in it - as shown above. Using C# 9's lambda discard parameters can also make things a little clearer.
- All of the passing of test objects (arranged prerequisites, return values ..) between the provided delegates (as opposed to having a single test method) comes at a performance cost - though I've not run any explicit tests to validate the extent of this. The fact that the VSTest adapter is little more than a skeleton likely counteracts it to some degree at the moment.
