using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a component that can be executed.
    /// </summary>
    public interface IExecutable
    {
        /// <summary>
        /// Executes this component.
        /// </summary>
        void Execute();

        /// <summary>
        /// Executes this component asynchronously.
        /// </summary>
        /// <returns>The task carrying out the operation.</returns>
        Task ExecuteAsync();
    }
}
