// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using SUNRUSE.Prompter.Persistence.Abstractions;
using SUNRUSE.Prompter.Persistence.Abstractions.TestHelpers;

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
