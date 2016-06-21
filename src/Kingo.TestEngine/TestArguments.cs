using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingo
{
    /// <summary>
    /// Represents a set of arguments that were used to execute a specific test.
    /// </summary>
    public sealed class TestArguments : IReadOnlyList<object>
    {
        private readonly object[] _arguments;

        private TestArguments(object[] arguments)
        {
            _arguments = arguments;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Join(", ", _arguments);
        }

        #region [====== ReadOnlyList ======]

        /// <inheritdoc />
        public int Count
        {
            get { return _arguments.Length; }
        }

        /// <inheritdoc />
        public object this[int index]
        {
            get { return _arguments[index]; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public IEnumerator<object> GetEnumerator()
        {
            return _arguments.Cast<object>().GetEnumerator();
        }

        #endregion

        #region [====== Factory Methods ======]

        /// <summary>
        /// Represents an empty set of arguments.
        /// </summary>
        public static readonly TestArguments None = new TestArguments(new object[0]);           

        /// <summary>
        /// Creates and returns a set containing several arguments.
        /// </summary>
        /// <param name="arguments">The arguments used for the test.</param>
        /// <returns>A new instance of the <see cref="TestArguments" /> class.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="arguments"/> is <c>null</c>.
        /// </exception>
        public static TestArguments Define(params object[] arguments)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }
            if (arguments.Length == 0)
            {
                return None;
            }
            return new TestArguments(arguments);
        }

        #endregion
    }
}
