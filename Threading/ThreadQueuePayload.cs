namespace Craiel.UnityEssentials.Threading
{
    using Contracts;

    public class ThreadQueuePayload : IThreadQueueOperationPayload
    {
        public object Data { get; set; }
    }
}
