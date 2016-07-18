using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.DataAccessLayer
{
    public class PagedList<T> where T : class
    {
        public int TotalResults { get; set; }
        public IEnumerable<T> Entities { get; set; }
    }
}
