using Shallow.SQL.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shallow.SQL.Test.Models
{
    [TableName("User")]
    internal class User : ModelTable<User>
    {
        internal static readonly ModelQuery<User> Query = ModelQuery.GetInstanceOf<User>();

        public string? Name { get; set; }
        public string? Email { get; set; }

        public Phone PhoneNumber
            => this.HasOne(Phone.Query);
        public Product[] Products 
            => this.BelongsToMany(Product.Query);
    }
}
