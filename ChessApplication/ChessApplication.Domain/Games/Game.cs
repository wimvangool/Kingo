using System;
using Kingo.BuildingBlocks;
using Kingo.BuildingBlocks.ComponentModel.Server.Domain;

namespace Kingo.ChessApplication.Games
{
    /// <summary>
    /// Represents a game.
    /// </summary>
    public sealed class Game : AggregateEventStream<Guid, DateTimeOffset>
    {
        private readonly Guid _id;
        private Guid _whitePlayerId;
        private Guid _blackPlayerId;
        private DateTimeOffset _version;

        private Game(Guid id)
        {
            _id = id;            
            _version = DateTimeOffset.MinValue;

            RegisterEventHandler<GameStartedEvent>(Handle);
        }

        internal static Game StartNewGame(Guid playerIdOne, Guid playerIdTwo)
        {
            Guid whitePlayerId;
            Guid blackPlayerId;

            if (AssignWhiteToPlayerOne())
            {
                whitePlayerId = playerIdOne;
                blackPlayerId = playerIdTwo;
            }
            else
            {
                whitePlayerId = playerIdTwo;
                blackPlayerId = playerIdOne;
            }
            var game = new Game(Guid.NewGuid());

            game.Publish((id, version) => new GameStartedEvent(id, version)
            {
                WhitePlayerId = whitePlayerId,
                BlackPlayerId = blackPlayerId
            });
            return game;
        }

        private static bool AssignWhiteToPlayerOne()
        {
            // TODO: Make truly random.
            return Clock.Current.UtcDateAndTime().Millisecond < 500;
        }

        #region [====== Id & Version ======]

        /// <inheritdoc />
        public override Guid Id
        {
            get { return _id; }
        }

        /// <inheritdoc />
        protected override DateTimeOffset Version
        {
            get { return _version; }
            set { SetVersion(ref _version, value); }
        }

        /// <inheritdoc />
        protected override DateTimeOffset NewVersion()
        {
            return Clock.Current.UtcDateAndTime();
        }

        #endregion

        #region [====== Business Logic & Event Handlers ======]

        private void Handle(GameStartedEvent @event)
        {
            _whitePlayerId = @event.WhitePlayerId;
            _blackPlayerId = @event.BlackPlayerId;
        }

        #endregion
    }
}