![FlUnit Logo](src/FlUnitIcon.png)

# FlUnit

[![NuGet version (FlUnit)](https://img.shields.io/nuget/v/FlUnit.svg?style=flat-square)](https://www.nuget.org/packages/FlUnit/) [![NuGet downloads (FlUnit)](https://img.shields.io/nuget/dt/FlUnit.svg?style=flat-square)](https://www.nuget.org/packages/FlUnit/)

FlUnit is a test framework in which tests are written using [builders](https://en.wikipedia.org/wiki/Builder_pattern) that expose a [fluent interface](https://en.wikipedia.org/wiki/Fluent_interface), like this:

```csharp
public static Test SumOfEvenAndOdd => TestThat
  .GivenEachOf(() => new[] { 1, 3, 5 })
  .AndEachOf(() => new[] { 2, 4, 6 })
  .When((x, y) => x + y)
  .ThenReturns()
  .And((_, _, sum) => (sum % 2).Should().Be(1))
  .And((x, _, sum) => sum.Should().BeGreaterThan(x))
  .And((_, y, sum) => sum.Should().BeGreaterThan(y));
```

Available documentation can be found in the docs folder of this repository for the moment. Specifically, we have:
* **[User Guide](./docs/user-guide/README.md):** FlUnit's user guide is admittedly a little sparse. The degree to which it is expanded upon will correlate with FlUnit's popularity..
  * **[Getting Started](./docs/user-guide/getting-started.md):** Instructions for getting started with FlUnit.
  * **[Why FlUnit?](./docs/user-guide/why-flunit.md):** A brief overview on why you might want to use FlUnit (and why not).
  * **[Useful Patterns](./docs/user-guide/useful-patterns.md):** A few patterns that may prove useful when writing FlUnit tests
  * **[Test Run Configuration](./docs/user-guide/test-run-configuration.md):** What aspects of test execution can be configured, and how to accomplish this
  * **[Other Notes](./docs/user-guide/other-notes.md):** Assorted notes regarding the design of FlUnit and its usage
* **[Roadmap](./docs/roadmap.md):** Proper issue tracking would be overkill at this point, so there's just some bullet points to organise my thoughts
