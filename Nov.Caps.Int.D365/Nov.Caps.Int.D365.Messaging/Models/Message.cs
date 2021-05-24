using System;
namespace Nov.Caps.Int.D365.Messaging.Models
{
    public class Message<T>
    {
        public string Id { get; private set; }

        public string CorrelationId { get; private set; }

        public T Body { get; private set; }

        public string ReplyTo { get; private set; }

        public Message(string id, string correlationId, T body) : this(id, correlationId, body, String.Empty) { }

        public Message(string id, string correlationId, T body, string replyTo)
        {
            this.Id = id;
            this.CorrelationId = correlationId;
            this.Body = body;
            this.ReplyTo = replyTo;
        }
    }
}
