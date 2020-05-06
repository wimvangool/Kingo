using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class EndpointNameFormatTest
    {
        #region [====== MicroServiceBusEndpointStub ======]

        //[MicroServiceBusEndpointName("SomeHandler")]
        private class HandlerWithSpecificName<TMessage> : BasicEndpointHandler<TMessage> { }

        private sealed class AlphaCommand { }

        private sealed class BetaEvent { }

        private sealed class GammaRequest { }

        private sealed class DeltaResponse { }

        //[MicroServiceBusEndpointName("SomeCommand")]
        private sealed class CommandWithSpecificName { }

        #endregion

        #region [====== Parse ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parse_Throws_IfFormatIsNull()
        {
            EndpointNameFormat.Parse(null);
        }

        [TestMethod]
        public void Parse_ReturnsResolver_IfFormatIsEmpty()
        {
            var format = string.Empty;
            var resolver = EndpointNameFormat.Parse(format);

            Assert.IsNotNull(resolver);
            Assert.AreEqual(format, resolver.ToString());
        }

        [TestMethod]
        public void Parse_ReturnsResolver_IfFormatContainsNoPlaceHolders()
        {
            var format = Guid.NewGuid().ToString();
            var resolver = EndpointNameFormat.Parse(format);

            Assert.IsNotNull(resolver);
            Assert.AreEqual(format, resolver.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_Throws_IfFormatContainsEmptyPlaceholder()
        {
            EndpointNameFormat.Parse("[]");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_Throws_IfFormatContainsUnknownPlaceholder()
        {
            EndpointNameFormat.Parse("[unknown]");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_Throws_IfFormatContainsUnclosedOpeningBracket()
        {
            EndpointNameFormat.Parse("[service");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_Throws_IfFormatContainsUnopenedClosingBracket()
        {
            EndpointNameFormat.Parse("service]");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_Throws_IfFormatContainsDoubleOpeningBracket()
        {
            EndpointNameFormat.Parse("[ser[vice]");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_Throws_IfFormatContainsDoubleClosingBracket()
        {
            EndpointNameFormat.Parse("[ser]vice]");
        }

        [TestMethod]
        public void Parse_ReturnsResolver_IfFormatIsServiceNamePlaceholder()
        {
            var format = "[service]";
            var resolver = EndpointNameFormat.Parse(format);

            Assert.IsNotNull(resolver);
            Assert.AreEqual(format, resolver.ToString());
        }

        [TestMethod]
        public void Parse_ReturnsResolver_IfFormatIsHandlerNamePlaceholder()
        {
            var format = "[handler]";
            var resolver = EndpointNameFormat.Parse(format);

            Assert.IsNotNull(resolver);
            Assert.AreEqual(format, resolver.ToString());
        }

        [TestMethod]
        public void Parse_ReturnsResolver_IfFormatIsMessageNamePlaceholder()
        {
            var format = "[message]";
            var resolver = EndpointNameFormat.Parse(format);

            Assert.IsNotNull(resolver);
            Assert.AreEqual(format, resolver.ToString());
        }

        [TestMethod]
        public void Parse_ReturnsResolver_IfFormatIsContainsFixedTextAndPlaceholders()
        {
            var format = $"[service].[handler].{Guid.NewGuid()}";
            var resolver = EndpointNameFormat.Parse(format);

            Assert.IsNotNull(resolver);
            Assert.AreEqual(format, resolver.ToString());
        }

        #endregion

        #region [====== ResolveName ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolveName_Throws_IfEndpointIsNull()
        {
            EndpointNameFormat.Parse(string.Empty).ResolveName(null);
        }

        [TestMethod]
        public void ResolveName_ReturnsEmptyString_IfFormatIsEmpty()
        {
            var resolver = EndpointNameFormat.Parse(string.Empty);
            var name = ResolveName<object>(resolver);

            Assert.IsNotNull(name);
            Assert.AreEqual(0, name.Length);
        }

        [TestMethod]
        public void ResolveName_ReturnsFixedName_IfFormatContainsNoPlaceHolders()
        {
            var format = Guid.NewGuid().ToString();
            var resolver = EndpointNameFormat.Parse(format);
            var name = ResolveName<object>(resolver);

            Assert.AreEqual(format, name);
        }

        [TestMethod]
        public void ResolveName_ReturnsNameOfService_IfFormatIsServicePlaceholder()
        {
            var format = "[service]";
            var resolver = EndpointNameFormat.Parse(format);
            var name = ResolveName<object>(resolver);

            Assert.AreEqual("Default", name);
        }

        [TestMethod]
        public void ResolveName_ReturnsNameOfHandler_IfFormatIsHandlerPlaceholder()
        {
            var format = "[handler]";
            var resolver = EndpointNameFormat.Parse(format);
            var name = ResolveName<object>(resolver);

            Assert.AreEqual("BasicEndpoint", name);
        }

        [TestMethod]
        public void ResolveName_ReturnsNameOfHandler_IfFormatIsHandlerPlaceholder_And_HandlerHasNameAttribute()
        {
            var format = "[handler]";
            var resolver = EndpointNameFormat.Parse(format);
            var name = ResolveName(resolver, null, new HandlerWithSpecificName<object>());

            Assert.AreEqual("SomeHandler", name);
        }

        [TestMethod]
        public void ResolveName_ReturnsNameOfMessage_IfFormatIsNamePlaceholder()
        {
            var format = "[message]";
            var resolver = EndpointNameFormat.Parse(format);
            var name = ResolveName<object>(resolver);

            Assert.AreEqual("Object", name);
        }

        [TestMethod]
        public void ResolveName_ReturnsNameOfMessage_IfFormatIsNamePlaceholder_And_MessageIsCommand()
        {
            var format = "[message]";
            var resolver = EndpointNameFormat.Parse(format);
            var name = ResolveName<AlphaCommand>(resolver);

            Assert.AreEqual("Alpha", name);
        }

        [TestMethod]
        public void ResolveName_ReturnsNameOfMessage_IfFormatIsNamePlaceholder_And_MessageIsEvent()
        {
            var format = "[message]";
            var resolver = EndpointNameFormat.Parse(format);
            var name = ResolveName<BetaEvent>(resolver);

            Assert.AreEqual("Beta", name);
        }

        [TestMethod]
        public void ResolveName_ReturnsNameOfMessage_IfFormatIsNamePlaceholder_And_MessageIsQueryRequest()
        {
            var format = "[message]";
            var resolver = EndpointNameFormat.Parse(format);
            var name = ResolveName<GammaRequest>(resolver);

            Assert.AreEqual("Gamma", name);
        }

        [TestMethod]
        public void ResolveName_ReturnsNameOfMessage_IfFormatIsNamePlaceholder_And_MessageIsQueryResponse()
        {
            var format = "[message]";
            var resolver = EndpointNameFormat.Parse(format);
            var name = ResolveName<DeltaResponse>(resolver);

            Assert.AreEqual("Delta", name);
        }

        [TestMethod]
        public void ResolveName_ReturnsNameOfMessage_IfFormatIsNamePlaceholder_And_MessageHasNameAttribute()
        {
            var format = "[message]";
            var resolver = EndpointNameFormat.Parse(format);
            var name = ResolveName<CommandWithSpecificName>(resolver);

            Assert.AreEqual("SomeCommand", name);
        }

        [TestMethod]
        public void ResolveName_ReturnsExpectedName_IfFormatContainsFixedTextAndPlaceholders()
        {
            var format = "[service].[handler].[message]";
            var resolver = EndpointNameFormat.Parse(format);
            var name = ResolveName<object>(resolver, "MicroService");

            Assert.AreEqual("Micro.BasicEndpoint.Object", name);
        }

        private static string ResolveName<TMessage>(EndpointNameFormat resolver, string serviceName = null, IEndpointMessageHandler<TMessage> handler = null) =>
            resolver.ResolveName(new MicroServiceBusEndpointStub<TMessage>(serviceName ?? "Default", handler ?? new BasicEndpointHandler<TMessage>()));

        #endregion
    }
}
