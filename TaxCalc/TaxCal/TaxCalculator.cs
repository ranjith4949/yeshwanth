using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxCal
{
    public class TaxCalculator
    {
        protected double TaxRate;
        List<Products> lstProducts = new List<Products>();
        List<Promotion> lstPromotion = new List<Promotion>();
        List<Coupon> lstCoupon = new List<Coupon>();
        protected List<KeyValuePair<int, int>> orders = null;
        protected static TaxCalculator _instance = null;

        public TaxCalculator()
        {
            //below details will load from database
            TaxRate = 18;

            lstProducts.AddRange(new Products[]{
                                new Products{ProductId=1,ProdPrice=150,IsLuxury=true,CouponId=1},
                                new Products{ProductId=2,ProdPrice=700,IsLuxury=true,CouponId=2},
                                new Products{ProductId=3,ProdPrice=90,IsLuxury=false,CouponId=2},
                                new Products{ProductId=5,ProdPrice=1200,IsLuxury=true,CouponId=null},
                                new Products{ProductId=4,ProdPrice=18000,IsLuxury=false,CouponId=null}

            });

            lstPromotion.AddRange(new Promotion[]{
                                new Promotion{PromotionDisc=15,StartDt=Convert.ToDateTime("01/01/2019"),EndDt=Convert.ToDateTime("03/06/2019")},
                                new Promotion{PromotionDisc=20,StartDt=Convert.ToDateTime("01/01/2019"),EndDt=Convert.ToDateTime("02/02/2019")}

            });
            lstCoupon.AddRange(new Coupon[]{
                                new Coupon{CouponId=1,CouponDisc=15,StartDt=Convert.ToDateTime("01/01/2019"),EndDt=Convert.ToDateTime("03/05/2019")},
                                new Coupon{CouponId=2,CouponDisc=50,StartDt=Convert.ToDateTime("01/01/2019"),EndDt=Convert.ToDateTime("03/01/2019")},

            });
        }
        public static TaxCalculator InstancetaxCalculator
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TaxCalculator();
                }
                return _instance;
            }
        }
        public double GetTotalPrice(string state, int[] products)
        {
            bool isBefore = state.Contains("FL") || state.Contains("NM") || state.Contains("NV") ? true : false;
            double totalPrice = 0;
            bool isLuxury = false;
            var promdisc = 0; int? CpnId = null;

            if (products != null && products.Length > 0)
            {
                var prodprice = 0.00;
                promdisc = lstPromotion.Where(x => x.EndDt >= DateTime.Now).Select(x => x.PromotionDisc).FirstOrDefault();

                if (products.Length == 1)
                {
                    prodprice = lstProducts.Where(x => x.ProductId == products[0]).Select(x => x.ProdPrice).FirstOrDefault();
                    isLuxury = lstProducts.Where(x => x.ProductId == products[0]).Select(x => x.IsLuxury).FirstOrDefault();
                    CpnId = lstProducts.Where(x => x.ProductId == products[0]).Select(x => x.CouponId).FirstOrDefault();
                    if (isBefore)
                    {
                        totalPrice = prodprice + CalculateTax(prodprice, isLuxury);
                        if (CpnId != null) { totalPrice = ApplyCoupon(totalPrice, CpnId); }
                        if (promdisc > 0) { totalPrice = ApplyPromotion(totalPrice, promdisc); }
                    }
                    else
                    {
                        if (CpnId != null) totalPrice = ApplyCoupon(prodprice, CpnId);
                        if (promdisc > 0) { totalPrice = ApplyPromotion(totalPrice, promdisc); }
                        totalPrice = totalPrice + CalculateTax(totalPrice, isLuxury);
                    }
                }
                else if (products.Length > 1)
                {
                    foreach (var pId in products)
                    {
                        var indvProdPrice = lstProducts.Where(x => x.ProductId == pId).Select(x => x.ProdPrice).FirstOrDefault();
                        CpnId = lstProducts.Where(x => x.ProductId == pId).Select(x => x.CouponId).FirstOrDefault();
                        if (CpnId != null) { indvProdPrice = ApplyCoupon(indvProdPrice, CpnId); }
                        totalPrice = totalPrice + indvProdPrice;
                    }
                    if (isBefore)
                    {
                        totalPrice = totalPrice + CalculateTax(totalPrice, false);
                        if (promdisc > 0) { totalPrice = ApplyPromotion(totalPrice, promdisc); }
                    }
                    else
                    {
                        if (promdisc > 0) { totalPrice = ApplyPromotion(totalPrice, promdisc); }
                        totalPrice = totalPrice + CalculateTax(totalPrice, false);
                    }
                }
            }
            return Math.Round(totalPrice, 2);

        }
        private double CalculateTax(double TotalPrice, bool isLuxury)
        {
            return Math.Round(TotalPrice * (isLuxury ? (2 * TaxRate) : TaxRate) / 100);
        }
        private double ApplyPromotion(double amt, double promdisc)
        {
            return amt = amt - (amt * promdisc / 100);
        }
        private double ApplyCoupon(double amt, int? cpnId)
        {
            var coupndisc = lstCoupon.Where(x => x.CouponId == cpnId && x.EndDt > DateTime.Now).Select(x => x.CouponDisc).FirstOrDefault();
            return amt = amt - (amt * coupndisc / 100);
        }
    }
}
