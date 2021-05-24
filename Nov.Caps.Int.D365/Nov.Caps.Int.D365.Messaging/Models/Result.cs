namespace Nov.Caps.Int.D365.Messaging.Models
{
    public class Result
    {
        public static Result Ok(Message<Payload<dynamic>> message)
        {
            return new Result(message, null);
        }

        public static Result Err(Message<Failure> failure)
        {
            return new Result(null, failure);
        }

        public Message<Payload<dynamic>> Response { get; }

        public Message<Failure> Failure { get; }

        public bool IsOk
        {
            get { return this.Failure == null; }
        }

        private Result(Message<Payload<dynamic>> message, Message<Failure> failure)
        {
            this.Response = message;
            this.Failure = failure;
        }
    }
}
