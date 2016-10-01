using System;
using AVD.Common.Attributes;

namespace AVD.Common.Primitives
{
    /// <summary>
    /// Used for a password which should never be written to logs.
    /// </summary>
    [DoNotLogApiContent]
    public class AVDPassword : AVDStringBase
    {
        //public String Value { get; set; }
        public static implicit operator AVDPassword(string key)
        {
            if (key == null)
                return null;

            return new AVDPassword
            {
                Value = key
            };
        }

        public static implicit operator String(AVDPassword password)
        {
            return password?.Value;
        }

        public override string ToString()
        {
            // do not change
            return Value;
        }
    }
}
