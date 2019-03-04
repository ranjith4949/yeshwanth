using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaxCal;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetTaxAmountSuccess()
        {
            int[] testProdcuts = { 1, 2, 5 };
            double expectedTotal = 2033.62;
            double actual = TaxCalculator.InstancetaxCalculator.GetTotalPrice("FL", testProdcuts);
            Assert.AreEqual(expectedTotal, actual);
        }

        [TestMethod]
        public void GetTaxAmountFail()
        {
            int[] testProdcuts = { 1, 2, 5 };
            double expectedTotal = 0.00;
            double actual = TaxCalculator.InstancetaxCalculator.GetTotalPrice("FA", testProdcuts);
            Assert.AreNotEqual(expectedTotal, actual);
        }
    }
}
