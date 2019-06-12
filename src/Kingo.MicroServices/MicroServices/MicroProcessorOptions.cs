namespace Kingo.MicroServices
{
    internal sealed class MicroProcessorOptions : IMicroProcessorOptions
    {
        public MicroProcessorOptions()
        {
            UnitOfWorkMode = UnitOfWorkMode.MultiThreaded;
        }

        private MicroProcessorOptions(MicroProcessorOptions options)
        {
            UnitOfWorkMode = options.UnitOfWorkMode;
        }

        public UnitOfWorkMode UnitOfWorkMode
        {
            get;
            set;
        }

        public MicroProcessorOptions Copy() =>
            new MicroProcessorOptions(this);
    }
}
