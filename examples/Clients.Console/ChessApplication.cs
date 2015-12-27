using System;
using System.Threading;
using Clients.ConsoleApp.States;

namespace Clients.ConsoleApp
{
    internal sealed class ChessApplication
    {
        private ChessApplicationState _state;

        private ChessApplication()
        {
            _state = new StoppedState(this);
        }

        private bool IsRunning
        {
            get { return _state.IsRunningState; }
        }

        private void ProcessNextCommand()
        {
            _state.ProcessNextCommand();
        }

        internal void SwitchTo(ChessApplicationState state)
        {
            Interlocked.Exchange(ref _state, state).OnExitting();

            state.OnEntering();
        }        

        private static void Main(string[] args)
        {
            var application = new ChessApplication();

            application.SwitchTo(new StartedState(application));

            do
            {
                application.ProcessNextCommand();
            }
            while (application.IsRunning);
        }

        #region [====== ConsoleColor ======]

        private sealed class ConsoleColorScope : IDisposable
        {
            private readonly ConsoleColor _oldColor;
            private bool _isDisposed;

            internal ConsoleColorScope(ConsoleColor newColor)
            {
                _oldColor = Console.ForegroundColor;

                Console.ForegroundColor = newColor;
            }

            public void Dispose()
            {
                if (_isDisposed)
                {
                    return;
                }
                Console.ForegroundColor = _oldColor;

                _isDisposed = true;
            }
        }

        internal static IDisposable UseColor(ConsoleColor color)
        {
            return new ConsoleColorScope(color);
        }

        #endregion
    }
}
