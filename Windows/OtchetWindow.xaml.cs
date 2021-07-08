using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Word;
using System.IO;
using Microsoft.Win32;
using System.Data;
using System.Diagnostics;
using Polyclinic.Entities;

namespace Polyclinic.Windows
{
    public partial class OtchetWindow : System.Windows.Window
    {
        MainWindow Window;
        word.Application app;
        word.Document doc;
        System.Threading.Tasks.Task TaskCreateNewApp;
        string[] columns = new string[] { "Код приёма","Фамилия врача", "Имя врача",
        "Отчество врача", "Специальность врача", "Процент отчисления на зарплату",
        "Фамилия пациента", "Имя пациента", "Отчество пациента",
        "Дата рождения пациента", "Адрес пациента", "Дата приема",
        "Стоимость приема", "Зарплата" };
        List<TableOtchet> tableOtchet;
        ExcelHelper excelHelper;
        public OtchetWindow()
        {
            InitializeComponent();
            using (PolyclinicDBContext context = new PolyclinicDBContext())
            {
                var result = from priem in context.Приём_пациентов.AsNoTracking().AsParallel()
                             join doc in context.Врачи.AsNoTracking().AsParallel() on priem.Код_врача equals doc.Код_врача
                             join pacient in context.Пациенты.AsNoTracking().AsParallel() on priem.Код_пациента equals pacient.Код_пациента
                             select new TableOtchet
                             {
                                 Код_приёма = priem.Код_приёма,
                                 Фамилия_врача = doc.Фамилия_врача,
                                 Имя_врача = doc.Имя_врача,
                                 Отчество_врача = doc.Отчество_врача,
                                 Специальность_врача = doc.Специальность_врача,
                                 Процент_отчисления_на_зарплату = doc.Процент_отчисления_на_зарплату,
                                 Фамилия_пациента = pacient.Фамилия_пациента,
                                 Имя_пациента = pacient.Имя_пациента,
                                 Отчество_пациента = pacient.Отчество_пациента,
                                 Дата_рождения_пациента = pacient.Дата_рождения_пациента,
                                 Адрес_пациента = pacient.Адрес_пациента,
                                 Дата_приема = priem.Дата_приема,
                                 Стоимость_приема = priem.Стоимость_приема,
                                 Зарплата = priem.Зарплата
                             };
                dgOtchet.ItemsSource = tableOtchet = result.OrderBy(item => item.Дата_приема).ToList();
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Window = new MainWindow();
        }
        void Save(System.Data.DataTable dataTable, string tbOtchetSelectedDate, string tbOtchet1SelectedDate,
            Stopwatch stopwatch)
        {

            stopwatch.Start();
            word.Bookmarks bookmarks = null;

            SaveFileDialog savedialog = new SaveFileDialog();
            savedialog.Title = "Сохранить файл как...";
            savedialog.Filter = "Документ Word (*.docx)|*.docx|" +
                "Документ Word 97-2003 (*.doc)|*.doc|" +
                "PDF (*.pdf)|*.pdf|" +
                "Текст в формате RTF (*.rtf)|*.rtf|" +
                "Текст OpenDocument (*.odt)|*.odt";

            try
            {
                TaskCreateNewApp = System.Threading.Tasks.Task.Run(() =>
                {
                    app = new word.Application();
                });

                stopwatch.Stop();
                if (savedialog.ShowDialog() == false)
                {
                    if (doc != null)
                    {
                        doc.Close(false);
                        doc = null;
                    }
                    TaskCreateNewApp.Wait();
                    app.Quit();
                    return;
                }
                stopwatch.Start();

                TaskCreateNewApp.Wait();

                string source = $@"{Directory.GetCurrentDirectory()}/Шаблон отчёта.docx";
                doc = app.Documents.Open(source);
                doc.Activate();
                bookmarks = doc.Bookmarks;
                word.Range range = bookmarks["Таблица"].Range;

                var Table = doc.Tables.Add(range, dataTable.Rows.Count + 1,
                    dataTable.Columns.Count, 14);

                Parallel.Invoke(() =>
                {
                    Parallel.For(1, dataTable.Columns.Count + 1, (int i) =>
                    {
                        Table.Cell(1, i).Range.Text = columns[i - 1];
                    });
                },
                () =>
                {
                    Parallel.For(0, dataTable.Rows.Count, (int i) =>
                    {
                        Parallel.For(0, dataTable.Columns.Count, (int j) =>
                        {
                            Table.Cell(i + 2, j + 1).Range.Text = dataTable.Rows[i].ItemArray.ElementAt(j).ToString();
                        });
                    });
                },
                () =>
                {
                    bookmarks["Дата"].Range.Text += $" {DateTime.Now.ToShortDateString()}";
                });

                if (tbOtchetSelectedDate != null && tbOtchet1SelectedDate != null)
                {
                    bookmarks["Отчёт"].Range.Text += " по диапазону дат с " + tbOtchetSelectedDate + " по " + tbOtchet1SelectedDate;
                }
                else if (tbOtchetSelectedDate != null && tbOtchet1SelectedDate == null)
                {
                    bookmarks["Отчёт"].Range.Text += " по диапазону дат с " + tbOtchetSelectedDate + " по текущей";
                }
                else if (tbOtchetSelectedDate == null && tbOtchet1SelectedDate != null)
                {
                    bookmarks["Отчёт"].Range.Text += " по диапазону дат с самой ранней по " + tbOtchet1SelectedDate;
                }
                else if (tbOtchetSelectedDate == null && tbOtchet1SelectedDate == null)
                {
                    bookmarks["Отчёт"].Range.Text += " по всем датам";
                }

                if (doc != null && app != null)
                {
                    string format = savedialog.FileName.Substring(savedialog.FileName.Length - 4);

                    stopwatch.Stop();
                    if (format == ".pdf")
                        doc.SaveAs2(savedialog.FileName, WdSaveFormat.wdFormatPDF);
                    else if (format == ".doc")
                        doc.SaveAs2(savedialog.FileName, WdSaveFormat.wdFormatDocument);
                    else if (format == ".rtf")
                        doc.SaveAs2(savedialog.FileName, WdSaveFormat.wdFormatRTF);
                    else if (format == ".odt")
                        doc.SaveAs2(savedialog.FileName, WdSaveFormat.wdFormatOpenDocumentText);
                    else
                        doc.SaveAs2(savedialog.FileName);
                    MessageBox.Show($"Отчёт успешно создан! \nВремя затраченное на выполнение алгоритма - {stopwatch.Elapsed.TotalSeconds}", "Результат");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
                
            }
            finally
            {
                if (doc != null && app != null)
                {
                    doc.Close(false);
                    doc = null;
                    app.Quit();
                }
            }
        }
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            System.Data.DataTable dataTable = GetDataTable();

            tbOtchet.IsEnabled = false;
            tbOtchet1.IsEnabled = false;
            Search.IsEnabled = false;

            string tbOtchetSelectedDate = tbOtchet.SelectedDate?.ToShortDateString();
            string tbOtchet1SelectedDate = tbOtchet1.SelectedDate?.ToShortDateString();
            stopwatch.Stop();
            await System.Threading.Tasks.Task.Run(() => Save(dataTable, tbOtchetSelectedDate,
                tbOtchet1SelectedDate, stopwatch));

            tbOtchet.IsEnabled = true;
            tbOtchet1.IsEnabled = true;
            Search.IsEnabled = true;
        }

        private System.Data.DataTable GetDataTable()
        {
            System.Data.DataTable dataTable = new System.Data.DataTable();
            TableOtchet dr;
            DataRow dataRow;

            while (dataTable.Columns.Count < dgOtchet.Columns.Count)
                dataTable.Columns.Add();

            for (int i = 0; i < dgOtchet.Items.Count; i++)
            {
                dr = dgOtchet.Items[i] as TableOtchet;

                dataRow = dataTable.NewRow();
                dataRow.ItemArray = new string[14]
                {
                    dr.Код_приёма.ToString(),
                    dr.Фамилия_врача,
                    dr.Имя_врача,
                    dr.Отчество_врача,
                    dr.Специальность_врача,
                    dr.Процент_отчисления_на_зарплату.ToString(),
                    dr.Фамилия_пациента,
                    dr.Имя_пациента,
                    dr.Отчество_пациента,
                    dr.Дата_рождения_пациента.Value.ToShortDateString(),
                    dr.Адрес_пациента,
                    dr.Дата_приема.Value.ToShortDateString(),
                    dr.Стоимость_приема.ToString().Replace(',','.'),
                    dr.Зарплата.ToString().Replace(',','.')
                };

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        private void SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            using (PolyclinicDBContext context = new PolyclinicDBContext())
            {
                try
                {
                    List<TableOtchet> priem = new List<TableOtchet>();

                    if (tbOtchet.SelectedDate != null && tbOtchet1.SelectedDate != null)
                    {
                        priem = tableOtchet.Where(item =>
                            item.Дата_приема >= tbOtchet.SelectedDate.Value &&
                            item.Дата_приема <= tbOtchet1.SelectedDate.Value)
                            .OrderBy(item => item.Дата_приема).ToList();
                    }
                    else if (tbOtchet.SelectedDate != null && tbOtchet1.SelectedDate == null)
                    {
                        priem = tableOtchet.Where(item =>
                            item.Дата_приема >= tbOtchet.SelectedDate.Value)
                            .OrderBy(item => item.Дата_приема).ToList();
                    }
                    else if (tbOtchet.SelectedDate == null && tbOtchet1.SelectedDate != null)
                    {
                        priem = tableOtchet.Where(item =>
                            item.Дата_приема <= tbOtchet1.SelectedDate.Value)
                            .OrderBy(item => item.Дата_приема).ToList();
                    }
                    else if (tbOtchet.SelectedDate == null && tbOtchet1.SelectedDate == null)
                    {
                        priem = tableOtchet.OrderBy(item => item.Дата_приема).ToList();
                    }
                    dgOtchet.ItemsSource = priem;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка");
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Window.Show();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<TableOtchet> doctor;
            using (PolyclinicDBContext context = new PolyclinicDBContext())
            {
                var StrSearch = Search.Text;
                if (!string.IsNullOrEmpty(StrSearch))
                {
                    doctor = tableOtchet.Where(doc => doc.Код_приёма.ToString() == StrSearch
                    //||
                    //doc.Фамилия_врача.ToString().ToLower().Contains(StrSearch) ||
                    //doc.Имя_врача.ToString().ToLower().Contains(StrSearch) ||
                    //doc.Отчество_врача.ToString().ToLower().Contains(StrSearch) ||
                    //doc.Специальность_врача.ToString().ToLower().Contains(StrSearch) ||
                    //doc.Процент_отчисления_на_зарплату.ToString().Contains(StrSearch) ||
                    //doc.Фамилия_пациента.ToString().ToLower().Contains(StrSearch) ||
                    //doc.Имя_пациента.ToString().ToLower().Contains(StrSearch) ||
                    //doc.Отчество_пациента.ToString().ToLower().Contains(StrSearch) ||
                    //doc.Дата_рождения_пациента.ToString().Contains(StrSearch) ||
                    //doc.Адрес_пациента.ToString().Contains(StrSearch) ||
                    //doc.Дата_приема.ToString().Contains(StrSearch) ||
                    //doc.Стоимость_приема.ToString().Contains(StrSearch) ||
                    //doc.Зарплата.ToString().Contains(StrSearch)
                    ).ToList();
                }
                else
                    doctor = tableOtchet;
            }
            dgOtchet.ItemsSource = doctor;
        }

        private void btnSaveExcel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog savedialog = new SaveFileDialog();
            savedialog.Title = "Сохранить файл как..";
            savedialog.OverwritePrompt = false;
            savedialog.Filter = "Книга Excel (*.xlsx)|*.xlsx";
            if (dgOtchet.ItemsSource != null)
                if (savedialog.ShowDialog() == true)
                {
                    excelHelper = new ExcelHelper();
                    System.Data.DataTable dataTable = GetDataTable();
                    excelHelper.Write(dataTable, columns, savedialog.FileName);
                }
        }
    }
}