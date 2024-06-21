using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fnAsm2_4902_xx
{
    public partial class CustomerFrame : Form
    {
        private string UserName;
        private readonly ProductRepository _productRepository;

        public CustomerFrame(string userName)
        {
            InitializeComponent();

            this.UserName = userName;

            _productRepository = new ProductRepository();
            try
            {
                _productRepository.LoadProductsFromFile();
                dgvData.DataSource = _productRepository.GetProducts();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                _productRepository.LoadProductsFromFile();
                dgvData.DataSource = _productRepository.GetProducts();

                MessageBox.Show("Data loaded successfully!", "Success", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
            public string Category { get; set; }
            public string Price { get; set; }

            public Product(int id, string name, int quantity, string category, string price)
            {
                Id = id;
                Name = name;
                Quantity = quantity;
                Category = category;
                Price = price;
            }
        }

        public interface IProductRepository
        {
            void AddProduct(Product product);
            List<Product> GetProducts();
            bool ProductExists(int id);
            void LoadProductsFromFile();
            void SaveProductsToFile();
        }

        public class ProductRepository : IProductRepository
        {
            private readonly List<Product> _products = new List<Product>();
            private const string FilePath = "product.txt";

            public void AddProduct(Product product)
            {
                _products.Add(product);
                SaveProductsToFile();
            }

            public List<Product> GetProducts()
            {
                return _products.ToList();
            }

            public bool ProductExists(int id)
            {
                return _products.Any(product => product.Id == id);
            }

            public void LoadProductsFromFile()
            {
                _products.Clear();
                if (File.Exists(FilePath))
                {
                    using (StreamReader reader = new StreamReader(FilePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] parts = line.Split(',');
                            if (parts.Length == 5)
                            {
                                _products.Add(new Product(int.Parse(parts[0]), parts[1], int.Parse(parts[2]), parts[3], parts[4]));
                            }
                        }
                    }
                }
            }

            public void SaveProductsToFile()
            {
                using (StreamWriter writer = new StreamWriter(FilePath))
                {
                    foreach (var product in _products)
                    {
                        writer.WriteLine($"{product.Id},{product.Name},{product.Quantity},{product.Category},{product.Price}");
                    }
                }
            }
        }

        private void btnCart_Click(object sender, EventArgs e)
        {
            CartFrame cartFrame = new CartFrame(UserName);
            cartFrame.Show();
        }

        private void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count > 0)
            {
                var selectedRow = dgvData.SelectedRows[0];
                var productId = selectedRow.Cells["Id"].Value.ToString();
                var productName = selectedRow.Cells["Name"].Value.ToString();
                var quantity = 1; // Số lượng mặc định
                var category = selectedRow.Cells["Category"].Value.ToString();
                var price = selectedRow.Cells["Price"].Value.ToString();

                SaveProductToCart(productId, productName, quantity, UserName, category, price);
                MessageBox.Show("Product added to cart successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select a product first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveProductToCart(string productId, string productName, int quantity, string userName, string category, string price)
        {
            const string cartFilePath = "cart.txt";
            int newCartId = 1;

            if (File.Exists(cartFilePath))
            {
                var lines = File.ReadAllLines(cartFilePath);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (int.TryParse(parts[0], out int cartId) && cartId >= newCartId)
                    {
                        newCartId = cartId + 1;
                    }
                }
            }

            var cartLine = $"{newCartId},{productId},{productName},{quantity},{userName},{category},{price}";

            try
            {
                File.AppendAllText(cartFilePath, cartLine + Environment.NewLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving to cart: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();
                if (string.IsNullOrEmpty(searchTerm))
                {
                    MessageBox.Show("Please enter a search term.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var products = _productRepository.GetProducts();

                var filteredProducts = products.Where(product => product.Name.Contains(searchTerm)).ToList();

                dgvData.DataSource = null;
                dgvData.DataSource = filteredProducts;

                if (filteredProducts.Count == 0)
                {
                    MessageBox.Show("No books found matching the search criteria.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while searching: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
