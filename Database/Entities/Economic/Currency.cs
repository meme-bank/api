using OctopusAPI.Database.Entities.Core;

namespace OctopusAPI.Database.Entities.Economic
{
    public class Currency
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Photo MonochromePhoto { get; set; } // symbol
        public decimal ExchangeRate { get; set; } // against Leuro or another base currency (BC - Base Currency, AC - Alternative Currency, 1AC = 100BC => 0.01, 100AC = 1BC => 1.0)
        public int[] CountryIds { get; set; } // countries that uses this currency
        public int EmmissionCountryId { get; set; } // country that emits this currency
        public DateTime CreatedAt { get; set; }
    }
}