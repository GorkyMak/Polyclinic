using Polyclinic.Pages.Edits;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Polyclinic.Pages
{
    /// <summary>
    /// Логика взаимодействия для PacientsJoin.xaml
    /// </summary>
    public partial class PacientsJoin : Page
    {
        public PacientsJoin()
        {
            InitializeComponent();
            using (PolyclinicDBContext context = new PolyclinicDBContext())
                PacientJoinDataGrid.ItemsSource = context.Приём_пациентов.AsNoTracking().AsParallel().ToList();
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
            NavigationService.Navigate(new PacientsJoinEdit());
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.StateWOW = false;
            NavigationService.Navigate(new PacientsJoinEdit(PacientJoinDataGrid.SelectedItem as Приём_пациентов));
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            using (PolyclinicDBContext context = new PolyclinicDBContext())
            {
                var priem = PacientJoinDataGrid.SelectedItem as Приём_пациентов;
                context.Entry(priem).State = System.Data.Entity.EntityState.Deleted;
                context.Приём_пациентов.Remove(PacientJoinDataGrid.SelectedItem as Приём_пациентов);
                context.SaveChanges();
            }
            NavigationService.Refresh();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<Приём_пациентов> doctor;
            using (PolyclinicDBContext context = new PolyclinicDBContext())
            {
                var StrSearch = Search.Text;
                if (!string.IsNullOrEmpty(StrSearch))
                {
                    doctor = context.Приём_пациентов.AsNoTracking().Where(doc => doc.Код_приёма.ToString().Contains(StrSearch) ||
                    doc.Код_врача.ToString().Contains(StrSearch) ||
                    doc.Код_приёма.ToString().Contains(StrSearch) ||
                    doc.Дата_приема.ToString().Contains(StrSearch) ||
                    doc.Стоимость_приема.ToString().Contains(StrSearch)).ToList();
                }
                else
                    doctor = context.Приём_пациентов.AsNoTracking().ToList();
            }
            PacientJoinDataGrid.ItemsSource = doctor;
        }
    }
}
