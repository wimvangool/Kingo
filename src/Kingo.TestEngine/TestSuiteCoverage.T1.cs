using System;
using System.Text;
using Kingo.Resources;
using Kingo.Threading;

namespace Kingo
{
    /// <summary>
    /// Represents a record of specific paths that can be monitored during execution of a test-suite, so that
    /// can be determined whether or not a <see cref="TestSuite{T}" /> has covered all relevant paths in code.
    /// </summary>
    /// <typeparam name="TPathsEnum">Type of the enumeration used to indicate each path.</typeparam>
    public sealed class TestSuiteCoverage<TPathsEnum> where TPathsEnum : struct
    {
        private static readonly TPathsEnum _All = EnumOperators<TPathsEnum>.AllValuesCombined();
        private readonly ITestEngine _testEngine;
        private TPathsEnum _paths;

        private TestSuiteCoverage(ITestEngine testEngine)
        {
            _testEngine = testEngine;
        } 

        /// <summary>
        /// Records the specific <paramref name="path"/> as covered.
        /// </summary>
        /// <param name="path">The covered path(s).</param>
        public void Add(TPathsEnum path)
        {
            _paths = EnumOperators<TPathsEnum>.Or(_paths, path);
        }

        /// <summary>
        /// Asserts that all paths specified by <typeparamref name="TPathsEnum"/> are covered.
        /// </summary>
        /// <exception cref="Exception">
        /// If not all paths have been covered.
        /// </exception>
        public void AssertAllCovered()
        {
            if (_paths.Equals(_All))
            {
                return;
            }
            var errorMessage = new StringBuilder(ExceptionMessages.TestSuiteCoverage_MissingPaths);

            foreach (var value in EnumOperators<TPathsEnum>.AllValues())
            {
                if (EnumOperators<TPathsEnum>.IsDefined(value, _paths))
                {
                    continue;
                }
                errorMessage.AppendFormat("[{0}]", value);
            }
            throw _testEngine.NewTestFailedException(errorMessage.ToString());
        }

        private static readonly Context<TestSuiteCoverage<TPathsEnum>> _Context = new Context<TestSuiteCoverage<TPathsEnum>>();

        /// <summary>
        /// Returns the current, thread-specific instance.
        /// </summary>
        public static TestSuiteCoverage<TPathsEnum> Current
        {
            get { return _Context.Current; }
        }

        /// <summary>
        /// Creates and returns a new <see cref="ContextScope{T}" /> for a new <see cref="TestSuiteCoverage{T}" /> instance.
        /// </summary>
        /// <param name="testEngine">The associated test engine.</param>
        /// <returns>A new <see cref="ContextScope{T}" />.</returns>
        public static ContextScope<TestSuiteCoverage<TPathsEnum>> StartCoverageMeasurement(ITestEngine testEngine)
        {
            if (testEngine == null)
            {
                throw new ArgumentNullException(nameof(testEngine));
            }
            return _Context.OverrideThreadLocal(new TestSuiteCoverage<TPathsEnum>(testEngine));
        }
    }
}
