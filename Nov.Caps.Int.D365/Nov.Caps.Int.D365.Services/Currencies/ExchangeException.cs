using System;
using System.Collections.Generic;

namespace Nov.Caps.Int.D365.Services.Currencies
{
    public class ExchangeException : Exception
    {
        public Dictionary<string, Exception> Errors { get; }

        public ExchangeException(Dictionary<string, Exception> errors) :
            base($"Failed to update rates for the following currency codes: {string.Join(",", errors.Keys)}")
        {
            this.Errors = errors;
        }
    }
}
