﻿namespace Craiel.UnityEssentials.Contracts
{
    using System;

    public interface IThreadQueueOperation
    {
        bool Suceeded { get; set; }

        Func<IThreadQueueOperationPayload, bool> Action { get; }

        IThreadQueueOperationPayload Payload { get; set; }

        long QueueTime { get; }
        long ExecutionTime { get; set; }
    }
}
