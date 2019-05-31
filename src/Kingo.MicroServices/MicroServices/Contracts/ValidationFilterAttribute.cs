namespace Kingo.MicroServices.Contracts
{
    /// <summary>
    /// Serves as a base class for all filters that are designed to validate a message before it is being processed.
    /// </summary>
    public abstract class ValidationFilterAttribute : MicroProcessorFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationFilterAttribute" /> class.
        /// </summary>
        protected ValidationFilterAttribute() :
            base(MicroProcessorFilterStage.ValidationStage)
        {
            OperationTypes = MicroProcessorOperationTypes.AnyInput;
        }             
    }
}
