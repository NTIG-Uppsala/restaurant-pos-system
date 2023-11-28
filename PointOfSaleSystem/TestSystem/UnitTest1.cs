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

        public Window window;
        public ConditionFactory cf;

        [TestInitialize]
        public void Setup()

        {
            using (var automation = new UIA3Automation())
            {
                var app = Application.Launch(GetSolutionFolderPath() + @"\PointOfSaleSystem\bin\Debug\net8.0-windows\PointOfSaleSystem.exe");
                window = app.GetMainWindow(automation);


                ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());
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
        public void TestTotalPrice()
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
        public void TestCoffeeButton()
        {
            Button button = window.FindFirstDescendant(cf.ByName("Kaffe")).AsButton();
            ListBox productList = window.FindFirstDescendant(cf.ByAutomationId("productList")).AsListBox();
            ListBox quantityList = window.FindFirstDescendant(cf.ByAutomationId("quantityList")).AsListBox();

            // Checks if the list contains an item with the string "Kaffe"
            bool listContainsItem = productList.Items.Any(item => item.Text == "Kaffe");

            Trace.Assert(!listContainsItem);

            button.Click();

            Trace.Assert(listContainsItem);

            button.Click();

            Trace.Assert(quantityList.Items[1].Text == "2");
        }

        [TestMethod]
        public void TestResetPrice()
        {
            Button button = window.FindFirstDescendant(cf.ByAutomationId("addProductButton")).AsButton();
            Button buttonReset = window.FindFirstDescendant(cf.ByAutomationId("resetButton")).AsButton();

            ListBox productList = window.FindFirstDescendant(cf.ByAutomationId("productList")).AsListBox();

            TextBox totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsTextBox();

            button.Click();

            buttonReset.Click();

            Trace.Assert(totalPrice.Text == "0.00 kr");

            bool listIsEmpty = productList.Items.Length == 0;
            Trace.Assert(listIsEmpty);
        }
    }
}