using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using System.Diagnostics;

namespace TestSystem
{
    [TestClass]
    public class UnitTest1
    {
        public ConditionFactory cf;
        public Window window;

        [TestInitialize]
        public void Setup()
        {
            using var automation = new UIA3Automation();
            var app = Application.Launch(GetSolutionFolderPath() + @"\PointOfSaleSystem\bin\Debug\net6.0-windows\PointOfSaleSystem.exe");
            window = app.GetMainWindow(automation);
            cf = new ConditionFactory(new UIA3PropertyLibrary());
        }

        private static string GetSolutionFolderPath()
        {
            // Assuming the solution folder is two levels above the executable
            string executablePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string solutionFolderPath = Path.GetFullPath(Path.Combine(executablePath, @"..\..\..\..\.."));

            return solutionFolderPath;
        }

        [TestCleanup]

        public void Cleanup()
        {
            window?.AsWindow().Close();
        }

        [TestMethod]
        public void TestDefaultValue()
        {
            Label totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            Trace.Assert(totalPrice.Text == "0,00 kr");
        }

        [TestMethod]
        public void TestAddProduct()
        {
            Button button = window.FindFirstDescendant(cf.ByName("Kaffe")).AsButton();
            Label totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            button.Click();

            Trace.Assert(totalPrice.Text == "20,00 kr");
        }

        [TestMethod]
        public void TestAddMultipleProducts()
        {
            Button button = window.FindFirstDescendant(cf.ByName("Kaffe")).AsButton();
            Label totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            button.Click();
            button.Click();

            Trace.Assert(totalPrice.Text == "40,00 kr");
        }

        [TestMethod]
        public void TestResetPrice()
        {
            Button button = window.FindFirstDescendant(cf.ByName("Kaffe")).AsButton();
            Button buttonReset = window.FindFirstDescendant(cf.ByAutomationId("resetButton")).AsButton();
            Label totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            button.Click();
            buttonReset.Click();

            Trace.Assert(totalPrice.Text == "0,00 kr");
        }

        [TestMethod]
        public void TestAddProductAfterReset()
        {
            Button button = window.FindFirstDescendant(cf.ByName("Kaffe")).AsButton();
            Button buttonReset = window.FindFirstDescendant(cf.ByAutomationId("resetButton")).AsButton();
            Label totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            button.Click();
            buttonReset.Click();
            button.Click();

            Trace.Assert(totalPrice.Text == "20,00 kr");
        }

        [TestMethod]
        public void TestMultipleProductsWithDifferentPrice()
        {
            Button coffeeButton = window.FindFirstDescendant(cf.ByName("Kaffe")).AsButton();
            Button bunButton = window.FindFirstDescendant(cf.ByName("Bulle")).AsButton();
            Button cookieButton = window.FindFirstDescendant(cf.ByName("Kaka")).AsButton();

            coffeeButton.Click();
            bunButton.Click();
            cookieButton.Click();

            Label totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();
            Trace.Assert(totalPrice.Text == "55,00 kr");
        }
    }
}