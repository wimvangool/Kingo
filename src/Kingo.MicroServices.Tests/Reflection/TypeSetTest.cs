using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Reflection
{
    [TestClass]
    public sealed class TypeSetTest
    {        
        #region [====== Empty ======]

        [TestMethod]
        public void Empty_ReturnsTheEmptySet()
        {
            AssertIsEmpty(TypeSet.Empty);            
        }

        #endregion

        #region [====== Add(Type) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddType_Throws_IfTypeIsNull()
        {
            TypeSet.Empty.Add(null as Type);
        }

        [TestMethod]        
        public void AddType_AddsTheSpecifiedType_IfTypeIsNotNull_And_SetDoesNotContainTypeYet()
        {
            var set = TypeSet.Empty.Add(typeof(object));

            AssertContainsExactly(set, typeof(object));            
        }

        [TestMethod]
        public void AddType_DoesNotAddTheSpecifiedType_IfTypeIsNotNull_And_SetAlreadyContainsType()
        {
            var set = TypeSet.Empty.Add(typeof(object)).Add(typeof(object));

            AssertContainsExactly(set, typeof(object));            
        }

        [TestMethod]
        public void AddTypeOfT_AddsTheSpecifiedType_IfTypeIsNotNull_And_SetDoesNotContainTypeYet()
        {
            var set = TypeSet.Empty.Add<object>();

            AssertContainsExactly(set, typeof(object));            
        }

        [TestMethod]
        public void AddTypeOfT_AddsTheSpecifiedType_IfTypeIsNotNull_And_TypeWasRemovedBefore()
        {
            var set = TypeSet.Empty
                .Add<object>()
                .Remove<object>()
                .Add<object>();

            AssertContainsExactly(set, typeof(object));
        }

        [TestMethod]
        public void AddTypeOfT_DoesNotAddTheSpecifiedType_IfTypeIsNotNull_And_SetAlreadyContainsType()
        {
            var set = TypeSet.Empty.Add<object>().Add<object>();

            AssertContainsExactly(set, typeof(object));            
        }

        #endregion

        #region [====== Add(IEnumerable<Type>) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddEnumerable_Throws_IfCollectionIsNull()
        {
            TypeSet.Empty.Add(null as IEnumerable<Type>);
        }

        [TestMethod]
        public void AddEnumerable_AddsAllSpecifiedTypes_IfCollectionIsNotNull()
        {
            var set = TypeSet.Empty.Add(new[] { typeof(object), null, typeof(object), typeof(string) });

            AssertContainsExactly(set, typeof(object), typeof(string));           
        }

        #endregion

        #region [====== AddAssemblies(Assemly[]) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddAssemblyArray_Throws_IfCollectionIsNull()
        {
            TypeSet.Empty.Add(null as Assembly[]);
        }

        [TestMethod]        
        public void AddAssemblyArray_AddsAllTypesOfSpecifiedAssemblies_IfCollectionIsNotNull()
        {
            var set = TypeSet.Empty.Add(
                typeof(object).Assembly,
                typeof(TypeSetTest).Assembly,
                typeof(TypeSet).Assembly);

            AssertContainsAtLeast(set, typeof(object), typeof(int), typeof(TypeSetTest), typeof(TypeSet));
        }

        #endregion

        #region [====== Remove(Type) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveType_Throws_IfTypeIsNull()
        {
            TypeSet.Empty.Remove(null as Type);
        }

        [TestMethod]
        public void RemoveType_DoesNothing_IfSetIsEmpty()
        {
            AssertIsEmpty(TypeSet.Empty.Remove(typeof(object)));
        }

        [TestMethod]
        public void RemoveType_RemovesSpecifiedType_IfSetIsNotEmpty_And_SetContainsSpecifiedType()
        {
            AssertIsEmpty(TypeSet.Empty.Add(typeof(object)).Remove(typeof(object)));
        }

        [TestMethod]
        public void RemoveTypeOfT_DoesNothing_IfSetIsEmpty()
        {
            AssertIsEmpty(TypeSet.Empty.Remove(typeof(object)));
        }

        [TestMethod]
        public void RemoveTypeOfT_RemovesSpecifiedType_IfSetIsNotEmpty_And_SetContainsSpecifiedType()
        {
            AssertIsEmpty(TypeSet.Empty.Add(typeof(object)).Remove(typeof(object)));
        }

        #endregion

        #region [====== Remove(IEnumerable<Type>) ======]

        private static readonly IEnumerable<Type> _TypesToRemove = new [] { typeof(object), null, typeof(string) };

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveEnumerable_Throws_IfCollectionIsNull()
        {
            TypeSet.Empty.Remove(null as IEnumerable<Type>);
        }

        [TestMethod]
        public void RemoveEnumerable_DoesNothing_IfSetIsEmpty()
        {
            AssertIsEmpty(TypeSet.Empty.Remove(_TypesToRemove));
        }

        [TestMethod]
        public void RemoveEnumerable_RemovesAllSpecifiedTypes_IfSetIsNotEmpty()
        {
            var set = TypeSet.Empty
                .Add<object>()
                .Add<int>()
                .Remove(_TypesToRemove);

            AssertContainsExactly(set, typeof(int));
        }

        #endregion

        #region [====== AddAssemblies & RemoveAssemblies ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveAssemblyArray_Throws_IfCollectionIsNull()
        {
            TypeSet.Empty.Remove(null as Assembly[]);
        }

        [TestMethod]
        public void RemoveAssemblyArray_AddsAllTypesOfSpecifiedAssemblies_IfCollectionIsNotNull()
        {
            var set = TypeSet.Empty
                .Add<object>()
                .Add<string>()
                .Add<int>()
                .Add<TypeSet>()
                .Remove(typeof(object).Assembly);

            AssertContainsExactly(set, typeof(TypeSet));
        }

        #endregion

        #region [====== AddAssembliesFromCurrentDirectory & RemoveAssembliesFromCurrentDirectory ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddAssembliesFromCurrentDirectory_Throws_IfSearchPatternIsNull()
        {
            TypeSet.Empty.Add(null, string.Empty);
        }

        [TestMethod]
        public void AddAssembliesFromCurrentDirectory_DoesNothing_IfSearchPatternMatchesNoAssemblies()
        {
            AssertIsEmpty(TypeSet.Empty.Add("DoesNotExist.*"));
        }

        [TestMethod]
        public void AddAssembliesFromCurrentDirectory_AddsAllExpectedTypes_IfSearchPatternMatchesSomeAssemblies()
        {
            var set = TypeSet.Empty.Add("Kingo.*");

            AssertContainsAtLeast(set, typeof(TypeSet), typeof(TypeSet));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveAssembliesFromCurrentDirectory_Throws_IfSearchPatternIsNull()
        {
            TypeSet.Empty.Remove(null as string);
        }

        [TestMethod]
        public void RemoveAssembliesFromCurrentDirectory_DoesNothing_IfSearchPatternMatchesNoAssemblies()
        {
            var set = TypeSet.Empty.Add<object>().Remove("DoesNotExist.*");

            AssertContainsExactly(set, typeof(object));
        }

        [TestMethod]
        public void RemoveAssembliesFromCurrentDirectory_RemovesAllExpectedTypes_IfSearchPatternMatchesSomeAssemblies()
        {
            var set = TypeSet.Empty
                .Add<object>()
                .Add<TypeSet>()
                .Add<TypeSetTest>()
                .Remove("Kingo.*");

            AssertContainsExactly(set, typeof(object));
        }

        #endregion

        private static void AssertIsEmpty(TypeSet set) =>
            AssertContainsExactly(set);

        private static void AssertContainsExactly(TypeSet set, params Type[] types)
        {
            Assert.IsNotNull(set);
            Assert.AreEqual(types.Length, set.Count());

            foreach (var type in types)
            {
                Assert.IsTrue(set.Contains(type));
            }
        }

        private static void AssertContainsAtLeast(TypeSet set, params Type[] types)
        {
            Assert.IsNotNull(set);
            Assert.IsTrue(types.Length <= set.Count());

            foreach (var type in types)
            {
                Assert.IsTrue(set.Contains(type));
            }
        }
    }
}
