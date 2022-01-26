namespace FlUnit
{
    internal static class AssertionExpressionHelpers
    {
        /// <summary>
        /// Determines an appropriate label, given an assertion expression.
        /// </summary>
        /// <param name="assertionExpression">The assertion expression.</param>
        /// <returns>An appropriate label for the assertion.</returns>
        public static string ToAssertionDescription(string assertionExpression)
        {
            // For lambdas, use only the body.
            // Yes, this logic won't work if its a non-lambda that contains a lambda.
            // Could regex on ^ (...) => ..., but might be slow.
            // Users can override by providing description to work around, so leaving it like this, at least for the moment.
            var lambdaIndex = assertionExpression.IndexOf("=>");
            if (lambdaIndex > -1 && assertionExpression.Length > lambdaIndex + 3)
            {
                assertionExpression = assertionExpression.Substring(lambdaIndex + 3).Trim();
            }

            return assertionExpression;
        }
    }
}
