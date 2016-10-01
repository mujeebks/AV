using System;
using AVD.DataAccessLayer.Models;
using Newtonsoft.Json;

namespace AVD.Common.Primitives
{
    /// <summary>
    /// A decimal/currency type that is rounded to 4 decimal places.
    /// </summary>
    /// <remarks>This used to be stored as doubles and now is stored as decimals.</remarks>
    [Serializable]
    public class AVDMoney : IEquatable<AVDMoney>
    {
        public enum AVDMoneyDisplayType
        {
            None,
            CurrencyString,
            SymbolOnly
        }
        [JsonIgnore]
        public AVDCurrency Currency { get; set; }

        public decimal Amount { get; set; }

      
        public String CurrencyCode
        {
            get
            {
                return Currency.CurrencyCode;
            }
            set
            {
                Currency = AVDCurrency.Get(value);
            }
        }
        
        public String Symbol
        {
            get
            {
                return Currency.Symbol;
            }
        }

        public AVDMoney()
        {
            Amount = 0m;
            Currency = new AVDCurrency
            {
                CurrencyId = 0,
                CurrencyCode = "NA",
                Symbol = ""
            };
        }

        /// <summary>
        /// Represents AVDMoney with the given currency and amount of 0
        /// </summary>
        /// <param name="currency"></param>
        public AVDMoney(AVDCurrency currency)
            : this(0m, currency)
        {
        }

        /// <summary>
        /// Represents an amount of AVDMoney in a specific currency.
        /// </summary>
        public AVDMoney(decimal amount, AVDCurrency currency)
            : this()
        {
            Currency = currency;
            Amount = amount;
        }

        /// <summary>
        /// Represents an amount of AVDMoney in a specific currency.
        /// </summary>
        public AVDMoney(decimal amount, int currencyId)
            : this(amount, AVDCurrency.Get(currencyId))
        {
        }

        /// <summary>
        /// Represents an amount of AVDMoney in a specific currency.
        /// </summary>
        public AVDMoney(decimal amount, String isoCurrencyCode)
            : this(amount, AVDCurrency.Get(isoCurrencyCode))
        { }


        /// <summary>
        /// Represents an amount of AVDMoney in a specific currency.
        /// </summary>
        public AVDMoney(decimal amount, CurrencyTypes currencyType)
            : this(amount, AVDCurrency.Get(currencyType))
        { }


        /// <summary>
        /// Represents an amount of AVDMoney in a specific currency.
        /// </summary>
        public AVDMoney(string amount, String isoCurrencyCode)
            : this(decimal.Parse(amount), AVDCurrency.Get(isoCurrencyCode))
        { }


        /// <summary>
        /// Represents an amount of AVDMoney in a specific currency.
        /// </summary>
        public AVDMoney(CurrencyTypes currencyType)
            : this(0m, AVDCurrency.Get(currencyType))
        { }

        #region Standard operator overloading

        public static AVDMoney operator -(AVDMoney m1, AVDMoney m2)
        {
            // Allow operations on NULLs for convenience
            if (m1 == null)
                return m2 * -1;
            if (m2 == null)
                return m1;

            CheckCurrencies(m1, m2);
            return new AVDMoney(m1.Amount - m2.Amount, m1.Currency);
        }

        public static AVDMoney operator -(AVDMoney m1, decimal d)
        {
            // Allow operations on NULLs for convenience
            if (m1 == null)
                throw new ArgumentNullException("m1", @"AVDMoney can not be null");

            return new AVDMoney(m1.Amount - d, m1.Currency);
        }

        public static AVDMoney operator +(AVDMoney m1, AVDMoney m2)
        {
            if (m1 == null)
                return m2;
            if (m2 == null)
                return m1;

            CheckCurrencies(m1, m2);
            return new AVDMoney(m1.Amount + m2.Amount, m1.Currency);
        }


        public static AVDMoney operator +(AVDMoney m1, decimal d)
        {
            if (m1 == null)
                throw new ArgumentNullException("m1", @"AVDMoney can not be null");

            return new AVDMoney(m1.Amount + d, m1.Currency);
        }

        public static AVDMoney operator *(AVDMoney m1, decimal d)
        {
            if (m1 == null)
                throw new ArgumentNullException("m1", @"AVDMoney can not be null");

            return new AVDMoney(m1.Amount * d, m1.Currency);
        }

        // e.g. multiplying by number of pax
        public static AVDMoney operator *(AVDMoney m1, int d)
        {
            if (m1 == null)
                throw new ArgumentNullException("m1", @"AVDMoney can not be null");

            return new AVDMoney(m1.Amount * d, m1.Currency);
        }

        public static AVDMoney operator *(int d, AVDMoney m1)
        {
            if (m1 == null)
                throw new ArgumentNullException("m1", @"AVDMoney can not be null");

            return new AVDMoney(m1.Amount * d, m1.Currency);
        }

        public static AVDMoney operator /(AVDMoney m1, decimal d)
        {
            if (m1 == null)
                throw new ArgumentNullException("m1", @"AVDMoney can not be null");

            return new AVDMoney(m1.Amount / d, m1.Currency);
        }

        public static decimal operator /(AVDMoney m1, AVDMoney m2)
        {
            if (m1 == null || m2 == null)
                throw new ApplicationException("Can only divide between two non-null money amounts");

            CheckCurrencies(m1, m2);
            return m1.Amount / m2.Amount;
        }

        public static bool operator ==(AVDMoney m1, AVDMoney m2)
        {
            if (Object.ReferenceEquals(m1, m2))
                return true;

            if (Object.ReferenceEquals(m1, null) || Object.ReferenceEquals(m2, null))
                return false;

            return (m1.Currency == m2.Currency && m1.Amount == m2.Amount);

        }

        public static bool operator !=(AVDMoney m1, AVDMoney m2)
        {
            return !(m1 == m2);
        }

        public static bool operator >(AVDMoney m1, AVDMoney m2)
        {
            CheckCurrencies(m1, m2);
            return m1.Amount > m2.Amount;
        }

        public static bool operator <(AVDMoney m1, AVDMoney m2)
        {
            CheckCurrencies(m1, m2);
            return m1.Amount < m2.Amount;
        }

        public static bool operator >=(AVDMoney m1, AVDMoney m2)
        {
            CheckCurrencies(m1, m2);
            return m1.Amount >= m2.Amount;
        }

        public static bool operator <=(AVDMoney m1, AVDMoney m2)
        {
            CheckCurrencies(m1, m2);

            return m1.Amount <= m2.Amount;
        }

        public static bool operator >(AVDMoney m1, decimal m2)
        {
            return m1.Amount > m2;
        }

        public static bool operator <(AVDMoney m1, decimal m2)
        {
            return m1.Amount < m2;
        }

        public static bool operator >=(AVDMoney m1, decimal m2)
        {
            return m1.Amount >= m2;
        }

        public static bool operator <=(AVDMoney m1, decimal m2)
        {
            if (m1 == null)
                return true;

            return m1.Amount <= m2;
        }

        public static bool operator ==(AVDMoney m1, decimal m2)
        {
            if (ReferenceEquals(m1, null))
                return false;
            return m1.Amount == m2;
        }

        public static bool operator !=(AVDMoney m1, decimal m2)
        {
            if (ReferenceEquals(m1, null))
                return true;

            return m1.Amount != m2;
        }

        public static bool operator ==(decimal m2, AVDMoney m1)
        {
            if (ReferenceEquals(m1, null))
                return false;
            return m1.Amount == m2;
        }
        public static bool operator !=(decimal m2, AVDMoney m1)
        {
            if (ReferenceEquals(m1, null))
                return true;

            return m1.Amount != m2;
        }

        public static AVDMoney operator -(AVDMoney m1)
        {
            return m1 * -1;
        }

        public override bool Equals(object obj)
        {
            if (null == obj)
                return false;

            if (obj.GetType() != typeof(AVDMoney))
                return false;

            return this == (AVDMoney)obj;
        }

        public override int GetHashCode()
        {
            return Amount.GetHashCode() ^ Currency.GetHashCode();
        }

        #endregion

        private static void CheckCurrencies(AVDMoney m1, AVDMoney m2)
        {
            //check if the amount of both objects is more than zero
            if (m1 != null && m2 != null)
            {
                if (m1.Amount != 0 && m2.Amount != 0)
                {
                    if (!(m1.Currency == m2.Currency))
                        throw new InvalidOperationException(
                            String.Format("Cannot perform an operation on two different currencies ({0} and {1})", m1, m2));
                }
            }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(AVDMoney other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this == other;
        }

        public AVDMoney Ceiling()
        {
            return new AVDMoney(Math.Ceiling(this.Amount), this.Currency);
        }

        public AVDMoney Floor()
        {
            return new AVDMoney(Math.Floor(this.Amount), this.Currency);
        }

        public AVDMoney Round()
        {
            return new AVDMoney(Math.Round(this.Amount), this.Currency);
        }

        public AVDMoney Round(int decimals)
        {
            return new AVDMoney(Math.Round(this.Amount, decimals), this.Currency);
        }

        public static AVDMoney Min(AVDMoney m1, AVDMoney m2)
        {
            CheckCurrencies(m1, m2);

            if (m1.Amount < m2.Amount)
                return (AVDMoney)m1.MemberwiseClone();
            else
                return (AVDMoney)m2.MemberwiseClone();
        }

        public static AVDMoney Max(AVDMoney m1, AVDMoney m2)
        {
            CheckCurrencies(m1, m2);

            if (m1.Amount > m2.Amount)
                return (AVDMoney)m1.MemberwiseClone();
            else
                return (AVDMoney)m2.MemberwiseClone();
        }

        /// <summary>
        /// Will subtract b from a. If a or b is null, that value will be considered a 0.
        /// If both are NULL, NULL will be returned.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="nullStateMustBeSame">If true, it will throw an exception if one of a or b is NULL (but not both)</param>
        /// <exception cref="ApplicationException"></exception>
        /// <returns></returns>
        public static AVDMoney SubtractNull(AVDMoney a, AVDMoney b, bool nullStateMustBeSame = false)
        {
            if (nullStateMustBeSame)
            {
                if (a == null && b != null || a != null && b == null)
                    throw new ApplicationException("A or B is NULL - both must be either not null or null");
            }

            if (a == null)
                return b * -1;

            if (b == null)
                return a;

            return a - b;
        }

        /// <summary>
        /// Will add a to b. If a or b is null, that value will be considered a 0.
        /// If both are NULL, NULL will be returned.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="nullStateMustBeSame">If true, it will throw an exception if one of a or b is NULL (but not both)</param>
        /// <returns></returns>
        public static AVDMoney AddNull(AVDMoney a, AVDMoney b, bool nullStateMustBeSame = false)
        {
            if (nullStateMustBeSame)
            {
                if (a == null && b != null || a != null && b == null)
                    throw new ApplicationException("A or B is NULL - both must be either not null or null");
            }

            if (a == null)
                return b;

            if (b == null)
                return a;

            return a + b;
        }

        #region IComparable Members

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: 
        ///                     Value 
        ///                     Meaning 
        ///                     Less than zero 
        ///                     This object is less than the <paramref name="other"/> parameter.
        ///                     Zero 
        ///                     This object is equal to <paramref name="other"/>. 
        ///                     Greater than zero 
        ///                     This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.
        ///                 </param>
        public int CompareTo(AVDMoney other)
        {
            if (other == null)
            {
                return -1;
            }

            return Amount.CompareTo((other).Amount);
        }

        #endregion

        public static bool IsNullOrZero(AVDMoney AVDMoney)
        {
            return AVDMoney == null || AVDMoney == 0m;
        }

        /// <summary>
        /// Returns the absolute value of a <see cref="AVDMoney"/>.
        /// </summary>
        /// <returns></returns>
        public AVDMoney Abs()
        {
            return new AVDMoney(Math.Abs(Amount), Currency);
        }

        /// <summary>
        /// Returns the AVDMoney object as String.
        /// </summary>
        /// <returns>ex format: USD $500.00 </returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", this.CurrencyCode, this.Amount.ToString("c"));
        }

        /// <summary>
        /// Returns the AVDMoney object as String with a specific string format.
        /// </summary>
        /// <returns>ex format: USD $500.00 </returns>
        public string ToString(CurrencyFormat format)
        {
            string currencyFormat = string.Empty;

            if (format == CurrencyFormat.ClientItinerary)
            {
                if (this.CurrencyCode == CurrencyTypes.USD.ToString())
                    currencyFormat = string.Format("{0} {1}", this.CurrencyCode, this.Amount.ToString("c"));
                else
                    currencyFormat = string.Format("{0} {1}", this.CurrencyCode, this.Amount.ToString("c").Replace('$', ' '));
            }

            return currencyFormat;
        }
    }
}

