using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SUNRUSE.Prompter.Persistence.Abstractions;

namespace SUNRUSE.Prompter.Persistence.Files
{
    /// <summary>Implements <see cref="IEventStore"/> using a file per event or snapshot.</summary>
    public sealed class FilesEventStore : IEventStore
    {
        /// <summary>The path to where the files will be written to/read from.</summary>
        public readonly string BasePath;

        /// <inheritdoc />
        public FilesEventStore(string basePath)
        {
            BasePath = basePath;
        }

        private string GetBasePath(string entityTypeName, Guid entityId)
        {
            return Path.Combine(BasePath, entityTypeName, entityId.ToString());
        }

        private string GetEventsPath(string entityTypeName, Guid entityId)
        {
            return Path.Combine(GetBasePath(entityTypeName, entityId), "Events");
        }

        private string GetEventPath(string entityTypeName, Guid entityId, int eventId)
        {
            return Path.Combine(GetEventsPath(entityTypeName, entityId), eventId.ToString());
        }

        private string GetSnapshotsPath(string entityTypeName, Guid entityId)
        {
            return Path.Combine(GetBasePath(entityTypeName, entityId), "Snapshots");
        }

        private string GetSnapshotPath(string entityTypeName, Guid entityId, int atEventId)
        {
            return Path.Combine(GetSnapshotsPath(entityTypeName, entityId), atEventId.ToString());
        }

        /// <inheritdoc />
        public Task<ImmutableArray<byte>> GetEvent(string entityTypeName, Guid entityId, int eventId)
        {
            return Task.FromResult(File.ReadAllBytes(GetEventPath(entityTypeName, entityId, eventId)).ToImmutableArray());
        }

        /// <inheritdoc />
        public Task<EventStoreStatistics> GetStatistics(string entityTypeName, Guid entityId)
        {
            var greatestEventId = (int?)null;
            var greatestSnapshotId = (int?)null;

            var eventsPath = GetEventsPath(entityTypeName, entityId);
            if (Directory.Exists(eventsPath))
            {
                greatestEventId = Directory
                    .EnumerateFiles(eventsPath)
                    .Select(filename => int.Parse(Path.GetFileName(filename)))
                    .Cast<int?>()
                    .Max();

                var snapshotsPath = GetSnapshotsPath(entityTypeName, entityId);
                if (Directory.Exists(snapshotsPath))
                {
                    greatestSnapshotId = Directory
                        .EnumerateFiles(snapshotsPath)
                        .Select(filename => int.Parse(Path.GetFileName(filename)))
                        .Cast<int?>()
                        .Max();
                }
            }

            return Task.FromResult(new EventStoreStatistics(greatestEventId, greatestSnapshotId));
        }

        /// <inheritdoc />
        public Task<ImmutableArray<byte>> GetSnapshot(string entityTypeName, Guid entityId, int snapshotId)
        {
            return Task.FromResult(File.ReadAllBytes(GetSnapshotPath(entityTypeName, entityId, snapshotId)).ToImmutableArray());
        }

        /// <inheritdoc />
        public Task PersistEvent(string entityTypeName, Guid entityId, int eventId, ImmutableArray<byte> data)
        {
            var eventsPath = GetEventsPath(entityTypeName, entityId);
            Directory.CreateDirectory(eventsPath);

            File.WriteAllBytes(GetEventPath(entityTypeName, entityId, eventId), data.ToArray());
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task PersistSnapshot(string entityTypeName, Guid entityId, int snapshotId, ImmutableArray<byte> data)
        {
            var snapshotsPath = GetSnapshotsPath(entityTypeName, entityId);
            Directory.CreateDirectory(snapshotsPath);
            File.WriteAllBytes(GetSnapshotPath(entityTypeName, entityId, snapshotId), data.ToArray());
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Dispose() { }
    }
}
