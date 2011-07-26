using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FubuMVC.Conventions
{
    public static class PropertyExtensions
    {
        public static IEnumerable<PropertyInfo> PropertiesWhere(this Type type, Func<PropertyInfo, bool> predicate)
        {
            return type
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(predicate);
        }
    }
}