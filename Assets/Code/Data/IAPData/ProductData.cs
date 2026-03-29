namespace Code.Data
{
    [System.Serializable]
    public class ProductData
    {
        public int id;
        public string tag;
        public string name;
        public string description;
        public string icon;
        public string iconSmall;
        public int price;
        public string currency;
        public string currencySymbol;
        public bool isSubscription;
        public int period;
        public int trialPeriod;
    }
}
