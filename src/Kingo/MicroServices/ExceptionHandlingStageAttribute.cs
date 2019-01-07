namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a base-class for all filter-attributes that are executed during the exception handling stage
    /// of the microprocessor pipeline and are designed to catch and handle any exception that are thrown
    /// while invoking a message handler or executing a query.
    /// </summary>
    public abstract class ExceptionHandlingStageAttribute : MicroProcessorFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlingStageAttribute" /> class.
        /// </summary>
        protected ExceptionHandlingStageAttribute() :
            base(MicroProcessorFilterStage.ExceptionHandlingStage)
        {
            OperationTypes = MicroProcessorOperationTypes.Input;
        }
    }
}
