using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Domain.Models
{
    public class CurrencyRate
    {
        public int Id { get; set; }
        public string CurrencyCode { get; set; } = null!;
        public decimal Rate { get; set; }
        public DateTime Date { get; set; }
    }
}
