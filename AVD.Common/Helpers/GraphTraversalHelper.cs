using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AVD.Common.Caching;
using AVD.Common.Logging;

namespace AVD.Common.Helpers
{
    public class GraphTraversalHelper
    {
        private static ICachingManager cachingManager;

        static GraphTraversalHelper()
        {
            cachingManager = new CachingManager();
        }

        private string cacheKeyPrefix;
        private Func<Type, bool> visitType;
        private Func<object, bool> action;

        /// <summary>
        /// This will traverse an object group and recursively examine its non-string class properties. 
        /// 
        /// If 'visitType' is true, then 'action' will be performed.
        /// 
        /// If 'action' is false then this type will not be visited again.
        /// </summary>
        /// <param name="cacheKeyPrefix">The prefix used for the cache key to aid in pruning the search space</param>
        /// <param name="visitType">If true, this may be a valid node for searching. Action will be performed.</param>
        /// <param name="action">If true, this object will be recursively examined.</param>
        /// <example>For a string search, condition => return typeof() = string OR class, action = getvalue().trim(), return false;</example>
        public GraphTraversalHelper(String cacheKeyPrefix, Func<Type, bool> visitType, Func<object, bool> action)
        {
            this.cacheKeyPrefix = cacheKeyPrefix;
            this.visitType = visitType;
            this.action = action;
        }

        /// <summary>
        /// This will recursively go through each property in the given object. If the condition for the property is true,
        /// the action will be called. If the result of action is true then this will recursively examine that object.
        /// 
        /// If the condition is false, the action will not be called but the object will be recursively examined if it is a class.
        /// </summary>
        public void DoWork(object value)
        {
            TraverseGraph(value);
        }

        private bool TraverseGraph(object value)
        {
            Logger.Instance.LogFunctionEntry(GetType().Name + "-" + cacheKeyPrefix, "ScanObjectGraph");

            
            // If the value is null we can't scan the object
            if (value == null)
                return false;

            if(!value.GetType().IsClass)
                throw new ApplicationException("Only works with classes");

            var key = cachingManager.GetHashCodeKey(cacheKeyPrefix + "-ScanObjectGraph", value.GetType().FullName);

            // Cache whether this object requires us to execute the following code or not.
            bool val;

            if (cachingManager.TryGet(key, out val) && val)
                return false;

            bool hasPossibleStringInObject = false;

            bool continueRecursive = true;

            // If this is a type we need to perform an action on, then do so.
            if (visitType(value.GetType()))
            {
                hasPossibleStringInObject = true;
                continueRecursive = action(value);
            }

            if (continueRecursive)
            {
                // Examine properties or list items in this type.
                try
                {
                    // If its a list, we need to check its contents
                    if (value as IEnumerable != null)
                    {
                        bool first = true;

                        foreach (var v in value as IEnumerable)
                        {
                            if (first)
                            {
                                if(!(v.GetType().IsClass && v.GetType() != typeof(string)))
                                    continue;

                                first = false;
                            }

                            if (TraverseGraph(v))
                                hasPossibleStringInObject = true;
                        }
                    }
                    else
                    {
                        var properties = value.GetType().GetProperties();

                        foreach (var p in properties)
                        {
                            if (p.PropertyType.IsClass && p.PropertyType != typeof(string)) // is a primitive other than string
                                if (TraverseGraph(p.GetValue(value)))
                                    hasPossibleStringInObject = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error(GetType().Name + "-" + cacheKeyPrefix, "ScanObjectGraph", ex);
                }
            }

            if (hasPossibleStringInObject == false)
                cachingManager.Add(key, true);

            Logger.Instance.LogFunctionExit(GetType().Name + "-" + cacheKeyPrefix, "ScanObjectGraph");

            return hasPossibleStringInObject;
        }
    }
}
