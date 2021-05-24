namespace Nov.Caps.Int.D365.Models.Common
{
    public class Currency
    {
        public string Code { get; private set; }

        public double Rate { get; private set; }

        public Currency(string code, double rate)
        {
            this.Code = code;
            this.Rate = rate;
        }
    }
}
