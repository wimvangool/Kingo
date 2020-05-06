namespace Kingo.MicroServices
{
    internal sealed class MicroProcessorOptions : IMicroProcessorOptions
    {
        private MicroProcessorOptions(MicroProcessorOptions options)
        {
            UnitOfWorkMode = options.UnitOfWorkMode;
            Endpoints = options.Endpoints.Copy();
        }

        public MicroProcessorOptions()
        {
            UnitOfWorkMode = UnitOfWorkMode.MultiThreaded;
            Messages = new MessageFactoryBuilder();
            Endpoints = new MicroProcessorEndpointOptions();
        }

        public MicroProcessorOptions Copy() =>
            new MicroProcessorOptions(this);

        public override string ToString() =>
            $"{nameof(UnitOfWorkMode)} = {UnitOfWorkMode}";

        public UnitOfWorkMode UnitOfWorkMode
        {
            get;
            set;
        }

        public MessageFactoryBuilder Messages
        {
            get;
        }

        public MicroProcessorEndpointOptions Endpoints
        {
            get;
        }
    }
}
