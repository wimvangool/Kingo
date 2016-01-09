using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    public sealed class SqlGameRepository : EventStreamRepository<Guid, int, Game>, IGameRepository
    {
        Task<Game> IGameRepository.GetByKeyAsync(Guid gameId)
        {
            return GetByKeyAsync(gameId);
        }

        void IGameRepository.Add(Game game)
        {
            Add(game);
        }

        protected override Task<Game> SelectByKeyAsync(Guid key)
        {
            throw new NotImplementedException();
        }

        protected override async Task<bool> FlushEventsAsync(Game aggregate, int? originalVersion, IList<IVersionedObject<Guid, int>> events)
        {
            using (var command = DatabaseCommand.CreateInsertEventsCommand("sp_Games_InsertEvents", aggregate, events, originalVersion))
            {
                return await command.ExecuteNonQueryAsync();
            }
        }        
    }
}
