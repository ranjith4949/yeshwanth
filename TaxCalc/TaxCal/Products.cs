using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxCal
{
    public class Products
    {
        public int ProductId { get; set; }
        public double ProdPrice { get; set; }
        public bool IsLuxury { get; set; }
        public int? CouponId { get; set; }
    }
}
