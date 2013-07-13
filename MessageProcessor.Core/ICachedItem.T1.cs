
namespace YellowFlare.MessageProcessing
{
    public interface ICachedItem<T>
    {
        bool TryGetValue(out T value);

        void Invalidate();
    }
}
