using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Reflection
{
    public partial class TypeExtensionsTest
    {
        [TestMethod]
        public void GetInterfacesOfType_ReturnsNoInterfaces_IfClassDoesNotImplementAnyInterfacesOfSpecifiedInterfaceType()
        {
            var interfaceTypes = typeof(object).GetInterfacesOfType<IDisposable>().ToArray();

            Assert.AreEqual(0, interfaceTypes.Length);
        }

        [TestMethod]
        public void GetInterfacesOfType_ReturnsExpectedInterface_IfClassImplementsSpecifiedInterfaceType()
        {
            var interfaceTypes = typeof(Disposable).GetInterfacesOfType<IDisposable>().ToArray();

            Assert.AreEqual(1, interfaceTypes.Length);
            Assert.AreSame(typeof(IDisposable), interfaceTypes[0]);
        }

        [TestMethod]
        public void GetInterfacesOfType_ReturnsExpectedInterface_IfStructImplementsSpecifiedInterfaceType()
        {
            var interfaceTypes = typeof(int).GetInterfacesOfType<IEquatable<int>>().ToArray();

            Assert.AreEqual(1, interfaceTypes.Length);
            Assert.AreSame(typeof(IEquatable<int>), interfaceTypes[0]);
        }

        [TestMethod]
        public void GetInterfacesOfType_ReturnsExpectedInterface_IfStructImplementsSpecifiedInterfaceTypeDefinition()
        {
            var interfaceTypes = typeof(int).GetInterfacesOfType(typeof(IEquatable<>)).ToArray();

            Assert.AreEqual(1, interfaceTypes.Length);
            Assert.AreSame(typeof(IEquatable<int>), interfaceTypes[0]);
        }

        [TestMethod]
        public void GetInterfacesOfType_ReturnsNoInterfaces_IfInterfaceIsNotRelatedToSpecifiedInterfaceType()
        {
            var interfaceTypes = typeof(IDisposable).GetInterfacesOfType<IEquatable<int>>().ToArray();

            Assert.AreEqual(0, interfaceTypes.Length);
        }

        [TestMethod]
        public void GetInterfacesOfType_ReturnsExpectedInterface_IfInterfaceIsEqualToSpecifiedInterfaceType()
        {
            var interfaceTypes = typeof(IDisposable).GetInterfacesOfType<IDisposable>().ToArray();

            Assert.AreEqual(1, interfaceTypes.Length);
            Assert.AreSame(typeof(IDisposable), interfaceTypes[0]);
        }

        [TestMethod]
        public void GetInterfacesOfType_ReturnsExpectedInterface_IfInterfaceClosedVersionOfSpecifiedInterfaceTypeDefinition()
        {
            var interfaceTypes = typeof(IEquatable<int>).GetInterfacesOfType(typeof(IEquatable<>)).ToArray();

            Assert.AreEqual(1, interfaceTypes.Length);
            Assert.AreSame(typeof(IEquatable<int>), interfaceTypes[0]);
        }

        [TestMethod]
        public void GetInterfacesOfType_ReturnsExpectedInterface_IfInterfaceInheritsSpecifiedInterfaceType()
        {
            var interfaceTypes = typeof(IEnumerable<object>).GetInterfacesOfType<IEnumerable>().ToArray();

            Assert.AreEqual(1, interfaceTypes.Length);
            Assert.AreSame(typeof(IEnumerable), interfaceTypes[0]);
        }

        [TestMethod]
        public void GetInterfacesOfType_ReturnsExpectedInterface_IfInterfaceInheritsSpecifiedClosedGenericInterfaceType()
        {
            var interfaceTypes = typeof(IReadOnlyCollection<object>).GetInterfacesOfType<IEnumerable<object>>().ToArray();

            Assert.AreEqual(1, interfaceTypes.Length);
            Assert.AreSame(typeof(IEnumerable<object>), interfaceTypes[0]);
        }

        [TestMethod]
        public void GetInterfacesOfType_ReturnsExpectedInterface_IfInterfaceInheritsSpecifiedOpenGenericInterfaceType()
        {
            var interfaceTypes = typeof(IReadOnlyCollection<object>).GetInterfacesOfType(typeof(IEnumerable<>)).ToArray();

            Assert.AreEqual(1, interfaceTypes.Length);
            Assert.AreSame(typeof(IEnumerable<object>), interfaceTypes[0]);
        }
    }
}
