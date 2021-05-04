namespace Craiel.UnityEssentials.Runtime.Threading
{
    using Contracts;

    public class ThreadQueuePayload : IThreadQueueOperationPayload
    {
        public object Data { get; set; }
    }
}
