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
                                    number_of_events_at_time_of_creation INTEGER NOT NULL,
                                    data BLOB NOT NULL
                                );
                                CREATE UNIQUE INDEX IF NOT EXISTS prompter_v1_event_idx ON prompter_v1_event (entity_type_name, entity_id, number_of_events_at_time_of_creation);

                                CREATE TABLE IF NOT EXISTS prompter_v1_snapshot (
                                    entity_type_name TEXT NOT NULL,
                                    entity_id BLOB NOT NULL,
                                    number_of_events_at_time_of_creation INTEGER NOT NULL,
                                    data BLOB NOT NULL
                                );
                                CREATE UNIQUE INDEX IF NOT EXISTS prompter_v1_snapshot_idx ON prompter_v1_snapshot (entity_type_name, entity_id, number_of_events_at_time_of_creation)
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
                        AND number_of_events_at_time_of_creation = @number_of_events_at_time_of_creation
                ";
                command.Parameters.AddWithValue("entity_type_name", entityTypeName);
                command.Parameters.AddWithValue("entity_id", entityId);
                command.Parameters.AddWithValue("number_of_events_at_time_of_creation", eventId);
                return (((byte[])await command.ExecuteScalarAsync()).ToImmutableArray());
            }
        }

        /// <inheritdoc />
        public async Task<ImmutableArray<byte>> GetSnapshot(string entityTypeName, Guid entityId, int atEventId)
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
                        AND number_of_events_at_time_of_creation = @number_of_events_at_time_of_creation
                ";
                command.Parameters.AddWithValue("entity_type_name", entityTypeName);
                command.Parameters.AddWithValue("entity_id", entityId);
                command.Parameters.AddWithValue("number_of_events_at_time_of_creation", atEventId);
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
                            MAX(number_of_events_at_time_of_creation)
                        FROM 
                            prompter_v1_event 
                        WHERE
                            entity_type_name = @entity_type_name
                            AND entity_id = @entity_id
                    ) number_of_events, (
                        SELECT 
                            MAX(number_of_events_at_time_of_creation)
                        FROM 
                            prompter_v1_snapshot
                        WHERE
                            entity_type_name = @entity_type_name
                            AND entity_id = @entity_id
                    ) number_of_events_at_time_of_latest_snapshot_creation
                ";
                command.Parameters.AddWithValue("entity_type_name", entityTypeName);
                command.Parameters.AddWithValue("entity_id", entityId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    reader.Read();
                    var numberOfEvents = reader["number_of_events"];
                    var numberOfEventsAtTimeOfLatestSnapshotCreation = reader["number_of_events_at_time_of_latest_snapshot_creation"];
                    return new EventStoreStatistics(numberOfEvents == DBNull.Value ? null : (int?)(long)numberOfEvents, numberOfEventsAtTimeOfLatestSnapshotCreation == DBNull.Value ? null : (int?)(long)numberOfEventsAtTimeOfLatestSnapshotCreation);
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
                        number_of_events_at_time_of_creation, 
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
                        number_of_events_at_time_of_creation, 
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
