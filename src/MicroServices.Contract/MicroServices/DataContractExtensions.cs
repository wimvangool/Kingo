using System;
using System.Collections.Generic;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Contains extension methods for objects that implement the <see cref="IDataContract" /> interface.
    /// </summary>
    public static class DataContractExtensions
    {
        /// <summary>
        /// Updates the specified <paramref name="dataContract"/> to the latest available version and casts
        /// the result to an instance of <typeparamref name="TDataContract"/>.
        /// </summary>
        /// <typeparam name="TDataContract">Expected type of the latest version.</typeparam>
        /// <param name="dataContract">The contract to update.</param>
        /// <returns>The latest available version.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dataContract"/> is <c>null</c>.
        /// </exception>
        public static TDataContract UpdateToLatestVersion<TDataContract>(this IDataContract dataContract)
        {
            var latestVersion = dataContract.UpdateToLatestVersion();

            try
            {
                return (TDataContract) latestVersion;
            }
            catch (InvalidCastException exception)
            {
                throw NewInvalidCastException(dataContract.GetType(), latestVersion.GetType(), typeof(TDataContract), exception);
            }
        }

        private static Exception NewInvalidCastException(Type dataContractType, Type latestVersionType, Type expectedType, Exception innerException)
        {
            var messageFormat = ExceptionMessages.DataContractExtensions_InvalidCast;
            var message = string.Format(messageFormat, dataContractType.FriendlyName(), latestVersionType.FriendlyName(), expectedType.FriendlyName());
            return new DataContractUpdateFailedException(message, innerException);
        }

        /// <summary>
        /// Updates the specified <paramref name="dataContract"/> to the latest available version.
        /// </summary>
        /// <param name="dataContract">The contract to update.</param>
        /// <returns>The latest available version.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dataContract"/> is <c>null</c>.
        /// </exception>
        public static IDataContract UpdateToLatestVersion(this IDataContract dataContract)
        {
            if (dataContract == null)
            {
                throw new ArgumentNullException(nameof(dataContract));
            }
            var dataContractTypes = new HashSet<Type>() { dataContract.GetType() };
            var latestVersion = dataContract;

            while (latestVersion.TryUpdateToNextVersion(out var nextVersion))
            {
                if (dataContractTypes.Add(nextVersion.GetType()))
                {
                    latestVersion = nextVersion;
                    continue;
                }
                throw NewCircularReferenceDetectedException(dataContract.GetType(), latestVersion.GetType(), nextVersion.GetType());
            }
            return latestVersion;
        }

        private static Exception NewCircularReferenceDetectedException(Type dataContractType, Type latestVersionType, Type nextVersionType)
        {
            var messageFormat = ExceptionMessages.DataContractExtensions_CircularReference;
            var message = string.Format(messageFormat, dataContractType.FriendlyName(), latestVersionType.FriendlyName(), nextVersionType.FriendlyName());
            return new DataContractUpdateFailedException(message);
        }
    }
}
