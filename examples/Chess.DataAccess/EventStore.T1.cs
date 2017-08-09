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
        where TAggregate : class, IEventStream<Guid, int>
    {
        private const string _EventsTableType = "dbo.Events";

        #region [====== Select ======]

        protected override async Task<EventStreamHistory<Guid, int, TAggregate>> SelectHistoryByKeyAsync(Guid key, ITypeToContractMap map)
        {
            using (var command = CreateSelectCommand(key))
            {
                return new EventStreamHistory<Guid, int, TAggregate>(DeserializeEvents(await command.ExecuteDataReaderAsync(), map), false);                
            }            
        }

        private static DatabaseCommand CreateSelectCommand(Guid key)
        {
            var command = new DatabaseCommand("sp_Events_Select");
            command.Parameters.AddWithValue(DatabaseCommand.KeyParameter, key);
            return command;
        }

        private static IEnumerable<IEvent<Guid, int>> DeserializeEvents(DbDataReader reader, ITypeToContractMap map)
        {
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    yield return DeserializeEvent(reader, map);
                }
            }
        }

        private static IEvent<Guid, int> DeserializeEvent(IDataRecord record, ITypeToContractMap map)
        {
            var value = record.GetString(0);
            var typeInfo = record.GetString(1);
            var type = map.GetType(typeInfo);

            return (IEvent<Guid, int>) Serializer.Deserialize(value, type);
        }

        #endregion

        #region [====== Insert ======]

        protected override async Task<bool> InsertEventsAsync(SnapshotToSave<Guid, int> snapshot, int? originalVersion, IEnumerable<EventToSave<Guid, int>> events)
        {
            using (var command = CreateInsertCommand(snapshot, originalVersion, events))
            {
                return await command.ExecuteNonQueryAsync();
            }            
        }

        private static DatabaseCommand CreateInsertCommand(SnapshotToSave<Guid, int> snapshot, int? originalVersion, IEnumerable<EventToSave<Guid, int>> events)
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

        private static DataTable ConvertToTable(IEnumerable<EventToSave<Guid, int>> events)
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
