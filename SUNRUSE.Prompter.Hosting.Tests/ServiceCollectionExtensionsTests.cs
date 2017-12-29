// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace SUNRUSE.Prompter.Hosting.Tests
{
    public sealed class ServiceCollectionExtensionsTests
    {
        [Fact, Trait("Type", "Unit")]
        public void AddHostingDoesNotChangeExistingRegistrations()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton("Test String");
            var obj = new object();
            serviceCollection.AddSingleton(obj);

            serviceCollection.AddHosting();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            Assert.Equal("Test String", serviceProvider.GetRequiredService<string>());
            Assert.Same(obj, serviceProvider.GetRequiredService<object>());
        }

        [Fact, Trait("Type", "Unit")]
        public void AddHostingReturnsServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            var returned = serviceCollection.AddHosting();

            Assert.Same(serviceCollection, returned);
        }

        [Fact, Trait("Type", "Unit")]
        public void AddHostingInjectsTimeoutFactoryAsITimeoutFactory()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddHosting();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            Assert.IsType<TimeoutFactory>(serviceProvider.GetRequiredService<ITimeoutFactory>());
        }
    }
}
