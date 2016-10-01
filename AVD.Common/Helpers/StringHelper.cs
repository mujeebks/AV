using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using AVD.Common.Caching;
using AVD.Common.Logging;

namespace AVD.Common.Helpers
{
    public static class StringHelper
    {
        public static Stream GenerateStreamFromString(string input)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(input);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
         
        private static ICachingManager cachingManager;

        static StringHelper()
        {
            cachingManager = new CachingManager();
        }

        /// <summary>
        /// This will recursively go through each property in the given object and trim all strings
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void TrimStringProperties(object value)
        {
            GraphTraversalHelper o = new GraphTraversalHelper("TrimStringProperties", DoesObjectContainAPropertyWithAString, TrimAllStringProperties);

            o.DoWork(value);
        }

        /// <summary>
        /// We want to perform the condition on strings only.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool DoesObjectContainAPropertyWithAString(Type p)
        {
            return p.GetProperties().Any(t => t.PropertyType == typeof (String));
        }

        public static bool TrimAllStringProperties(object obj)
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                if (prop.CanWrite && prop.PropertyType == typeof (String))
                {
                    var val = prop.GetValue(obj);

                    if (val != null)
                    {
                        // ACTION!!!
                        prop.SetValue(obj, ((string)val).Trim());
                    }
                }
            }

            // If there are no classes in the children, we don't need to go further
            return obj.GetType().GetProperties().Any(t => t.PropertyType.IsClass);
        }

        /// <summary>
        /// Removes all non-word and space characters
        /// </summary>
        /// <remarks>Matches on [^\w\s] with RegexOptions ECMAScript</remarks>
        public static string RemoveSpecialCharacters(this string name)
        {
            if (name == null) return null;
            if (string.IsNullOrEmpty(name)) return string.Empty;
            string result = Regex.Replace(name, @"[^\w\s]", string.Empty, RegexOptions.ECMAScript);

            return result;
        }

        /// <summary>
        /// Replace all non-word and space characters
        /// </summary>
        /// <remarks>Matches on [^\w\s] with RegexOptions ECMAScript</remarks>
        public static string ReplaceSpecialCharacters(this string name, char replaceWith)
        {
            if (name == null) return null;
            if (string.IsNullOrEmpty(name)) return string.Empty;
            string result = Regex.Replace(name, @"[^\w\s]", replaceWith.ToString(), RegexOptions.ECMAScript);

            return result;
        }

        /// <summary>
        /// Returns true if both strings
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreSpecialCharactersIgnoreCase(this string value1, string value2)
        {
            if (value1 == null) return false;
            if (value2 == null) return false;

            return value1.RemoveSpecialCharacters()
                .Equals(value2.RemoveSpecialCharacters(), StringComparison.InvariantCultureIgnoreCase);
        }

    }
}
