using System.Security.Claims;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Represents an operation that is executed by a <see cref="IMicroProcessor" /> as part of a test.
    /// </summary>
    public abstract class MicroProcessorTestOperationInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorTestOperationInfo" /> class.
        /// </summary>
        protected MicroProcessorTestOperationInfo()
        {
            Id = MicroProcessorTestOperationId.NewOperationId();
        }

        /// <summary>
        /// Gets or sets the id of this operation, which is used to store its results in the
        /// <see cref="MicroProcessorTestContext" />.
        /// </summary>
        public MicroProcessorTestOperationId Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user that will be assigned to the operation. If not specified,
        /// the test will use <see cref="ClaimsPrincipal.Current"/>.
        /// </summary>
        public ClaimsPrincipal User
        {
            get;
            set;
        }
    }
}
