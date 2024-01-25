using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Input;
using FlaUI.UIA3;
using System.Diagnostics;
using System.Windows.Forms;
using Application = FlaUI.Core.Application;
using DataGridViewRow = FlaUI.Core.AutomationElements.DataGridViewRow;

namespace TestSystem
{
    [TestClass]
    public class BaseFunctionTests
    {
        public ConditionFactory cf = null!;
        public Window window = null!;

        [TestInitialize]
        public void Setup()
        {
            using var automation = new UIA3Automation();
            string CurrentDirectory = "../../../../";
            string executablePathFromSrc = "PointOfSaleSystem/bin/Release/net6.0-windows/PointOfSaleSystem.exe";
            string RestaurantPosPath = Path.Combine(CurrentDirectory, executablePathFromSrc);
            RestaurantPosPath = Path.GetFullPath(RestaurantPosPath);
            var app = Application.Launch(RestaurantPosPath);
            window = app.GetMainWindow(automation).AsWindow();
            cf = new ConditionFactory(new UIA3PropertyLibrary());
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

            Trace.Assert(totalPrice.Text == "0,00 kr" ^ totalPrice.Text == "0.00 kr");
        }

        [TestMethod]
        public void TestProductButton()
        {
            var button = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            button.Click();

            Trace.Assert(totalPrice.Text == "10,00 kr" ^ totalPrice.Text == "10.00 kr");
        }

        [TestMethod]
        public void TestMultipleProductButtons()
        {
            var button = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            button.Click();
            button.Click();

            Trace.Assert(totalPrice.Text == "20,00 kr" ^ totalPrice.Text == "20.00 kr");
        }

        [TestMethod]
        public void TestResetPrice()
        {
            var button = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
            var resetButton = window.FindFirstDescendant(cf.ByAutomationId("resetButton")).AsButton();
            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            button.Click();
            resetButton.Click();

            var popup = window.ModalWindows.FirstOrDefault().AsWindow();
            // The automation ID of the yes button is always 6
            var yesButton = popup.FindFirstChild(cf.ByAutomationId("6"));
            yesButton.Click();

            Trace.Assert(totalPrice.Text == "0,00 kr" ^ totalPrice.Text == "0.00 kr");

            var itemTable = window.FindFirstDescendant(cf.ByAutomationId("productWindow")).AsDataGridView();

            var productListHasBeenReset = itemTable.Rows.Length == 0;
            Trace.Assert(productListHasBeenReset);
        }

        [TestMethod]
        public void TestResetPriceNoButton()
        {
            var button = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
            var resetButton = window.FindFirstDescendant(cf.ByAutomationId("resetButton")).AsButton();
            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            button.Click();
            resetButton.Click();

            var popup = window.ModalWindows.FirstOrDefault().AsWindow();
            // The automation ID of the no button is always 7
            var noButton = popup.FindFirstChild(cf.ByAutomationId("7"));
            noButton.Click();

            var itemTable = window.FindFirstDescendant(cf.ByAutomationId("productWindow")).AsDataGridView();

            Trace.Assert(totalPrice.Text == "10,00 kr" ^ totalPrice.Text == "10.00 kr");
            var productListHasNotBeenReset = itemTable.Rows.Length > 0;
            Trace.Assert(productListHasNotBeenReset);
        }

        [TestMethod]
        public void TestAddProductAfterReset()
        {
            var button = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
            var resetButton = window.FindFirstDescendant(cf.ByAutomationId("resetButton")).AsButton();
            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            button.Click();
            resetButton.Click();

            var popup = window.ModalWindows.FirstOrDefault().AsWindow();
            // The automation ID of the yes button is always 6
            var yesButton = popup.FindFirstChild(cf.ByAutomationId("6"));
            yesButton.Click();

            button.Click();

            Trace.Assert(totalPrice.Text == "10,00 kr" ^ totalPrice.Text == "10.00 kr");
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
            Trace.Assert(totalPrice.Text == "118,00 kr" ^ totalPrice.Text == "118.00 kr");
        }
        [TestMethod]
        public void TestPayButton()
        {
            var button = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
            var payButton = window.FindFirstDescendant(cf.ByAutomationId("payButton")).AsButton();
            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();

            button.Click();
            payButton.Click();

            var popup = window.ModalWindows.FirstOrDefault().AsWindow();
            var yesButton = popup.FindFirstChild(cf.ByName("OK"));
            yesButton.Click();

            Trace.Assert(totalPrice.Text == "0,00 kr" ^ totalPrice.Text == "0.00 kr");

            var itemTable = window.FindFirstDescendant(cf.ByAutomationId("productWindow")).AsDataGridView();

            var orderHasBeenPaid = itemTable.Rows.Length == 0;
            Trace.Assert(orderHasBeenPaid);
        }
    }
    [TestClass]
    public class CategoryTests
    {
        public ConditionFactory cf = null!;
        public Window window = null!;

        [TestInitialize]
        public void Setup()
        {
            using var automation = new UIA3Automation();
            string CurrentDirectory = "../../../../";
            string executablePathFromSrc = "PointOfSaleSystem/bin/Release/net6.0-windows/PointOfSaleSystem.exe";
            string RestaurantPosPath = Path.Combine(CurrentDirectory, executablePathFromSrc);
            RestaurantPosPath = Path.GetFullPath(RestaurantPosPath);
            var app = Application.Launch(RestaurantPosPath);
            window = app.GetMainWindow(automation).AsWindow();
            cf = new ConditionFactory(new UIA3PropertyLibrary());
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

            var pizzaButton = window.FindFirstDescendant(cf.ByName("Hawaii")).AsButton();

            Trace.Assert(pizzaButton != null);
        }

        [TestMethod]
        public void TestCategoryClick()
        {
            var categoryButton = window.FindFirstDescendant(cf.ByName("Pizza")).AsButton();

            categoryButton.Click();

            var desiredItemButton = window.FindFirstDescendant(cf.ByName("Calzone")).AsButton();

            desiredItemButton.Click();

            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("totalPrice")).AsLabel();
            Trace.Assert(totalPrice.Text == "90,00 kr" ^ totalPrice.Text == "90.00 kr");
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
            Trace.Assert(totalPrice.Text == "90,00 kr" ^ totalPrice.Text == "90.00 kr");
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
            public ConditionFactory cf = null!;
            public Window window = null!;

            [TestInitialize]
            public void Setup()
            {
                using var automation = new UIA3Automation();
                string CurrentDirectory = "../../../../";
                string executablePathFromSrc = "PointOfSaleSystem/bin/Release/net6.0-windows/PointOfSaleSystem.exe";
                string RestaurantPosPath = Path.Combine(CurrentDirectory, executablePathFromSrc);
                RestaurantPosPath = Path.GetFullPath(RestaurantPosPath);
                var app = Application.Launch(RestaurantPosPath);
                window = app.GetMainWindow(automation).AsWindow();
                cf = new ConditionFactory(new UIA3PropertyLibrary());
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

                // Verify the added product details
                var itemNameTextBlock = window.FindFirstDescendant(cf.ByAutomationId("BearnaiseNameTextBlock"));
                Trace.Assert(itemNameTextBlock.Name == "Bearnaise");

                var itemAmountTextBlock = window.FindFirstDescendant(cf.ByAutomationId("BearnaiseAmountTextBlock"));
                Trace.Assert(itemAmountTextBlock.Name == "1");

                var itemPriceTextBlock = window.FindFirstDescendant(cf.ByAutomationId("BearnaisePriceTextBlock"));
                Trace.Assert(itemPriceTextBlock.Name == "10,00 kr" ^ itemPriceTextBlock.Name == "10.00 kr");
            }

            [TestMethod]
            public void TestAddAmount()
            {
                var button = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
                button.Click();
                button.Click();

                // Verify the added product details
                var itemNameTextBlock = window.FindFirstDescendant(cf.ByAutomationId("BearnaiseNameTextBlock"));
                Trace.Assert(itemNameTextBlock.Name == "Bearnaise");

                var itemAmountTextBlock = window.FindFirstDescendant(cf.ByAutomationId("BearnaiseAmountTextBlock"));
                Trace.Assert(itemAmountTextBlock.Name == "2");

                var itemPriceTextBlock = window.FindFirstDescendant(cf.ByAutomationId("BearnaisePriceTextBlock"));
                Trace.Assert(itemPriceTextBlock.Name == "20,00 kr" ^ itemPriceTextBlock.Name == "20.00 kr");
            }

            [TestMethod]
            public void TestIncreaseAmountButton()
            {
                var productButton = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
                productButton.Click();

                var plusButton = window.FindFirstDescendant(cf.ByAutomationId("BearnaiseIncreaseAmountButton"));
                plusButton.Click();

                var itemAmountTextBlock = window.FindFirstDescendant(cf.ByAutomationId("BearnaiseAmountTextBlock"));
                Trace.Assert(itemAmountTextBlock.Name == "2");

                var itemPriceTextBlock = window.FindFirstDescendant(cf.ByAutomationId("BearnaisePriceTextBlock"));
                Trace.Assert(itemPriceTextBlock.Name == "20,00 kr" ^ itemPriceTextBlock.Name == "20.00 kr");
            }

            [TestMethod]
            public void TestDecreaseAmountButton()
            {
                var productButton = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
                productButton.Click();
                productButton.Click();

                var minusButton = window.FindFirstDescendant(cf.ByAutomationId("BearnaiseDecreaseAmountButton"));
                minusButton.Click();

                var itemAmountTextBlock = window.FindFirstDescendant(cf.ByAutomationId("BearnaiseAmountTextBlock"));
                Trace.Assert(itemAmountTextBlock.Name == "1");

                var itemPriceTextBlock = window.FindFirstDescendant(cf.ByAutomationId("BearnaisePriceTextBlock"));
                Trace.Assert(itemPriceTextBlock.Name == "10,00 kr" ^ itemPriceTextBlock.Name == "10.00 kr");
            }

            [TestMethod]
            public void TestDecreaseAmountButtonStaysAtOne()
            {
                var productButton = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
                productButton.Click();

                var minusButton = window.FindFirstDescendant(cf.ByAutomationId("BearnaiseDecreaseAmountButton"));
                minusButton.Click();

                var itemAmountTextBlock = window.FindFirstDescendant(cf.ByAutomationId("BearnaiseAmountTextBlock"));
                Trace.Assert(itemAmountTextBlock.Name == "1");

                var itemPriceTextBlock = window.FindFirstDescendant(cf.ByAutomationId("BearnaisePriceTextBlock"));
                Trace.Assert(itemPriceTextBlock.Name == "10,00 kr" ^ itemPriceTextBlock.Name == "10.00 kr");
            }

            [TestMethod]
            public void TestQuantityKeyPad()
            {
                var productButton = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
                productButton.Click();

                var EditButton = window.FindFirstDescendant(cf.ByAutomationId("EditButton")).AsButton();
                EditButton.Click();

                var KeypadResult = window.FindFirstDescendant(cf.ByAutomationId("QuantityKeypadResult")).AsTextBox();
                var QuantityButton1 = window.FindFirstDescendant(cf.ByAutomationId("QuantityKeypad1")).AsButton();


                QuantityButton1.Click();
                QuantityButton1.Click();

                Trace.Assert(KeypadResult.Name == "11");
            }

            [TestMethod]
            public void TestQuantityKeyPadReset()
            {
                var productButton = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
                productButton.Click();

                var EditButton = window.FindFirstDescendant(cf.ByAutomationId("EditButton")).AsButton();
                EditButton.Click();


                var QuantityButton1 = window.FindFirstDescendant(cf.ByAutomationId("QuantityKeypad1")).AsButton();

                QuantityButton1.Click();
                QuantityButton1.Click();

                var KeypadResult = window.FindFirstDescendant(cf.ByAutomationId("QuantityKeypadResult")).AsTextBox();

                Trace.Assert(KeypadResult.Name == "11");

                var backAmount = window.FindFirstDescendant(cf.ByAutomationId("QuantityKeypadBack")).AsButton();
                backAmount.Click();

                Trace.Assert(KeypadResult.Name == "1");
            }

            [TestMethod]
            public void TestQuantityKeyPadEnter()
            {
                var productButton = window.FindFirstDescendant(cf.ByName("Bearnaise")).AsButton();
                productButton.Click();

                var EditButton = window.FindFirstDescendant(cf.ByAutomationId("EditButton")).AsButton();
                EditButton.Click();

                var KeypadResult = window.FindFirstDescendant(cf.ByAutomationId("QuantityKeypadResult")).AsTextBox();

                var QuantityButton3 = window.FindFirstDescendant(cf.ByAutomationId("QuantityKeypad3")).AsButton();

                var enterAmount = window.FindFirstDescendant(cf.ByAutomationId("QuantityKeypadEnter")).AsButton();

                QuantityButton3.Click();

                Trace.Assert(KeypadResult.Name == "3");

                enterAmount.Click();

                // Verify the added product details
                var itemPriceTextBlock = window.FindFirstDescendant(cf.ByAutomationId("BearnaisePriceTextBlock"));
                Trace.Assert(itemPriceTextBlock.Name == "30,00 kr" ^ itemPriceTextBlock.Name == "30.00 kr");

                var itemAmountTextBlock = window.FindFirstDescendant(cf.ByAutomationId("BearnaiseAmountTextBlock"));
                Trace.Assert(itemAmountTextBlock.Name == "3");

                var itemNameTextBlock = window.FindFirstDescendant(cf.ByAutomationId("BearnaiseNameTextBlock"));
                Trace.Assert(itemNameTextBlock.Name == "Bearnaise");
            }
        }
    }
}
