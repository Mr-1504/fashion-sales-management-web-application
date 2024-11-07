namespace BTL_LTWeb.Services
{
    public class Datum
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string bin { get; set; }
        public string shortName { get; set; }
        public string logo { get; set; }
        public int transferSupported { get; set; }
        public int lookupSupported { get; set; }
        public string short_name { get; set; }
        public int support { get; set; }
        public int isTransfer { get; set; }
        public string swift_code { get; set; }
    }

    public class Bank
    {
        public string code { get; set; }
        public string desc { get; set; }
        public IList<Datum> data { get; set; }
    }
}
