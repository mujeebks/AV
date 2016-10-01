using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.Common.Primitives.TypeConverters
{
    public class AVDStringTypeConverter : TypeConverter
    {
        private Type type;

        public AVDStringTypeConverter(Type type)
        {
                this.type = type;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == type)
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

            var ins = (AVDStringBase)Activator.CreateInstance(type);

            ins.Value = value.ToString();

            return ins;
        }
    }
}
