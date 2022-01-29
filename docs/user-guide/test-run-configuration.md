# Test Run Configuration

*(NB: not yet published - implemented in v0.14+)*

If VSTest is being used, FlUnit test configuration can be provided in a "FlUnit" element in the .runsettings file. See the [annotated example](../../src/Example.TestProject/.runsettings) in the example test project.

Configuration that applies to individual tests can be overridden by individual tests through the use of the `UsingConfiguration` builder method. Configuration overrides can be specified at any point up until the `When` clause is specified.
