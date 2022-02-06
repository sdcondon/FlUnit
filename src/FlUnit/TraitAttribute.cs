using System;
using System.Reflection;

namespace FlUnit
{
    /// <summary>
    /// Attribute used to decorate a test, test class, or assembly with arbitrary name/value pairs ("traits").
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class TraitAttribute : Attribute, ITraitProvider
	{
		private readonly Trait trait;

		/// <summary>
		/// Creates a new instance of the <see cref="TraitAttribute"/> class.
		/// </summary>
		/// <param name="name">The trait name.</param>
		public TraitAttribute(string name) => trait = new Trait(name);

		/// <summary>
		/// Creates a new instance of the <see cref="TraitAttribute"/> class.
		/// </summary>
		/// <param name="name">The trait name.</param>
		/// <param name="value">The trait value.</param>
		public TraitAttribute(string name, string value) => trait = new Trait(name, value);

		/// <inheritdoc/>
		public Trait GetTrait(PropertyInfo testProperty) => trait;
	}
}
