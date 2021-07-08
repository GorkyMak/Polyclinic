using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Polyclinic.Pages.Edits
{
    /// <summary>
    /// Логика взаимодействия для PacientsJoinEdit.xaml
    /// </summary>
    public partial class PacientsJoinEdit : Page
    {
        PolyclinicDB polyclinicDB = new PolyclinicDB();
        StringBuilder EmptyLineName = new StringBuilder();
        bool EmptyLineBool;
        public PacientsJoinEdit()
        {
            InitializeComponent();
            tb1.IsEnabled = true;
            if (!Properties.Settings.Default.StateWOW)
                Edit1.Content = "Изменить";
            else
                Edit1.Content = "Добавить";
        }

        public PacientsJoinEdit(Приём_пациентов priem)
        {
            InitializeComponent();
            tb1.Text = priem.Код_приёма.ToString();
            tb2.Text = priem.Код_врача.ToString();
            tb3.Text = priem.Код_пациента.ToString();
            tb4.Text = priem.Дата_приема.ToString();
            tb5.Text = priem.Стоимость_приема.ToString();
            tb1.IsEnabled = false;
            if (!Properties.Settings.Default.StateWOW)
                Edit1.Content = "Изменить";
            else
                Edit1.Content = "Добавить";
        }

        private void Edit1_Click(object sender, RoutedEventArgs e)
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
                        var iddoc = Int32.Parse(tb2.Text);
                        var idpac = Int32.Parse(tb3.Text);
                        if (Properties.Settings.Default.StateWOW)
                        {
                            context.Приём_пациентов.Add(new Приём_пациентов
                            {
                                Код_приёма = id,
                                Код_врача = iddoc,
                                Код_пациента = idpac,
                                Дата_приема = DateTime.Parse(tb4.Text),
                                Стоимость_приема = decimal.Parse(tb5.Text)
                            });
                        }
                        else
                        {
                            var pacient = context.Приём_пациентов.AsParallel().FirstOrDefault(item => item.Код_приёма == id);
                            pacient.Код_врача = iddoc;
                            pacient.Код_пациента = idpac;
                            pacient.Дата_приема = DateTime.Parse(tb4.Text);
                            pacient.Стоимость_приема = decimal.Parse(tb5.Text);
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