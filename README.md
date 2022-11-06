![FlUnit Logo](src/FlUnit-128.png)

# FlUnit

[![NuGet version (FlUnit)](https://img.shields.io/nuget/v/FlUnit.svg?style=flat-square)](https://www.nuget.org/packages/FlUnit/) [![NuGet downloads (FlUnit)](https://img.shields.io/nuget/dt/FlUnit.svg?style=flat-square)](https://www.nuget.org/packages/FlUnit/)

This repo contains the source code for the FlUnit NuGet package.

## Package Overview

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

The FlUnit NuGet package is the core library of FlUnit - it contains all of the builder stuff that you directly consume to write your FlUnit tests.

## Package Documentation

Available documentation can be found in the docs folder of this repository for the moment. Specifically, we have:

* **[User Guide](./docs/user-guide/README.md):** FlUnit's user guide is admittedly a little sparse. The degree to which it is expanded upon will correlate with FlUnit's popularity..
  * **[Getting Started](./docs/user-guide/getting-started.md):** Instructions for getting started with FlUnit
  * **[Useful Patterns](./docs/user-guide/useful-patterns.md):** A few patterns that may prove useful when writing FlUnit tests
  * **[Advanced Functionality](./docs/user-guide/advanced-functionality.md):** Details of functionality not covered in "getting started"
  * **[Other Notes](./docs/user-guide/other-notes.md):** Assorted notes regarding the design of FlUnit and its usage
* **[Roadmap](./docs/roadmap.md):** Proper issue tracking would be overkill at this point, so there are just some bullet points to organise my thoughts

## Issues and Contributions

I'm not expecting anyone to want to get involved given the relatively low download figures on NuGet, but please feel free to do so.
I do keep a vague eye on the issues tab, and will add a CONTRIBUTING.md if anyone drops me a message expressing interest.

## See Also

Like this? If so, you might also want to check out:

* **[FluentAssertions](https://fluentassertions.com/):** Assertion framework that pairs very well with FlUnit, for obvious reasons.
* **[The projects that use FlUnit](https://github.com/sdcondon/FlUnit/network/dependents):** see these projects for real examples of FlUnit usage
* **[My GitHub Profile!](https://github.com/sdcondon):** My profile README lists a whole bunch of stuff that I've made.