using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace PointOfSaleSystem
{
    public class ButtonDisplayLogicService
    {
        public int CategoryPanelPosition { get; set; } = 0;
        public const int CategoryLimit = 5;
        public int ProductPanelPosition { get; set; } = 0;
        public const int ProductLimit = 35;

        // Get a dynamic list of displayed categories
        public dynamic GetDisplayedCategories(ObservableCollection<DatabaseCategory> categories, TextBlock CategoryPageNumber)
        {
            // Get a subset of categories based on the current panel position
            var newCategory = categories.Skip(CategoryPanelPosition).Take(CategoryLimit);

            var currentCategoryPage = (CategoryPanelPosition / CategoryLimit) + 1;
            var totalCategoryPages = (GetClosestMultiple(categories.Count, CategoryLimit) / CategoryLimit) + 1;

            CategoryPageNumber.Text = Convert.ToString($"{currentCategoryPage}/{totalCategoryPages}");

            return newCategory;
        }

        // Get a dynamic list of displayed products
        public dynamic GetDisplayedProducts(ObservableCollection<Product> currentProducts, TextBlock ProductPageNumber)
        {
            // Get a subset of currentProducts based on the current panel position
            var newDisplayedProducts = currentProducts.Skip(ProductPanelPosition).Take(ProductLimit);

            var currentProductPage = (ProductPanelPosition / ProductLimit) + 1;
            var totalProductPages = (GetClosestMultiple(currentProducts.Count, ProductLimit) / ProductLimit) + 1;

            ProductPageNumber.Text = Convert.ToString($"{currentProductPage}/{totalProductPages}");

            return newDisplayedProducts;
        }

        // Helper method to get the closest multiple of a divisor for a dividend
        public static int GetClosestMultiple(int dividend, int divisor)
        {
            var quotient = dividend / divisor;
            var lowerMultiple = divisor * quotient;

            return lowerMultiple;
        }
    }
}
