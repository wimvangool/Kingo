using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Kingo.MicroServices.DataAnnotations;

namespace Kingo.Games
{
    /// <summary>
    /// This command can be used to start a new game.
    /// </summary>
    public sealed class StartGameCommand : RequestMessage
    {
        /// <summary>
        /// A unique identifier to assign to the new game.
        /// </summary>
        [Required]
        public Guid GameId
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the player that will be playing the game.
        /// </summary>
        [Required]
        public string PlayerName
        {
            get;
            set;
        }
    }
}
