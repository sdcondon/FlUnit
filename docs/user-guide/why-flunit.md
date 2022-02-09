# Why FlUnit?

FlUnits notable strengths include:
- Succinct & readable.
  - I would argue that the resultant reduced thinking time & confusion risk significantly mitigates any performance shortfalls (which I should stress I don't necessarily know are there, end-to-end - but see the "cons" section for some suspicions).
  - In particular, the enforced structure for tests (notably, no interlacing of action and assertion) pushes you to write easily understandable tests.
- A richer model for tests than that found in many other test frameworks (without requiring the verbose code required by frameworks such as MSpec) makes a few things possible, some of which are demonstrated in the "getting started" guidance, above.
  - Parameterised tests are easy without requiring awkward attribute-based parameter retrieval. Note that this is essentially because the pre-requisites are passed to the "When" delegate - meaning that *all* tests are parameterised.
  - The "arrange" clauses of a test don't have to be counted as part of the test proper, providing an easy way to distinguish inconclusive tests (because their arrangements failed) from failed ones - providing some assistance to the isolation of issues.
  - Specifying each assertion separately means we can record them as a separate results of the test - providing an easy way to achieve the best practice of a single assertion per test result without requiring complex code factoring when a single action should have multiple consequences. Further, we can use language & framework features to name said results automatically (CallerExpressionArgument for .NET 6+, LINQ expressions for earlier versions). This makes it easy to write tests that show what is being asserted at a glance, without requiring test failure (or unhelpfully long test names). This in turn makes your tests more discoverable, and ultimately plays a small part in making your production code easier to maintain.

FlUnit's notable weaknesses include:
- The enforced test structure can make certain scenarios mildly awkward. Consider for example what is needed to check the value of an out parameter.
- All of the passing of test objects (arranged prerequisites, return values ..) between the provided delegates (as opposed to having a single test method) comes at a performance cost - though I've not run any explicit tests to validate the extent of this. The fact that the VSTest adapter is little more than a skeleton likely counteracts it to some degree at the moment.
- Delegate params get unwieldy for even a modest number of separate "Given" clauses. Of course, can always do a single Given of, say, an anonymous object with a bunch of things in it - as shown above. Using C# 9's lambda discard parameters can also make things a little clearer.
