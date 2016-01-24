using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a set of <see cref="Assembly">Assemblies</see> that make up a conceptual layer of an application.
    /// </summary>
    public sealed class Layer : IEnumerable<Type>
    {
        private readonly HashSet<Assembly> _assemblies;

        private Layer(IEnumerable<Assembly> assemblies)
        {
            _assemblies = new HashSet<Assembly>(assemblies);
        }

        /// <inheritdoc />
        public IEnumerator<Type> GetEnumerator()
        {
            return _assemblies.SelectMany(assembly => assembly.GetTypes()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Join(", ", _assemblies.Select(assembly => assembly.GetName().Name).OrderBy(name => name));            
        }

        #region [====== Factory Methods ======]

        /// <summary>
        /// Represents an empty layer.
        /// </summary>
        public static Layer Empty = new Layer(Enumerable.Empty<Assembly>());

        /// <summary>
        /// Creates and returns a layer made up of all assemblies that are found in the directory in which the calling assembly
        /// has been deployed.
        /// </summary>
        /// <param name="searchPattern">The pattern that is used to match specified files/assemblies.</param>
        /// <param name="searchOption">
        /// Indicates whether or not only the top-level directory is to be searched.        
        /// </param>
        /// <returns>A new layer.</returns>
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public static Layer FromCurrentDirectory(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return FromDirectory(CurrentDirectory(), searchPattern, searchOption);
        }

        private static string CurrentDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
        }             

        /// <summary>
        /// Creates and returns a layer made up of all assemblies that are found in the specified directory or directories,
        /// using the specified <paramref name="searchPattern"/>.
        /// </summary>
        /// <param name="path">A path pointing to a specific directory.</param>
        /// <param name="searchPattern">The pattern that is used to match specified files/assemblies.</param>
        /// <param name="searchOption">
        /// Indicates whether or not only the top-level directory specified by <paramref name="path"/> is to be searched.        
        /// </param>
        /// <returns>A new layer.</returns>
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
        public static Layer FromDirectory(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {            
            return new Layer(FindAssemblies(path, searchPattern, searchOption));
        }        

        private static IEnumerable<Assembly> FindAssemblies(string path, string searchPattern, SearchOption searchOption)
        {
            return from file in Directory.GetFiles(path, searchPattern, searchOption)
                   where file.EndsWith(".dll")
                   select Assembly.LoadFrom(file);
        }

        /// <summary>
        /// Creates and returns a layer made up of all specified assemblies.
        /// </summary>
        /// <param name="assemblies">A collection of assemblies</param>
        /// <returns>A new layer.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        public static Layer FromAssemblies(params Assembly[] assemblies)
        {
            return FromAssemblies(assemblies as IEnumerable<Assembly>);
        }

        /// <summary>
        /// Creates and returns a layer made up of all specified assemblies.
        /// </summary>
        /// <param name="assemblies">A collection of assemblies</param>
        /// <returns>A new layer.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        public static Layer FromAssemblies(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException("assemblies");
            }
            return new Layer(assemblies.WhereNotNull());
        }

        /// <summary>
        /// Creates and returns a union of the specified <paramref name="sets"/>.
        /// </summary>
        /// <param name="sets">The sets to unite into a single set.</param>
        /// <returns>A union of the specified <paramref name="sets"/>.</returns>
        /// <remarks>
        /// This method will ignore <c>null</c>-references in <paramref name="sets"/>.
        /// </remarks>
        public static Layer Join(params Layer[] sets)
        {
            return Join(sets as IEnumerable<Layer>);
        }

        /// <summary>
        /// Creates and returns a union of the specified <paramref name="sets"/>.
        /// </summary>
        /// <param name="sets">The sets to unite into a single set.</param>
        /// <returns>A union of the specified <paramref name="sets"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sets"/> is <c>null</c>.
        /// </exception>
        public static Layer Join(IEnumerable<Layer> sets)
        {
            return new Layer(sets.WhereNotNull().SelectMany(set => set._assemblies));
        }

        #endregion

        #region [====== Operator Overloads ======]       

        /// <summary>
        /// Joins to sets of assemblies into one.
        /// </summary>
        /// <param name="left">Left operator.</param>
        /// <param name="right">Right operator.</param>
        /// <returns>A joined assembly.</returns>
        public static Layer operator +(Layer left, Layer right)
        {
            if (ReferenceEquals(left, null))
            {
                return right;
            }
            if (ReferenceEquals(right, null))
            {
                return left;
            }
            return Join(left, right);
        }

        #endregion
    }
}
