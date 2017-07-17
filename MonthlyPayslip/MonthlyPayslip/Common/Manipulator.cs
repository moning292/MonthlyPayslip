using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonthlyPayslip.Common
{
    public static class Manipulator
    {

        public static double FromPercentageString(this string value)
        {
            return double.Parse(value.Replace("%", "")) / 100;
        }

        public static double TaxCalculator(this string value)
        {
            double annualTax = double.Parse(value);
            if (annualTax <= 18200)
            {
                return 0;
            }
            else if (annualTax > 18200 && annualTax <= 37000)
            {
                return (annualTax - 18200) * 0.19;
            }
            else if (annualTax > 37000 && annualTax <= 80000)
            {
                return 3572 + ((annualTax - 37000) * 0.325);
            }
            else if (annualTax > 80000 && annualTax <= 180000)
            {
                return 17547 + ((annualTax - 80000) * 0.37);
            }
            else if (annualTax > 180000)
            {
                return 54547 + ((annualTax - 180000) * 0.45);
            }
            else
            {
                return 0;
            }
        }
    }
}