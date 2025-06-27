using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.EcbGateway.Models
{
    public class CurrencyRate
    {
        public string Currency { get; set; }
        public decimal Rate { get; set; }
        public DateTime Date { get; set; }
    }
}
