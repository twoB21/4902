using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static fnAsm2_4902_xx.ProductRepository;

namespace fnAsm2_4902_xx
{
    public partial class AdminFrame : Form
    {
        private string UserName;
        private readonly ProductRepository _productRepository;
        private readonly OrderRepository _orderRepository;

        public AdminFrame(string userName)
        {
            InitializeComponent();

            
            
            UserName = userName;

            _productRepository = new ProductRepository();
            _orderRepository = new OrderRepository();

            _productRepository.LoadProductFromFile();
            dgvData.DataSource = _productRepository.GetProducts();
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var id = Convert.ToInt32(txtId.Text);
                var nameProduct = txtNameProduct.Text.Trim();
                var quantity = Convert.ToInt32(txtQuantity.Text.Trim());
                var category = cbbCategory.Text.Trim();
                var price = txtPrice.Text.Trim();

                if (string.IsNullOrWhiteSpace(id.ToString()) || string.IsNullOrWhiteSpace(nameProduct) ||
                    string.IsNullOrWhiteSpace(quantity.ToString()) || string.IsNullOrWhiteSpace(category) || string.IsNullOrWhiteSpace(price))
                {
                    MessageBox.Show("Please fill in all fields!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (_productRepository.ProductExists(id))
                {
                    MessageBox.Show($"Book with ID {id} already exists!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                    return;
                }
                _productRepository.AddProduct(new Product(id, nameProduct, quantity, category, price));
                dgvData.DataSource = _productRepository.GetProducts();

                MessageBox.Show("Data saved successfully!", "Success", MessageBoxButtons.OK,
                MessageBoxIcon.Information);

                txtId.Text = string.Empty;
                txtNameProduct.Text = string.Empty;
                txtQuantity.Text = string.Empty;
                cbbCategory.SelectedIndex = -1;
                txtPrice.Text = string.Empty;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                _productRepository.LoadProductFromFile();
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

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count > 0)
            {
                var selectecProduct = (Product)dgvData.SelectedRows[0].DataBoundItem;

                txtId.Text = selectecProduct.Id.ToString();
                txtNameProduct.Text = selectecProduct.Name;
                txtQuantity.Text = selectecProduct.Quantity.ToString();
                txtPrice.Text = selectecProduct.Price.ToString();
                cbbCategory.Text = selectecProduct.Category.ToString();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                var id = Convert.ToInt32(txtId.Text.Trim());
                var nameProduct = txtNameProduct.Text.Trim();
                var quantity = Convert.ToInt32(txtQuantity.Text.Trim());
                var category = cbbCategory.Text.Trim();
                var price = txtPrice.Text.Trim();

                if (!string.IsNullOrWhiteSpace(id.ToString()) && !string.IsNullOrWhiteSpace(nameProduct) &&
                    !string.IsNullOrWhiteSpace(quantity.ToString()) && !string.IsNullOrWhiteSpace(category) && !string.IsNullOrWhiteSpace(price))
                {
                    _productRepository.UpadteProduct(id, nameProduct, quantity, category, price);
                    dgvData.DataSource = _productRepository.GetProducts();

                    MessageBox.Show("Data updated successfully!", "Success", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                    txtId.Text = string.Empty;
                    txtNameProduct.Text = string.Empty;
                    txtQuantity.Text = string.Empty;
                    cbbCategory.SelectedIndex = -1;
                    txtPrice.Text = string.Empty;
                }
                else
                {
                    MessageBox.Show("Please fill in all fields!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count > 0)
            {
                var selectedIndex = dgvData.SelectedRows[0].Index;
                var selectedProduct = (Product)dgvData.SelectedRows[0].DataBoundItem;

                _productRepository.DeleteProduct(selectedProduct.Id);
                dgvData.DataSource = _productRepository.GetProducts();

                MessageBox.Show("Book deleted successfully!", "Success", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select a book to delete!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            try
            {
                _orderRepository.LoadOrdersFromFile();
                dgvData.DataSource = null;
                dgvData.DataSource = _orderRepository.GetOrders();

                dgvData.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

                MessageBox.Show("Order data loaded successfully!", "Success", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading order data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void btnLoad_Click_1(object sender, EventArgs e)
        {
            try
            {
                _productRepository.LoadProductFromFile();
                dgvData.DataSource = _productRepository.GetProducts();

                MessageBox.Show("Data loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

    public class ProductRepository
    {
        private readonly List<Product> _products = new List<Product>();
        private const string FilePath = "product.txt";

        public void AddProduct(Product product)
        {
            _products.Add(product);
            SaveProductToFile();
        }

        public List<Product> GetProducts()
        {
            return _products.ToList();
        }

        public bool ProductExists(int id)
        {
            return _products.Any(product => product.Id == id);
        }

        public void LoadProductFromFile()
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
        public void SaveProductToFile()
        {
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                foreach (var product in _products)
                {
                    writer.WriteLine($"{product.Id}, {product.Name}, {product.Quantity}, {product.Category}, {product.Price}");
                }
            }
        }
        public void UpadteProduct(int id, string nameProduct, int quantity, string category, string price)
        {
            var productUpdate = _products.FirstOrDefault(product => product.Id == id);
            if (productUpdate != null)
            {

                productUpdate.Name = nameProduct;
                productUpdate.Quantity = quantity;
                productUpdate.Category = category;
                productUpdate.Price = price;
                SaveProductToFile();
            }
        }
        public void DeleteProduct(int id)
        {
            var productToDelete = _products.FirstOrDefault(product => product.Id == id);
            if (productToDelete != null)
            {
                _products.Remove(productToDelete);
                SaveProductToFile();
            }
        }
        public class Order
        {
            public string CustomerName { get; set; }
            public string Address { get; set; }
            public string PhoneNumber { get; set; }
            public string TotalAmount { get; set; }
            public string ProductIds { get; set; }
            public string ProductNames { get; set; }

            public Order(string customerName, string address, string phoneNumber, string totalAmount, string productIds, string productNames)
            {
                CustomerName = customerName.Trim();
                Address = address.Trim();
                PhoneNumber = phoneNumber.Trim();
                TotalAmount = totalAmount.Trim();
                ProductIds = productIds.Trim();
                ProductNames = productNames.Trim();

            }

            private decimal ParseDecimal(string amount)
            {
                string cleanedAmount = amount.Replace("$", "").Replace(",", "").Trim();
                return decimal.Parse(cleanedAmount);
            }
        }


        public class OrderRepository
        {
            private readonly List<Order> _orders = new List<Order>();
            private const string FilePath = "order.txt";

            public List<Order> GetOrders()
            {
                return _orders.ToList();
            }

            public void LoadOrdersFromFile()
            {
                _orders.Clear();
                if (File.Exists(FilePath))
                {
                    using (StreamReader reader = new StreamReader(FilePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] parts = line.Split(',');

                            if (parts.Length == 6)
                            {
                                string customerName = parts[0].Trim();
                                string address = parts[1].Trim();
                                string phoneNumber = parts[2].Trim();
                                string totalAmount = parts[3].Trim(); 
                                string productIds = parts[4].Trim();
                                string productNames = parts[5].Trim();

                                _orders.Add(new Order(customerName, address, phoneNumber, totalAmount, productIds, productNames));
                            }
                        }
                    }
                }
            }

        }
    }        
}
