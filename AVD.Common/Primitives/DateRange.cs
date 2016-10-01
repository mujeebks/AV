using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace AVD.Common.Primitives
{
    /// <summary>
    /// Represents a range of two dates. This will only take the date portion and will ignore any time element.
    /// </summary>
    public struct DateRange : IValidatableObject
    {
        private DateTime from;
        private DateTime to;
        public DateTime From { get { return from; } set { from = value.Date; } }
        public DateTime To { get { return to; } set { to = value.Date; } }

        public int Days
        {
            get { return (to.Date - from.Date).Days + 1; } //to make it inclusive, as of now for purposes found in quoting insurance
        }

        public DateRange(DateTime from, DateTime to)
        {
            this = new DateRange{ From = from, To = to};
        }

        public DateRange(DateTime from, int duration)
        {
            this = new DateRange { From = from, To = from.AddDays(duration) };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(Days < 0)
                yield return new ValidationResult("From date must be greater than to To date");
        }
    }
}
