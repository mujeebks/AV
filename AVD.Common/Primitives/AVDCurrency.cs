using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.DataAccessLayer.Models
{
    [Serializable]
    public struct AVDCurrency
    {
        public int CurrencyId { get; set; }

        public static bool operator ==(AVDCurrency m1, AVDCurrency m2)
        {
            return m1.CurrencyId == m2.CurrencyId;

        }

        public static bool operator !=(AVDCurrency m1, AVDCurrency m2)
        {
            return m1.CurrencyId != m2.CurrencyId;

        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string CurrencyCode { get; set; }

        public string Symbol { get; set; }

        private static List<AVDCurrency> Currencies;

        public static AVDCurrency Get(int currencyId)
        {
            /*if (currencyId == 0)
                currencyId = 1;*/
            return Currencies.Single(t => t.CurrencyId == currencyId);
        }
        internal static AVDCurrency Get(string isoCurrencyCode)
        {
            if (!string.IsNullOrEmpty(isoCurrencyCode))
                return Currencies.Single(t => t.CurrencyCode == isoCurrencyCode);
            else
                return new AVDCurrency();
        }
        internal static AVDCurrency Get(CurrencyTypes currencyType)
        {
            return Currencies.Single(t => t.CurrencyCode == currencyType.ToString());
        }
        public static void Init(List<AVDCurrency> currencyList)
        {
            Currencies = currencyList;
        }
    }
}
