using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class EndpointNameResolverTest
    {
        #region [====== Constructor ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfDefaultNameFormatIsNull()
        {
            new EndpointNameResolver(null);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Constructor_Throws_IfDefaultNameFormatIsNotValid()
        {
            new EndpointNameResolver("[service");
        }

        [TestMethod]
        public void Constructor_ReturnsResolver_IfDefaultNameFormatIsValid()
        {
            var format = Guid.NewGuid().ToString();
            var resolver = new EndpointNameResolver(format);

            Assert.AreEqual(format, resolver.DefaultNameFormat.ToString());
        }

        #endregion

        #region [====== AddNameFormat ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNameFormatOfType_Throws_IfFormatIsNull()
        {
            CreateResolver().AddNameFormat<object>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void AddNameFormatOfType_Throws_IfFormatIsNotValid()
        {
            CreateResolver().AddNameFormat<object>("[]");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNameFormat1_Throws_IfMessageTypesIsNull()
        {
            CreateResolver().AddNameFormat(string.Empty, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNameFormat2_Throws_IfMessageTypesIsNull()
        {
            CreateResolver().AddNameFormat(string.Empty, null as IEnumerable<Type>);
        }

        #endregion

        #region [====== ResolveName ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolveName_Throws_IfEndpointIsNull()
        {
            var format = Guid.NewGuid().ToString();
            var resolver = new EndpointNameResolver(format);

            resolver.ResolveName(null);
        }

        [TestMethod]
        public void ResolveName_ReturnsDefaultEndpointName_IfEndpointHasNoSpecificName()
        {
            var format = Guid.NewGuid().ToString();
            var resolver = new EndpointNameResolver(format);
            var name = resolver.ResolveName(CreateEndpoint<object>());

            Assert.AreEqual(format, name);
        }

        [TestMethod]
        public void ResolveName_ReturnsSpecificEndpointName_IfEndpointHasSpecificName_And_SpecificNameIsFixedValue()
        {
            var format = Guid.NewGuid().ToString();
            var resolver = CreateResolver();
            resolver.AddNameFormat<object>(format);
            var name = resolver.ResolveName(CreateEndpoint<object>());

            Assert.AreEqual(format, name);
        }

        [TestMethod]
        public void ResolveName_ReturnsSpecificEndpointName_IfEndpointHasSpecificName_And_SpecificNameIsSetMoreThanOnce()
        {
            var formatA = Guid.NewGuid().ToString();
            var formatB = Guid.NewGuid().ToString();
            var resolver = CreateResolver();
            resolver.AddNameFormat<object>(formatA);
            resolver.AddNameFormat<object>(formatB);
            var name = resolver.ResolveName(CreateEndpoint<object>());

            Assert.AreEqual(formatB, name);
        }

        [TestMethod]
        public void ResolveName_ReturnsSpecificEndpointName_IfEndpointHasSpecificName_And_SpecificNameHasPlaceholders()
        {
            var resolver = CreateResolver();
            resolver.AddNameFormat<object>("[service].[handler].[message]");
            var name = resolver.ResolveName(CreateEndpoint<object>());

            Assert.AreEqual("Default.BasicEndpoint.Object", name);
        }

        private static EndpointNameResolver CreateResolver() =>
            new EndpointNameResolver(Guid.NewGuid().ToString());

        private static IMicroServiceBusEndpoint CreateEndpoint<TMessage>() =>
            new MicroServiceBusEndpointStub<TMessage>("DefaultService", new BasicEndpointHandler<TMessage>());

        #endregion
    }
}
