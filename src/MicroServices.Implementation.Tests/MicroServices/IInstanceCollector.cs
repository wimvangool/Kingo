namespace Kingo.MicroServices
{
    public interface IInstanceCollector
    {
        void Add(object instance);

        void AssertInstanceCountIs(int count);
    }
}
