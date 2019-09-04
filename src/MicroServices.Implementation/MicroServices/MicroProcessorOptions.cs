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
            Endpoints = new MicroProcessorEndpointOptions();
        }

        public MicroProcessorOptions Copy() =>
            new MicroProcessorOptions(this);

        public UnitOfWorkMode UnitOfWorkMode
        {
            get;
            set;
        }

        public MicroProcessorEndpointOptions Endpoints
        {
            get;
        }

        public override string ToString() =>
            $"{nameof(UnitOfWorkMode)} = {UnitOfWorkMode}";
    }
}
