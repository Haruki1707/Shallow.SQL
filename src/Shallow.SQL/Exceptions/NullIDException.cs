using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shallow.SQL.Exceptions
{
    internal class NullIDException : Exception
    {
        public NullIDException()
        {
        }

        public NullIDException(string message)
            : base(message)
        {
        }
    }
}
