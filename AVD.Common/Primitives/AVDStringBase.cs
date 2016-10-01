using System;
using System.Collections.Generic;
using System.ComponentModel;
using AVD.Common.Primitives.TypeConverters;

namespace AVD.Common.Primitives
{
    /// <summary>
    /// Base class for a primitive type string
    /// </summary>
    [TypeConverter(typeof(AVDStringTypeConverter))]
    public class AVDStringBase
    {
        public string Value { get; set; }

    }
}
