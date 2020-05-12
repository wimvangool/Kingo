using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Kingo.Collections.Generic;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Serves as a base-class for collections of specific <see cref="MicroProcessorComponent" /> types
    /// that are to be added to a <see cref="IServiceCollection"/>.
    /// </summary>
    public abstract class MicroProcessorComponentCollection : IReadOnlyCollection<MicroProcessorComponent>
    {
        private readonly Dictionary<Type, MicroProcessorComponent> _components;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorComponentCollection" /> class.
        /// </summary>
        protected MicroProcessorComponentCollection()
        {
            _components = new Dictionary<Type, MicroProcessorComponent>();
        }

        #region [====== IReadOnlyCollection ======]

        /// <inheritdoc />
        public int Count =>
            _components.Count;                

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<MicroProcessorComponent> GetEnumerator() =>
            _components.Values.GetEnumerator();

        #endregion

        #region [====== Add ======]

        /// <summary>
        /// Adds all types defined in the assemblies that match the specified search criteria to this collection.
        /// </summary>
        /// <param name="searchPattern">The pattern that is used to match specified files/assemblies.</param>
        /// <param name="path">A path pointing to a specific directory. If <c>null</c>, the <see cref="TypeSet.CurrentDirectory"/> is used.</param>
        /// <param name="searchOption">
        /// Indicates whether or not only the top-level directory is to be searched.        
        /// </param>
        /// <returns>A new set containing all types from the specified assemblies.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="searchPattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission to access the specified path or its files.
        /// </exception>
        public void Add(string searchPattern, string path = null, SearchOption searchOption = SearchOption.TopDirectoryOnly) =>
            Add(TypeSet.Empty.Add(searchPattern, path, searchOption));

        /// <summary>
        /// Adds all types of the specified <paramref name="types"/> that satisfy the constraints of this
        /// collection's component to this collection.
        /// </summary>
        /// <param name="types">The types to add.</param>
        /// <returns>The number of components that were added to this collection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> is <c>null</c>.
        /// </exception>
        public int Add(params Type[] types) =>
            Add(types.AsEnumerable());

        /// <summary>
        /// Adds all types of the specified <paramref name="types"/> that satisfy the constraints of this
        /// collection's component to this collection.
        /// </summary>
        /// <param name="types">The types to add.</param>
        /// <returns>The number of components that were added to this collection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> is <c>null</c>.
        /// </exception>
        public int Add(IEnumerable<Type> types) =>
            types.WhereNotNull().Sum(type => Add(type) ? 1 : 0);

        /// <summary>
        /// Adds the specified <typeparamref name="TComponent "/> as a component if and only if this type
        /// satisfies the constraints of this collection's component type.
        /// </summary>
        /// <returns><c>true</c> if the type was added; otherwise <c>false</c>.</returns>
        public bool Add<TComponent>() where TComponent : class =>
            Add(typeof(TComponent));

        /// <summary>
        /// Adds the specified <paramref name="type"/> as a component if and only if this type satisfies
        /// the constraints of this collection's component type.
        /// </summary>
        /// <param name="type">The type to add.</param>
        /// <returns><c>true</c> if the type was added; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public bool Add(Type type)
        {
            if (MicroProcessorComponent.IsMicroProcessorComponent(type, out var component))
            {
                return Add(component);
            }
            return false;
        }

        /// <summary>
        /// Adds the specified <paramref name="component"/> to this collection if its type
        /// matches the criteria of this collection.
        /// </summary>
        /// <param name="component">The component to add.</param>
        /// <returns><c>true</c> if the type was added; otherwise <c>false</c>.</returns>
        protected virtual bool Add(MicroProcessorComponent component)
        {
            _components[component.Type] = component;
            return true;
        }

        #endregion

        #region [====== AddSpecificComponentsTo ======]

        /// <summary>
        /// Adds types and mappings to the specified <paramref name="services"/> that are specific to this collection.
        /// </summary>
        /// <param name="services">A service collection.</param>
        /// <returns>The resulting collection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="services"/> is <c>null</c>.
        /// </exception>
        protected internal virtual IServiceCollection AddSpecificComponentsTo(IServiceCollection services) =>
            services ?? throw new ArgumentNullException(nameof(services));

        #endregion
    }
}
