using System;
using System.Collections.Generic;
using System.Text;

namespace Bacchus.Shared
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductCategory { get; set; }
        public DateTimeOffset BiddingEndDate { get; set; }
    }
}
