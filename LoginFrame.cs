using System;
using System.IO;
using System.Windows.Forms;

namespace fnAsm2_4902_xx
{
    public partial class LoginFrame : Form
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IRoleService _roleService;
        private readonly IAccountFactory _accountFactory;

        public LoginFrame(IAuthenticationService authenticationService, IRoleService roleService, IAccountFactory accountFactory)
        {
            _authenticationService = authenticationService;
            _roleService = roleService;
            _accountFactory = accountFactory;

            InitializeComponent();

            FileInitializer.EnsureUserdataFileExists(); // Ensure file exists when initializing the form
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtEmail.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowLoginFailedMessage("Username or password cannot be empty.");
                return;
            }

            try
            {
                if (_authenticationService.Authenticate(username, password))
                {
                    string role = _roleService.GetUserRole(username);
                    if (role != null)
                    {
                        ShowMainFrameAccordingToRole(role);
                    }
                    else
                    {
                        ShowRoleError();
                    }
                }
                else
                {
                    ShowLoginFailedMessage("Username or password is incorrect.");
                }
            }
            catch (IOException ex)
            {
                HandleException(ex, "Error occurred while logging in.");
            }
        }

        private void ShowMainFrameAccordingToRole(string role)
        {
            switch (role)
            {
                case "admin":
                    ShowAdminFrame();
                    break;
                case "customer":
                    ShowCustomerFrame();
                    break;
                default:
                    ShowRoleError();
                    break;
            }
        }

        private void ShowAdminFrame()
        {
            AdminFrame adminFrame = new AdminFrame(txtEmail.Text);
            adminFrame.Show();
        }

        private void ShowCustomerFrame()
        {
            CustomerFrame customerFrame = new CustomerFrame(txtEmail.Text);
            customerFrame.Show();
        }

        private void ShowRoleError()
        {
            MessageBox.Show(this, "Error: Unrecognized role", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowLoginFailedMessage(string message)
        {
            MessageBox.Show(this, message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void HandleException(Exception ex, string message)
        {
            Console.WriteLine($"{message} Exception: {ex.Message}");
            MessageBox.Show(this, $"{message} See console for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnSignup_Click(object sender, EventArgs e)
        {
            SignUpFrame signUpFrame = new SignUpFrame(_accountFactory);
            signUpFrame.Show();
        }
    }

    public interface IAuthenticationService
    {
        bool Authenticate(string username, string password);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly string _userDataFilePath = "userData.txt";

        public bool Authenticate(string username, string password)
        {
            try
            {
                using (StreamReader reader = new StreamReader(_userDataFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] userInfo = line.Split(',');
                        if (userInfo.Length >= 3 && userInfo[1] == username && userInfo[2] == password)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                throw new IOException("Error reading user data file.", ex);
            }
            return false;
        }
    }

    public interface IRoleService
    {
        string GetUserRole(string username);
    }

    public class RoleService : IRoleService
    {
        private readonly string _userDataFilePath = "userData.txt";

        public string GetUserRole(string username)
        {
            try
            {
                using (StreamReader reader = new StreamReader(_userDataFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] userInfo = line.Split(',');
                        if (userInfo.Length >= 5 && userInfo[1] == username)
                        {
                            return userInfo[4];
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                throw new IOException("Error reading user data file.", ex);
            }
            return null;
        }
    }

    public class FileInitializer
    {
        private static readonly string _userDataFilePath = "userData.txt";

        public static void EnsureUserdataFileExists()
        {
            try
            {
                if (!File.Exists(_userDataFilePath))
                {
                    using (StreamWriter writer = new StreamWriter(_userDataFilePath))
                    {
                        writer.WriteLine("1,admin@example.com,admin,Admin,admin");
                    }
                }
            }
            catch (IOException ex)
            {
                throw new IOException("Error creating user data file.", ex);
            }
        }
    }
}
