using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class DefaultMessageKindResolverTest
    {
        #region [====== Commands ======]

        private sealed class Command { }

        private sealed class SomeCommand { }

        private sealed class Weirdcommand { }

        #endregion

        #region [====== Events ======]

        private sealed class Event { }

        private sealed class SomeEvent { }

        private sealed class Weirdevent { }

        #endregion

        #region [====== Requests ======]

        private sealed class Request { }

        private sealed class SomeRequest { }

        private sealed class Weirdrequest { }

        #endregion

        #region [====== Responses ======]

        private sealed class Response { }

        private sealed class SomeResponse { }

        private sealed class Weirdresponse { }

        #endregion

        private readonly IMessageKindResolver _resolver;

        public DefaultMessageKindResolverTest()
        {
            _resolver = new DefaultMessageKindResolver();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolveMessageKind_Throws_IfContentIsNull()
        {
            _resolver.ResolveMessageKind(null as object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolveMessageKind_Throws_IfContentTypeIsNull()
        {
            _resolver.ResolveMessageKind(null);
        }

        [TestMethod]
        public void ResolveMessageKind_ReturnsUndefined_IfTypeNameDoesNotEndWithKnownMessageKind()
        {
            AssertMessageKind(MessageKind.Undefined, new object(), new Weirdcommand(), new Weirdevent(), new Weirdrequest(), new Weirdresponse());
        }

        [TestMethod]
        public void ResolveMessageKind_ReturnsCommand_IfTypeNameEndsWithCommand()
        {
            AssertMessageKind(MessageKind.Command, new Command(), new SomeCommand());
        }

        [TestMethod]
        public void ResolveMessageKind_ReturnsEvent_IfTypeNameEndsWithEvent()
        {
            AssertMessageKind(MessageKind.Event, new Event(), new SomeEvent());
        }

        [TestMethod]
        public void ResolveMessageKind_ReturnsRequest_IfTypeNameEndsWithRequest()
        {
            AssertMessageKind(MessageKind.Request, new Request(), new SomeRequest());
        }

        [TestMethod]
        public void ResolveMessageKind_ReturnsResponse_IfTypeNameEndsWithResponse()
        {
            AssertMessageKind(MessageKind.Response, new Response(), new SomeResponse());
        }

        private void AssertMessageKind(MessageKind expectedKind, params object[] values)
        {
            foreach (var value in values)
            {
                Assert.AreEqual(expectedKind, _resolver.ResolveMessageKind(value));
            }
        }
    }
}
