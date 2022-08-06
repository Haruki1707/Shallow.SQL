using Shallow.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpquent.Tester.Models
{
    internal class Product : ModelTable<Product>
    {
        internal static readonly ModelQuery<Product> Query = ModelQuery.GetInstanceOf<Product>();

        public string? Name { get; set; }
        public string? Description { get; set; }
        public float? Price { get; set; }

        public User[] Users 
            => this.BelongsToMany(User.Query);
    }
}
