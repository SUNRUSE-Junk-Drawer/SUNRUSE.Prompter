using Microsoft.Extensions.DependencyInjection;
using SUNRUSE.Prompter.Persistence.Abstractions;
using Xunit;

namespace SUNRUSE.Prompter.Persistence.Memory.Tests
{
    public sealed class ServiceCollectionExtensionsTests
    {
        [Fact, Trait("Type", "Unit")]
        public void AddMemoryEventStoreDoesNotChangeExistingRegistrations()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton("Test String");
            var obj = new object();
            serviceCollection.AddSingleton(obj);

            serviceCollection.AddMemoryEventStore();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            Assert.Equal("Test String", serviceProvider.GetRequiredService<string>());
            Assert.Same(obj, serviceProvider.GetRequiredService<object>());
        }

        [Fact, Trait("Type", "Unit")]
        public void AddMemoryEventStoreReturnsServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            var returned = serviceCollection.AddMemoryEventStore();

            Assert.Same(serviceCollection, returned);
        }

        [Fact, Trait("Type", "Unit")]
        public void AddMemoryEventStoreInjectsMemoryEventStoreAsIEventStore()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddMemoryEventStore();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            Assert.IsType<MemoryEventStore>(serviceProvider.GetRequiredService<IEventStore>());
        }
    }
}
