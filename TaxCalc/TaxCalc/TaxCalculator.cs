using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxCalc
{
    public class TaxCalculator
    {
        protected double TaxRate;
        public TaxCalculator(double _taxRate)
        {
            TaxRate = _taxRate;
        }
        List<Products> lstProducts = new List<Products>();
        List<Promotion> lstPromotion = new List<Promotion>();
        List<Coupon> lstCoupon = new List<Coupon>();
        protected List<KeyValuePair<int, int>> orders = null;

        //LoadTaxSettings();
        //GetTaxAmount("FA");

        public void LoadTaxSettings()
        {
            //"FL","NM","NV","GA","NY"
            orders = new List<KeyValuePair<int, int>>();
            //first order
            orders.Add(new KeyValuePair<int, int>(1, 1));
            //orders.Add(new KeyValuePair<int, int>(1, 2));
            orders.Add(new KeyValuePair<int, int>(1, 4));
            //second order
            // orders.Add(new KeyValuePair<int, int>(2, 3));

            lstProducts.AddRange(new Products[]{
                                new Products{ProductId=1,TaxId=1,ProdPrice=150,IsLuxury=true,IsCoupon=true},
                                new Products{ProductId=2,TaxId=2,ProdPrice=700,IsLuxury=true,IsCoupon=false},
                                new Products{ProductId=3,TaxId=2,ProdPrice=90,IsLuxury=true,IsCoupon=true},
                                new Products{ProductId=5,TaxId=3,ProdPrice=1200,IsLuxury=true,IsCoupon=false},
                                new Products{ProductId=4,TaxId=4,ProdPrice=18000,IsLuxury=false,IsCoupon=false}

            });

            lstPromotion.AddRange(new Promotion[]{
                                new Promotion{PromotionDisc=15,StartDt=Convert.ToDateTime("01/01/2019"),EndDt=Convert.ToDateTime("03/03/2019")},
                                new Promotion{PromotionDisc=20,StartDt=Convert.ToDateTime("01/01/2019"),EndDt=Convert.ToDateTime("02/02/2019")}

            });
            lstCoupon.AddRange(new Coupon[]{
                                new Coupon{CouponDisc=15,StartDt=Convert.ToDateTime("01/01/2019"),EndDt=Convert.ToDateTime("03/05/2019")},
                                new Coupon{CouponDisc=50,StartDt=Convert.ToDateTime("01/01/2019"),EndDt=Convert.ToDateTime("03/01/2019")},

            });
        }

        public double GetTaxAmount(string state)//int pId, bool isBefore)
        {
            //int pId = 0;
            bool isBefore = state.Contains("FL") || state.Contains("NM") || state.Contains("NV") ? true : false;
            double totalwithtax = 0;
            bool isCpn = false, isLuxury = false;
            var promdisc = 0;
            if (orders != null && orders.Count > 0)
            {
                List<int> allOrders = (from oId in orders select oId.Key).Distinct().ToList();

                foreach (var oi in allOrders)
                {
                    var prodList = orders.Where(x => x.Key == oi).Select(x => x.Value).ToList();
                    if (prodList != null && prodList.Count > 0)
                    {
                        var prodprice = 0.00;
                        foreach (var pId in prodList)
                        {
                            prodprice = prodprice + lstProducts.Where(x => x.ProductId == pId).Select(x => x.ProdPrice).FirstOrDefault();
                            isCpn = lstProducts.Where(x => x.ProductId == pId).Select(x => x.IsCoupon).FirstOrDefault();
                            isLuxury = lstProducts.Where(x => x.ProductId == pId).Select(x => x.IsLuxury).FirstOrDefault();
                            promdisc = lstPromotion.Where(x => x.EndDt >= DateTime.Now).Select(x => x.PromotionDisc).FirstOrDefault();
                        }
                        if (isBefore)
                        {
                            totalwithtax = totalwithtax + Math.Round(prodprice * (isLuxury ? (2 * TaxRate) : TaxRate) / 100);
                            if (promdisc > 0) { totalwithtax = ApplyPromotion(totalwithtax, promdisc); }
                            if (isCpn) { totalwithtax = ApplyCoupon(totalwithtax); }
                        }
                        else
                        {
                            if (promdisc > 0) { prodprice = ApplyPromotion(prodprice, promdisc); }
                            if (isCpn) prodprice = ApplyCoupon(prodprice);
                            totalwithtax = totalwithtax + Math.Round(prodprice * (isLuxury ? (2 * TaxRate) : TaxRate) / 100);

                        }
                    }
                }
            }
            return totalwithtax;
        }

        double ApplyPromotion(double amt, double promdisc)
        {
            return amt = amt - (amt * promdisc / 100);
        }
        double ApplyCoupon(double amt)
        {
            var coupndisc = lstCoupon.Where(x => x.EndDt > DateTime.Now).Select(x => x.CouponDisc).FirstOrDefault();
            return amt = amt - (amt * coupndisc / 100);
        }
    }
    public class Products
    {
        public int ProductId { get; set; }
        public int TaxId { get; set; }
        public double ProdPrice { get; set; }
        public bool IsLuxury { get; set; }
        public bool IsCoupon { get; set; }

    }

    public class Promotion
    {
        public int PromotionDisc { get; set; }
        public DateTime? StartDt { get; set; }
        public DateTime? EndDt { get; set; }

    }

    public class Coupon
    {
        public int CouponDisc { get; set; }
        public DateTime? StartDt { get; set; }
        public DateTime? EndDt { get; set; }

    }
}
