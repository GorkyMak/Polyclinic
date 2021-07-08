using System;
using System.Windows;
using Polyclinic.Pages;
using Polyclinic.Windows;

namespace Polyclinic
{
    public partial class MainWindow : Window
    {
        OtchetWindow otchetWindow;
        Authorized authorized;
        public MainWindow()
        {
            InitializeComponent();
            if (Properties.Settings.Default.UserRole.ToString() == "Администратор")
            {
                btnCreateUser.Visibility = Visibility.Visible;
            }
            if (Properties.Settings.Default.UserRole.ToString() == "Врач" ||
                Properties.Settings.Default.UserRole.ToString() == "Администратор")
            {
                btnPacients.Visibility = Visibility.Visible;
                btnPacientsJoin.Visibility = Visibility.Visible;
                btnOtchet.Visibility = Visibility.Visible;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UserName.Content = "Вы вошли как \n" + Properties.Settings.Default.UserRole;
        }
        private void btnDoctors_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Uri("Pages/Doctors.xaml", UriKind.Relative));
        }

        private void btnPacientsJoin_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Uri("Pages/PacientsJoin.xaml", UriKind.Relative));
        }

        private void btnPacients_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Uri("Pages/Pacients.xaml", UriKind.Relative));
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            authorized = new Authorized();
            this.Close();
            authorized.Show();
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoForward)
                MainFrame.GoForward();
        }

        private void btnOtchet_Click(object sender, RoutedEventArgs e)
        {
            otchetWindow = new OtchetWindow();
            this.Close();
            otchetWindow.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (otchetWindow != null || authorized != null)
                return;
            Environment.Exit(0);
        }

        private void btnCreateUser_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CreateUser());
        }
    }
}