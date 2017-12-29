// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using Microsoft.Extensions.DependencyInjection;
using System;

namespace SUNRUSE.Prompter.Hosting
{
    /// <summary>Extension methods for installing hosting services into <see cref="IServiceCollection"/>s.</summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>Installs the shared hosting services into a <see cref="IServiceCollection"/> instance as singletons.</summary>
        /// <typeparam name="TServiceCollection">The <see cref="IServiceCollection"/> <see cref="Type"/> to install singletons into.</typeparam>
        /// <param name="serviceCollection">The <typeparamref name="TServiceCollection"/> to install singletons into.</param>
        /// <returns>The given <paramref name="serviceCollection"/>.</returns>
        public static TServiceCollection AddHosting<TServiceCollection>(this TServiceCollection serviceCollection) where TServiceCollection : IServiceCollection
        {
            serviceCollection.AddSingleton<ITimeoutFactory, TimeoutFactory>();
            return serviceCollection;
        }
    }
}
