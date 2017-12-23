using SUNRUSE.Prompter.Persistence.Abstractions;
using SUNRUSE.Prompter.Persistence.Abstractions.TestHelpers;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SUNRUSE.Prompter.Persistence.Sqlite.Tests
{
    public sealed class SqliteEventStoreFileTests : EventStoreTestsBase, IDisposable
    {
        public SqliteEventStoreFileTests()
        {
            if (Directory.Exists("Test Directory For Sqlite Event Store")) Directory.Delete("Test Directory For Sqlite Event Store", true);
        }

        protected override IEventStore CreateInstance()
        {
            return new SqliteEventStore("Data Source=Test Directory For Sqlite Event Store/With Multiple/Nested Subdirectories/To Database; Mode=ReadWriteCreate");
        }

        public void Dispose()
        {
            if (Directory.Exists("Test Directory For Sqlite Event Store")) Directory.Delete("Test Directory For Sqlite Event Store", true);
        }

        [Fact, Trait("Type", "Unit")]
        public void SetsConnectionString()
        {
            using (var eventStore = CreateInstance() as SqliteEventStore)
            {
                Assert.Equal("Data Source=Test Directory For Sqlite Event Store/With Multiple/Nested Subdirectories/To Database; Mode=ReadWriteCreate", eventStore.ConnectionString);
            }
        }

        [Fact, Trait("Type", "Integration")]
        public void CreatesDatabaseFileLazily()
        {
            using (var eventStore = CreateInstance() as SqliteEventStore)
            {
                Assert.False(Directory.Exists("Test Directory For Sqlite Event Store"));
            }
        }

        [Fact, Trait("Type", "Integration")]
        public async Task CreatesExpectedFile()
        {
            using (var eventStore = CreateInstance())
            {
                await InsertInterleaved(eventStore, ImmutableArray.Create(CompetingSequenceWithSameEntityTypeName, CompetingSequenceWithSameEntityId));

                var expected = new[]
                {
                    Path.Combine("Test Directory For Sqlite Event Store", "With Multiple", "Nested Subdirectories", "To Database")
                }
                    .OrderBy(filename => filename);

                var actual = Directory
                    .EnumerateFiles("Test Directory For Sqlite Event Store", "*.*", SearchOption.AllDirectories)
                    .OrderBy(filename => filename)
                    .ToImmutableArray();

                Assert.Equal(expected, actual);
            }
        }
    }
}
