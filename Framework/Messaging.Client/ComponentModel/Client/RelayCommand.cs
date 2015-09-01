using System;

namespace ServiceComponents.ComponentModel.Client
{
    /// <summary>
    /// Represents a command that delegates it's work to some specified functions.
    /// </summary>
    public class RelayCommand : RelayCommand<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand" /> class.
        /// </summary>
        /// <param name="execute">The function to execute when this command is executed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="execute"/> is <c>null</c>.
        /// </exception>
        public RelayCommand(Action execute)
            : base(ToParameterizedAction(execute)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand" /> class.
        /// </summary>
        /// <param name="execute">The function to execute when this command is executed.</param>
        /// <param name="canExecute">
        /// Optional predicate function that is used to check whether or not this command can be executed
        /// based on a certain parameter. If this parameter is <c>null</c>, this command is assumed to be
        /// able to execute with any parameter of the right type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="execute"/> is <c>null</c>.
        /// </exception>
        public RelayCommand(Action execute, Func<bool> canExecute)
            : base(ToParameterizedAction(execute), ToParameterizedFunc(canExecute)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand" /> class.
        /// </summary>
        /// <param name="execute">The function to execute when this command is executed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="execute"/> is <c>null</c>.
        /// </exception>
        public RelayCommand(Action<object> execute)
            : base(execute) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand" /> class.
        /// </summary>
        /// <param name="execute">The function to execute when this command is executed.</param>
        /// <param name="canExecute">
        /// Optional predicate function that is used to check whether or not this command can be executed
        /// based on a certain parameter. If this parameter is <c>null</c>, this command is assumed to be
        /// able to execute with any parameter of the right type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="execute"/> is <c>null</c>.
        /// </exception>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
            : base(execute, canExecute) { }

        /// <summary>
        /// Simply assigns <paramref name="parameterIn"/> to <paramref name="parameterOut"/> and returns <c>true</c>.
        /// </summary>
        /// <param name="parameterIn">The incoming parameter.</param>
        /// <param name="parameterOut">
        /// When this method has executed, refers to the same object as <paramref name="parameterIn"/>.
        /// </param>
        /// <returns><c>true</c>.</returns>
        protected override bool TryConvert(object parameterIn, out object parameterOut)
        {
            parameterOut = parameterIn;
            return true;
        }

        private static Action<object> ToParameterizedAction(Action execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            return parameter => execute.Invoke();
        }

        private static Func<object, bool> ToParameterizedFunc(Func<bool> canExecute)
        {
            if (canExecute == null)
            {
                return null;
            }
            return parameter => canExecute.Invoke();
        }
    }
}
