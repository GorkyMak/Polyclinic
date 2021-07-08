using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Polyclinic.Pages.Edits
{
    /// <summary>
    /// Логика взаимодействия для PacientsEdit.xaml
    /// </summary>
    public partial class PacientsEdit : Page
    {
        PolyclinicDB polyclinicDB = new PolyclinicDB();
        StringBuilder EmptyLineName = new StringBuilder();
        bool EmptyLineBool;
        public PacientsEdit()
        {
            InitializeComponent();
            tb1.IsEnabled = true;
            if (!Properties.Settings.Default.StateWOW)
                Edit.Content = "Изменить";
            else
                Edit.Content = "Добавить";
        }

        public PacientsEdit(Пациенты pacient)
        {
            InitializeComponent();
            tb1.Text = pacient.Код_пациента.ToString();
            tb2.Text = pacient.Фамилия_пациента.ToString();
            tb3.Text = pacient.Имя_пациента.ToString();
            tb4.Text = pacient.Отчество_пациента.ToString();
            tb5.Text = pacient.Дата_рождения_пациента.Value.ToShortDateString();
            tb6.Text = pacient.Адрес_пациента.ToString();
            tb1.IsEnabled = false;
            if (!Properties.Settings.Default.StateWOW)
                Edit.Content = "Изменить";
            else
                Edit.Content = "Добавить";
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            EmptyLineName.Clear();
            EmptyLineBool = false;
            try
            {
                for (int i = 0; i < sp.Children.Count - 1; i++)
                {
                    if (sp.Children[i] is TextBox)
                        if ((sp.Children[i] as TextBox).Text == "")
                        {
                            if (EmptyLineBool == true)
                                EmptyLineName.Append(", ");
                            EmptyLineName.Append((sp.Children[i - 1] as Label).Content.ToString());
                            EmptyLineBool = true;
                        }
                }
                using (PolyclinicDBContext context = new PolyclinicDBContext())
                {
                    if (EmptyLineBool == true)
                    {
                        MessageBox.Show("Введите: " + EmptyLineName, "Ошибка. Не все поля заполнены");
                    }
                    else
                    {
                        var id = Int32.Parse(tb1.Text);
                        if (Properties.Settings.Default.StateWOW)
                        {
                            context.Пациенты.Add(new Пациенты
                            {
                                Код_пациента = id,
                                Фамилия_пациента = tb2.Text,
                                Имя_пациента = tb3.Text,
                                Отчество_пациента = tb4.Text,
                                Дата_рождения_пациента = DateTime.Parse(tb5.Text),
                                Адрес_пациента = tb6.Text
                            });
                        }
                        else
                        {
                            var pacient = context.Пациенты.AsParallel().FirstOrDefault(item => item.Код_пациента == id);
                            pacient.Фамилия_пациента = tb2.Text;
                            pacient.Имя_пациента = tb3.Text;
                            pacient.Отчество_пациента = tb4.Text;
                            pacient.Дата_рождения_пациента = DateTime.Parse(tb5.Text);
                            pacient.Адрес_пациента = tb6.Text;
                        }
                        context.SaveChanges();
                        NavigationService.GoBack();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }
    }
}
