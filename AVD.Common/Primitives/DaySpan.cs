using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.Common.Primitives
{
    /// <summary>
    /// Represents a range of two days
    /// </summary>
    public struct DaySpan
    {
        public int From { get; set; }
        public int To { get; set; }

        public DaySpan(int from, int to)
        {
            this = new DaySpan
            {
                From = from, To = to
            };
        }
    }
}
