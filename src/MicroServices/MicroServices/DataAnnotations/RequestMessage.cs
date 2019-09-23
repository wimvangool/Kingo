using System;

namespace Kingo.MicroServices.DataAnnotations
{
    /// <summary>
    /// Serves as a base-class for all request-messages such as commands or query-requests.
    /// </summary>
    [Serializable]
    public abstract class RequestMessage : ValidatableObject, IDataContract
    {
        bool IDataContract.TryUpdateToNextVersion(out IDataContract nextVersion) =>
            TryUpdateToNextVersion(out nextVersion);

        /// <summary>
        /// Attempts to update this message to the next version. By default, this
        /// method returns <c>false</c>.
        /// </summary>
        /// <param name="nextVersion">
        /// If this method returns <c>true</c>, this parameter will refer to the next version;
        /// otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if a next version exists and was assigned to <paramref name="nextVersion"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        protected virtual bool TryUpdateToNextVersion(out IDataContract nextVersion)
        {
            nextVersion = null;
            return false;
        }
    }
}
