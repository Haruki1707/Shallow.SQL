using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shallow.SQL.Exceptions
{
    internal class ColumnNotFoundException : Exception
    {
        public ColumnNotFoundException()
        {
        }

        public ColumnNotFoundException(string message)
            : base(message)
        {
        }
    }
}
