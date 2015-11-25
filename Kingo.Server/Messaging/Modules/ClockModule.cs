using System;
using System.Threading.Tasks;
using Kingo.Clocks;

namespace Kingo.Messaging.Modules
{
    /// <summary>
    /// This modules associates a <see cref="IClock" /> to the thread that will be handling the message.
    /// </summary>
    public sealed class ClockModule : MessageHandlerModule
    {
        private readonly IClock _clock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClockModule" /> class.
        /// </summary>
        /// <param name="clock"></param>
        public ClockModule(IClock clock)
        {
            if (clock == null)
            {
                throw new ArgumentNullException("clock");
            }
            _clock = clock;
        }

        /// <inheritdoc />
        public override async Task InvokeAsync(IMessageHandler handler)
        {
            using (Clock.OverrideAsyncLocal(_clock))
            {
                await handler.InvokeAsync();
            }
        }        
    }
}
