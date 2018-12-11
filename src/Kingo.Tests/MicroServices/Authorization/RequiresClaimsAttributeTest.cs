using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Authorization
{
    [TestClass]
    public sealed class RequiresClaimsAttributeTest
    {
        private const string _ClaimType = "Claim-X";

        private sealed class SomeCommand { }

        private sealed class HandlerWhichRequiresCertainClaims : IMessageHandler<SomeCommand>
        {
            [RequiresClaims(_ClaimType)]
            public Task HandleAsync(SomeCommand message, MessageHandlerContext context) =>
                Task.CompletedTask;
        }

        private sealed class QueryOne : IQuery<object>
        {
            [RequiresClaims(_ClaimType)]
            public Task<object> ExecuteAsync(QueryContext context) =>
                Task.FromResult(new object());
        }

        private sealed class QueryTwo : IQuery<object, object>
        {
            [RequiresClaims(_ClaimType)]
            public Task<object> ExecuteAsync(object message, QueryContext context) =>
                Task.FromResult(message);
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
