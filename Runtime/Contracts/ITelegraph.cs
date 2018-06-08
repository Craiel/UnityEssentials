namespace Craiel.UnityEssentials.Runtime.Contracts
{
    using Msg;

    /// <summary>
    /// Any object implementing the <see cref="ITelegraph"/> interface can act as the sender or the receiver of a <see cref="Telegram"/>
    /// </summary>
    public interface ITelegraph
    {
        /// <summary>
        /// Handles the telegram just received
        /// </summary>
        /// <param name="message">The telegram</param>
        /// <returns><code>true</code> if the telegram has been successfully handled; <code>false</code> otherwise</returns>
        bool HandleMessage(Telegram message);
    }
}
