
namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents a factory class that can be used to dynamically create a pipeline of
    /// <see cref="IMessageHandlerBehavior">behaviors</see> on top of a handler.
    /// </summary>        
    public class MessageHandlerPipelineFactory
    {        
        internal IMessageCommand CreatePipeline<TMessage>(IMessageHandler<TMessage> handler, TMessage message, MessageProcessorContext context)
            where TMessage : class
        {
            // 1) First, the core-command is created which contains all business logic.
            // 2) On top op that command, the custom behaviors are applied, if specified.
            // 3) As a final step, the StackManager-command is put at the front of the pipeline,
            //    which pushes and pops the command on and off the stack.            
            IMessageCommand command = new MessageCommand<TMessage>(handler, message);            
            command = ApplyCustomBehaviorsTo(command);            
            command = new MessageProcessorStackManagerCommand(command, context);            
            return command;
        }
        
        protected virtual IMessageCommand ApplyCustomBehaviorsTo(IMessageCommand command)
        {
            return command;
        }     
    }
}
