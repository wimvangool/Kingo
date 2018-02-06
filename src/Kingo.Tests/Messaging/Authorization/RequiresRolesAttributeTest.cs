﻿using System.Threading.Tasks;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Authorization
{
    [TestClass]
    public sealed class RequiresRolesAttributeTest
    {
        private const string _RequiredRole = "Role-X";

        private sealed class HandlerWhichRequiresCertainRoles : IMessageHandler<object>
        {
            [RequiresRoles(_RequiredRole)]
            public Task HandleAsync(object message, IMicroProcessorContext context) =>
                AsyncMethod.Void;
        }

        private sealed class QueryOne : IQuery<object>
        {
            [RequiresRoles(_RequiredRole)]
            public Task<object> ExecuteAsync(IMicroProcessorContext context) =>
                AsyncMethod.Value(new object());
        }

        private sealed class QueryTwo : IQuery<object, object>
        {
            [RequiresRoles(_RequiredRole)]
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