﻿using System;
using System.Runtime.Serialization;
using Kingo.Clocks;
using Kingo.MicroServices.DataContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataContractSerializer = Kingo.MicroServices.DataContracts.DataContractSerializer;

namespace Kingo.MicroServices.Configuration
{
    [TestClass]
    public sealed class ConfigureDataContractsTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== DataContracts ======]

        [DataContract(Name = nameof(SomeDataContract))]
        private sealed class DataContractWithImplicitNamespace { }

        [DataContract(Name = nameof(SomeDataContract), Namespace = "http://explicit-namespace/")]
        private sealed class DataContractWithExplicitNamespace { }

        #endregion

        [TestMethod]
        public void ConfigureDataContracts_RegistersDataContractSerializerFactory_IfNoDataContractsAreAdded()
        {
            Processor.ConfigureDataContracts();

            var serializerFactory = CreateSerializerFactory();
            var serializer = serializerFactory.CreateSerializer();

            Assert.IsNotNull(serializer);
            Assert.IsInstanceOfType(serializer, typeof(DataContractSerializer));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_ReturnsFalse_IsTypeHasNoDataContractContractAttribute()
        {
            Processor.ConfigureDataContracts(dataContracts =>
            {
                Assert.IsFalse(dataContracts.Add<object>());
            });

            var serializerFactory = CreateSerializerFactory();
            var serializer = serializerFactory.CreateSerializer();

            serializer.Serialize(new object());
        }

        [TestMethod]
        public void Add_ReturnsTrue_IsTypeIsDataContract_And_NamespaceIsSpecifiedByDataContractAttribute()
        {
            Processor.ConfigureDataContracts(dataContracts =>
            {
                Assert.IsTrue(dataContracts.Add<DataContractWithExplicitNamespace>());
            });

            var serializerFactory = CreateSerializerFactory();
            var serializer = serializerFactory.CreateSerializer();

            var dataContract = new DataContractWithExplicitNamespace();
            var dataContractBlob = serializer.Serialize(dataContract);

            Assert.AreEqual("http://explicit-namespace/somedatacontract", dataContractBlob.ContentType.ToString());
        }

        [TestMethod]
        public void Add_ReturnsTrue_IsTypeIsDataContract_And_NamespaceIsSpecifiedByAssemblyAttribute()
        {
            Processor.ConfigureDataContracts(dataContracts =>
            {
                Assert.IsTrue(dataContracts.Add<DataContractWithImplicitNamespace>());
            });

            var serializerFactory = CreateSerializerFactory();
            var serializer = serializerFactory.CreateSerializer();

            var dataContract = new DataContractWithImplicitNamespace();
            var dataContractBlob = serializer.Serialize(dataContract);

            Assert.AreEqual("http://implicit-namespace/somedatacontract", dataContractBlob.ContentType.ToString());
        }

        [TestMethod]
        public void Add_ReturnsTrue_IsTypeIsDataContract_And_Name_And_NamespaceAreNotSpecified_But_DefaultContractNamespaceIsSpecified()
        {
            Processor.ConfigureDataContracts(dataContracts =>
            {
                Assert.IsTrue(dataContracts.Add<SomeDataContract>());
            });

            var serializerFactory = CreateSerializerFactory();
            var serializer = serializerFactory.CreateSerializer();

            var dataContract = new SomeDataContract() { Value = Clock.SystemClock.LocalDateAndTime().Millisecond };
            var dataContractBlob = serializer.Serialize(dataContract);

            Assert.AreEqual("http://implicit-namespace-default/kingo.microservices.datacontracts.somedatacontract", dataContractBlob.ContentType.ToString());
            Assert.AreEqual(dataContract, serializer.Deserialize(dataContractBlob));
        }

        [TestMethod]
        public void Add_ReturnsTrue_IsTypeIsDataContract_And_Name_And_NamespaceAreNotSpecified_And_NoDefaultContractNamespaceIsSpecified()
        {
            Processor.ConfigureDataContracts(dataContracts =>
            {
                Assert.IsTrue(dataContracts.Add<SomeCommand>());
            });

            var serializerFactory = CreateSerializerFactory();
            var serializer = serializerFactory.CreateSerializer();

            var dataContract = new SomeCommand()
            {
                PropertyA = Guid.NewGuid().ToString(),
                PropertyB = Clock.SystemClock.LocalDateAndTime().Millisecond
            };
            var dataContractBlob = serializer.Serialize(dataContract);

            Assert.AreEqual("http://schemas.datacontract.org/2004/07/kingo.microservices.datacontracts.somecommand", dataContractBlob.ContentType.ToString());
        }



        private IDataContractSerializerFactory CreateSerializerFactory() =>
            CreateProcessor().ServiceProvider.GetRequiredService<IDataContractSerializerFactory>();
    }
}