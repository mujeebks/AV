using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AVD.Common.Primitives;
using AVD.DataAccessLayer.Models;

namespace System.Linq
{
    public static class Extensions
    {

        /// <summary>
        /// This will take in two lists and join them in pairs based on the keyselector.
        /// </summary>
        public static AVDMoney Sum(this IEnumerable<AVDMoney> listA)
        {
            if (!listA.Any())
                return new AVDMoney(0, CurrencyTypes.USD);
            //throw new InvalidOperationException();

            return listA.Aggregate((a, b) => a + b);
        }


        /// <summary>
        /// This will take in two lists and join them in pairs based on the keyselector.
        /// </summary>
        public static IEnumerable<AVDMoney> SumByCurrency(this IEnumerable<AVDMoney> listA)
        {
            listA = listA.Where(tt => tt != null);

            if (!listA.Any())
                return new List<AVDMoney> { new AVDMoney(0,CurrencyTypes.USD)};
            //throw new InvalidOperationException();

            List<AVDMoney> t = new List<AVDMoney>();
            foreach (var g in listA.GroupBy(y => y.CurrencyCode))
            {
                t.Add(g.Aggregate((a, b) => a + b));
            }
            return t;
        }

        /// <summary>
        /// This will take in two lists and join them in pairs based on the keyselector.
        /// </summary>
        public static IEnumerable<AVDMoney> SubtractByCurrency(this IEnumerable<AVDMoney> listA)
        {
            if (!listA.Any())
                return new List<AVDMoney> { new AVDMoney(0, CurrencyTypes.USD) };
            //throw new InvalidOperationException();

            List<AVDMoney> t = new List<AVDMoney>();
            foreach (var g in listA.GroupBy(y => y.CurrencyCode))
            {
                t.Add(g.Aggregate((a, b) => a - b));
            }
            return t;
        }


        /// <summary>
        /// This will take in two lists and join them in pairs based on the keyselector.
        /// </summary>
        public static IEnumerable<AVDMoney> SumByCurrency(this IEnumerable<IEnumerable<AVDMoney>> listA)
        {
            if (!listA.Any())
                return new List<AVDMoney>();

            //Get all the available currency type's
            List<AVDMoney> source = new List<AVDMoney>();
            foreach (var currencyType in listA)
            {
                foreach (var currency in currencyType)
                {
                    source.Add(currency);
                }
            }

            //Sum the currency by grouping it
            List<AVDMoney> t = new List<AVDMoney>();
            foreach (var g in source.GroupBy(y => y.CurrencyCode))
            {
                t.Add(g.Aggregate((a, b) => a + b));
            }
            return t;
        }


        //
        // Summary:
        //     Computes the sum of the sequence of nullable System.Decimal values that are
        //     obtained by invoking a transform function on each element of the input sequence.
        //
        // Parameters:
        //   source:
        //     A sequence of values that are used to calculate a sum.
        //
        //   selector:
        //     A transform function to apply to each element.
        //
        // Type parameters:
        //   TSource:
        //     The type of the elements of source.
        //
        // Returns:
        //     The sum of the projected values.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     source or selector is null.
        //
        //   System.OverflowException:
        //     The sum is larger than System.Decimal.MaxValue.
        public static AVDMoney Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, AVDMoney> selector)
        {
            return Sum(source.Select(selector));
        }

        /// <summary>
        /// To get description text from Enum
        /// </summary>
        /// <param name="element"></param>
        /// <returns>description string</returns>
        public static string GetDescription(this Enum element)
        {
            Type type = element.GetType();

            MemberInfo[] memberInfo = type.GetMember(element.ToString());

            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0)
                {
                    return ((DescriptionAttribute)attributes[0]).Description;
                }
            }

            return element.ToString();
        }
    }
}
