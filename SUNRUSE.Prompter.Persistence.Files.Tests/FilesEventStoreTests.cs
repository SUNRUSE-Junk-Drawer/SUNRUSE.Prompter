using SUNRUSE.Prompter.Persistence.Abstractions;
using SUNRUSE.Prompter.Persistence.Abstractions.Tests;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SUNRUSE.Prompter.Persistence.Files.Tests
{
    public sealed class FilesEventStoreTests : EventStoreTestsBase, IDisposable
    {
        public FilesEventStoreTests()
        {
            if (Directory.Exists("Test Directory For Files Event Store")) Directory.Delete("Test Directory For Files Event Store", true);
        }

        protected override IEventStore CreateInstance()
        {
            return new FilesEventStore("Test Directory For Files Event Store/With Multiple/Nested Subdirectories");
        }

        public void Dispose()
        {
            if (Directory.Exists("Test Directory For Files Event Store")) Directory.Delete("Test Directory For Files Event Store", true);
        }

        [Fact, Trait("Type", "Unit")]
        public void SetsBasePath()
        {
            using (var eventStore = CreateInstance() as FilesEventStore)
            {
                Assert.Equal("Test Directory For Files Event Store/With Multiple/Nested Subdirectories", eventStore.BasePath);
            }
        }

        [Fact, Trait("Type", "Integration")]
        public void CreatesBasePathLazily()
        {
            using (var eventStore = CreateInstance() as FilesEventStore)
            {
                Assert.False(Directory.Exists("Test Directory For Files Event Store"));
            }
        }

        [Fact, Trait("Type", "Integration")]
        public async Task CreatesExpectedFiles()
        {
            using (var eventStore = CreateInstance())
            {
                await InsertInterleaved(eventStore, ImmutableArray.Create(CompetingSequenceWithSameEntityTypeName, CompetingSequenceWithSameEntityId));

                var expected = new[]
                {
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityTypeName.EntityId.ToString(), "Events", "0"),
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityTypeName.EntityId.ToString(), "Events", "1"),
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityTypeName.EntityId.ToString(), "Snapshots", "1"),
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityTypeName.EntityId.ToString(), "Events", "2"),
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityTypeName.EntityId.ToString(), "Events", "3"),
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityTypeName.EntityId.ToString(), "Snapshots", "3"),
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityTypeName.EntityId.ToString(), "Events", "4"),
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityTypeName.EntityId.ToString(), "Events", "5"),
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityTypeName.EntityId.ToString(), "Events", "6"),
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityTypeName.EntityTypeName, CompetingSequenceWithSameEntityTypeName.EntityId.ToString(), "Events", "7"),

                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityId.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId.ToString(), "Events", "0"),
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityId.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId.ToString(), "Snapshots", "0"),
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityId.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId.ToString(), "Events", "1"),
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityId.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId.ToString(), "Events", "2"),
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityId.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId.ToString(), "Snapshots", "2"),
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityId.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId.ToString(), "Events", "3"),
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityId.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId.ToString(), "Events", "4"),
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityId.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId.ToString(), "Snapshots", "4"),
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityId.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId.ToString(), "Events", "5"),
                    Path.Combine("Test Directory For Files Event Store", "With Multiple", "Nested Subdirectories", CompetingSequenceWithSameEntityId.EntityTypeName, CompetingSequenceWithSameEntityId.EntityId.ToString(), "Snapshots", "5")
                }
                    .OrderBy(filename => filename);

                var actual = Directory
                    .EnumerateFiles("Test Directory For Files Event Store", "*.*", SearchOption.AllDirectories)
                    .OrderBy(filename => filename)
                    .ToImmutableArray();

                Assert.Equal(expected, actual);
            }
        }
    }
}
