using System;
using System.Collections.Generic;
using System.Text;

namespace Shallow.SQL.Exceptions
{
    internal class ServerOrDatabaseNotFoundException : Exception
    {
        public ServerOrDatabaseNotFoundException()
        {
        }

        public ServerOrDatabaseNotFoundException(string message)
            : base(message)
        {
        }
    }
}
