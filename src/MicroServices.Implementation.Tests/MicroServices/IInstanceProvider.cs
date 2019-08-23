namespace Kingo.MicroServices
{
    internal interface IInstanceCollector
    {
        void Add(object instance);

        void AssertInstanceCountIs(int count);
    }
}
