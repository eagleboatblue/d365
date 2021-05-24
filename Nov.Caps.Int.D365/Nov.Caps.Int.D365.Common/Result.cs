using System;
namespace Nov.Caps.Int.D365.Common
{
    public class Result<TData>
    {
        public TData Data { get; }

        public Exception Exception { get; }

        public bool IsOk
        {
            get
            {
                return this.Exception == null;
            }
        }

        public Result(TData data)
        {
            this.Data = data;
        }

        public Result(Exception exception)
        {
            this.Exception = exception;
        }
    }
}
