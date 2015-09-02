using System;
using System.Threading.Tasks;
using Kingo.BuildingBlocks.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Messaging.Modules
{
    [TestClass]
    public sealed class ClockModuleTest
    {
        #region [====== ClockSpy ======]

        private sealed class ClockSpy : IMessageHandler<DefaultMessage>
        {
            private IClock _clock;

            public Task HandleAsync(DefaultMessage message)
            {
                return AsyncMethod.RunSynchronously(() =>
                {
                    _clock = Clock.Current;
                });
            }

            internal void VerifyThatClockWas(IClock clock)
            {
                Assert.AreSame(clock, _clock);
            }
        }

        private sealed class DefaultMessage : Message<DefaultMessage>
        {
            public override DefaultMessage Copy()
            {
                return new DefaultMessage();
            }
        }

        #endregion

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfClockIsNull()
        {
            new ClockModule(null);
        }

        [TestMethod]
        public void Module_SetsClockWhileMessageIsProcessed()
        {
            var spy = new ClockSpy();
            var message = new DefaultMessage();
            var handler = new MessageHandlerWrapper<DefaultMessage>(message, spy);

            var module = new ClockModule(HighResolutionClock.Default);
            var clockBefore = Clock.Current;
            
            module.InvokeAsync(handler).Wait();

            Assert.AreSame(clockBefore, Clock.Current);

            spy.VerifyThatClockWas(HighResolutionClock.Default);
        }
    }
}
