using System;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Challenges
{
    public sealed class SqlChallengeRepository : SnapshotRepository<Guid, int, Challenge>, IChallengeRepository
    {
        #region [====== Getting & Updating ======]

        Task<Challenge> IChallengeRepository.GetByIdAsync(Guid challengeId)
        {
            return GetByKeyAsync(challengeId);
        }

        protected override async Task<Challenge> SelectByKeyAsync(Guid key)
        {
            using (var command = DatabaseCommand.CreateSelectByKeyCommand("sp_Challenges_SelectByKey", key))
            {
                return await command.ExecuteAggregateAsync<Challenge>();
            }
        }      

        protected override async Task UpdateAsync(Challenge aggregate, int originalVersion)
        {
            using (var command = DatabaseCommand.CreateUpdateCommand<Guid, int, Challenge>("sp_Challenges_Update", aggregate, originalVersion))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        #endregion

        #region [====== Adding & Inserting ======]

        void IChallengeRepository.Add(Challenge challenge)
        {
            Add(challenge);
        }

        protected override async Task InsertAsync(Challenge aggregate)
        {
            using (var command = DatabaseCommand.CreateInsertCommand<Guid, int, Challenge>("sp_Challenges_Insert", aggregate))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        #endregion
    }
}
