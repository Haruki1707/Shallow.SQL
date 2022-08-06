using Shallow.SQL.Attributes;
using Shallow.SQL.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Shallow.SQL.Extensions
{
    internal static class TypeExtension
    {
        internal static Column[] GetProps(this Type type)
        {
            List<Column> cols = new List<Column>();

            foreach (var property in type.GetProperties()
                .Where(prop => prop.GetSetMethod(true) != null))
            {
                cols.Add(new Column()
                {
                    VarName = property.Name,
                    ColumnName = ((ColumnName)property.GetCustomAttribute(typeof(ColumnName)))?.Name
                });
            }

            return cols.ToArray();
        }

        internal static string[] GetRelations(this Type type)
            => type.GetProperties()
                .Where(prop => prop.GetSetMethod() == null)
                .Select(prop => prop.Name)
                .ToArray();

        internal static MethodInfo GetGetMethod(this Type type, string Name)
            => type.GetProperty(Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .GetGetMethod();
    }
}
