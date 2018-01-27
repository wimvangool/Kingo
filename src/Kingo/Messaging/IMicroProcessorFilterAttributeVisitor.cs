namespace Kingo.Messaging
{    
    internal interface IMicroProcessorFilterAttributeVisitor
    {                
        void Visit(ExceptionHandlingFilterAttribute filter);
        
        void Visit(AuthorizationFilterAttribute filter);
       
        void Visit(ValidationFilterAttribute filter);
       
        void Visit(ProcessingFilterAttribute filter);
    }
}
