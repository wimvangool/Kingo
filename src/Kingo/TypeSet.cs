using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;

namespace Kingo
{
    /// <summary>
    /// Represents an immutable set of types.
    /// </summary>
    public sealed class TypeSet : IEnumerable<Type>
    {
        #region [====== TypeCollections ======]

        private abstract class TypeCollection : IEnumerable<Type>
        {
            public abstract IEnumerator<Type> GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() =>
                GetEnumerator();

            public virtual bool Contains(Type type) =>
                this.Any(t => t == type);

            public virtual TypeCollection Add(IEnumerable<Type> types) =>
                new IncludedTypeCollection(this, types);

            public virtual TypeCollection Add(Type type) =>
                new IncludedTypeCollection(this, type);

            public virtual TypeCollection Remove(IEnumerable<Type> types) =>
                new ExcludedTypeCollection(this, types);

            public virtual TypeCollection Remove(Type type) =>
                new ExcludedTypeCollection(this, type);
        }

        private sealed class EmptyCollection : TypeCollection
        {
            public override IEnumerator<Type> GetEnumerator() =>
                Enumerable.Empty<Type>().GetEnumerator();

            public override bool Contains(Type type) =>
                false;

            public override TypeCollection Add(IEnumerable<Type> types) =>
                new IncludedTypeCollection(Enumerable.Empty<Type>(), types);

            public override TypeCollection Add(Type type) =>
                new IncludedTypeCollection(Enumerable.Empty<Type>(), type);

            public override TypeCollection Remove(IEnumerable<Type> types) =>
                this;

            public override TypeCollection Remove(Type type) =>
                this;
        }

        private sealed class IncludedTypeCollection : TypeCollection
        {
            private readonly IEnumerable<Type> _typeCollection;
            private readonly Type[] _types;

            public IncludedTypeCollection(IEnumerable<Type> typeCollection, IEnumerable<Type> types)
            {
                _typeCollection = typeCollection;
                _types = types.ToArray();
            }

            public IncludedTypeCollection(IEnumerable<Type> typeCollection, Type type)
            {
                _typeCollection = typeCollection;
                _types = new [] { type };
            }

            public override IEnumerator<Type> GetEnumerator() =>
                _types.Concat(_typeCollection).GetEnumerator();            
        } 
        
        private sealed class ExcludedTypeCollection : TypeCollection
        {
            private readonly IEnumerable<Type> _typeCollection;
            private readonly Type[] _types;

            public ExcludedTypeCollection(IEnumerable<Type> typeCollection, IEnumerable<Type> types) :
                this(typeCollection, types.ToArray()) { }

            public ExcludedTypeCollection(IEnumerable<Type> typeCollection, Type type) :
                this(typeCollection, new [] { type }) { }

            private ExcludedTypeCollection(IEnumerable<Type> typeCollection, Type[] types)
            {
                _typeCollection = typeCollection;
                _types = types;
            }

            public override IEnumerator<Type> GetEnumerator() =>
                _typeCollection.Where(type => !_types.Contains(type)).GetEnumerator();           
        }

        #endregion

        /// <summary>
        /// Represents the empty set.
        /// </summary>
        public static readonly TypeSet Empty = new TypeSet(new EmptyCollection());

        private readonly TypeCollection _types;

        private TypeSet(TypeCollection types)
        {
            _types = types;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{this.Count()} items";

        #region [====== IEnumerable<Type> ======]        

        /// <inheritdoc />
        public IEnumerator<Type> GetEnumerator() =>
            _types.Distinct().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        #endregion

        #region [====== Contains ======]

        /// <summary>
        /// Determines whether or not this set contains the specified <typeparamref name="TItem"/>.
        /// </summary>
        /// <typeparam name="TItem">The type to check.</typeparam>
        /// <returns><c>true</c> if this set contains the specified <typeparamref name="TItem"/>; otherwise <c>false</c>.</returns>        
        public bool Contains<TItem>() =>
            Contains(typeof(TItem));

        /// <summary>
        /// Determines whether or not this set contains the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if this set contains the specified <paramref name="type"/>; otherwise <c>false</c>.</returns>
        public bool Contains(Type type) =>
            _types.Contains(type);

        #endregion

        #region [====== AddAssemblies ======]

        /// <summary>
        /// Adds all types defined in the assemblies that are located in the current directory and match the specified
        /// <paramref name="searchPattern"/> to this set.
        /// </summary>
        /// <param name="searchPattern">The pattern that is used to match specified files/assemblies.</param>
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
        /// The caller does not have the required permission
        /// </exception>
        public TypeSet AddAssembliesFromCurrentDirectory(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly) =>
            AddAssembliesFromDirectory(CurrentDirectory(), searchPattern, searchOption);

        /// <summary>
        /// Adds all types defined in the assemblies that are located in the specified <paramref name="path"/> and match the specified
        /// <paramref name="searchPattern"/> to this set.
        /// </summary>
        /// <param name="path">A path pointing to a specific directory.</param>
        /// <param name="searchPattern">The pattern that is used to match specified files/assemblies.</param>
        /// <param name="searchOption">
        /// Indicates whether or not only the top-level directory specified by <paramref name="path"/> is to be searched.        
        /// </param>
        /// <returns>A new set containing all types from the specified assemblies.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> or <paramref name="searchPattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="path"/> is not a valid path, <paramref name="searchPattern"/> is not a valid search-pattern,
        /// or <paramref name="searchOption"/> is not a valid <see cref="SearchOption" />.
        /// </exception>
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public TypeSet AddAssembliesFromDirectory(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly) =>
            AddAssemblies(FindAssemblies(path, searchPattern, searchOption));

        /// <summary>
        /// Adds all types from the specified <paramref name="assemblies"/> to this set.
        /// </summary>
        /// <param name="assemblies">A collection of assemblies.</param>
        /// <returns>A new set where all types from the specified <paramref name="assemblies"/> were added.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        public TypeSet AddAssemblies(params Assembly[] assemblies) =>
            AddAssemblies(assemblies as IEnumerable<Assembly>);

        /// <summary>
        /// Adds all types from the specified <paramref name="assemblies"/> to this set.
        /// </summary>
        /// <param name="assemblies">A collection of assemblies.</param>
        /// <returns>A new set where all types from the specified <paramref name="assemblies"/> were added.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        public TypeSet AddAssemblies(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }
            return Add(assemblies.WhereNotNull().SelectMany(assembly => assembly.GetTypes()));
        }

        #endregion

        #region [====== Add ======]

        /// <summary>
        /// Adds all the specified <paramref name="types"/> to this set.
        /// </summary>
        /// <param name="types">A collection of types.</param>
        /// <returns>A new set containing all the specified <paramref name="types"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> is <c>null</c>.
        /// </exception>
        public TypeSet Add(IEnumerable<Type> types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }
            return new TypeSet(_types.Add(types.WhereNotNull()));
        }

        /// <summary>
        /// Adds the specified <typeparamref name="TItem" /> to the set.
        /// </summary>
        /// <typeparam name="TItem">The type to add to this set.</typeparam>
        /// <returns>A new set containing the specified <typeparamref name="TItem"/>.</returns>
        public TypeSet Add<TItem>() =>
            Add(typeof(TItem));

        /// <summary>
        /// Adds the specified <paramref name="type" /> to the set.
        /// </summary>
        /// <param name="type">The type to add to this set.</param>
        /// <returns>A new set containing the specified <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public TypeSet Add(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return new TypeSet(_types.Add(type));
        }

        #endregion

        #region [====== RemoveAssemblies ======]

        /// <summary>
        /// Removes all types defined in the assemblies that are located in the current directory and match the specified
        /// <paramref name="searchPattern"/> from this set.
        /// </summary>
        /// <param name="searchPattern">The pattern that is used to match specified files/assemblies.</param>
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
        /// The caller does not have the required permission
        /// </exception>
        public TypeSet RemoveAssembliesFromCurrentDirectory(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly) =>
            RemoveAssembliesFromDirectory(CurrentDirectory(), searchPattern, searchOption);

        /// <summary>
        /// Adds all types defined in the assemblies that are located in the specified <paramref name="path"/> and match the specified
        /// <paramref name="searchPattern"/> from this set.
        /// </summary>
        /// <param name="path">A path pointing to a specific directory.</param>
        /// <param name="searchPattern">The pattern that is used to match specified files/assemblies.</param>
        /// <param name="searchOption">
        /// Indicates whether or not only the top-level directory specified by <paramref name="path"/> is to be searched.        
        /// </param>
        /// <returns>A new set containing all types from the specified assemblies.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> or <paramref name="searchPattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="path"/> is not a valid path, <paramref name="searchPattern"/> is not a valid search-pattern,
        /// or <paramref name="searchOption"/> is not a valid <see cref="SearchOption" />.
        /// </exception>
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public TypeSet RemoveAssembliesFromDirectory(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly) =>
            RemoveAssemblies(FindAssemblies(path, searchPattern, searchOption));

        /// <summary>
        /// Removes all types from the specified <paramref name="assemblies"/> from this set.
        /// </summary>
        /// <param name="assemblies">A collection of assemblies.</param>
        /// <returns>A new set where all types from the specified <paramref name="assemblies"/> have been removed.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        public TypeSet RemoveAssemblies(params Assembly[] assemblies) =>
            RemoveAssemblies(assemblies as IEnumerable<Assembly>);

        /// <summary>
        /// Removes all types from the specified <paramref name="assemblies"/> from this set.
        /// </summary>
        /// <param name="assemblies">A collection of assemblies.</param>
        /// <returns>A new set where all types from the specified <paramref name="assemblies"/> have been removed.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        public TypeSet RemoveAssemblies(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }
            return Remove(assemblies.WhereNotNull().SelectMany(assembly => assembly.GetTypes()));
        }

        #endregion

        #region [====== Remove ======]

        /// <summary>
        /// Removes all specified <paramref name="types"/> from this set.
        /// </summary>
        /// <param name="types">A collection of types to remove.</param>
        /// <returns>A new set where all the specified <paramref name="types"/> have been removed.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> is <c>null</c>.
        /// </exception>
        public TypeSet Remove(IEnumerable<Type> types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }
            return new TypeSet(_types.Remove(types));
        }

        /// <summary>
        /// Removes the specified <typeparamref name="TItem"/> from this set.
        /// </summary>
        /// <typeparam name="TItem">The type to remove.</typeparam>
        /// <returns>A new set where the specified <typeparamref name="TItem"/> has been removed.</returns>
        public TypeSet Remove<TItem>() =>
            Remove(typeof(TItem));

        /// <summary>
        /// Removes the specified <paramref name="type"/> from this set.
        /// </summary>
        /// <param name="type">The type to remove.</param>
        /// <returns>A new set where the specified <paramref name="type"/> has been removed.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public TypeSet Remove(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return new TypeSet(_types.Remove(type));
        }

        #endregion

        private static string CurrentDirectory() =>
            Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);

        private static IEnumerable<Assembly> FindAssemblies(string path, string searchPattern, SearchOption searchOption) => from file in Directory.GetFiles(path, searchPattern, searchOption)
                                                                                                                             where file.EndsWith(".dll")
                                                                                                                             select Assembly.LoadFrom(file);
    }
}
