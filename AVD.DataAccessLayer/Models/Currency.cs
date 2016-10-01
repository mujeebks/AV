using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AVD.DataAccessLayer;

namespace AVD.DataAccessLayer.Models
{
    public partial class Currency : BaseModel
    {
        public Currency()
        {
           
        }

        [Key]
        public int CurrencyId { get; set; }
        public string Code { get; set; }
        public string CurrencyName { get; set; }
        public string Symbol { get; set; }
       
    }
}
