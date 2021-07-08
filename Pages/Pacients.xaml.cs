using Polyclinic.Pages.Edits;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Polyclinic.Pages
{
    /// <summary>
    /// Логика взаимодействия для Pacients.xaml
    /// </summary>
    public partial class Pacients : Page
    {
        public Pacients()
        {
            InitializeComponent();
            using (PolyclinicDBContext context = new PolyclinicDBContext())
                PacientsDataGrid.ItemsSource = context.Пациенты.AsNoTracking().AsParallel().ToList();
            if (Properties.Settings.Default.UserRole == "Пациент" || Properties.Settings.Default.UserRole == "Посетитель")
            {
                Add.IsEnabled = false;
                Change.IsEnabled = false;
                Delete.IsEnabled = false;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.StateWOW = true;
            NavigationService.Navigate(new PacientsEdit());
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.StateWOW = false;
            NavigationService.Navigate(new PacientsEdit(PacientsDataGrid.SelectedItem as Пациенты));
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            using (PolyclinicDBContext context = new PolyclinicDBContext())
            {
                var pacient = PacientsDataGrid.SelectedItem as Пациенты;
                context.Entry(pacient).State = System.Data.Entity.EntityState.Deleted;
                context.Пациенты.Remove(pacient);
                context.SaveChanges();
            }
            NavigationService.Refresh();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<Пациенты> doctor;
            using (PolyclinicDBContext context = new PolyclinicDBContext())
            {
                var StrSearch = Search.Text.ToLower();
                if (!string.IsNullOrEmpty(StrSearch))
                {
                    doctor = context.Пациенты.AsNoTracking().Where(doc => doc.Код_пациента.ToString().Contains(StrSearch) ||
                    doc.Фамилия_пациента.ToLower().Contains(StrSearch) ||
                    doc.Имя_пациента.ToLower().Contains(StrSearch) ||
                    doc.Отчество_пациента.ToLower().Contains(StrSearch) ||
                    doc.Дата_рождения_пациента.ToString().Contains(StrSearch) ||
                    doc.Адрес_пациента.ToString().ToLower().Contains(StrSearch)).ToList();
                }
                else
                    doctor = context.Пациенты.AsNoTracking().ToList();
            }
            PacientsDataGrid.ItemsSource = doctor;
        }
    }
}
