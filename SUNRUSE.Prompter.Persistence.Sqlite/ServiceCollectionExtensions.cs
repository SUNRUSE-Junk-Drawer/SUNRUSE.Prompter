// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using Microsoft.Extensions.DependencyInjection;
using SUNRUSE.Prompter.Persistence.Abstractions;
using System;

namespace SUNRUSE.Prompter.Persistence.Sqlite
{
    /// <summary>Extension methods for installing <see cref="SqliteEventStore"/> into <see cref="IServiceCollection"/>s.</summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>Installs a <see cref="SqliteEventStore"/> into a <see cref="IServiceCollection"/> instance as its singleton <see cref="IEventStore"/>.</summary>
        /// <typeparam name="TServiceCollection">The <see cref="IServiceCollection"/> <see cref="Type"/> to install a <see cref="SqliteEventStore"/> into.</typeparam>
        /// <param name="serviceCollection">The <typeparamref name="TServiceCollection"/> to install a <see cref="SqliteEventStore"/> into.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <returns>The given <paramref name="serviceCollection"/>.</returns>
        public static TServiceCollection AddSqliteEventStore<TServiceCollection>(this TServiceCollection serviceCollection, string connectionString) where TServiceCollection : IServiceCollection
        {
            serviceCollection.AddSingleton<IEventStore>(serviceProvider => new SqliteEventStore(connectionString));
            return serviceCollection;
        }
    }
}
