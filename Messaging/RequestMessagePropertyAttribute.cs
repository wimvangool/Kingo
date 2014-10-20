namespace System.ComponentModel.Messaging
{
    /// <summary>
    /// This attribute can be applied to properties of the <see cref="IRequestMessage"/>-implementor to
    /// indicate the property carries business data.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class RequestMessagePropertyAttribute : Attribute
    {
        private readonly PropertyChangedOption _option;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessagePropertyAttribute" /> class.
        /// </summary>
        /// <param name="option">
        /// The option that will determine the <see cref="IRequestMessage" />-behavior when the property changes.
        /// </param>
        public RequestMessagePropertyAttribute(PropertyChangedOption option)
        {
            _option = option;
        }

        /// <summary>
        /// The option that will determine the <see cref="IRequestMessage" />-behavior when the property changes.
        /// </summary>
        public PropertyChangedOption Option
        {
            get { return _option; }
        }
    }
}
