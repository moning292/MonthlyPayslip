using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonthlyPayslip;
using MonthlyPayslip.Controllers;
using System.Web;
using Moq;
using System.Web.Routing;
using System.IO;
using System.Resources;
using MvcContrib.TestHelper;

namespace MonthlyPayslip.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {

        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("This is the employee monthly payslip calculator.", result.ViewBag.Message);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Calculate_Show()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Calculate() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Calculate", result.ViewBag.Title);
        }

        [TestMethod]
        public void Calculate_CorrectCSV()
        {
            var fileData = UTF8Encoding.UTF8.GetBytes("David,Rudd,60050,9%,01 March - 31 March");
            var fileStream = new MemoryStream(fileData);

            var fileToUpload = new Mock<HttpPostedFileBase>();
            fileToUpload.Setup(f => f.ContentLength).Returns((int)fileStream.Length);
            fileToUpload.Setup(f => f.FileName).Returns("test.csv");
            fileToUpload.Setup(f => f.ContentType).Returns("csv");
            fileToUpload.Setup(f => f.InputStream).Returns(fileStream);

            HomeController controller = new HomeController();
            var result = controller.Calculate(fileToUpload.Object) as RedirectToRouteResult;

            Assert.IsTrue(result.RouteValues["action"].Equals("Result"));
        }

        [TestMethod]
        public void Calculate_CorrectMultipleLineCSV()
        {
            var fileData = UTF8Encoding.UTF8.GetBytes("David,Rudd,60050,9%,01 March - 31 March\nDavid,Rudd,60050,9%,01 March - 31 March\n");
            var fileStream = new MemoryStream(fileData);

            var fileToUpload = new Mock<HttpPostedFileBase>();
            fileToUpload.Setup(f => f.ContentLength).Returns((int)fileStream.Length);
            fileToUpload.Setup(f => f.FileName).Returns("test.csv");
            fileToUpload.Setup(f => f.ContentType).Returns("csv");
            fileToUpload.Setup(f => f.InputStream).Returns(fileStream);

            HomeController controller = new HomeController();
            var result = controller.Calculate(fileToUpload.Object) as RedirectToRouteResult;

            Assert.IsTrue(result.RouteValues["action"].Equals("Result"));
        }

        [TestMethod]
        public void Calculate_IncorrectCSV()
        {
            var fileData = UTF8Encoding.UTF8.GetBytes("TTTTT");
            var fileStream = new MemoryStream(fileData);

            var fileToUpload = new Mock<HttpPostedFileBase>();
            fileToUpload.Setup(f => f.ContentLength).Returns((int)fileStream.Length);
            fileToUpload.Setup(f => f.FileName).Returns("test.csv");
            fileToUpload.Setup(f => f.ContentType).Returns("csv");
            fileToUpload.Setup(f => f.InputStream).Returns(fileStream);

            HomeController controller = new HomeController();
            var result = controller.Calculate(fileToUpload.Object) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Incorrect data format in the CSV File. Check the data again!", result.ViewBag.Message);
        }

        [TestMethod]
        public void Calculate_IncorrectNameDataTypeCSV()
        {
            var fileData = UTF8Encoding.UTF8.GetBytes("1111,2222,60050,9%,01 March - 31 March");
            var fileStream = new MemoryStream(fileData);

            var fileToUpload = new Mock<HttpPostedFileBase>();
            fileToUpload.Setup(f => f.ContentLength).Returns((int)fileStream.Length);
            fileToUpload.Setup(f => f.FileName).Returns("test.csv");
            fileToUpload.Setup(f => f.ContentType).Returns("csv");
            fileToUpload.Setup(f => f.InputStream).Returns(fileStream);

            HomeController controller = new HomeController();
            var result = controller.Calculate(fileToUpload.Object) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Incorrect data format in the CSV File. Check the data again!", result.ViewBag.Message);
        }

        [TestMethod]
        public void Calculate_EmptyCSV()
        {
            var fileData = UTF8Encoding.UTF8.GetBytes("");
            var fileStream = new MemoryStream(fileData);

            var fileToUpload = new Mock<HttpPostedFileBase>();
            fileToUpload.Setup(f => f.ContentLength).Returns((int)fileStream.Length);
            fileToUpload.Setup(f => f.FileName).Returns("test.csv");
            fileToUpload.Setup(f => f.ContentType).Returns("csv");
            fileToUpload.Setup(f => f.InputStream).Returns(fileStream);

            HomeController controller = new HomeController();
            var result = controller.Calculate(fileToUpload.Object) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Nothing in the CSV file", result.ViewBag.Message);
        }

        [TestMethod]
        public void Calculate_Wrong_FileType()
        {
            var fileData = UTF8Encoding.UTF8.GetBytes("TTTTT");
            var fileStream = new MemoryStream(fileData);

            var fileToUpload = new Mock<HttpPostedFileBase>();
            fileToUpload.Setup(f => f.ContentLength).Returns((int)fileStream.Length);
            fileToUpload.Setup(f => f.FileName).Returns("test.txt");
            fileToUpload.Setup(f => f.ContentType).Returns("txt");
            fileToUpload.Setup(f => f.InputStream).Returns(fileStream);

            HomeController controller = new HomeController();
            var result = controller.Calculate(fileToUpload.Object) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("It's not a CSV file", result.ViewBag.Message);
        }

        [TestMethod]
        public void Result_WithCorrectTempData()
        {
            var test = new List<string>();
            test.Add("David,Rudd,60050,9%,01 March - 31 March");
            TestControllerBuilder builder = new TestControllerBuilder();
            var tempData = new TempDataDictionary();
            tempData.Add("rawCSV", test);
            HomeController controller = new HomeController();
            builder.InitializeController(controller);
            controller.TempData = tempData;
            var result = controller.Result() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(controller.Session["plist"]);

        }

        [TestMethod]
        public void Result_WithOutTempData()
        {
            TestControllerBuilder builder = new TestControllerBuilder();
            HomeController controller = new HomeController();
            builder.InitializeController(controller);

            var result = controller.Result() as RedirectToRouteResult;

            Assert.IsTrue(result.RouteValues["action"].Equals("Index"));
            Assert.IsNotNull(result);
            Assert.IsNull(controller.Session["plist"]);

        }

        [TestMethod]
        public void ExportCSV()
        {
            var test = new List<MonthlyPayslip.Models.Payslip>();
            var ps = new MonthlyPayslip.Models.Payslip();
            ps.FirstName = "ddd";
            ps.LastName = "dddd";
            ps.IncomeTax = 111111;
            ps.NetIncome = 99999;
            ps.PayPeriod = "March";
            ps.IncomeTax = 555;
            ps.Super = 444;
            test.Add(ps);

            TestControllerBuilder builder = new TestControllerBuilder();

            HomeController controller = new HomeController();
            builder.InitializeController(controller);
            builder.Session["plist"] = test;
       
            var result = controller.ExportCSV() as FileContentResult;

            Assert.IsNotNull(result);
        }
    }
}
