using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using System.Diagnostics;

namespace TestSystem
{
    [TestClass]
    public class BaseFunctionTests
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
            Button button = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
            Label totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            button.Click();

            Trace.Assert(totalPrice.Text == "10,00 kr");
        }

        [TestMethod]
        public void TestAddMultipleProducts()
        {
            Button button = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
            Label totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            button.Click();
            button.Click();

            Trace.Assert(totalPrice.Text == "20,00 kr");
        }

        [TestMethod]
        public void TestResetPrice()
        {
            Button button = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
            Button buttonReset = window.FindFirstDescendant(cf.ByAutomationId("resetButton")).AsButton();
            Label totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            button.Click();
            buttonReset.Click();

            Trace.Assert(totalPrice.Text == "0,00 kr");
        }

        [TestMethod]
        public void TestAddProductAfterReset()
        {
            Button button = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
            Button buttonReset = window.FindFirstDescendant(cf.ByAutomationId("resetButton")).AsButton();
            Label totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            button.Click();
            buttonReset.Click();
            button.Click();

            Trace.Assert(totalPrice.Text == "10,00 kr");
        }

        [TestMethod]
        public void TestMultipleProductsWithDifferentPrice()
        {
            Button earlyItemButton = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
            Button middleItemButton = window.FindFirstDescendant(cf.ByName("Cider")).AsButton();
            Button lateItemButton = window.FindFirstDescendant(cf.ByName("Macchiato")).AsButton();

            earlyItemButton.Click();
            middleItemButton.Click();
            lateItemButton.Click();

            Label totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();
            Trace.Assert(totalPrice.Text == "118,00 kr");
        }
    }
    [TestClass]
    public class CategoryTests
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

        // Categories tests
        [TestMethod]
        public void TestCategories()
        {
            Button categoryButton = window.FindFirstDescendant(cf.ByName("Såser")).AsButton();

            categoryButton.Click();
        }

        [TestMethod]
        public void TestCategoryClick()
        {
            Button categoryButton = window.FindFirstDescendant(cf.ByName("Såser")).AsButton();

            categoryButton.Click();

            Button desiredItemButton = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();

            desiredItemButton.Click();

            Label totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();
            Trace.Assert(totalPrice.Text == "10,00 kr");
        }

        [TestMethod]
        public void TestReturn()
        {
            Button firstCategoryButton = window.FindFirstDescendant(cf.ByName("Såser")).AsButton();

            firstCategoryButton.Click();

            Button desiredItemButton = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();

            Trace.Assert(desiredItemButton != null);

            Button returnButton = window.FindFirstDescendant(cf.ByAutomationId("Return")).AsButton();

            returnButton.Click();

            Button popularButton = window.FindFirstDescendant(cf.ByName("Kaffe")).AsButton();

            Trace.Assert(popularButton != null);
        }

        [TestMethod]
        public void TestSavePrice()
        {
            Button categoryButton = window.FindFirstDescendant(cf.ByName("Såser")).AsButton();

            categoryButton.Click();

            Button desiredItemButton = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();

            desiredItemButton.Click();

            Button returnButton = window.FindFirstDescendant(cf.ByAutomationId("Return")).AsButton();

            returnButton.Click();

            Label totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();
            Trace.Assert(totalPrice.Text == "10,00 kr");
        }

        [TestMethod]
        public void TestMostPopularItem()
        {
            Button popularButton = window.FindFirstDescendant(cf.ByName("Kaffe")).AsButton();

            Trace.Assert(popularButton != null);
        }

        [TestMethod]
        public void TestMostPopularItemPriority()
        {
            Button popularButton = window.FindFirstDescendant(cf.ByName("Kaffe")).AsButton();

            Trace.Assert(popularButton != null);

            Button shouldNotExistButton = window.FindFirstDescendant(cf.ByName("Hawaii")).AsButton();

            Trace.Assert(shouldNotExistButton == null);
        }

        [TestMethod]
        public void TestSlidePages()
        {
            Button firstPageItemButton = window.FindFirstDescendant(cf.ByName("Kaffe")).AsButton();

            Trace.Assert(firstPageItemButton != null);

            Button nextSlideButton = window.FindFirstDescendant(cf.ByAutomationId("NextProductButton")).AsButton();

            nextSlideButton.Click();

            firstPageItemButton = window.FindFirstDescendant(cf.ByName("Kaffe")).AsButton();

            Trace.Assert(firstPageItemButton == null);
        }

        [TestMethod]
        public void TestCategoryPages()
        {
            Button newCategoryButton = window.FindFirstDescendant(cf.ByName("Öl/Cider")).AsButton();

            Trace.Assert(newCategoryButton == null);

            Button categoryButton = window.FindFirstDescendant(cf.ByName("Såser")).AsButton();

            Trace.Assert(categoryButton != null);

            Button nextCategoryButton = window.FindFirstDescendant(cf.ByAutomationId("NextCategoryButton")).AsButton();

            nextCategoryButton.Click();

            categoryButton = window.FindFirstDescendant(cf.ByName("Såser")).AsButton();

            Trace.Assert(categoryButton == null);

            newCategoryButton = window.FindFirstDescendant(cf.ByName("Öl/Cider")).AsButton();

            Trace.Assert(newCategoryButton != null);

        }

        [TestMethod]
        public void TestHiddenItem()
        {
            Button hiddenProduct = window.FindFirstDescendant(cf.ByName("Sleepy Bulldog Pale Ale")).AsButton();

            Trace.Assert(hiddenProduct == null);
        }
    }
}