using System;

namespace FlUnit
{
    /// <summary>
    /// Attribute used to decorate a test method, test class, or assembly with arbitrary name/value pairs ("traits").
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class TraitAttribute : Attribute, ITestDecorator
	{
		/// <summary>
		/// Creates a new instance of the <see cref="TraitAttribute"/> class.
		/// </summary>
		/// <param name="name">The trait name.</param>
		/// <param name="value">The trait value.</param>
		public TraitAttribute(string name, string value) => (Name, Value) = (name, value);

		/// <summary>
		/// Gets the name of the trait.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the value of the trait.
		/// </summary>
		public string Value { get; }

		/// <inheritdoc />
		public Test Apply(Test test)
        {
			if (test is TraitedTest traitedTest)
            {
				// May not keep this, but will do for now.
				// TraitedTest is mutable to facilitate reduction of the amount of nesting when there are multiple <see cref="TraitAttributes"/>.
				// Not infallible (requires all decorators of the same type to be applied together), though there are
				// ways to deal with that (e.g. TestDecorator base class, and search down through decorated tests instead of this).
				traitedTest.AddTrait(Name, Value);
				return traitedTest;
            }
            else
            {
				return new TraitedTest(test, Name, Value);
            }
        }
	}
}
