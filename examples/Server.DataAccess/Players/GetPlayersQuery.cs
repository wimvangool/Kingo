using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Players
{
    public sealed class GetPlayersQuery : IQuery<GetPlayersRequest, GetPlayersResponse>
    {
        public async Task<GetPlayersResponse> ExecuteAsync(GetPlayersRequest message)
        {
            using (var command = new DatabaseCommand("sp_Players_GetPlayers"))
            using (var reader = await command.ExecuteDataReader())
            {
                return FromReader(reader);
            }
        }

        private static GetPlayersResponse FromReader(SqlDataReader reader)
        {
            return new GetPlayersResponse(CreateRegisteredPlayers(reader));
        }

        private static IEnumerable<RegisteredPlayer> CreateRegisteredPlayers(SqlDataReader reader)
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
            return new RegisteredPlayer(record.GetString(0));
        }
    }
}
