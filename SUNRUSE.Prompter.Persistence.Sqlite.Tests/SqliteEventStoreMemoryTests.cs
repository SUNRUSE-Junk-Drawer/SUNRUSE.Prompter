using SUNRUSE.Prompter.Persistence.Abstractions;
using SUNRUSE.Prompter.Persistence.Abstractions.TestHelpers;
using Xunit;

namespace SUNRUSE.Prompter.Persistence.Sqlite.Tests
{
    public sealed class SqliteEventStoreMemoryTests : NonRestartableEventStoreTestsBase
    {
        protected override IEventStore CreateInstance()
        {
            return new SqliteEventStore("Data Source=:memory:");
        }

        [Fact, Trait("Type", "Unit")]
        public void SetsConnectionString()
        {
            using (var eventStore = CreateInstance() as SqliteEventStore)
            {
                Assert.Equal("Data Source=:memory:", eventStore.ConnectionString);
            }
        }
    }
}
