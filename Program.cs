using System;
using System.Windows.Forms;

namespace fnAsm2_4902_xx
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Khởi tạo các đối tượng cần thiết
            IAuthenticationService authService = new AuthenticationService();
            IRoleService roleService = new RoleService();
            IAccountFactory accountFactory = new AccountFactory();

            // Khởi tạo và hiển thị form đăng nhập
            LoginFrame loginFrame = new LoginFrame(authService, roleService, accountFactory);
            Application.Run(loginFrame);
        }
    }
}
