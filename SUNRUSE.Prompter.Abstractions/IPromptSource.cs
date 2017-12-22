using System;
using System.Threading.Tasks;

namespace SUNRUSE.Prompter.Abstractions
{
    /// <summary>A source of <see cref="Prompt"/>s.</summary>
    public interface IPromptSource
    {
        /// <summary>Call when the user selects a <see cref="Control"/> respresenting a button.</summary>
        /// <param name="buttonId">The <see cref="Control.ButtonId"/> of the pressed <see cref="Control"/>.</param>
        /// <param name="promptId">The <see cref="Prompt.PromptId"/> of the <see cref="Prompt"/> the user selected from.</param>
        /// <param name="sessionId">The <see cref="Guid"/> of the user's session ID.</param>
        Task Select(string buttonId, Guid promptId, Guid sessionId);

        /// <summary>Requests the <see cref="Prompt"/> the user should be shown.</summary>
        /// <param name="sessionId">The <see cref="Guid"/> of the user's session ID.  Generate one if none is known (see ISessionGenerator in the Hosting project).</param>
        /// <returns>The <see cref="Prompt"/> to show to the user.</returns>
        Task<Prompt> Refresh(Guid sessionId);
    }
}
