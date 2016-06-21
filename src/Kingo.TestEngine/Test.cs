namespace Kingo
{
    /// <summary>
    /// Represents a test that was executed with a specified set of arguments.
    /// </summary>
    public sealed class Test
    {        
        private Test(string name, TestArguments arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        /// <summary>
        /// Name of the test.
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Arguments of the test.
        /// </summary>
        public TestArguments Arguments
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Name}({Arguments})";
        }

        /// <summary>
        /// Creates and returns a new <see cref="Test" /> with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of the test.</param>
        /// <param name="arguments">Arguments of the test.</param>
        /// <returns>A new <see cref="Test" />.</returns>        
        public static Test CreateTest(string name = null, TestArguments arguments = null)
        {            
            return new Test(name ?? "<Unknown>", arguments ?? TestArguments.None);
        }
    }
}
