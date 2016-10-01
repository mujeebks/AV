using System;
using System.ComponentModel;
using System.Globalization;

namespace AVD.Common.Caching
{
    /// <summary>
    /// This class converts from a string into a strongly typed cache key.
    /// </summary>
    public class CachedItemKeyTypeConverter : TypeConverter
    { 
        private Type keyType;

        public CachedItemKeyTypeConverter(Type type)
        {
            if (type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(CachedItemKey<>)
                && type.GetGenericArguments().Length == 1)
            {
                keyType = type.GetGenericArguments()[0];
            }
            else
            {
                throw new ArgumentException("Incompatible type", "type");
            }
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value == null)
                return null;
               
            var ins = Activator.CreateInstance(typeof(CachedItemKey<>).MakeGenericType(keyType));
            
            ((ICachedItemKey)ins).Token = value.ToString();

            return ins;
            //return base.ConvertFrom(context, culture, value);
        }
    }
}