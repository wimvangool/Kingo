using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using Kingo.Collections.Generic;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Serves as a base-class for collections of specific <see cref="MicroProcessorComponent" /> types
    /// that are to be added to a <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TComponentType">Type of the components in this collection.</typeparam>
    public abstract class MicroProcessorComponentCollection<TComponentType> : IMicroProcessorComponentCollection
        where TComponentType : MicroProcessorComponent
    {
        private readonly Dictionary<Type, TComponentType> _components;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorComponentCollection{TComponentType}" /> class.
        /// </summary>
        protected MicroProcessorComponentCollection()
        {
            _components = new Dictionary<Type, TComponentType>();
        }

        /// <summary>
        /// Returns all components that have been added to this collection.
        /// </summary>
        protected IReadOnlyCollection<TComponentType> Components =>
            _components.Values;

        #region [====== IReadOnlyCollection ======]

        /// <inheritdoc />
        public int Count =>
            _components.Count;                

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<MicroProcessorComponent> GetEnumerator() =>
            Components.GetEnumerator();

        #endregion

        #region [====== Add ======]

        /// <summary>
        /// Adds all types defined in the assemblies that match the specified search criteria to the
        /// searchable type-set.
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
        /// collection's <typeparamref name="TComponentType"/> to this collection.
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
        /// collection's <typeparamref name="TComponentType"/> to this collection.
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
        /// satisfies the constraints of this collection's <typeparamref name="TComponentType"/> type.
        /// </summary>
        /// <returns><c>true</c> if the type was added; otherwise <c>false</c>.</returns>
        public bool Add<TComponent>() where TComponent : class =>
            Add(typeof(TComponent));

        /// <summary>
        /// Adds the specified <paramref name="type"/> as a component if and only if this type satisfies
        /// the constraints of this collection's <typeparamref name="TComponentType"/> type.
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
                if (IsComponentType(component, out var supportedComponent))
                {
                    Add(supportedComponent);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds the specified <paramref name="component"/> to this collection if the same type wasn't added before.
        /// </summary>
        /// <param name="component">The component to add.</param>
        protected void Add(TComponentType component) =>
            _components[component.Type] = component;

        /// <summary>
        /// Determines whether or not the specified <paramref name="component" /> satisfies the constraints
        /// of this collection's <typeparamref name="TComponentType" /> type and converts it to this type if it does. 
        /// </summary>
        /// <param name="component">The component to check.</param>
        /// <param name="componentType">
        /// If <paramref name="component"/> satisfies the constraints of <typeparamref name="TComponentType"/>, this parameter
        /// will be assigned a new instance of type <typeparamref name="TComponentType"/> based on the specified <paramref name="component"/>.
        /// </param>
        /// <returns><c>true</c> if the conversion was made; otherwise <c>false</c>.</returns>
        protected abstract bool IsComponentType(MicroProcessorComponent component, out TComponentType componentType);

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
        public virtual IServiceCollection AddSpecificComponentsTo(IServiceCollection services) =>
            services ?? throw new ArgumentNullException(nameof(services));

        #endregion
    }
}
