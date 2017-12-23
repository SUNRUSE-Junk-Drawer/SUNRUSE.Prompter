using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using SUNRUSE.Prompter.Persistence.Abstractions;

namespace SUNRUSE.Prompter.Persistence.Memory
{
    /// <summary>Implements <see cref="IEventStore"/> using an in-memory data set.</summary>
    /// <remarks>This does not work in a clustered environment and will be lost when the application terminates.</remarks>
    public sealed class MemoryEventStore : IEventStore
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, Dictionary<int, ImmutableArray<byte>>>> Events = new ConcurrentDictionary<string, ConcurrentDictionary<Guid, Dictionary<int, ImmutableArray<byte>>>>();
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, Dictionary<int, ImmutableArray<byte>>>> Snapshots = new ConcurrentDictionary<string, ConcurrentDictionary<Guid, Dictionary<int, ImmutableArray<byte>>>>();

        private Dictionary<int, ImmutableArray<byte>> GetEvents(string entityTypeName, Guid entityId)
        {
            return Events
                .GetOrAdd(entityTypeName, unused => new ConcurrentDictionary<Guid, Dictionary<int, ImmutableArray<byte>>>())
                .GetOrAdd(entityId, unused => new Dictionary<int, ImmutableArray<byte>>());
        }

        private Dictionary<int, ImmutableArray<byte>> GetSnapshots(string entityTypeName, Guid entityId)
        {
            return Snapshots
                .GetOrAdd(entityTypeName, unused => new ConcurrentDictionary<Guid, Dictionary<int, ImmutableArray<byte>>>())
                .GetOrAdd(entityId, unused => new Dictionary<int, ImmutableArray<byte>>());
        }

        /// <inheritdoc />
        public Task<ImmutableArray<byte>> GetEvent(string entityTypeName, Guid entityId, int eventId)
        {
            return Task.FromResult(GetEvents(entityTypeName, entityId)[eventId]);
        }

        /// <inheritdoc />
        public Task<EventStoreStatistics> GetStatistics(string entityTypeName, Guid entityId)
        {
            return Task.FromResult(new EventStoreStatistics
            (
                GetEvents(entityTypeName, entityId).Keys.DefaultIfEmpty(-1).Max() + 1, 
                GetSnapshots(entityTypeName, entityId).Keys.DefaultIfEmpty(0).Max()
            ));
        }

        /// <inheritdoc />
        public Task<ImmutableArray<byte>> GetSnapshot(string entityTypeName, Guid entityId, int atEventId)
        {
            return Task.FromResult(GetSnapshots(entityTypeName, entityId)[atEventId]);
        }

        /// <inheritdoc />
        public Task PersistEvent(string entityTypeName, Guid entityId, int eventId, ImmutableArray<byte> data)
        {
            GetEvents(entityTypeName, entityId)[eventId] = data;
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task PersistSnapshot(string entityTypeName, Guid entityId, int snapshotId, ImmutableArray<byte> data)
        {
            GetSnapshots(entityTypeName, entityId)[snapshotId] = data;
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Dispose() { }
    }
}
