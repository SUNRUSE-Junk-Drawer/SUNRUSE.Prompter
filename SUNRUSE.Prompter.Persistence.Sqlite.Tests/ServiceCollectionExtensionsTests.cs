using Microsoft.Extensions.DependencyInjection;
using SUNRUSE.Prompter.Persistence.Abstractions;
using Xunit;

namespace SUNRUSE.Prompter.Persistence.Sqlite.Tests
{
    public sealed class ServiceCollectionExtensionsTests
    {
        [Fact, Trait("Type", "Unit")]
        public void AddSqliteEventStoreDoesNotChangeExistingRegistrations()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton("Test String");
            var obj = new object();
            serviceCollection.AddSingleton(obj);

            serviceCollection.AddSqliteEventStore("Test Connection String");

            var serviceProvider = serviceCollection.BuildServiceProvider();
            Assert.Equal("Test String", serviceProvider.GetRequiredService<string>());
            Assert.Same(obj, serviceProvider.GetRequiredService<object>());
        }

        [Fact, Trait("Type", "Unit")]
        public void AddSqliteEventStoreReturnsServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            var returned = serviceCollection.AddSqliteEventStore("Test Connection String");

            Assert.Same(serviceCollection, returned);
        }

        [Fact, Trait("Type", "Unit")]
        public void AddSqliteEventStoreInjectsSqliteEventStoreAsIEventStore()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSqliteEventStore("Test Connection String");

            var serviceProvider = serviceCollection.BuildServiceProvider();
            Assert.IsType<SqliteEventStore>(serviceProvider.GetRequiredService<IEventStore>());
        }

        [Fact, Trait("Type", "Unit")]
        public void AddSqliteEventStoreUsesTheGivenConnectionString()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSqliteEventStore("Test Connection String");

            var serviceProvider = serviceCollection.BuildServiceProvider();
            Assert.Equal("Test Connection String", Assert.IsType<SqliteEventStore>(serviceProvider.GetRequiredService<IEventStore>()).ConnectionString);
        }
    }
}
