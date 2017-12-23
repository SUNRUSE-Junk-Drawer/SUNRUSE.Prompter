// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using Microsoft.Extensions.DependencyInjection;
using SUNRUSE.Prompter.Persistence.Abstractions;
using System;

namespace SUNRUSE.Prompter.Persistence.Memory
{
    /// <summary>Extension methods for installing <see cref="MemoryEventStore"/> into <see cref="IServiceCollection"/>s.</summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>Installs a <see cref="MemoryEventStore"/> into a <see cref="IServiceCollection"/> instance as its singleton <see cref="IEventStore"/>.</summary>
        /// <typeparam name="TServiceCollection">The <see cref="IServiceCollection"/> <see cref="Type"/> to install a <see cref="MemoryEventStore"/> into.</typeparam>
        /// <param name="serviceCollection">The <typeparamref name="TServiceCollection"/> to install a <see cref="MemoryEventStore"/> into.</param>
        /// <returns>The given <paramref name="serviceCollection"/>.</returns>
        public static TServiceCollection AddMemoryEventStore<TServiceCollection>(this TServiceCollection serviceCollection) where TServiceCollection : IServiceCollection
        {
            serviceCollection.AddSingleton<IEventStore, MemoryEventStore>();
            return serviceCollection;
        }
    }
}
