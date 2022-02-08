namespace FlUnit
{
    /// <summary>
    /// Interface for representataions of test traits.
    /// </summary>
    public interface ITrait
    {
        /// <summary>
        /// Gets the name of the trait.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the value of the trait, if any.
        /// </summary>
        string Value { get; }
    }
}
