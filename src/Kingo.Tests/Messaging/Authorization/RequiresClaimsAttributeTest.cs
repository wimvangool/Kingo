using System.Threading.Tasks;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Authorization
{
    [TestClass]
    public sealed class RequiresClaimsAttributeTest
    {
        private const string _ClaimType = "Claim-X";

        private sealed class SomeCommand { }

        private sealed class HandlerWhichRequiresCertainClaims : IMessageHandler<SomeCommand>
        {
            [RequiresClaims(_ClaimType)]
            public Task HandleAsync(SomeCommand message, IMicroProcessorContext context) =>
                AsyncMethod.Void;
        }

        private sealed class QueryOne : IQuery<object>
        {
            [RequiresClaims(_ClaimType)]
            public Task<object> ExecuteAsync(IMicroProcessorContext context) =>
                AsyncMethod.Value(new object());
        }

        private sealed class QueryTwo : IQuery<object, object>
        {
            [RequiresClaims(_ClaimType)]
            public Task<object> ExecuteAsync(object message, IMicroProcessorContext context) =>
                AsyncMethod.Value(message);
        }

        private MicroProcessor _processor;

        [TestInitialize]
        public void Setup()
        {
            _processor = new MicroProcessor();
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedRequestException))]
        public async Task HandleAsync_Throws_IfMessageHandlerRequiresMissingClaim()
        {
            await _processor.HandleAsync(new SomeCommand(), new HandlerWhichRequiresCertainClaims());
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedRequestException))]
        public async Task ExecuteAsync_Throws_IfQueryOneRequiresMissingRole()
        {
            await _processor.ExecuteAsync(new QueryOne());
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedRequestException))]
        public async Task ExecuteAsync_Throws_IfQueryTwoRequiresMissingRole()
        {
            await _processor.ExecuteAsync(new object(), new QueryTwo());
        }
    }
}
