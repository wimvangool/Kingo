using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Messaging.Validation;

namespace Kingo.Samples.Chess.Users
{    
    public sealed class UserTable : IMessageHandler<UserRegisteredEvent>, IUserAdministration
    {
        #region [====== Updates ======]

        async Task IMessageHandler<UserRegisteredEvent>.HandleAsync(UserRegisteredEvent message)
        {
            using (var command = new DatabaseCommand("sp_Players_Insert"))
            {
                command.Parameters.AddWithValue(DatabaseCommand.KeyParameter, message.UserId);
                command.Parameters.AddWithValue("Name", message.UserName);

                await command.ExecuteNonQueryAsync();
            }
        }

        #endregion

        #region [====== HasBeenRegistered ======]

        public async Task<bool> HasBeenRegisteredAsync(Identifier userName)
        {
            using (var command = new DatabaseCommand("sp_Players_HasBeenRegistered"))
            {
                command.Parameters.AddWithValue("Name", userName.ToString());

                return await command.ExecuteScalarAsync<int>() > 0;
            }
        }

        #endregion

        #region [====== SelectAll ======]

        public static async Task<GetRegisteredUsersResponse> SelectAllAsync(GetPlayersRequest message)
        {
            using (var command = new DatabaseCommand("sp_Players_SelectAll"))
            using (var reader = await command.ExecuteDataReaderAsync())
            {
                return FromReader(reader);
            }
        }

        private static GetRegisteredUsersResponse FromReader(DbDataReader reader)
        {
            return new GetRegisteredUsersResponse(CreateRegisteredPlayers(reader));
        }

        private static IEnumerable<RegisteredUser> CreateRegisteredPlayers(DbDataReader reader)
        {
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    yield return CreateRegisteredPlayer(reader);
                }
            }
        }

        private static RegisteredUser CreateRegisteredPlayer(IDataRecord record)
        {
            return new RegisteredUser(record.GetGuid(0), record.GetString(1));
        }

        #endregion                    
    }
}
