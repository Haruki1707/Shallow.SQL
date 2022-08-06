using Shallow.SQL;
using Shallow.SQL.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpquent.Tester.Models
{
    internal class Phone : ModelTable<Phone>
    {
        internal static readonly ModelQuery<Phone> Query = ModelQuery.GetInstanceOf<Phone>();

        public int Number { get; set; }

        public User Users 
            => BelongsTo(User.Query);
    }
}
