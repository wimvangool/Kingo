using System;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Players
{
    public sealed class SqlPlayerRepository : SnapshotRepository<Guid, int, Player>, IPlayerRepository
    {
        private const string _Name = "Name";

        async Task<bool> IPlayerRepository.HasBeenRegisteredAsync(Identifier playerName)
        {
            using (var command = new DatabaseCommand("sp_Players_HasBeenRegistered"))
            {
                command.Parameters.AddWithValue(_Name, playerName.ToString());
                
                return await command.ExecuteScalarAsync<int>() > 0;
            }            
        }

        void IPlayerRepository.Add(Player player)
        {
            Add(player);
        }

        protected override Task<Player> SelectByKeyAsync(Guid key)
        {
            throw new NotImplementedException();
        }

        protected override async Task InsertAsync(Player aggregate)
        {
            using (var command = DatabaseCommand.CreateInsertCommand<Guid, int, Player>("sp_Players_Insert", aggregate))
            {
                command.Parameters.AddWithValue(_Name, aggregate.Name.ToString());

                await command.ExecuteNonQueryAsync();
            }            
        }

        protected override Task UpdateAsync(Player aggregate, int originalVersion)
        {
            throw new NotImplementedException();
        }               
    }
}
