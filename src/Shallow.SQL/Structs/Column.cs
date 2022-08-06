using System;
using System.Collections.Generic;
using System.Text;

namespace Shallow.SQL.Structs
{
    internal class Column
    {
        private string columnName = null;

        public string VarName { get; set; }
        public string ColumnName { 
            get => columnName != null ? columnName : VarName;
            set => columnName = value;
        }
    }
}
