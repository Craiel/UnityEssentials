namespace Assets.Scripts.Craiel.Essentials.Threading
{
    using Contracts;

    public class ThreadQueuePayload : IThreadQueueOperationPayload
    {
        public object Data { get; set; }
    }
}
