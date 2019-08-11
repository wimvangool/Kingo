namespace Kingo.MicroServices.Contracts
{
    /// <summary>
    /// When implemented by a class, represents a data transfer object.
    /// </summary>    
    public interface IDataContract
    {
        /// <summary>
        /// Attempts to update this data contract to the next version.
        /// </summary>
        /// <param name="nextVersion">
        /// If this method returns <c>true</c>, this parameter will refer to the next version;
        /// otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if a next version exists and was assigned to <paramref name="nextVersion"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        bool TryUpdateToNextVersion(out IDataContract nextVersion);
    }
}
