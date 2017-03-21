namespace Kingo.Messaging
{
    internal abstract class MicroProcessorMethod<TContext> where TContext : MicroProcessorContext
    {
        protected abstract MicroProcessor Processor
        {
            get;
        }

        protected abstract TContext Context
        {
            get;
        }
    }
}
