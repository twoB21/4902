using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace fnAsm2_4902_xx
{
    public partial class CartFrame : Form
    {
        private string userName;
        private readonly List<CartFrame.Cart> _cart = new List<CartFrame.Cart>();
        private readonly List<Product> _products = new List<Product>();
        private const string FilePath = "cart.txt";
        private const string OrderFilePath = "order.txt";
        private const string ProductFilePath = "product.txt";

        public CartFrame(string userName)
        {
            InitializeComponent();
            this.userName = userName;

            LoadCartFromFile();
            LoadUserCart();
            CalculateTotalPrice();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCartFromFile();
                LoadUserCart();
                CalculateTotalPrice();
                MessageBox.Show("Data loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count > 0)
            {
                var selectedCart = (CartFrame.Cart)dgvData.SelectedRows[0].DataBoundItem;
                DeleteCart(selectedCart.CartId);
                LoadUserCart();
                CalculateTotalPrice();
                MessageBox.Show("Cart item deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select a cart item to delete!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUserCart()
        {
            dgvData.DataSource = GetCartByUser(userName);
        }

        private List<CartFrame.Cart> GetCartByUser(string userName)
        {
            return _cart.Where(c => c.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private void LoadCartFromFile()
        {
            _cart.Clear();
            if (File.Exists(FilePath))
            {
                using (StreamReader reader = new StreamReader(FilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length == 7)
                        {
                            _cart.Add(new CartFrame.Cart(
                                int.Parse(parts[0]),
                                int.Parse(parts[1]),
                                parts[2],
                                int.Parse(parts[3]),
                                parts[4],
                                parts[5],
                                parts[6]
                            ));
                        }
                    }
                }
            }
            else
            {
                throw new FileNotFoundException($"File {FilePath} not found.");
            }
        }

        private void CalculateTotalPrice()
        {
            decimal totalPrice = 0;
            foreach (var cartItem in _cart.Where(c => c.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)))
            {
                if (decimal.TryParse(cartItem.Price, out decimal itemPrice))
                {
                    totalPrice += itemPrice * cartItem.Quantity;
                }
            }
            lblTotalPrice.Text = totalPrice.ToString("C");
        }

        private void LoadProductFromFile()
        {
            _products.Clear();
            if (File.Exists(ProductFilePath))
            {
                using (StreamReader reader = new StreamReader(ProductFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length == 5)
                        {
                            _products.Add(new Product(
                                int.Parse(parts[0]),
                                parts[1],
                                parts[3],
                                int.Parse(parts[2])
                            ));
                        }
                    }
                }
            }
            else
            {
                throw new FileNotFoundException($"File {ProductFilePath} not found.");
            }
        }

        private void SaveProductToFile()
        {
            using (StreamWriter writer = new StreamWriter(ProductFilePath))
            {
                foreach (var product in _products)
                {
                    writer.WriteLine($"{product.ProductId},{product.ProductName},{product.Quantity},{product.Category},{product.Price}");
                }
            }
        }

        public class Cart
        {
            public int CartId { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public string UserName { get; set; }
            public string Category { get; set; }
            public string Price { get; set; }

            public Cart(int cartId, int productId, string productName, int quantity, string userName, string category, string price)
            {
                CartId = cartId;
                ProductId = productId;
                ProductName = productName;
                Quantity = quantity;
                UserName = userName;
                Category = category;
                Price = price;
            }
        }

        public class Product
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string Category { get; set; }
            public int Quantity { get; set; }
            public string Price { get; set; }

            public Product(int productId, string productName, string category, int quantity)
            {
                ProductId = productId;
                ProductName = productName;
                Category = category;
                Quantity = quantity;
            }
        }

        private void DeleteCart(int id)
        {
            var cartToDelete = _cart.FirstOrDefault(cart => cart.CartId == id);
            if (cartToDelete != null)
            {
                _cart.Remove(cartToDelete);
                SaveCartToFile();
            }
        }

        private void SaveCartToFile()
        {
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                foreach (var cart in _cart)
                {
                    writer.WriteLine($"{cart.CartId},{cart.ProductId},{cart.ProductName},{cart.Quantity},{cart.UserName},{cart.Category},{cart.Price}");
                }
            }
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string address = txtAddress.Text.Trim();
            string phoneNumber = txtPhoneNumber.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(phoneNumber))
            {
                MessageBox.Show("Please enter your name, address, and phone number.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal totalPrice = 0;
            List<int> productIds = new List<int>();
            List<string> productNames = new List<string>();
            List<CartFrame.Cart> userCartItems = _cart.Where(c => c.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)).ToList();

            LoadProductFromFile();

            foreach (var cartItem in userCartItems)
            {
                var product = _products.FirstOrDefault(p => p.ProductId == cartItem.ProductId);
                if (product == null || product.Quantity <= 0)
                {
                    MessageBox.Show($"Product {cartItem.ProductName} is out of stock!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            foreach (var cartItem in userCartItems)
            {
                if (decimal.TryParse(cartItem.Price, out decimal itemPrice))
                {
                    totalPrice += itemPrice * cartItem.Quantity;
                }
                productIds.Add(cartItem.ProductId);
                productNames.Add(cartItem.ProductName);

                var product = _products.First(p => p.ProductId == cartItem.ProductId);
                product.Quantity -= 1;
            }

            string productIdString = string.Join(".", productIds);
            string productNameString = string.Join(".", productNames);

            using (StreamWriter writer = new StreamWriter(OrderFilePath, true))
            {
                writer.WriteLine($"{name},{address},{phoneNumber},{totalPrice:C},({productIdString}),({productNameString})");
            }

            _cart.RemoveAll(c => c.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
            SaveCartToFile();
            LoadUserCart();
            CalculateTotalPrice();

            txtAddress.Text = string.Empty;
            txtName.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty;

            MessageBox.Show("Order placed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void txtAddress_TextChanged(object sender, EventArgs e)
        {

        }

        private void lablName_Click(object sender, EventArgs e)
        {

        }
    }
}
