namespace Craiel.UnityEssentials.Runtime.Msg
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Contracts;
    using Enums;

    /// <summary>
    /// A Telegram is the container of a message. The <see cref="MessageDispatcher"/> manages telegram life-cycle.
    /// </summary>
    public class Telegram : IComparable, IComparable<Telegram>, IEquatable<Telegram>, IPoolable
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        /// <summary>
        /// Creates an empty {@code Telegram}
        /// </summary>
        public Telegram()
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        /// <summary>
        /// The agent that sent this telegram
        /// </summary>
        public ITelegraph Sender { get; set; }

        /// <summary>
        /// The agent that is to receive this telegram
        /// </summary>
        public ITelegraph Receiver { get; set; }

        /// <summary>
        /// The message type
        /// </summary>
        public int Message { get; set; }

        /// <summary>
        /// Messages can be dispatched immediately or delayed for a specified amount of time. 
        /// If a delay is necessary, this field is stamped with the time the message should be dispatched
        /// </summary>
        public float Timestamp { get; set; }

        /// <summary>
        /// The return receipt status of this telegram
        /// </summary>
        public TelegramReturnReceiptStatus ReturnReceiptStatus { get; set; }

        /// <summary>
        /// Any additional information that may accompany the message
        /// </summary>
        public object ExtraInfo { get; set; }

        public void Reset()
        {
            this.Sender = null;
            this.Receiver = null;
            this.Message = 0;
            this.ReturnReceiptStatus = TelegramReturnReceiptStatus.Unneeded;
            this.ExtraInfo = null;
            this.Timestamp = 0;
        }

        public int CompareTo(Telegram other)
        {
            if (this.Equals(other))
            {
                return 0;
            }

            return this.Timestamp - other.Timestamp < 0 ? -1 : 1;
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public bool Equals(Telegram other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(this.Sender, other.Sender) && Equals(this.Receiver, other.Receiver) && this.Message == other.Message && this.Timestamp.Equals(other.Timestamp) && this.ReturnReceiptStatus == other.ReturnReceiptStatus && Equals(this.ExtraInfo, other.ExtraInfo);
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((Telegram)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Sender != null ? this.Sender.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (this.Receiver != null ? this.Receiver.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.Message;
                hashCode = (hashCode * 397) ^ this.Timestamp.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)this.ReturnReceiptStatus;
                hashCode = (hashCode * 397) ^ (this.ExtraInfo != null ? this.ExtraInfo.GetHashCode() : 0);
                return hashCode;
            }
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo((Telegram)obj);
        }
    }
}
