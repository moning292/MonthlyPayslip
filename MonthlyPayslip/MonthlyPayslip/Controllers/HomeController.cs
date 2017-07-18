using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using MonthlyPayslip.Common;
using MonthlyPayslip.Models;
using System.Text.RegularExpressions;

namespace MonthlyPayslip.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "This is the employee monthly payslip calculator.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Please feel free to contact...";

            return View();
        }

        //Method: show payslip calculation page
        public ActionResult Calculate()
        {
            ViewBag.Title = "Calculate";
            return View();
        }

        //Method: Payslip calculation
        [HttpPost]
        public ActionResult Calculate(HttpPostedFileBase upload)
        {
            var rawCSVList = new List<string>();
            var PayslipList = new List<Payslip>();

            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    if (upload.FileName.EndsWith(".csv") || upload.FileName.EndsWith(".CSV"))
                    {
                        Stream stream = upload.InputStream;
                        StreamReader sr;
                        using (sr = new StreamReader(stream))
                        {
                            while (!sr.EndOfStream)
                            {
                                var splits = sr.ReadLine().Split(';');
                                rawCSVList.Add(Regex.Replace(splits[0], @"\s+", ""));
                            }
                            Regex rgx = new Regex(@"([A-Z])\w+,([A-Z])\w+,(?:\d*\.)?\d+,(?:\d*\.)?\d+%,(?:\d*\.)?\d+([A-Z])\w+-(?:\d*\.)?\d+([A-Z])\w+");
                            foreach (var rawCSVRow in rawCSVList) {
                                if (!rgx.IsMatch(rawCSVRow))
                                {
                                    ViewBag.Message = "Incorrect data format in the CSV File. Check the data again!";
                                    return View();
                                }
                            }

                            TempData["rawCSV"] = rawCSVList;
                            return RedirectToAction("Result");
                        }
                    }else
                    {
                        ViewBag.Message = "It's not a CSV file";
                        return View();
                    }

                }
                else
                {
                    ViewBag.Message = "Nothing in the CSV file";
                    return View();
                }
            }
            ViewBag.Message = "Can't parse data!";
            return View();
        }

        //Method: Display payslip list result
        public ActionResult Result()
        {
            //var req_data = new List<string>();
            var rawCSVList = (List<string>)TempData["rawCSV"];
            var PayslipList = new List<Payslip>();

            if (rawCSVList != null)
            {
                foreach (var rawCSVRow in rawCSVList)
                {
                    var ps = new Payslip();
                    var payslipArray = rawCSVRow.ToString().Split(',');
                    //First Name
                    ps.FirstName = payslipArray[0];
                    //Last Name
                    ps.LastName = payslipArray[1];
                    //Annual Salary
                    var salary = Double.Parse(payslipArray[2]);
                    //Monthly Salary
                    var msalary = salary / 12;
                    //Rounded Monthly Salary
                    ps.GrossIncome = Math.Round(msalary, MidpointRounding.AwayFromZero);
                    //Annual Tax
                    var tax = Manipulator.TaxCalculator(salary.ToString());
                    //Monthly Tax
                    var mtax = tax / 12;
                    //Rounded Monthly Tax
                    ps.IncomeTax = Math.Round(mtax, MidpointRounding.AwayFromZero);
                    //Monthly Net Income
                    ps.NetIncome = ps.GrossIncome - ps.IncomeTax;
                    //Super Percentage
                    var super = Manipulator.FromPercentageString(payslipArray[3]);
                    //Monthly Super
                    var m_super = ps.GrossIncome * super;
                    //Rounded Monthly Super
                    ps.Super = Math.Round(m_super, MidpointRounding.AwayFromZero);
                    //Pay Period
                    ps.PayPeriod = payslipArray[4];

                    PayslipList.Add(ps);
                }
            }
            else {

                //No object found then redirect to main page
                return RedirectToAction("Index");
            }
            Session["plist"] = PayslipList;
            return View(PayslipList);
        }

        //Method: Export to CSV file
        public FileContentResult ExportCSV()
        {
            var psList = (List<Payslip>)Session["plist"];
            var result = Payslip.SerializeAsCSV(psList);
            return File(new System.Text.UTF8Encoding().GetBytes(result), "text/csv", "MonthlyPayslip.csv");
        }
    }
}