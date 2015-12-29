using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Challenges
{
    [MessageHandler(InstanceLifetime.PerResolve, MessageSources.InternalMessageBus)]
    public sealed class GetPendingChallengesQuery : IQuery<GetPendingChallengesRequest, GetPendingChallengesResponse>,
                                                    IMessageHandler<PlayerChallengedEvent>
    {
        private const string _ChallengeKey = "ChallengeKey";
        private const string _SenderKey = "SenderKey";        
        private const string _ReceiverKey = "ReceiverKey";

        async Task IMessageHandler<PlayerChallengedEvent>.HandleAsync(PlayerChallengedEvent message)
        {
            using (var command = new DatabaseCommand("sp_Challenges_InsertPendingChallenge"))
            {
                command.Parameters.AddWithValue(_ChallengeKey, message.ChallengeId);
                command.Parameters.AddWithValue(_SenderKey, message.SenderId);
                command.Parameters.AddWithValue(_ReceiverKey, message.ReceiverId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<GetPendingChallengesResponse> ExecuteAsync(GetPendingChallengesRequest message)
        {
            using (var command = new DatabaseCommand("sp_Challenges_GetPendingChallenges"))
            {
                command.Parameters.AddWithValue(_ReceiverKey, Session.Current.PlayerId);

                using (var reader = await command.ExecuteDataReaderAsync())
                {
                    return FromReader(reader);
                }
            }
        }        

        private static GetPendingChallengesResponse FromReader(DbDataReader reader)
        {
            return new GetPendingChallengesResponse(CreatePendingChallenges(reader));
        }

        private static IEnumerable<PendingChallenge> CreatePendingChallenges(DbDataReader reader)
        {
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    yield return CreatePendingChallenge(reader);
                }
            }
        }

        private static PendingChallenge CreatePendingChallenge(IDataRecord record)
        {
            return new PendingChallenge(record.GetGuid(0), record.GetString(1));
        }
    }
}
