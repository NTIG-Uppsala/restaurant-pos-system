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
            var executablePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            var solutionFolderPath = Path.GetFullPath(Path.Combine(executablePath, @"..\..\..\..\.."));

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
            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            Trace.Assert(totalPrice.Text == "0,00 kr");
        }

        [TestMethod]
        public void TestProductButton()
        {
            var button = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            button.Click();

            Trace.Assert(totalPrice.Text == "10,00 kr");
        }

        [TestMethod]
        public void TestMultipleProductButtons()
        {
            var button = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            button.Click();
            button.Click();

            Trace.Assert(totalPrice.Text == "20,00 kr");
        }

        [TestMethod]
        public void TestResetPrice()
        {
            var button = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
            var buttonReset = window.FindFirstDescendant(cf.ByAutomationId("resetButton")).AsButton();
            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            button.Click();
            buttonReset.Click();

            Trace.Assert(totalPrice.Text == "0,00 kr");

            var itemTable = window.FindFirstDescendant(cf.ByAutomationId("")).AsListBox();

            var productListHasBeenReset = itemTable.Items.Length == 0;
            Trace.Assert(productListHasBeenReset);
        }

        [TestMethod]
        public void TestAddProductAfterReset()
        {
            var button = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
            var buttonReset = window.FindFirstDescendant(cf.ByAutomationId("resetButton")).AsButton();
            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            button.Click();
            buttonReset.Click();
            button.Click();

            Trace.Assert(totalPrice.Text == "10,00 kr");
        }

        [TestMethod]
        public void TestMultipleProductsWithDifferentPrice()
        {
            var earlyItemButton = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
            var middleItemButton = window.FindFirstDescendant(cf.ByName("Cider")).AsButton();
            var lateItemButton = window.FindFirstDescendant(cf.ByName("Macchiato")).AsButton();

            earlyItemButton.Click();
            middleItemButton.Click();
            lateItemButton.Click();

            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();
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
            var executablePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            var solutionFolderPath = Path.GetFullPath(Path.Combine(executablePath, @"..\..\..\..\.."));

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
            var categoryButton = window.FindFirstDescendant(cf.ByName("Pizza")).AsButton();

            categoryButton.Click();
        }

        [TestMethod]
        public void TestCategoryClick()
        {
            var categoryButton = window.FindFirstDescendant(cf.ByName("Pizza")).AsButton();

            categoryButton.Click();

            var desiredItemButton = window.FindFirstDescendant(cf.ByName("Calzone")).AsButton();

            desiredItemButton.Click();

            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();
            Trace.Assert(totalPrice.Text == "90,00 kr");
        }

        [TestMethod]
        public void TestReturn()
        {
            var firstCategoryButton = window.FindFirstDescendant(cf.ByName("Pizza")).AsButton();

            firstCategoryButton.Click();

            var desiredItemButton = window.FindFirstDescendant(cf.ByName("Calzone")).AsButton();

            Trace.Assert(desiredItemButton != null);

            var returnButton = window.FindFirstDescendant(cf.ByAutomationId("Return")).AsButton();

            returnButton.Click();

            var popularButton = window.FindFirstDescendant(cf.ByName("Kaffe")).AsButton();

            Trace.Assert(popularButton != null);
        }

        [TestMethod]
        public void TestSavePrice()
        {
            var categoryButton = window.FindFirstDescendant(cf.ByName("Pizza")).AsButton();

            categoryButton.Click();

            var desiredItemButton = window.FindFirstDescendant(cf.ByName("Calzone")).AsButton();

            desiredItemButton.Click();

            var returnButton = window.FindFirstDescendant(cf.ByAutomationId("Return")).AsButton();

            returnButton.Click();

            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();
            Trace.Assert(totalPrice.Text == "90,00 kr");
        }

        [TestMethod]
        public void TestMostPopularItem()
        {
            var popularButton = window.FindFirstDescendant(cf.ByName("Kaffe")).AsButton();

            Trace.Assert(popularButton != null);
        }

        [TestMethod]
        public void TestMostPopularItemPriority()
        {
            var popularButton = window.FindFirstDescendant(cf.ByName("Kaffe")).AsButton();

            Trace.Assert(popularButton != null);

            var shouldNotExistButton = window.FindFirstDescendant(cf.ByName("Hawaii")).AsButton();

            Trace.Assert(shouldNotExistButton == null);
        }

        [TestMethod]
        public void TestSlidePages()
        {
            var firstPageItemButton = window.FindFirstDescendant(cf.ByName("Kaffe")).AsButton();

            Trace.Assert(firstPageItemButton != null);

            var nextSlideButton = window.FindFirstDescendant(cf.ByAutomationId("NextProductButton")).AsButton();

            nextSlideButton.Click();

            firstPageItemButton = window.FindFirstDescendant(cf.ByName("Kaffe")).AsButton();

            Trace.Assert(firstPageItemButton == null);
        }

        [TestMethod]
        public void TestCategoryPages()
        {
            var newCategoryButton = window.FindFirstDescendant(cf.ByName("Öl/Cider")).AsButton();

            Trace.Assert(newCategoryButton == null);

            var categoryButton = window.FindFirstDescendant(cf.ByName("Pizza")).AsButton();

            Trace.Assert(categoryButton != null);

            var nextCategoryButton = window.FindFirstDescendant(cf.ByAutomationId("NextCategoryButton")).AsButton();

            nextCategoryButton.Click();

            categoryButton = window.FindFirstDescendant(cf.ByName("Pizza")).AsButton();

            Trace.Assert(categoryButton == null);

            newCategoryButton = window.FindFirstDescendant(cf.ByName("Öl/Cider")).AsButton();

            Trace.Assert(newCategoryButton != null);

        }

        [TestMethod]
        public void TestHiddenItem()
        {
            var hiddenProduct = window.FindFirstDescendant(cf.ByName("Sleepy Bulldog Pale Ale")).AsButton();

            Trace.Assert(hiddenProduct == null);
        }

        [TestMethod]
        public void TestProductPageNumberItem()
        {
            var pageNumber = window.FindFirstDescendant(cf.ByAutomationId("ProductPageNumber")).AsTextBox();

            Trace.Assert(pageNumber.Name == "1/2");

            var nextSlideButton = window.FindFirstDescendant(cf.ByAutomationId("NextProductButton")).AsButton();

            nextSlideButton.Click();

            Trace.Assert(pageNumber.Name == "2/2");
        }

        [TestMethod]
        public void TestCategoryPageNumberItem()
        {
            var pageNumber = window.FindFirstDescendant(cf.ByAutomationId("CategoryPageNumber")).AsTextBox();

            Trace.Assert(pageNumber.Name == "1/2");

            var nextSlideButton = window.FindFirstDescendant(cf.ByAutomationId("NextCategoryButton")).AsButton();

            nextSlideButton.Click();

            Trace.Assert(pageNumber.Name == "2/2");
        }
        [TestClass]
        public class ProductWindowTests
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
                var executablePath = System.Reflection.Assembly.GetEntryAssembly().Location;
                var solutionFolderPath = Path.GetFullPath(Path.Combine(executablePath, @"..\..\..\..\.."));

                return solutionFolderPath;
            }

            [TestCleanup]

            public void Cleanup()
            {
                window?.AsWindow().Close();
            }

            [TestMethod]
            public void TestProductAdded()
            {
                var button = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
                button.Click();

                var itemTable = window.FindFirstDescendant(cf.ByAutomationId("")).AsListBox();

                var itemNameHasBeenAdded = itemTable.Items.Any(item => item.Text == "Bearnaise");
                Trace.Assert(itemNameHasBeenAdded);

                var itemPriceHasBeenAdded = itemTable.Items.Any(item => item.Text == "10");
                Trace.Assert(itemPriceHasBeenAdded);

                var itemAmountHasBeenAdded = itemTable.Items.Any(item => item.Text == "1");
                Trace.Assert(itemAmountHasBeenAdded);
            }

            [TestMethod]
            public void TestAddAmount()
            {
                var button = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
                button.Click();
                button.Click();

                var itemTable = window.FindFirstDescendant(cf.ByAutomationId("")).AsListBox();

                var itemNameHasBeenAdded = itemTable.Items.Any(item => item.Text == "Bearnaise");
                Trace.Assert(itemNameHasBeenAdded);

                var itemPriceHasBeenAdded = itemTable.Items.Any(item => item.Text == "20");
                Trace.Assert(itemPriceHasBeenAdded);

                var itemAmountHasBeenAdded = itemTable.Items.Any(item => item.Text == "2");
                Trace.Assert(itemAmountHasBeenAdded);
            }

        }
    }
}
