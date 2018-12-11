using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Authorization
{
    [TestClass]
    public sealed class RequiresRolesAttributeTest
    {
        private const string _RequiredRole = "Role-X";

        private sealed class HandlerWhichRequiresCertainRoles : IMessageHandler<object>
        {
            [RequiresRoles(_RequiredRole)]
            public Task HandleAsync(object message, MessageHandlerContext context) =>
                Task.CompletedTask;
        }

        private sealed class QueryOne : IQuery<object>
        {
            [RequiresRoles(_RequiredRole)]
            public Task<object> ExecuteAsync(QueryContext context) =>
                Task.FromResult(new object());
        }

        private sealed class QueryTwo : IQuery<object, object>
        {
            [RequiresRoles(_RequiredRole)]
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
        public async Task HandleAsync_Throws_IfMessageHandlerRequiresMissingRole()
        {
            await _processor.HandleAsync(new object(), new HandlerWhichRequiresCertainRoles());
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
