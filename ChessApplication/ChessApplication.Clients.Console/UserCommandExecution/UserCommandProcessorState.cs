using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.BuildingBlocks.Messaging;
using Kingo.BuildingBlocks.Threading;

namespace Kingo.ChessApplication.UserCommandExecution
{
    internal abstract class UserCommandProcessorState : IUserCommandProcessor
    {        
        private readonly Dictionary<string, Func<UserCommandArgumentStack, Task<bool>>> _commandProcessors;

        protected UserCommandProcessorState()
        {
            _commandProcessors = new Dictionary<string, Func<UserCommandArgumentStack, Task<bool>>>()
            {
                { "Exit-Application", ExecuteExitCommand }
            };
        }

        protected abstract IMessageProcessor Processor
        {
            get;
        }

        protected void Register<TCommand>(string name, Func<UserCommandArgumentStack, TCommand> parser, IMessageHandler<TCommand> handler)
            where TCommand : class, IMessage<TCommand>
        {
            Register(name, async arguments =>
            {
                var message = parser.Invoke(arguments);

                arguments.ThrowIfNotEmpty();

                await handler.HandleAsync(message);

                return true;
            });
        }        

        protected void Register(string name, Func<UserCommandArgumentStack, Task<bool>> commandProcessor)
        {
            _commandProcessors[name] = commandProcessor;
        }

        internal void WaitForNextCommand()
        {
            WaitForInput(ToString());
        }

        protected virtual void WaitForInput(string text)
        {
            Console.ForegroundColor = StateColor;

            try
            {
                Console.Write(@"{0}> ", text);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        protected virtual ConsoleColor StateColor
        {
            get { return ConsoleColor.DarkCyan; }
        }

        public override string ToString()
        {
            var stateName = GetType().Name;
            if (stateName.EndsWith("State"))
            {
                return stateName.Substring(0, stateName.Length - 5);
            }
            return stateName;
        }

        public async Task<bool> ExecuteCommandAsync(string name, UserCommandArgumentStack arguments)
        {
            Func<UserCommandArgumentStack, Task<bool>> commandProcessor;

            if (_commandProcessors.TryGetValue(name, out commandProcessor))
            {
                return await commandProcessor.Invoke(arguments);
            }
            throw NewUnknownCommandException(name);
        }

        protected virtual Task<bool> ExecuteExitCommand(UserCommandArgumentStack arguments)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                arguments.ThrowIfNotEmpty();
                return false;
            });            
        }

        private static Exception NewUnknownCommandException(string name)
        {
            const string messageFormat = "Unknown command: '{0}'.";
            var message = string.Format(messageFormat, name);
            return new UserCommandExecutionException(message);
        } 
    }
}
