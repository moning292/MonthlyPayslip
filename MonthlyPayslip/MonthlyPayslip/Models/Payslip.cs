using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MonthlyPayslip.Models
{
    public class Payslip
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PayPeriod { get; set; }
        public double GrossIncome { get; set; }
        public double IncomeTax { get; set; }
        public double NetIncome { get; set; }
        public double Super { get; set; }

        public static string SerializeAsCSV(List<Payslip> payslipList)
        {
            string res = "";
                foreach (Payslip pay in payslipList)
                {
                    res = res + string.Format(pay.FirstName + "," + pay.LastName + "," + pay.PayPeriod + "," + pay.GrossIncome + "," + pay.IncomeTax + "," + pay.NetIncome + "," + pay.Super + "\n");
                }
            return res;
        }

    }
}