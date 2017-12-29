// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace SUNRUSE.Prompter.Hosting
{
    /// <summary>Creates <see cref="ITimeout"/> instances.</summary>
    public interface ITimeoutFactory
    {
        /// <summary>Creates a new <see cref="ITimeout"/>.</summary>
        /// <param name="duration">The <see cref="TimeSpan"/> before <paramref name="toDispose"/> is <see cref="IDisposable.Dispose"/>d.</param>
        /// <param name="toDispose">The <see cref="IDisposable"/> instance to create a <see cref="ITimeout"/> for.</param>
        /// <returns>A <see cref="ITimeout"/> for <paramref name="duration"/>/<paramref name="toDispose"/>.</returns>
        ITimeout Create(TimeSpan duration, IDisposable toDispose);
    }

    /// <inheritdoc />
    public sealed class TimeoutFactory : ITimeoutFactory
    {
        /// <inheritdoc />
        public ITimeout Create(TimeSpan duration, IDisposable toDispose)
        {
            return new Timeout(duration, toDispose);
        }
    }
}
