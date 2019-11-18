using System;
using Kingo.MicroServices;

namespace Kingo.MasterMind.GameService.Games
{
    /// <summary>
    /// Represents an event that is published when a new game has just been started.
    /// </summary>
    public sealed class GameStartedEvent : Message
    {
        /// <summary>
        /// Unique identifier for the game.
        /// </summary>
        public Guid GameId
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the player.
        /// </summary>
        public string PlayerName
        {
            get;
            set;
        }
    }
}
