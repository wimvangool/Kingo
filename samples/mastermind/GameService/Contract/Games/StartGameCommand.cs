using System;
using System.ComponentModel.DataAnnotations;
using Kingo.MicroServices.DataAnnotations;

namespace Kingo.MasterMind.GameService.Games
{
    /// <summary>
    /// This command can be used to start a new game.
    /// </summary>
    public sealed class StartGameCommand : RequestMessage
    {
        /// <summary>
        /// A unique identifier to assign to the new game.
        /// </summary>
        [NotDefault]
        public Guid GameId
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the player that will be playing the game.
        /// </summary>
        [Required(AllowEmptyStrings = false), PlayerName]
        public string PlayerName
        {
            get;
            set;
        }
    }
}
