using System;
using System.Collections.Generic;
using System.Text;

namespace Shallow.SQL.Attributes
{
    public class ColumnName : Attribute
    {
        internal string Name;
        public ColumnName(string ColumnName)
        {
            this.Name = ColumnName;
        }
    }
}
