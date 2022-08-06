using Humanizer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shallow.SQL.Extensions
{
    internal static class StringExtension
    {
        internal static string Alias(this string text)
        {
            string alias = "";

            foreach (string word in text.Humanize(LetterCasing.Title).Split(' '))
                alias += word.Substring(0, 2);
            return alias;
        }
    }
}
