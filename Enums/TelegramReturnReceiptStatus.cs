namespace Craiel.UnityEssentials.Enums
{
    public enum TelegramReturnReceiptStatus
    {
        /// <summary>
        /// Indicates that the sender doesn't need any return receipt
        /// </summary>
        Unneeded = 0,

        /// <summary>
        /// Indicates that the sender needs the return receipt
        /// </summary>
        Needed = 1,

        /// <summary>
        /// Indicates that the return receipt has been sent back to the original sender of the telegram
        /// </summary>
        Sent = 2
    }
}
