using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace EntityFX.IotSimulator.Engine
{
    public static class ObjectExtensions
    {
        public static object ToObject(this IDictionary<string, object> source)
        {
            dynamic eo = source.Aggregate(new ExpandoObject() as IDictionary<string, object>,
                                        (a, p) => { a.Add(p.Key, p.Value); return a; });

            return eo;
        }

        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        }
    }
}