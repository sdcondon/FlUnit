namespace FlUnit.Adapters
{
    /// <summary>
    /// Representation of a test trait.
    /// </summary>
    public sealed class Trait : ITrait
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ITrait"/> class.
        /// </summary>
        /// <param name="name">The name of the trait.</param>
        public Trait(string name) => Name = name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ITrait"/> class.
        /// </summary>
        /// <param name="name">The name of the trait.</param>
        /// <param name="value">The value of the trait.</param>
        public Trait(string name, string value) => (Name, Value) = (name, value);

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Value { get; }
    }
}
