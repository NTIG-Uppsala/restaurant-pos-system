using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace PointOfSaleSystem
{
    public class BusinessLogicService
    {
        public int categoryPanelPosition = 0;
        public readonly int CategoryLimit = 5;
        public int productPanelPosition = 0;
        public readonly int ProductLimit = 35;

        // Get a dynamic list of displayed categories
        public dynamic GetDisplayedCategories(ObservableCollection<DatabaseCategory> categories, TextBlock CategoryPageNumber)
        {
            // Get a subset of categories based on the current panel position
            var newCategory = categories.Skip(categoryPanelPosition).Take(CategoryLimit);

            var currentCategoryPage = (categoryPanelPosition / CategoryLimit) + 1;
            var totalCategoryPages = (GetClosestMultiple(categories.Count, CategoryLimit) / CategoryLimit) + 1;

            CategoryPageNumber.Text = Convert.ToString($"{currentCategoryPage}/{totalCategoryPages}");

            return newCategory;
        }

        // Get a dynamic list of displayed products
        public dynamic GetDisplayedProducts(ObservableCollection<Product> currentProducts, TextBlock ProductPageNumber)
        {
            // Get a subset of currentProducts based on the current panel position
            var newDisplayedProducts = currentProducts.Skip(productPanelPosition).Take(ProductLimit);

            var currentProductPage = (productPanelPosition / ProductLimit) + 1;
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
