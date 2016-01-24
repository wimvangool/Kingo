using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess
{
    public abstract class EventStore<TAggregate> : EventStore<Guid, int, TAggregate>
        where TAggregate : class, IAggregateRoot<Guid, int> , IWritableEventStream<Guid, int>
    {
        private const string _EventsTableType = "dbo.Events";

        #region [====== Select ======]

        protected override async Task<ISnapshot<Guid, int>> SelectByKeyAsync(Guid key, ITypeToContractMap map)
        {
            using (var command = CreateSelectCommand(key))
            {
                return DeserializeSnapshot(await command.ExecuteDataReaderAsync(), map);
            }            
        }

        private static DatabaseCommand CreateSelectCommand(Guid key)
        {
            var command = new DatabaseCommand("sp_Events_Select");
            command.Parameters.AddWithValue(DatabaseCommand.KeyParameter, key);
            return command;
        }

        private static ISnapshot<Guid, int> DeserializeSnapshot(DbDataReader reader, ITypeToContractMap map)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [====== Insert ======]

        protected override async Task<bool> InsertEventsAsync(Snapshot<Guid, int> snapshot, int? originalVersion, IEnumerable<Event<Guid, int>> events)
        {
            using (var command = CreateInsertCommand(snapshot, originalVersion, events))
            {
                return await command.ExecuteNonQueryAsync();
            }            
        }

        private static DatabaseCommand CreateInsertCommand(Snapshot<Guid, int> snapshot, int? originalVersion, IEnumerable<Event<Guid, int>> events)
        {
            // TODO: Insert snapshot every X events.
            var command = new DatabaseCommand("sp_Events_Insert");
            command.Parameters.AddWithValue(DatabaseCommand.KeyParameter, snapshot.Value.Key);
            command.Parameters.AddWithValue(DatabaseCommand.VersionParameter, snapshot.Value.Version);
            command.Parameters.AddWithValue(DatabaseCommand.ValueParameter, DBNull.Value);
            command.Parameters.AddWithValue(DatabaseCommand.TypeParameter, DBNull.Value);
            command.Parameters.AddWithValue(DatabaseCommand.OriginalVersionParameter, (object) originalVersion ?? DBNull.Value);

            var eventsParameter = command.Parameters.AddWithValue("Events", ConvertToTable(events));
            eventsParameter.SqlDbType = SqlDbType.Structured;
            eventsParameter.TypeName = _EventsTableType;
            return command;            
        }

        private static DataTable ConvertToTable(IEnumerable<Event<Guid, int>> events)
        {
            var table = CreateEventsTable();

            foreach (var @event in events)
            {
                table.Rows.Add(@event.Value.Version, Serializer.Serialize(@event.Value), @event.Contract);
            }
            return table;
        }

        private static DataTable CreateEventsTable()
        {
            var table = new DataTable(_EventsTableType);
            table.Columns.Add(DatabaseCommand.VersionParameter);
            table.Columns.Add(DatabaseCommand.ValueParameter);
            table.Columns.Add(DatabaseCommand.TypeParameter);
            return table;
        } 

        #endregion
    }
}
