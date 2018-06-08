namespace Craiel.UnityEssentials.Runtime.Msg
{
    /// <summary>
    /// The <see cref="MessageManager"/> is a singleton <see cref="MessageDispatcher"/> in charge of the creation, dispatch, and management of telegrams
    /// </summary>
    public class MessageManager : MessageDispatcher
    {
        public static readonly MessageManager Instance = new MessageManager();

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        // Avoid instantiation outside of this class
        private MessageManager()
        {
        }
    }
}
