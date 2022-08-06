using Shallow.SQL.Exceptions;
using Shallow.SQL.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Shallow.SQL.Extensions
{
    internal static class ModelTableExtension
    {
        internal static object GetValueOrFail<U>(this ModelTable<U> modelTable, string propertyName, Column[] _props)
        {
            string realPropertyName = _props.Where(p => string.Equals(p.ColumnName, propertyName, StringComparison.OrdinalIgnoreCase)).Select(p => p.VarName).FirstOrDefault();
            if(realPropertyName != null)
            {
                PropertyInfo prop = modelTable.GetType().GetProperty(realPropertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (prop != null)
                    return prop.GetValue(modelTable);
            }
            else
            {
                FieldInfo field = modelTable.GetType().GetField("_hiddenColumns", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (field != null)
                {
                    var tempDic = (Dictionary<string, object>)field.GetValue(modelTable);
                    if(tempDic.ContainsKey(propertyName))
                        return tempDic[propertyName];
                }
            }
            
            throw new PropertyNotFoundException($"{propertyName} property/column not found");
        }
    }
}
