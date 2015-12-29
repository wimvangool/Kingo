﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Players.RegisterPlayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.ChallengePlayer
{
    [TestClass]
    public sealed class ReceiverNotRegisteredScenario : WriteOnlyScenario<ChallengePlayerCommand>
    {
        private readonly PlayerIsRegisteredScenario _playerIsRegistered;

        public ReceiverNotRegisteredScenario()
        {
            _playerIsRegistered = new PlayerIsRegisteredScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return _playerIsRegistered;
        }

        protected override ChallengePlayerCommand When()
        {
            return new ChallengePlayerCommand(Guid.NewGuid(), Guid.NewGuid());
        }

        protected override Session CreateSession()
        {
            var player = _playerIsRegistered.PlayerRegisteredEvent;

            return new Session(player.PlayerId, player.PlayerName);
        }        

        [TestMethod]
        public override async Task ThenAsync()
        {            
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}