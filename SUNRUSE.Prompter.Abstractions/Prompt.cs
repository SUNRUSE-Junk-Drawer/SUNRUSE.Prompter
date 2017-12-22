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

        /// <summary><see cref="string"/>s describing (usually paths to) images to display as background layers.</summary>
        /// <remarks>Each is drawn after those before it, so the first is "bottom" and the last is "top" if visualized as layers.</remarks>
        public readonly ImmutableArray<string> BackgroundLayers;

        /// <inheritdoc />
        public Prompt(Guid promptId, ImmutableArray<Control> controls, ImmutableArray<string> backgroundLayers = default(ImmutableArray<string>))
        {
            PromptId = promptId;
            Controls = controls;
            BackgroundLayers = backgroundLayers.IsDefault ? ImmutableArray<string>.Empty : backgroundLayers;
        }
    }
}
