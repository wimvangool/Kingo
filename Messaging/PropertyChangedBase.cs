using System.Linq.Expressions;

namespace System.ComponentModel.Messaging
{
    /// <summary>
    /// Provides a basic implementation of the <see cref="INotifyPropertyChanged" /> interface.
    /// </summary>
    public abstract class PropertyChangedBase : INotifyPropertyChanged
    {
        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;                

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event, signalling all properties have changed.
        /// </summary>
        protected virtual void OnAllPropertiesChanged()
        {
            OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event, signaling the specified <paramref name="property"/> has changed.
        /// </summary>
        /// <typeparam name="TProperty">Type of the property value.</typeparam>
        /// <param name="property">The property that changed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="property"/> is <c>null</c>.
        /// </exception>
        protected void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(GetPropertyNameOf(property)));
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event, signaling the specified property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <remarks>
        /// A <c>null </c> or <c>string.Empty</c> value for <paramref name="propertyName"/> indicates that
        /// all properties have changed.
        /// </remarks>
        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event.
        /// </summary>
        /// <param name="e">Arguments that contain the name of the property that has changed.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged.Raise(this, e);
        }

        /// <summary>
        /// Returns the name of the property that was specified in the form of an <see cref="Expression" />.
        /// </summary>
        /// <param name="property">The property to examine.</param>
        /// <returns>The name of the specified property.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="property"/> is <c>null</c>.
        /// </exception>
        protected static string GetPropertyNameOf(Expression property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            var lambdaExpression = (LambdaExpression) property;
            var unaryExpression = lambdaExpression.Body as UnaryExpression;
            var memberExpression = unaryExpression == null
                ? (MemberExpression) lambdaExpression.Body
                : (MemberExpression) unaryExpression.Operand;

            return memberExpression.Member.Name;
        }
    }
}
