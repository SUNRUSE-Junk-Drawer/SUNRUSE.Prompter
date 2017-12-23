using Microsoft.Data.Sqlite;
using SUNRUSE.Prompter.Persistence.Abstractions;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SUNRUSE.Prompter.Persistence.Sqlite
{
    /// <summary>Implements <see cref="IEventStore"/> using a Sqlite database connection.</summary>
    public sealed class SqliteEventStore : IEventStore
    {
        /// <summary>The connection string to use.</summary>
        public readonly string ConnectionString;

        /// <inheritdoc />
        public SqliteEventStore(string connectionString)
        {
            ConnectionString = connectionString;
        }

        private SqliteConnection Connection;
        private readonly Mutex InitializedLock = new Mutex();

        private async Task CheckInitialized()
        {
            if (Connection != null) return;

            var locked = false;
            try
            {
                locked = InitializedLock.WaitOne();
                if (!locked) throw new Exception("A mutex unexpectedly returned false while synchronizing initialization");

                if (Connection != null) return;

                try
                {
                    string path = Path.GetDirectoryName(new SqliteConnectionStringBuilder(ConnectionString).DataSource);
                    if (!string.IsNullOrWhiteSpace(path)) Directory.CreateDirectory(path);
                    Connection = new SqliteConnection(ConnectionString);
                    await Connection.OpenAsync();

                    using (var transaction = Connection.BeginTransaction())
                    {
                        using (var command = Connection.CreateCommand())
                        {
                            command.CommandText = @"
                                CREATE TABLE IF NOT EXISTS prompter_v1_event (
                                    entity_type_name TEXT NOT NULL,
                                    entity_id BLOB NOT NULL,
                                    event_id INTEGER NOT NULL,
                                    data BLOB NOT NULL
                                );
                                CREATE UNIQUE INDEX IF NOT EXISTS prompter_v1_event_idx ON prompter_v1_event (entity_type_name, entity_id, event_id);

                                CREATE TABLE IF NOT EXISTS prompter_v1_snapshot (
                                    entity_type_name TEXT NOT NULL,
                                    entity_id BLOB NOT NULL,
                                    snapshot_id INTEGER NOT NULL,
                                    data BLOB NOT NULL
                                );
                                CREATE UNIQUE INDEX IF NOT EXISTS prompter_v1_snapshot_idx ON prompter_v1_snapshot (entity_type_name, entity_id, snapshot_id)
                            ";
                            await command.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();
                    }
                }
                catch
                {
                    if (Connection != null)
                    {
                        Connection.Dispose();
                        Connection = null;
                    }
                    throw;
                }
            }
            finally
            {
                if (locked) InitializedLock.ReleaseMutex();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (Connection != null)
            {
                Connection.Dispose();
                Connection = null;
            }
            InitializedLock.Dispose();
        }

        /// <inheritdoc />
        public async Task<ImmutableArray<byte>> GetEvent(string entityTypeName, Guid entityId, int eventId)
        {
            await CheckInitialized();
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        data 
                    FROM 
                        prompter_v1_event 
                    WHERE 
                        entity_type_name = @entity_type_name
                        AND entity_id = @entity_id
                        AND event_id = @event_id
                ";
                command.Parameters.AddWithValue("entity_type_name", entityTypeName);
                command.Parameters.AddWithValue("entity_id", entityId);
                command.Parameters.AddWithValue("event_id", eventId);
                return (((byte[])await command.ExecuteScalarAsync()).ToImmutableArray());
            }
        }

        /// <inheritdoc />
        public async Task<ImmutableArray<byte>> GetSnapshot(string entityTypeName, Guid entityId, int snapshotId)
        {
            await CheckInitialized();
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        data 
                    FROM 
                        prompter_v1_snapshot
                    WHERE 
                        entity_type_name = @entity_type_name
                        AND entity_id = @entity_id
                        AND snapshot_id = @snapshot_id
                ";
                command.Parameters.AddWithValue("entity_type_name", entityTypeName);
                command.Parameters.AddWithValue("entity_id", entityId);
                command.Parameters.AddWithValue("snapshot_id", snapshotId);
                return (((byte[])await command.ExecuteScalarAsync()).ToImmutableArray());
            }
        }

        /// <inheritdoc />
        public async Task<EventStoreStatistics> GetStatistics(string entityTypeName, Guid entityId)
        {
            await CheckInitialized();
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT (
                        SELECT 
                            MAX(event_id)
                        FROM 
                            prompter_v1_event 
                        WHERE
                            entity_type_name = @entity_type_name
                            AND entity_id = @entity_id
                    ) greatest_event_id, (
                        SELECT 
                            MAX(snapshot_id)
                        FROM 
                            prompter_v1_snapshot
                        WHERE
                            entity_type_name = @entity_type_name
                            AND entity_id = @entity_id
                    ) greatest_snapshot_id
                ";
                command.Parameters.AddWithValue("entity_type_name", entityTypeName);
                command.Parameters.AddWithValue("entity_id", entityId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    reader.Read();
                    var greatestEventId = reader["greatest_event_id"];
                    var greatestSnapshotId = reader["greatest_snapshot_id"];
                    return new EventStoreStatistics(greatestEventId == DBNull.Value ? null : (int?)(long)greatestEventId, greatestSnapshotId == DBNull.Value ? null : (int?)(long)greatestSnapshotId);
                }
            }
        }

        /// <inheritdoc />
        public async Task PersistEvent(string entityTypeName, Guid entityId, int eventId, ImmutableArray<byte> data)
        {
            await CheckInitialized();
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO prompter_v1_event (
                        entity_type_name, 
                        entity_id, 
                        event_id, 
                        data
                    ) VALUES (
                        @entity_type_name, 
                        @entity_id, 
                        @event_id,
                        @data
                    )
                ";
                command.Parameters.AddWithValue("entity_type_name", entityTypeName);
                command.Parameters.AddWithValue("entity_id", entityId);
                command.Parameters.AddWithValue("event_id", eventId);
                command.Parameters.AddWithValue("data", data.ToArray());
                await command.ExecuteNonQueryAsync();
            }
        }

        /// <inheritdoc />
        public async Task PersistSnapshot(string entityTypeName, Guid entityId, int snapshotId, ImmutableArray<byte> data)
        {
            await CheckInitialized();
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO prompter_v1_snapshot (
                        entity_type_name, 
                        entity_id, 
                        snapshot_id, 
                        data
                    ) VALUES (
                        @entity_type_name, 
                        @entity_id, 
                        @snapshot_id,
                        @data
                    )
                ";
                command.Parameters.AddWithValue("entity_type_name", entityTypeName);
                command.Parameters.AddWithValue("entity_id", entityId);
                command.Parameters.AddWithValue("snapshot_id", snapshotId);
                command.Parameters.AddWithValue("data", data.ToArray());
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
