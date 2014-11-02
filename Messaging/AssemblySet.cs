using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents a set of <see cref="Assembly">Assemblies</see>.
    /// </summary>
    public sealed class AssemblySet : IEnumerable<Assembly>
    {
        private readonly HashSet<Assembly> _assemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblySet" /> class.
        /// </summary>
        public AssemblySet()
        {
            _assemblies = new HashSet<Assembly>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblySet" /> class.
        /// </summary>
        /// <param name="assemblies">A collection of assemblies.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        public AssemblySet(IEnumerable<Assembly> assemblies)
        {
            _assemblies = new HashSet<Assembly>(assemblies);
        }

        /// <summary>
        /// Adds the specified <paramref name="assemblyFile" /> to this set.
        /// </summary>
        /// <param name="assemblyFile">The filename or path of an assembly.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblyFile"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="assemblyFile"/> is an empty string.
        /// </exception>
        /// <exception cref="SecurityException">
        /// The application has insufficient rights to load the specified <paramref name="assemblyFile"/>.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="assemblyFile"/> does not point to a valid assembly or could otherwise not be loaded.
        /// </exception>
        public void Add(string assemblyFile)
        {
            Add(Assembly.LoadFrom(assemblyFile));
        }

        /// <summary>
        /// Adds the specified <paramref name="assembly"/> to this set.
        /// </summary>
        /// <param name="assembly">The assembly to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assembly"/> is <c>null</c>.
        /// </exception>
        public void Add(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
            _assemblies.Add(assembly);
        }

        /// <summary>
        /// Returns a collection containing of all types that are part of this set of assemblies.
        /// </summary>
        /// <returns>A collection of types.</returns>
        public IEnumerable<Type> GetTypes()
        {
            return _assemblies.SelectMany(assembly => assembly.GetTypes());
        }

        /// <summary>
        /// Determines if the specified <paramref name="type"/> is declared in this set of assemblies.
        /// </summary>
        /// <param name="type">A certain type.</param>
        /// <returns>
        /// <c>true</c> if the specified type is declared in this set of assemblies; otherwise,
        /// also if <paramref name="type"/> is <c>null</c>, returns <c>false</c>.        
        /// </returns>
        public bool Contains(Type type)
        {
            return type != null && _assemblies.Contains(type.Assembly);
        }

        /// <inheritdoc />
        public IEnumerator<Assembly> GetEnumerator()
        {
            return _assemblies.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _assemblies.GetEnumerator();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Join(Environment.NewLine, _assemblies.Select(assembly => assembly.GetName().ToString()).OrderBy(name => name));
        }

        /// <summary>
        /// Returns an <see cref="AssemblySet" /> that contains the current (calling) assembly.
        /// </summary>
        /// <returns>An <see cref="AssemblySet" /> that contains the current (calling) assembly.</returns>
        public static AssemblySet CurrentAssembly()
        {
            return new AssemblySet { Assembly.GetCallingAssembly() };
        }

        /// <summary>
        /// Creates and returns a set of assemblies that are found in the specified directory.
        /// </summary>
        /// <param name="path">A path pointing to a specific directory.</param>        
        /// <returns>A set of assemblies that are found in the specified location(s).</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="path"/> is not a valid path.
        /// </exception>
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public static AssemblySet FromDirectory(string path)
        {
            return FromDirectory(path, "*.dll");
        }

        /// <summary>
        /// Creates and returns a set of assemblies that are found in the specified directory,
        /// using the specified <paramref name="searchPattern"/>.
        /// </summary>
        /// <param name="path">A path pointing to a specific directory.</param>
        /// <param name="searchPattern">The pattern that is used to match specified files/assemblies.</param>        
        /// <returns>A set of assemblies that are found in the specified location(s).</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> or <paramref name="searchPattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="path"/> is not a valid path or <paramref name="searchPattern"/> is not a valid search-pattern.
        /// </exception>
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public static AssemblySet FromDirectory(string path, string searchPattern)
        {
            return FromDirectory(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Creates and returns a set of assemblies that are found in the specified directory or directories,
        /// using the specified <paramref name="searchPattern"/>.
        /// </summary>
        /// <param name="path">A path pointing to a specific directory.</param>
        /// <param name="searchPattern">The pattern that is used to match specified files/assemblies.</param>
        /// <param name="searchOption">
        /// Indicates whether or not only the top-level directory specified by <paramref name="path"/> is to be searched.        
        /// </param>
        /// <returns>A set of assemblies that are found in the specified location(s).</returns>
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
        public static AssemblySet FromDirectory(string path, string searchPattern, SearchOption searchOption)
        {
            return new AssemblySet(FindAssemblies(path, searchPattern, searchOption));
        }

        private static IEnumerable<Assembly> FindAssemblies(string path, string searchPattern, SearchOption searchOption)
        {
            return from file in Directory.GetFiles(path, searchPattern, searchOption)
                   where file.EndsWith(".dll")
                   select Assembly.LoadFrom(file);
        }

        /// <summary>
        /// Creates and returns a union of the specified <paramref name="sets"/>.
        /// </summary>
        /// <param name="sets">The sets to unite into a single set.</param>
        /// <returns>A union of the specified <paramref name="sets"/>.</returns>
        /// <remarks>
        /// This method will ignore <c>null</c>-references in <paramref name="sets"/>.
        /// </remarks>
        public static AssemblySet Join(params AssemblySet[] sets)
        {
            return new AssemblySet(sets.Where(set => set != null).SelectMany(set => set._assemblies));
        }
    }
}
