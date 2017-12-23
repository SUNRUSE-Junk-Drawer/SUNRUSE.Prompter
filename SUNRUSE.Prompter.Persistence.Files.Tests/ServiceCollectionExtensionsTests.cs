using Microsoft.Extensions.DependencyInjection;
using SUNRUSE.Prompter.Persistence.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SUNRUSE.Prompter.Persistence.Files.Tests
{
    public sealed class ServiceCollectionExtensionsTests
    {
        [Fact, Trait("Type", "Unit")]
        public void AddFilesEventStoreDoesNotChangeExistingRegistrations()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton("Test String");
            var obj = new object();
            serviceCollection.AddSingleton(obj);

            serviceCollection.AddFilesEventStore("Test Base Path");

            var serviceProvider = serviceCollection.BuildServiceProvider();
            Assert.Equal("Test String", serviceProvider.GetRequiredService<string>());
            Assert.Same(obj, serviceProvider.GetRequiredService<object>());
        }

        [Fact, Trait("Type", "Unit")]
        public void AddFilesEventStoreReturnsServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            var returned = serviceCollection.AddFilesEventStore("Test Base Path");

            Assert.Same(serviceCollection, returned);
        }

        [Fact, Trait("Type", "Unit")]
        public void AddFilesEventStoreInjectsFilesEventStoreAsIEventStore()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddFilesEventStore("Test Base Path");

            var serviceProvider = serviceCollection.BuildServiceProvider();
            Assert.IsType<FilesEventStore>(serviceProvider.GetRequiredService<IEventStore>());
        }

        [Fact, Trait("Type", "Unit")]
        public void AddFilesEventStoreUsesTheGivenBasePath()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddFilesEventStore("Test Base Path");

            var serviceProvider = serviceCollection.BuildServiceProvider();
            Assert.Equal("Test Base Path", Assert.IsType<FilesEventStore>(serviceProvider.GetRequiredService<IEventStore>()).BasePath);
        }
    }
}
