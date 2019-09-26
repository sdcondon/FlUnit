# Flutter

An experimental **flu**en**t te**st f**r**amework.

```
Test WidgetCanFlooABarble => TestThat
    .Given(new Widget("widget1", 4))
    .And(new Barble("warbly"))
    .When((w, b) => w.Floo(b))
    .Then((w, b, t) => t.Result.ShouldBe(true))
    .And((w, b, t) => b.IsFlooed.ShouldBe(true))
    .And((w, b, t) => w.HasFlooed.ShouldBe(true));
```

## References

* https://github.com/microsoft/vstest-docs/blob/master/RFCs/0004-Adapter-Extensibility.md
* https://devblogs.microsoft.com/devops/writing-a-visual-studio-2012-unit-test-adapter/