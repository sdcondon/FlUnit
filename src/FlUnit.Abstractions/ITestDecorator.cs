namespace FlUnit
{
    /// <summary>
    /// Test runners should look for attributes attached to tests, test classes or test assemblies that implement this interface,
    /// and <see cref="Apply(Test)"/> them to each of the tests that they cover.
    /// </summary>
    public interface ITestDecorator
    {
        /// <summary>
        /// Applies this decorator to a given test.
        /// </summary>
        /// <param name="test">The test to decorate.</param>
        /// <returns>The decorated test.</returns>
        Test Apply(Test test);
    }
}
