# Flutter

An experimental **flu**en**t te**st f**r**amework.

```
Test TheWidgetShouldFlooABarble => Test
    .Given(new Widget("widget1", 4))
    .And(new Barble("warbly"))
    .When((w, b) => w.Floo(b))
    .Then((w, b, t) => t.Result.ShouldBe(true))
    .And((w, b, t) => b.IsFlooed.ShouldBe(true))
    .And((w, b, t) => w.HasFlooed.ShouldBe(true));
```