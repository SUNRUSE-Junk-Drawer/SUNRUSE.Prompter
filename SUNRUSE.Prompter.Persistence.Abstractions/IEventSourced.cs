using System;

namespace SUNRUSE.Prompter.Persistence.Abstractions
{
    /// <summary>Implemented to describe how events accumulate to create state.</summary>
    /// <typeparam name="TEvent">The base <see cref="Type"/> of events which can occur.  Immutable.</typeparam>
    /// <typeparam name="TState">The base <see cref="Type"/> of states which can be reached.  Immutable.</typeparam>
    public interface IEventSourced<TEvent, TState>
    {
        /// <summary>Creates a new <typeparamref name="TState"/> representing a blank slate.</summary>
        /// <returns>A new <typeparamref name="TState"/> representing a blank slate.</returns>
        TState Start();

        /// <summary>Creates a new <typeparamref name="TState"/> by applying a <typeparamref name="TEvent"/> to an existing <typeparamref name="TState"/>.</summary>
        /// <param name="event">The <typeparamref name="TEvent"/> to apply.</param>
        /// <param name="to">The <typeparamref name="TState"/> to apply <paramref name="event"/> to.</param>
        /// <returns>A new <typeparamref name="TState"/> representing <paramref name="to"/> after applying <paramref name="event"/>.</returns>
        TState Apply(TEvent @event, TState to);
    }
}
