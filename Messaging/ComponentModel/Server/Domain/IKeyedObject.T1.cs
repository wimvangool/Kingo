namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// When implemented by a class, represents an object that can be identified by a unique key.
    /// </summary>
    /// <typeparam name="TKey">Type of the key or identifier of the object.</typeparam>
    public interface IKeyedObject<out TKey> where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// The key or identifier of the object.
        /// </summary>
        TKey Key
        {
            get;
        }
    }
}
