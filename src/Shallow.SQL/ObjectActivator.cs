using Shallow.SQL.Attributes;
using Shallow.SQL.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shallow.SQL
{
    internal class ObjectActivator<T>
    {
        private Type _type;
        private T _object;

        public ObjectActivator()
        {
            _type = typeof(T);
            _object = (T)Activator.CreateInstance(_type);
        }

        public ObjectActivator(object insObject)
        {
            _type = typeof(T);
            _object = (T)(insObject != null ? insObject : Activator.CreateInstance(_type));
        }

        internal void SetValue(string To, object Value)
        {
            PropertyInfo prop = _type.GetProperty(To, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);


            if (prop != null)
            {
                Type propType = prop.PropertyType;
                if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    propType = propType.GetGenericArguments()[0];


                prop.SetValue(_object, Convert.ChangeType(Value, propType));
            }
            else
            {
                FieldInfo field = _type.GetField("_hiddenColumns", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if(field != null)
                    ((Dictionary<string, object>)field.GetValue(_object)).Add(To, Value);
            }
        }

        internal T GetObject => _object;
    }
}
