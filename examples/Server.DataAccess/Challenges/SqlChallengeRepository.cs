using System;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Challenges
{
    public sealed class SqlChallengeRepository : SnapshotRepository<Guid, int, Challenge>, IChallengeRepository
    {
        #region [====== Getting & Updating ======]

        protected override Task<Challenge> SelectByKeyAsync(Guid key)
        {
            throw new NotImplementedException();
        }

        protected override Task UpdateAsync(Challenge aggregate, int originalVersion)
        {
            throw new NotImplementedException();
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
