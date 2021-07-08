using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Polyclinic.Pages
{
    /// <summary>
    /// Логика взаимодействия для Doctors.xaml
    /// </summary>
    public partial class Doctors : Page
    {
        public Doctors()
        {
            InitializeComponent();
            using (PolyclinicDBContext context = new PolyclinicDBContext())
                DoctorsDataGrid.ItemsSource = context.Врачи.AsNoTracking().AsParallel().ToList();
            Task task = new Task(() => 
            {
                if (Properties.Settings.Default.UserRole == "Пациент" || Properties.Settings.Default.UserRole == "Посетитель")
                {
                    Add.IsEnabled = false;
                    Change.IsEnabled = false;
                    Delete.IsEnabled = false;
                }
            });
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.StateWOW = true;
            NavigationService.Navigate(new DoctorsEdit());
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.StateWOW = false;
            DoctorsEdit doctorsEdit = new DoctorsEdit(DoctorsDataGrid.SelectedItem as Врачи);
            NavigationService.Navigate(doctorsEdit);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            using (PolyclinicDBContext context = new PolyclinicDBContext())
            {
                var doctor = DoctorsDataGrid.SelectedItem as Врачи;
                context.Entry(doctor).State = System.Data.Entity.EntityState.Deleted;
                context.Врачи.Remove(doctor);
                context.SaveChanges();
            }
            NavigationService.Refresh();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<Врачи> doctor;
            using (PolyclinicDBContext context = new PolyclinicDBContext())
            {
                var StrSearch = Search.Text.ToLower();
                if (!string.IsNullOrEmpty(StrSearch))
                {
                    doctor = context.Врачи.AsNoTracking().Where(doc => doc.Код_врача.ToString().Contains(StrSearch) ||
                    doc.Фамилия_врача.ToLower().Contains(StrSearch) ||
                    doc.Имя_врача.ToLower().Contains(StrSearch) ||
                    doc.Отчество_врача.ToLower().Contains(StrSearch) ||
                    doc.Специальность_врача.ToLower().Contains(StrSearch) ||
                    doc.Процент_отчисления_на_зарплату.ToString().Contains(StrSearch)).ToList();
                }
                else
                    doctor = context.Врачи.AsNoTracking().ToList();
            }
            DoctorsDataGrid.ItemsSource = doctor;
        }
    }
}
