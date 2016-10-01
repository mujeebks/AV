using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVD.DataAccessLayer;
using AVD.DataAccessLayer.Models;
using AVD.DataAccessLayer.Repositories;

namespace AVD.Core
{
    public class CurrencyConfiguration
    {
        public static bool Initialized { get; private set; }

        static CurrencyConfiguration()
        {
            Initialized = false;
        }

        public static void Configure()
        {
            if (Initialized)
                return;
            
            Repository<Currency> currencyRepo = RepositoryFactory.Get<Currency>();
            List<AVDCurrency> currencyList = new List<AVDCurrency>();
            foreach (var currency in currencyRepo.GetAll()) 
            {
                currencyList.Add(new AVDCurrency 
                {
                    CurrencyCode = currency.Code,
                    CurrencyId = currency.CurrencyId,
                    Symbol = currency.Symbol
                });
            }
            AVDCurrency.Init(currencyList);
        }
    }
}
