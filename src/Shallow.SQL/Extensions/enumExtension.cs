using Shallow.SQL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shallow.SQL.Extensions
{
    internal static class enumExtension
    {
        internal static string Parse(this Operators value)
        {
            switch (value)
            {
                case Operators.Equals:
                    return "=";
                case Operators.Greater:
                    return ">";
                case Operators.Less:
                    return "<";
                case Operators.GreaterOrEqual:
                    return ">=";
                case Operators.LessOrEqual:
                    return "<=";
                case Operators.NotEqual:
                    return "<>";
                default:
                    return value.ToString();
            }
        }
    }
}
