using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.Common.Caching
{
    public interface ICachedItemKey
    {
        string Token { get; set; }
    }

    /// <summary>
    /// This represents a key/token for a cached item.
    /// </summary>
    /// <typeparam name="T">The type that was cached</typeparam>
    [TypeConverter(typeof(CachedItemKeyTypeConverter))]
    public struct CachedItemKey<T> : ICachedItemKey
    {
        [Required]
        public string Token { get; set; }
         
        public CachedItemKey(string token)
        {
            Token = token;
        } 

        public static implicit operator CachedItemKey<T>(string key)  
        {
            if (key == null)
                return null;

            return new CachedItemKey<T>
            {
                Token = key
            };
        }

        public static implicit operator String(CachedItemKey<T> cachedItemToken)  
        {
            return cachedItemToken.Token;
        }

        public override string ToString()
        {
            // do not change
            return Token;
        }
    }
}
