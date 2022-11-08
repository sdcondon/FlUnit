# FlUnit

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

The recommended initial learning path is as follows:

1. **[Getting Started](user-guide/getting-started.md):** Instructions for getting started with FlUnit.
1. **[Useful Patterns](user-guide/useful-patterns.md):** A few patterns that may prove useful when writing FlUnit tests
1. **[Advanced Functionality](user-guide/advanced-functionality.md):** Details of functionality not covered in "getting started"
1. **[Other Notes](user-guide/other-notes.md):** Assorted notes regarding the design of FlUnit and its usage
