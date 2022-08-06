using System;
using System.Collections.Generic;
using System.Text;

namespace Shallow.SQL.Exceptions
{
    internal class PropertyNotFoundException : Exception
    {
        public PropertyNotFoundException()
        {
        }

        public PropertyNotFoundException(string message)
            : base(message)
        {
        }
    }
}
