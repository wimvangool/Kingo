using Kingo.BuildingBlocks.Messaging.Constraints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication.Players
{
    [TestClass]
    public sealed class PlayerIsRegisteredScenario : UnitTestScenario<RegisterPlayerCommand>
    {
        internal PlayerRegisteredEvent PlayerRegisteredEvent
        {
            get { return GetDomainEventAt<PlayerRegisteredEvent>(0); }
        }

        protected override RegisterPlayerCommand When()
        {
            var username = PickValueFrom("Peter", "John", "Jeffrey");
            var password = PickValueFrom("abcdef", "welcome01", "password");

            return new RegisterPlayerCommand(username, password);
        }

        [TestMethod]
        public override void Then()
        {
            VerifyThatDomainEventCount().IsEqualTo(1);
            VerifyThatDomainEventAtIndex(0).IsInstanceOf<PlayerRegisteredEvent>().And((validator, @event) =>
            {
                validator.VerifyThat(() => @event.PlayerId).IsEqualTo(Message.PlayerId);
                validator.VerifyThat(() => @event.Username).IsEqualTo(Message.Username);                
            });
        }
    }
}
