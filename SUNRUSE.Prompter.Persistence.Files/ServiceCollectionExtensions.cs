using Microsoft.Extensions.DependencyInjection;
using SUNRUSE.Prompter.Persistence.Abstractions;
using System;

namespace SUNRUSE.Prompter.Persistence.Files
{
    /// <summary>Extension methods for installing <see cref="FilesEventStore"/> into <see cref="IServiceCollection"/>s.</summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>Installs a <see cref="FilesEventStore"/> into a <see cref="IServiceCollection"/> instance as its singleton <see cref="IEventStore"/>.</summary>
        /// <typeparam name="TServiceCollection">The <see cref="IServiceCollection"/> <see cref="Type"/> to install a <see cref="FilesEventStore"/> into.</typeparam>
        /// <param name="serviceCollection">The <typeparamref name="TServiceCollection"/> to install a <see cref="FilesEventStore"/> into.</param>
        /// <param name="basePath">The path to where the files will be written to/read from.</param>
        /// <returns>The given <paramref name="serviceCollection"/>.</returns>
        public static TServiceCollection AddFilesEventStore<TServiceCollection>(this TServiceCollection serviceCollection, string basePath) where TServiceCollection : IServiceCollection
        {
            serviceCollection.AddSingleton<IEventStore>(serviceProvider => new FilesEventStore(basePath));
            return serviceCollection;
        }
    }
}
