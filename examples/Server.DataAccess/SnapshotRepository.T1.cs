using System;
using System.Data.Common;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess
{
    public abstract class SnapshotRepository<TAggregate> : SnapshotRepository<Guid, int, TAggregate>
        where TAggregate : class, IAggregateRoot<Guid, int>
    {
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
            var command = new DatabaseCommand("sp_Snapshots_Select");
            command.Parameters.AddWithValue(DatabaseCommand.KeyParameter, key);
            return command;
        }

        private static ISnapshot<Guid, int> DeserializeSnapshot(DbDataReader reader, ITypeToContractMap map)
        {
            if (reader.HasRows && reader.Read())
            {
                var value = reader.GetString(0);
                var contract = reader.GetString(1);

                return (ISnapshot<Guid, int>) Serializer.Deserialize(value, map.GetType(contract));
            }
            return null;
        }

        #endregion

        #region [====== Insert ======]

        protected override async Task InsertAsync(Snapshot<Guid, int> snapshot)
        {
            using (var command = CreateInsertCommand(snapshot))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        private static DatabaseCommand CreateInsertCommand(Snapshot<Guid, int> snapshot)
        {
            var command = new DatabaseCommand("sp_Snapshots_Insert");
            command.Parameters.AddWithValue(DatabaseCommand.KeyParameter, snapshot.Value.Key);
            command.Parameters.AddWithValue(DatabaseCommand.VersionParameter, snapshot.Value.Version);
            command.Parameters.AddWithValue(DatabaseCommand.ValueParameter, Serializer.Serialize(snapshot.Value));
            command.Parameters.AddWithValue(DatabaseCommand.TypeParameter, snapshot.Contract);
            return command;
        }

        #endregion

        #region [====== Update ======]

        protected override async Task<bool> UpdateAsync(Snapshot<Guid, int> snapshot, int originalVersion)
        {
            using (var command = CreateUpdateCommand(snapshot, originalVersion))
            {
                return await command.ExecuteNonQueryAsync();
            }
        }

        private static DatabaseCommand CreateUpdateCommand(Snapshot<Guid, int> snapshot, int originalVersion)
        {
            var command = new DatabaseCommand("sp_Snapshots_Update");
            command.Parameters.AddWithValue(DatabaseCommand.KeyParameter, snapshot.Value.Key);
            command.Parameters.AddWithValue(DatabaseCommand.VersionParameter, snapshot.Value.Version);
            command.Parameters.AddWithValue(DatabaseCommand.ValueParameter, Serializer.Serialize(snapshot.Value));
            command.Parameters.AddWithValue(DatabaseCommand.TypeParameter, snapshot.Contract);
            command.Parameters.AddWithValue(DatabaseCommand.OriginalVersionParameter, originalVersion);
            return command;
        }

        #endregion
    }
}
