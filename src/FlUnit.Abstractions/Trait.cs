namespace FlUnit
{
    /// <summary>
    /// Representation of a test trait.
    /// </summary>
    public sealed class Trait
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Trait"/> class.
        /// </summary>
        /// <param name="name">The name of the trait.</param>
        public Trait(string name) => Name = name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Trait"/> class.
        /// </summary>
        /// <param name="name">The name of the trait.</param>
        /// <param name="value">The value of the trait.</param>
        public Trait(string name, string value) => (Name, Value) = (name, value);

        /// <summary>
        /// Gets the name of the trait.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the trait, if any.
        /// </summary>
        public string Value { get; }
    }
}
