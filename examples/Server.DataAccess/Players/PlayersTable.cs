using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Players
{
    [MessageHandler(InstanceLifetime.PerResolve, MessageSources.InternalMessageBus)]
    public sealed class PlayersTable : IMessageHandler<PlayerRegisteredEvent>, IPlayerAdministration
    {
        #region [====== Updates ======]

        async Task IMessageHandler<PlayerRegisteredEvent>.HandleAsync(PlayerRegisteredEvent message)
        {
            using (var command = new DatabaseCommand("sp_Players_Insert"))
            {
                command.Parameters.AddWithValue(DatabaseCommand.KeyParameter, message.PlayerId);
                command.Parameters.AddWithValue("Name", message.PlayerName);

                await command.ExecuteNonQueryAsync();
            }
        }

        #endregion

        #region [====== HasBeenRregistered ======]

        public async Task<bool> HasBeenRegisteredAsync(Identifier playerName)
        {
            using (var command = new DatabaseCommand("sp_Players_HasBeenRegistered"))
            {
                command.Parameters.AddWithValue("Name", playerName.ToString());

                return await command.ExecuteScalarAsync<int>() > 0;
            }
        }

        #endregion

        #region [====== SelectAll ======]

        public static async Task<GetPlayersResponse> SelectAllAsync(GetPlayersRequest message)
        {
            using (var command = new DatabaseCommand("sp_Players_SelectAll"))
            using (var reader = await command.ExecuteDataReaderAsync())
            {
                return FromReader(reader);
            }
        }

        private static GetPlayersResponse FromReader(DbDataReader reader)
        {
            return new GetPlayersResponse(CreateRegisteredPlayers(reader));
        }

        private static IEnumerable<RegisteredPlayer> CreateRegisteredPlayers(DbDataReader reader)
        {
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    yield return CreateRegisteredPlayer(reader);
                }
            }
        }

        private static RegisteredPlayer CreateRegisteredPlayer(IDataRecord record)
        {
            return new RegisteredPlayer(record.GetGuid(0), record.GetString(1));
        }

        #endregion                    
    }
}
