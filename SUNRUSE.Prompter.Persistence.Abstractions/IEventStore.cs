using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace SUNRUSE.Prompter.Persistence.Abstractions
{
    /// <summary>Describes the event IDs of a specific entity.</summary>
    public struct EventStoreStatistics
    {
        /// <summary>The greatest event ID.</summary>
        public readonly int GreatestEventId;

        /// <summary>The greatest snapshot ID.</summary>
        public readonly int GreatestSnapshotId;

        /// <inheritdoc />
        public EventStoreStatistics(int greatestEventId, int greatestSnapshotId)
        {
            GreatestEventId = greatestEventId;
            GreatestSnapshotId = greatestSnapshotId;
        }
    }

    /// <summary>Implemented by persistence stores.</summary>
    public interface IEventStore : IDisposable
    {
        /// <summary>Persists <see cref="byte"/>s representing an event.</summary>
        /// <param name="entityTypeName">The name of the entity type being persisted.  This can be considered a "namespace", "collection" or "table" of <paramref name="entityId"/>s.</param>
        /// <param name="entityId">The <see cref="Guid"/> of the entity instance being persisted.</param>
        /// <param name="eventId">The ID of the event to persist.</param>
        /// <param name="data">The <see cref="byte"/>s to persist.</param>
        Task PersistEvent(string entityTypeName, Guid entityId, int eventId, ImmutableArray<byte> data);

        /// <summary>Persists <see cref="byte"/> representing the result of applying every event previously persisted against an entity.</summary>
        /// <param name="entityTypeName">The name of the entity type being persisted.  This can be considered a "namespace", "collection" or "table" of <paramref name="entityId"/>s.</param>
        /// <param name="entityId">The <see cref="Guid"/> of the entity instance being persisted.</param>
        /// <param name="snapshotId">The ID of the snapshot to persist.</param>
        /// <param name="data">The <see cref="byte"/>s to persist.</param>
        Task PersistSnapshot(string entityTypeName, Guid entityId, int snapshotId, ImmutableArray<byte> data);

        /// <summary>Gets the <see cref="EventStoreStatistics"/> for a specified entity.</summary>
        /// <param name="entityTypeName">The name of the entity type being persisted.  This can be considered a "namespace", "collection" or "table" of <paramref name="entityId"/>s.</param>
        /// <param name="entityId">The <see cref="Guid"/> of the entity instance being queried.</param>
        /// <returns>A new <see cref="EventStoreStatistics"/> for <paramref name="entityTypeName"/>/<paramref name="entityId"/>.</returns>
        Task<EventStoreStatistics> GetStatistics(string entityTypeName, Guid entityId);

        /// <summary>Gets a specific event previously persisted against an entity.</summary>
        /// <param name="entityTypeName">The name of the entity type being persisted.  This can be considered a "namespace", "collection" or "table" of <paramref name="entityId"/>s.</param>
        /// <param name="entityId">The <see cref="Guid"/> of the entity instance being queried.</param>
        /// <param name="eventId">The ID of the event to get.</param>
        /// <returns>The <see cref="byte"/>s representing the requested event.</returns>
        Task<ImmutableArray<byte>> GetEvent(string entityTypeName, Guid entityId, int eventId);

        /// <summary>Gets a specific snapshot previously persisted against an entity.</summary>
        /// <param name="entityTypeName">The name of the entity type being persisted.  This can be considered a "namespace", "collection" or "table" of <paramref name="entityId"/>s.</param>
        /// <param name="entityId">The <see cref="Guid"/> of the entity instance being queried.</param>
        /// <param name="snapshotId">The ID of the snapshot to get.</param>
        /// <returns>The <see cref="byte"/>s representing the requested snapshot.</returns>
        Task<ImmutableArray<byte>> GetSnapshot(string entityTypeName, Guid entityId, int snapshotId);
    }
}
