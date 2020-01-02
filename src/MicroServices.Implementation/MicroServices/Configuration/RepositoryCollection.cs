using Kingo.Reflection;

namespace Kingo.MicroServices.Configuration
{
    /// <summary>
    /// Represents a collection of repositories.
    /// </summary>
    public sealed class RepositoryCollection : MicroProcessorComponentCollection
    {
        /// <inheritdoc />
        protected override bool Add(MicroProcessorComponent component)
        {
            if (component.Type.FriendlyName(false, false).EndsWith("Repository"))
            {
                return base.Add(component);
            }
            return false;
        }
    }
}
