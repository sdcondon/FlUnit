namespace FlUnit
{
    /// <summary>
    /// Interface for types representing a test trait.
    /// </summary>
    public interface ITrait
    {
        /// <summary>
        /// Gets the name of the trait.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the value of the trait.
        /// </summary>
        string Value { get; }
    }
}
