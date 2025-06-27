
namespace CurrencyExchangeRates.Domain.Entities
{
    public class CurrencyRate
    {
        public int Id { get; set; }
        public string CurrencyCode { get; set; } = null!;
        public decimal Rate { get; set; }
        public DateTime Date { get; set; }
    }
}
