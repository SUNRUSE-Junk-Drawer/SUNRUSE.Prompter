// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.Immutable;

namespace SUNRUSE.Prompter.Abstractions
{
    /// <summary>A <see cref="Control"/> shown on a <see cref="Prompt"/>.</summary>
    public sealed class Control
    {
        /// <summary>When <see langword="this"/> represents a button, a <see cref="string"/> identifying it, else, <see langword="null"/>.</summary>
        public readonly string ButtonId;

        /// <summary>Text to display on the left, or <see langword="null"/> if none.</summary>
        public readonly string LeftText;

        /// <summary>Text to display in the middle horizontally, or <see langword="null"/> if none.</summary>
        public readonly string MiddleText;

        /// <summary>Text to display on the right, or <see langword="null"/> if none.</summary>
        public readonly string RightText;

        /// <summary>A number of flags which specify styling.  These are server/client implementation-defined.</summary>
        public readonly ImmutableHashSet<string> StylingFlags;

        /// <inheritdoc />
        public Control
        (
            string buttonId = null,
            string leftText = null, string middleText = null, string rightText = null,
            ImmutableHashSet<string> stylingFlags = null
        )
        {
            ButtonId = buttonId;
            LeftText = leftText;
            MiddleText = middleText;
            RightText = rightText;
            StylingFlags = stylingFlags ?? ImmutableHashSet<string>.Empty;
        }
    }
}
