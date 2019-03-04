using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxCal;

namespace TaxCal
{
    class Program
    {
        static void Main(string[] args)
        {
            //"FL","NM","NV","GA","NY"
            int[] prodArray = { 1, 2, 5 };
            Console.WriteLine("Total Price:" + TaxCalculator.InstancetaxCalculator.GetTotalPrice("FL", prodArray));
            Console.ReadLine();
        }
    }
}
