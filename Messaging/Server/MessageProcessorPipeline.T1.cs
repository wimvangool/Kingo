using System.Collections.Generic;

namespace System.ComponentModel.Server
{
    internal abstract class MessageProcessorPipeline<TModule> : IMessageProcessorPipeline where TModule : IMessageProcessorPipeline
    {
        private readonly Stack<TModule> _modules;

        protected MessageProcessorPipeline(IEnumerable<TModule> modules)
        {
            _modules = PushModulesOnStack(modules);
        }

        public void Start()
        {
            foreach (var module in Modules)
            {
                module.Start();
            }
        }

        #region [====== Dispose ======]
        
        protected bool IsDisposed
        {
            get;
            private set;
        }
        
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
              
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                foreach (var module in _modules)
                {
                    module.Dispose();
                }
            }
            IsDisposed = true;
        }
        
        protected ObjectDisposedException NewObjectDisposedException()
        {
            return new ObjectDisposedException(GetType().Name);
        }

        #endregion

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
