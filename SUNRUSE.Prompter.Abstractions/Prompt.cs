// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Immutable;

namespace SUNRUSE.Prompter.Abstractions
{
    /// <summary>Represents a prompt to show the user.</summary>
    public sealed class Prompt
    {
        /// <summary>An unique <see cref="Guid"/> identifying the prompt.</summary>
        /// <remarks>This is used to ensure that responses are not intended for another <see cref="Prompt"/>.</remarks>
        public readonly Guid PromptId;

        /// <summary>The <see cref="Control"/>s prompted for.</summary>
        public readonly ImmutableArray<Control> Controls;

        /// <summary>A number of flags which specify styling.  These are server/client implementation-defined.</summary>
        public readonly ImmutableHashSet<string> StylingFlags;

        /// <inheritdoc />
        public Prompt(Guid promptId, ImmutableArray<Control> controls, ImmutableHashSet<string> stylingFlags = null)
        {
            PromptId = promptId;
            Controls = controls;
            StylingFlags = stylingFlags == null ? ImmutableHashSet<string>.Empty : stylingFlags;
        }
    }
}
