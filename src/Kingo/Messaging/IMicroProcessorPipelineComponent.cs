namespace Kingo.Messaging
{    
    internal interface IMicroProcessorPipelineComponent
    {        
        void Accept(IMicroProcessorFilterVisitor visitor);
    }
}
