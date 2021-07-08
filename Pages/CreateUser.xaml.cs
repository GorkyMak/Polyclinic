using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Polyclinic.Pages
{
    /// <summary>
    /// Логика взаимодействия для CreateUser.xaml
    /// </summary>
    public partial class CreateUser : Page
    {
        bool EmptyLineBool;
        StringBuilder EmptyLineName = new StringBuilder();
        public CreateUser()
        {
            InitializeComponent();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            
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
                        var User = context.Пользователи.AsParallel().FirstOrDefault(user => user.Логин == tb1.Text);
                        if(User != null)
                        {
                            MessageBox.Show("Такой пользователь уже существует", "Ошибка");
                            return;
                        }
                        context.Пользователи.Add(new Пользователи
                        {
                            Логин = tb1.Text,
                            Пароль = tb2.Password,
                            Роль = (cmbRole.SelectedItem as ComboBoxItem).Content.ToString()
                        });

                        context.SaveChanges();

                        for (int i = 0; i < sp.Children.Count - 1; i++)
                        {
                            if (sp.Children[i] is TextBox)
                            {
                                (sp.Children[i] as TextBox).Text = string.Empty;
                            }
                            else if (sp.Children[i] is ComboBox)
                            {
                                (sp.Children[i] as ComboBox).SelectedIndex = 2;
                            }
                        }
                        MessageBox.Show("Пользователь успешно добавлен", "Результат");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
            finally
            {
                EmptyLineName.Clear();
                EmptyLineBool = false;
            }
        }
    }
}
