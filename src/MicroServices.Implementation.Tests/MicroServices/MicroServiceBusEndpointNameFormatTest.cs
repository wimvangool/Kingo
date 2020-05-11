using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MicroServiceBusEndpointNameFormatTest
    {
        #region [====== Parse ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parse_Throws_IfFormatIsNull()
        {
            MicroServiceBusEndpointNameFormat.Parse(null);
        }

        [TestMethod]
        public void Parse_ReturnsResolver_IfFormatIsEmpty()
        {
            var format = string.Empty;
            var nameFormat = MicroServiceBusEndpointNameFormat.Parse(format);

            Assert.IsNotNull(nameFormat);
            Assert.AreEqual(format, nameFormat.ToString());
        }

        [TestMethod]
        public void Parse_ReturnsResolver_IfFormatContainsNoPlaceHolders()
        {
            var format = Guid.NewGuid().ToString();
            var nameFormat = MicroServiceBusEndpointNameFormat.Parse(format);

            Assert.IsNotNull(nameFormat);
            Assert.AreEqual(format, nameFormat.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_Throws_IfFormatContainsEmptyPlaceholder()
        {
            MicroServiceBusEndpointNameFormat.Parse("[]");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_Throws_IfFormatContainsUnknownPlaceholder()
        {
            MicroServiceBusEndpointNameFormat.Parse("[unknown]");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_Throws_IfFormatContainsUnclosedOpeningBracket()
        {
            MicroServiceBusEndpointNameFormat.Parse("[service");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_Throws_IfFormatContainsUnopenedClosingBracket()
        {
            MicroServiceBusEndpointNameFormat.Parse("service]");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_Throws_IfFormatContainsDoubleOpeningBracket()
        {
            MicroServiceBusEndpointNameFormat.Parse("[ser[vice]");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_Throws_IfFormatContainsDoubleClosingBracket()
        {
            MicroServiceBusEndpointNameFormat.Parse("[ser]vice]");
        }

        [TestMethod]
        public void Parse_ReturnsResolver_IfFormatIsServiceNamePlaceholder()
        {
            var format = "[service]";
            var nameFormat = MicroServiceBusEndpointNameFormat.Parse(format);

            Assert.IsNotNull(nameFormat);
            Assert.AreEqual(format, nameFormat.ToString());
        }

        [TestMethod]
        public void Parse_ReturnsResolver_IfFormatIsHandlerNamePlaceholder()
        {
            var format = "[handler]";
            var nameFormat = MicroServiceBusEndpointNameFormat.Parse(format);

            Assert.IsNotNull(nameFormat);
            Assert.AreEqual(format, nameFormat.ToString());
        }

        [TestMethod]
        public void Parse_ReturnsResolver_IfFormatIsMessageNamePlaceholder()
        {
            var format = "[message]";
            var nameFormat = MicroServiceBusEndpointNameFormat.Parse(format);

            Assert.IsNotNull(nameFormat);
            Assert.AreEqual(format, nameFormat.ToString());
        }

        [TestMethod]
        public void Parse_ReturnsResolver_IfFormatIsContainsFixedTextAndPlaceholders()
        {
            var format = $"[service].[handler].{Guid.NewGuid()}";
            var nameFormat = MicroServiceBusEndpointNameFormat.Parse(format);

            Assert.IsNotNull(nameFormat);
            Assert.AreEqual(format, nameFormat.ToString());
        }

        #endregion

        #region [====== FormatName ======]

        private const string _DefaultServiceName = "MyService";

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FormatName_Throws_IfServiceNameIsNull()
        {
            MicroServiceBusEndpointNameFormat.Parse(string.Empty).FormatName(null, typeof(object), typeof(object));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FormatName_Throws_IfMessageHandlerTypeIsNull()
        {
            MicroServiceBusEndpointNameFormat.Parse(string.Empty).FormatName(string.Empty, null, typeof(object));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FormatName_Throws_IfMessageTypeIsNull()
        {
            MicroServiceBusEndpointNameFormat.Parse(string.Empty).FormatName(string.Empty, typeof(object), null);
        }

        [TestMethod]
        public void FormatName_ReturnsEmptyString_IfFormatIsEmpty()
        {
            var nameFormat = MicroServiceBusEndpointNameFormat.Parse(string.Empty);
            var name = nameFormat.FormatName(_DefaultServiceName, typeof(object), typeof(object));

            Assert.AreEqual(nameFormat.ToString(), name);
        }

        [TestMethod]
        public void FormatName_ReturnsFixedName_IfFormatContainsNoPlaceHolders()
        {
            var nameFormat = MicroServiceBusEndpointNameFormat.Parse(Guid.NewGuid().ToString());
            var name = nameFormat.FormatName(_DefaultServiceName, typeof(object), typeof(object));

            Assert.AreEqual(nameFormat.ToString(), name);
        }

        [TestMethod]
        public void FormatName_ReturnsNameOfService_IfFormatIsServicePlaceholder()
        {
            var nameFormat = MicroServiceBusEndpointNameFormat.Parse("[service]");
            var name = nameFormat.FormatName(_DefaultServiceName, typeof(object), typeof(object));

            Assert.AreEqual(_DefaultServiceName, name);
        }

        [TestMethod]
        public void FormatName_ReturnsNameOfHandler_IfFormatIsHandlerPlaceholder()
        {
            var nameFormat = MicroServiceBusEndpointNameFormat.Parse("[handler]");
            var name = nameFormat.FormatName(_DefaultServiceName, typeof(object), typeof(int));

            Assert.AreEqual("Object", name);
        }

        [TestMethod]
        public void FormatName_ReturnsNameOfMessage_IfFormatIsNamePlaceholder()
        {
            var nameFormat = MicroServiceBusEndpointNameFormat.Parse("[message]");
            var name = nameFormat.FormatName(_DefaultServiceName, typeof(object), typeof(int));

            Assert.AreEqual("Int32", name);
        }

        [TestMethod]
        public void FormatName_ReturnsExpectedName_IfFormatContainsFixedTextAndPlaceholders()
        {
            var nameFormat = MicroServiceBusEndpointNameFormat.Parse("[service].[handler].[message]");
            var name = nameFormat.FormatName("ProductService", typeof(object), typeof(int));

            Assert.AreEqual("ProductService.Object.Int32", name);
        }

        #endregion
    }
}
