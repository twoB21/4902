using System;
using System.IO;
using System.Windows.Forms;

namespace fnAsm2_4902_xx
{
    public partial class SignUpFrame : Form
    {
        private readonly IAccountFactory _accountFactory;

        public SignUpFrame(IAccountFactory accountFactory)
        {
            _accountFactory = accountFactory;
            InitializeComponent();
        }

        private void btnSignup_Click(object sender, EventArgs e)
        {
            string username = txtEmail.Text;

            if (CheckUsernameExists(username))
            {
                MessageBox.Show(this, "Username already exists! Please choose a different username.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string password = txtPassword.Text;
            string birthday = txtBirthDay.Text;
            string name = txtName.Text;

            if (!IsValidBirthdayFormat(birthday))
            {
                MessageBox.Show(this, "Invalid date format for birthday! Please enter the date in dd/MM/yyyy format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                IAccount account = _accountFactory.CreateAccount(username, password, birthday, name);

                using (StreamWriter writer = new StreamWriter("userData.txt", true))
                {
                    writer.WriteLine($"{account.UserId},{account.Username},{account.Password},{account.Birthday},{account.Role},{account.Name}");
                }
                MessageBox.Show(this, "Account registration successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (IOException ex)
            {
                MessageBox.Show(this, "Error while writing to file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex.Message);
            }
        }

        private bool CheckUsernameExists(string username)
        {
            try
            {
                using (StreamReader reader = new StreamReader("userData.txt"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] userData = line.Split(',');
                        if (userData.Length >= 2 && userData[1] == username)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(this, "An error occurred while checking username existence", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        private bool IsValidBirthdayFormat(string birthday)
        {
            DateTime parsedDate;
            return DateTime.TryParseExact(birthday, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out parsedDate);
        }
    }

    public interface IAccount
    {
        int UserId { get; }
        string Username { get; }
        string Password { get; }
        string Birthday { get; }
        string Role { get; }
        string Name { get; }
    }

    public interface IAccountFactory
    {
        IAccount CreateAccount(string username, string password, string birthday, string name);
    }

    public class AccountFactory : IAccountFactory
    {
        private static int _userIdCounter = 0;

        public IAccount CreateAccount(string username, string password, string birthday, string name)
        {
            int userId = GenerateUserId();
            if (username.EndsWith("@admin.com"))
            {
                return new AdminAccount(userId, username, password, birthday, name);
            }
            else
            {
                return new CustomerAccount(userId, username, password, birthday, name);
            }
        }

        private int GenerateUserId()
        {
            int count = 0;
            try
            {
                using (StreamReader reader = new StreamReader("userData.txt"))
                {
                    while (reader.ReadLine() != null)
                    {
                        count++;
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return count + 1;
        }
    }

    public class CustomerAccount : IAccount
    {
        public int UserId { get; }
        public string Username { get; }
        public string Password { get; }
        public string Birthday { get; }
        public string Role { get; }
        public string Name { get; }

        public CustomerAccount(int userId, string username, string password, string birthday, string name)
        {
            UserId = userId;
            Username = username;
            Password = password;
            Birthday = birthday;
            Role = "customer";
            Name = name;
        }
    }

    public class AdminAccount : IAccount
    {
        public int UserId { get; }
        public string Username { get; }
        public string Password { get; }
        public string Birthday { get; }
        public string Role { get; }
        public string Name { get; }

        public AdminAccount(int userId, string username, string password, string birthday, string name)
        {
            UserId = userId;
            Username = username;
            Password = password;
            Birthday = birthday;
            Role = "admin";
            Name = name;
        }
    }
}
