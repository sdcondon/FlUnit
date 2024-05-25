# FlUnit Roadmap

Proper issue tracking and a formal plan would be overkill, so the "roadmap" consists of just a bullet list to organise my thoughts:

At the moment:

- *Nothing at the moment - working on other projects*.

On the to-do list for soon-ish:

- Basic test tidy-up support. Open questions here about if/when we should consider objects (prerequisites,
  test function return values) to be "owned" by the test, and thus its responsibility to dispose of. What
  is the ideal default behaviour, and by what mechanisms should we support deviation from that.
- Test run parameter support, attachment support
- Allow for instantiable test fixtures rather than just static test properties.
  Of course, test prerequisites and builder reusability perhaps offers an alternative way to approach this kind of thing, but there's
  almost certainly still value in this.

Other things on the to-do list:

- VSTest platform adapter internal improvements
  - Improvement of stack traces on test failure (eliminate FlUnit stack frames completely)
  - Get rid of some aspects of the core execution logic that are too influenced by VSTest
- Configurability:
  - Test case labelling could still be better after the minor improvement made in v1.2. Better support for custom test case labelling, and perhaps further improved default labelling. Currently mulling over some options, including:
	- ~~in default labelling, spot and eliminate *all* type names (even ones contained *within* prereq tostrings..).
	  not trivial, it seems - cant e.g. verify anon type names with gettype.
	  would probably require reflection - which id really rather avoid in a default behaviour.~~
	- ~~in default labelling, eliminate common result suffixes.
	  would require creating all results before submitting any.
	  would also rather avoid - hacky and the results look awkward anyway.
	  Rejected idea included only for completeness.~~
	- Add LabelledAs(delegate) builder method to override prereq labelling in a strongly-typed manner.
	  Not too tough, but overlaps/overrides other behaviours in a perhaps confusing way..?
	  i.e. would need to play nice with labelling strategy.
	  A promising option, though.
    - Add format string support to prerreqs.
	  "g" for current behaviour, "i" for test case index, integers for ToString of prereq of corresponding index.
	  Useless on its own, would need labelling strategy to make use of it (see need bullet points).
	  But seems a good idea.
    - Add config property for result label (e.g. `UsingConfiguration(c => c.ResultLabel = "{0} for test case {1:test case #@i}")` or `UsingConfiguration(c => c.ResultLabel = "{0} for test case {1:@0 + @1}")`).
	  useless on its own - requires the labelling strategy to use it.
    - Add config property for prereq format (e.g. `UsingConfiguration(c => c.PrerequisiteFormatString = "test case #@i")` or `UsingConfiguration(c => c.PrerequisiteFormatString = "@0 + @1")`).
      useless on its own - requires the labelling strategy to use it.
      Con: seems most useful for particular values for particular test cases, but config really for stuff across a whole test suite.
      when its for a particular test, `LabelledAs` feels more powerful?
  - Parallelisation:
    - in vstest adapter, look into uing its test parallelisation setting rather than one specific to flunit.
    - Expand on parallel partitioning control by allowing for by class name and namespace - whether thats treated as a special case or if we hook this into trait system is TBD.
  - Of strategy for duration records (which currently makes a "sensible" decision which may not be appropriate in all situations).
    Look at achieving greater accuracy in durations in the vstest adapter.
	Now that I realise you can record duration separately to start and end time.
	I could pause the the duration timing while doing framework-y things..
  - Take a look at configurability of test execution strategy in general. Things like:
    - Should the test action be re-run for each assertion (if e.g. they aren't isolated).  
	  Would (need to re-run "given" clauses too, and there's no guarantee the same values are returned, so would) need to check pre-requisite equality.
	  Something extra for test writers to think about, so prob best to leave default behaviour as NOT doing this.
    - Should different cases be different "Tests".
      *NTS: What this'd look like, probably: TestDiscovery to get Test and `Arrange` it.
      Look at resulting (potentially overridden) test config for appropriate granularity setting.
      Then return testmetadata that now optionally include case/assertion index.
      This index info would need to be included in VSTest case serialization (see VSTest.TestDiscoverer and TestContainer).
      TestRun would need to then act accordingly (TBD whether it could/should execute once but split the results, or rerun) based on the metadata.
      As with re-runs for assertions (see above), also need to worry test cases not being the same. And in this case it's worse since there is serialisation involved.
      Would need to allow different approaches (assume equality by position, leverage serialisable prereqs, etc).

Not going to do:
- QoL: Perhaps `Then/AndOfReturnValue(rv => rv.ShouldBe..)` and `Then/AndOfGiven1(g => g.Prop.ShouldBe..)` for succinctness? No - Lambda discards work pretty well (to my eyes at least), and `OfGiven1`, `OfGiven2` is better dealt with via complex prereq objects
- QoL: dependent assertions - some assertions only make sense if a prior assertion has succeeded (easy for method-based test frameworks, but not for us..). Such assertions should probably give an inconclusive result? Assertions that return a value (assert a value is of a particular type, cast and return it) also a possibility - though thats probably inviting unacceptable complexity. A basic version of this could be useful though - perhaps an `AndAlso` (echoing C# operator name) - which will make all following assertions inconclusive if any prior assertion failed? No - this is best left to assertion frameworks (e.g. FluentAssertions `Which`)
