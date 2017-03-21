namespace Kingo.Messaging
{    
    internal interface IMicroProcessorPipelineComponent
    {        
        void Accept(IMicroProcessorPipelineVisitor visitor);
    }
}
