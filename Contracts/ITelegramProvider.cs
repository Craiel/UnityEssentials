namespace Craiel.UnityEssentials.Contracts
{
    /// <summary>
    /// Telegram providers respond to {@link MessageDispatcher#addListener} by providing optional <see cref="Craiel.UnityEssentials.Msg.Telegram.ExtraInfo"/> 
    /// to be sent in a Telegram of a given type to the newly registered <see cref="ITelegraph"/>.
    /// </summary>
    public interface ITelegramProvider
    {
        /// <summary>
        /// Provides <see cref="Craiel.UnityEssentials.Msg.Telegram.ExtraInfo"/> to dispatch immediately when a <see cref="ITelegraph"/> is registered for the given message type
        /// </summary>
        /// <param name="message">the message type to provide</param>
        /// <param name="receiver">the newly registered Telegraph. Providers can provide different info depending on the targeted Telegraph</param>
        /// <returns>extra info to dispatch in a Telegram or null if nothing to dispatch</returns>
        object ProvideMessageInfo(int message, ITelegraph receiver);
    }
}
