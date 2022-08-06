using Shallow.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpquent.Tester.Models
{
    internal class User : ModelTable<User>
    {
        internal static readonly ModelQuery<User> Query = ModelQuery.GetInstanceOf<User>();

        public string? Name { get; set; }
        public string? Email { get; set; }

        public int PhoneNumber
            => this.HasOne(Phone.Query).Number;
        public Product[] Products 
            => this.BelongsToMany(Product.Query);
    }
}
