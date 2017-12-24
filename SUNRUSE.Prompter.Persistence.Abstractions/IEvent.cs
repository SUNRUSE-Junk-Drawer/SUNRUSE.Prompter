// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace SUNRUSE.Prompter.Persistence.Abstractions
{
    /// <summary>Implemented to describe how an event can change state.</summary>
    /// <typeparam name="TState">The base <see cref="Type"/> of states which can be reached.  Immutable.</typeparam>
    public interface IEvent<TState>
    {
        /// <summary>Creates a new <typeparamref name="TState"/> by applying <see langword="this"/> <see cref="IEvent{TState}"/> to an existing <typeparamref name="TState"/>.</summary>
        /// <param name="to">The <typeparamref name="TState"/> to apply <see langword="this"/> <see cref="IEvent{TState}"/> to.</param>
        /// <returns>A new <typeparamref name="TState"/> representing <paramref name="to"/> after applying <see langword="this"/> <see cref="IEvent{TState}"/>.</returns>
        TState Apply(TState to);
    }
}
