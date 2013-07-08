using System;

namespace YellowFlare.MessageProcessing
{
    public interface ICache
    {
        ICachedItem<T> Add<T>(T value);

        ICachedItem<T> Add<T>(T value, Action<T> valueRemovedCallback);
    }
}
