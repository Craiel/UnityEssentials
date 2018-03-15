namespace Craiel.UnityEssentials.Contracts
{
    using Enums;

    /// <summary>
    /// A <see cref="IPendingMessageCallback"/> is used by the <see cref="Craiel.UnityEssentials.Msg.MessageDispatcher.ScanQueue"/> method
    ///  of the <see cref="Craiel.UnityEssentials.Msg.MessageDispatcher"/> to report its pending messages individually.
    /// </summary>
    public interface IPendingMessageCallback
    {
        /// <summary>
        /// Reports a pending message
        /// </summary>
        /// <param name="delay">The remaining delay in seconds</param>
        /// <param name="sender">The message sender</param>
        /// <param name="receiver">The message receiver</param>
        /// <param name="message">The message code</param>
        /// <param name="extraInfo">Any additional information that may accompany the message</param>
        /// <param name="returnReceiptStatus">The return receipt status of the message</param>
        void Report(float delay, ITelegraph sender, ITelegraph receiver, int message, object extraInfo, TelegramReturnReceiptStatus returnReceiptStatus);
    }
}
