namespace System.ComponentModel.Server
{
    internal interface IMessageProcessorPipeline : IDisposable
    {
        void Start();
    }
}
