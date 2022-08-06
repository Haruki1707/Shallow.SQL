using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shallow.SQL.Exceptions
{
    internal class TableNotFoundException : Exception
    {
        public TableNotFoundException()
        {
        }

        public TableNotFoundException(string message)
            : base(message)
        {
        }
    }
}
