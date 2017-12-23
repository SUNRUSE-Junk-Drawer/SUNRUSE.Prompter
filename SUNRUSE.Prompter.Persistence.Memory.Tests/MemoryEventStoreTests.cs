using SUNRUSE.Prompter.Persistence.Abstractions;
using SUNRUSE.Prompter.Persistence.Abstractions.Tests;

namespace SUNRUSE.Prompter.Persistence.Memory.Tests
{
    public sealed class MemoryEventStoreTests : NonRestartableEventStoreTestsBase
    {
        protected override IEventStore CreateInstance()
        {
            return new MemoryEventStore();
        }
    }
}
