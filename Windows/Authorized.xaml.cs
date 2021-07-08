using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Polyclinic.Windows
{
    public partial class Authorized : Window
    {
        MainWindow mainWindow;
        public Authorized()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string tboxLog = tboxLogin.Text, tboxPass = tboxPassword.Password;
            Пользователи user = null;

            if (CheckFields(ref tboxLog, ref tboxPass) == false)
                return;

            user = FindUser(user, tboxLog);

            if (CheckUser(ref user, ref tboxPass) == false)
                return;

            LogIn(ref user);
        }

        private static bool CheckFields(ref string tboxLog, ref string tboxPass)
        {
            if (string.IsNullOrEmpty(tboxLog))
            {
                MessageBox.Show(string.IsNullOrEmpty(tboxPass) ?
                    "Поля не заполнены" :
                    "Логин не заполнен",
                    "Ошибка");
                return false;
            }

            if (string.IsNullOrEmpty(tboxPass))
            {
                MessageBox.Show("Пароль не заполнен", "Ошибка");
                return false;
            }

            return true;
        }

        private static Пользователи FindUser(Пользователи user, string tboxLog)
        {
            Task.Run(() => 
            { 
                using (PolyclinicDBContext context = new PolyclinicDBContext())
                    user = context.Пользователи.AsNoTracking().AsParallel().FirstOrDefault(item => item.Логин == tboxLog);
            }).Wait();
            return user;
        }
        private bool CheckUser(ref Пользователи user, ref string tboxPass)
        {
            if (user == null)
            {
                MessageBox.Show("Пользователя с таким логином не существует", "Ошибка");
                return false;
            }

            if (user.Пароль != tboxPass)
            {
                MessageBox.Show("Неверный пароль", "Ошибка");
                return false;
            }

            return true;
        }

        private void LogIn(ref Пользователи user)
        {
            Properties.Settings.Default.UserRole = user.Роль;
            mainWindow = new MainWindow();
            mainWindow.Show();
            if (mainWindow.IsLoaded)
                this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (mainWindow == null)
                Environment.Exit(0);
        }
    }
}