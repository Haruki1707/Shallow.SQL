using System;
using System.Collections.Generic;
using System.Text;

namespace Shallow.SQL.Attributes
{
    public class TableName : Attribute
    {
        internal string Name;
        public TableName(string name)
        {
            Name = name;
        }
    }
}
