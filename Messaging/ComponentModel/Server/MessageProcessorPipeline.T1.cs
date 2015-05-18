using System.Collections.Generic;

namespace System.ComponentModel.Server
{
    internal abstract class MessageProcessorPipeline<TModule>
    {
        private readonly Stack<TModule> _modules;

        protected MessageProcessorPipeline(IEnumerable<TModule> modules)
        {
            _modules = PushModulesOnStack(modules);
        }               

        protected IEnumerable<TModule> Modules
        {
            get { return _modules; }
        }

        private static Stack<TModule> PushModulesOnStack(IEnumerable<TModule> modules)
        {
            var stack = new Stack<TModule>();

            foreach (var module in modules)
            {
                stack.Push(module);
            }
            return stack;
        }        
    }
}
