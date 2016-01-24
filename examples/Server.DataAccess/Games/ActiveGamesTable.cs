using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Players;

namespace Kingo.Samples.Chess.Games
{
    [MessageHandler(InstanceLifetime.PerResolve, MessageSources.InternalMessageBus)]
    public sealed class ActiveGamesTable : IMessageHandler<GameStartedEvent>,
                                           IMessageHandler<GameForfeitedEvent>
    {
        private const string _GameKey = "GameKey";
        private const string _WhitePlayerKey = "WhitePlayerKey";
        private const string _BlackPlayerKey = "BlackPlayerKey";

        #region [====== Updates ======]

        async Task IMessageHandler<GameStartedEvent>.HandleAsync(GameStartedEvent message)
        {
            using (var command = new DatabaseCommand("sp_ActiveGames_Insert"))
            {
                command.Parameters.AddWithValue(_GameKey, message.GameId);
                command.Parameters.AddWithValue(_WhitePlayerKey, message.WhitePlayerId);
                command.Parameters.AddWithValue(_BlackPlayerKey, message.BlackPlayerId);

                await command.ExecuteNonQueryAsync();
            }
        }

        async Task IMessageHandler<GameForfeitedEvent>.HandleAsync(GameForfeitedEvent message)
        {
            using (var command = new DatabaseCommand("sp_ActiveGames_Delete"))
            {
                command.Parameters.AddWithValue(_GameKey, message.GameId);

                await command.ExecuteNonQueryAsync();
            }
        }

        #endregion

        #region [====== SelectByPlayer ======]

        public static async Task<GetActiveGamesResponse> SelectByPlayerAsync(GetActiveGamesRequest message)
        {
            using (var command = new DatabaseCommand("sp_ActiveGames_SelectByPlayer"))
            {
                command.Parameters.AddWithValue("PlayerKey", Session.Current.PlayerId);

                using (var reader = await command.ExecuteDataReaderAsync())
                {
                    return FromReader(reader);
                }
            }
        }

        private static GetActiveGamesResponse FromReader(DbDataReader reader)
        {
            return new GetActiveGamesResponse(CreateActiveGames(reader));
        }

        private static IEnumerable<ActiveGame> CreateActiveGames(DbDataReader reader)
        {
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    yield return CreateActiveGame(reader);
                }
            }
        }

        private static ActiveGame CreateActiveGame(IDataRecord record)
        {
            var gameId = record.GetGuid(0);
            var whitePlayer = new RegisteredPlayer(record.GetGuid(1), record.GetString(2));
            var blackPlayer = new RegisteredPlayer(record.GetGuid(3), record.GetString(4));

            return new ActiveGame(gameId, whitePlayer, blackPlayer);
        }

        #endregion        
    }
}
