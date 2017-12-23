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
            var numberOfPersistedEvents = 0;
            var numberOfPersistedEventsAtTimeOfLatestSnapshot = 0;

            var eventsPath = GetEventsPath(entityTypeName, entityId);
            if (Directory.Exists(eventsPath))
            {
                numberOfPersistedEvents = Directory
                    .EnumerateFiles(eventsPath)
                    .Select(filename => int.Parse(Path.GetFileName(filename)))
                    .DefaultIfEmpty(-1)
                    .Max() + 1;

                var snapshotsPath = GetSnapshotsPath(entityTypeName, entityId);
                if (Directory.Exists(snapshotsPath))
                {
                    numberOfPersistedEventsAtTimeOfLatestSnapshot = Directory
                        .EnumerateFiles(snapshotsPath)
                        .Select(filename => int.Parse(Path.GetFileName(filename)))
                        .DefaultIfEmpty(0)
                        .Max();
                }
            }

            return Task.FromResult(new EventStoreStatistics(numberOfPersistedEvents, numberOfPersistedEventsAtTimeOfLatestSnapshot));
        }

        /// <inheritdoc />
        public Task<ImmutableArray<byte>> GetSnapshot(string entityTypeName, Guid entityId, int atEventId)
        {
            return Task.FromResult(File.ReadAllBytes(GetSnapshotPath(entityTypeName, entityId, atEventId)).ToImmutableArray());
        }

        /// <inheritdoc />
        public Task PersistEvent(string entityTypeName, Guid entityId, ImmutableArray<byte> data)
        {
            var latestEvent = 0;

            var eventsPath = GetEventsPath(entityTypeName, entityId);
            if (Directory.Exists(eventsPath))
            {
                latestEvent = Directory
                    .EnumerateFiles(eventsPath)
                    .Select(filename => int.Parse(Path.GetFileName(filename)))
                    .DefaultIfEmpty(0)
                    .Max() + 1;
            }
            else
            {
                Directory.CreateDirectory(eventsPath);
            }

            File.WriteAllBytes(GetEventPath(entityTypeName, entityId, latestEvent), data.ToArray());
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task PersistSnapshot(string entityTypeName, Guid entityId, ImmutableArray<byte> data)
        {
            var latestEvent = 0;

            var eventsPath = GetEventsPath(entityTypeName, entityId);
            if (Directory.Exists(eventsPath))
            {
                latestEvent = Directory
                    .EnumerateFiles(eventsPath)
                    .Select(filename => int.Parse(Path.GetFileName(filename)))
                    .DefaultIfEmpty(-1)
                    .Max() + 1;
            }

            var snapshotsPath = GetSnapshotsPath(entityTypeName, entityId);
            if (!Directory.Exists(snapshotsPath)) Directory.CreateDirectory(snapshotsPath);
            File.WriteAllBytes(GetSnapshotPath(entityTypeName, entityId, latestEvent), data.ToArray());
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Dispose() { }
    }
}
