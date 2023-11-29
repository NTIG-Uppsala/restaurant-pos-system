using System.Diagnostics;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;

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
            using (var automation = new UIA3Automation())
            {
                var app = Application.Launch(GetSolutionFolderPath() + @"\PointOfSaleSystem\bin\Debug\net6.0-windows\PointOfSaleSystem.exe");
                window = app.GetMainWindow(automation);
                var newConditionFactory = new ConditionFactory(new UIA3PropertyLibrary());
                ConditionFactory cf = newConditionFactory;
            }
        }

        private static string GetSolutionFolderPath()
        {
            // Assuming the solution folder is two levels above the executable
            string executablePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string solutionFolderPath = Path.GetFullPath(Path.Combine(executablePath, @"..\..\..\..\.."));

            return solutionFolderPath;
        }

        [TestMethod]
        public void TestPriceCalculation()
        {
            Button button = window.FindFirstDescendant(cf.ByAutomationId("addProductButton")).AsButton();
            TextBox totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsTextBox();

            Trace.Assert(totalPrice.Text == "0.00 kr");

            button.Click();

            Trace.Assert(totalPrice.Text == "20.00 kr");

            button.Click();

            Trace.Assert(totalPrice.Text == "40.00 kr");
        }

        [TestMethod]
        public void TestResetPrice()
        {
            Button buttonReset = window.FindFirstDescendant(cf.ByAutomationId("resetButton")).AsButton();

            TextBox totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsTextBox();

            totalPrice.Text = "19.99 kr";

            buttonReset.Click();

            Trace.Assert(totalPrice.Text == "0.00 kr");
        }
    }
}