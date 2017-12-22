namespace SUNRUSE.Prompter.Abstractions
{
    /// <summary>A <see cref="Control"/> shown on a <see cref="Prompt"/>.</summary>
    public sealed class Control
    {
        /// <summary>When <see langword="this"/> represents a button, a <see cref="string"/> identifying it, else, <see langword="null"/>.</summary>
        public readonly string ButtonId;

        /// <summary>Describes (usually a path to) an icon to display on the left, or <see langword="null"/> if none.</summary>
        public readonly string LeftIcon;

        /// <summary>Text to display on the left, or <see langword="null"/> if none.  Displayed right of the <see cref="LeftIcon"/> if present.</summary>
        public readonly string LeftText;

        /// <summary>Text to display in the middle horizontally, or <see langword="null"/> if none.</summary>
        public readonly string MiddleText;

        /// <summary>Text to display on the right, or <see langword="null"/> if none.  Displayed left of the <see cref="RightIcon"/> if present.</summary>
        public readonly string RightText;

        /// <summary>Describes (usually a path to) an icon to display on the right, or <see langword="null"/> if none.</summary>
        public readonly string RightIcon;

        /// <summary><see langword="true"/> when <see langword="this"/> respresents the first <see cref="Control"/> in some kind of grouping.</summary>
        public readonly bool IsFirstInGroup;

        /// <summary><see langword="true"/> when <see langword="this"/> respresents the last <see cref="Control"/> in some kind of grouping.</summary>
        public readonly bool IsLastInGroup;

        /// <inheritdoc />
        public Control
        (
            string buttonId = null,
            string leftIcon = null, string leftText = null, string middleText = null, string rightText = null, string rightIcon = null,
            bool isFirstInGroup = false, bool isLastInGroup = false
        )
        {
            ButtonId = buttonId;
            LeftIcon = leftIcon;
            LeftText = leftText;
            MiddleText = middleText;
            RightText = rightText;
            RightIcon = rightIcon;
            IsFirstInGroup = isFirstInGroup;
            IsLastInGroup = isLastInGroup;
        }
    }
}
