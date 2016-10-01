using System;
using System.Linq;

namespace AVD.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
    public class DoNotLogApiContentAttribute : Attribute
    {

    }
}