using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Polyclinic.Pages
{
    /// <summary>
    /// Логика взаимодействия для DoctorsEdit.xaml
    /// </summary>
    public partial class DoctorsEdit : Page
    {
        PolyclinicDB polyclinicDB = new PolyclinicDB();
        StringBuilder EmptyLineName = new StringBuilder();
        bool EmptyLineBool;
        public DoctorsEdit()
        {
            InitializeComponent();
            tb1.IsEnabled = true;
            if (!Properties.Settings.Default.StateWOW)
                Edit.Content = "Изменить";
            else
                Edit.Content = "Добавить";
        }
        public DoctorsEdit(Врачи doctor)
        {
            InitializeComponent();
            tb1.Text = doctor.Код_врача.ToString();
            tb2.Text = doctor.Фамилия_врача.ToString();
            tb3.Text = doctor.Имя_врача.ToString();
            tb4.Text = doctor.Отчество_врача.ToString();
            tb5.Text = doctor.Специальность_врача.ToString();
            tb6.Text = doctor.Процент_отчисления_на_зарплату.ToString();
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
                                context.Врачи.Add(new Врачи
                                {
                                    Код_врача = id,
                                    Фамилия_врача = tb2.Text,
                                    Имя_врача = tb3.Text,
                                    Отчество_врача = tb4.Text,
                                    Специальность_врача = tb5.Text,
                                    Процент_отчисления_на_зарплату = Int32.Parse(tb6.Text)
                                });
                        }
                        else
                        {
                            var doctor = context.Врачи.AsParallel().FirstOrDefault(item => item.Код_врача == id);
                            doctor.Фамилия_врача = tb2.Text;
                            doctor.Имя_врача = tb3.Text;
                            doctor.Отчество_врача = tb4.Text;
                            doctor.Специальность_врача = tb5.Text;
                            doctor.Процент_отчисления_на_зарплату = Int32.Parse(tb6.Text);
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
