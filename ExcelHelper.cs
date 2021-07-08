using System;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace Polyclinic
{
    class ExcelHelper
    {
        Excel.Application application;
        Task CreateNewApp;
        Excel.Workbook workbook;
        Excel.Worksheet sheets;
        DataTable ReadDataTable = new DataTable();

        public ExcelHelper()
        {
            if (application == null)
                CreateNewApp = Task.Run(delegate ()
                {
                    application = new Excel.Application();
                });
        }

        public void CloseExcelFile()
        {
            application.Quit();
            Marshal.ReleaseComObject(sheets);
            Marshal.ReleaseComObject(workbook);
            Marshal.ReleaseComObject(application);
        }

        public async void Read(string Path, int Sheet)
        {
            try
            {
                await Task.Run(delegate ()
                {
                    CreateNewApp.Wait();
                    workbook = application.Workbooks.Open(Path);
                    sheets = workbook.Sheets[Sheet];
                    for (int i = 0; i < sheets.UsedRange.Columns.Count; i++)
                        ReadDataTable.Columns.Add();
                    for (int i = 1; i <= sheets.UsedRange.Rows.Count; i++)
                    {
                        DataRow dataRow = ReadDataTable.NewRow();
                        Parallel.For(1, sheets.UsedRange.Columns.Count + 1, delegate (int j)
                        {
                            if (sheets.Cells[i, j].Value != null)
                                dataRow[j - 1] = sheets.Cells[i, j].Value.ToString();
                        });
                        ReadDataTable.Rows.Add(dataRow);
                    }
                    workbook.Close(false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        public async void Write(DataTable dataTable, string Path)
        {

            try
            {
                await Task.Run(delegate ()
                {
                    CreateNewApp.Wait();
                    workbook = application.Workbooks.Add();
                    sheets = workbook.ActiveSheet;
                    for (int i = 1; i <= dataTable.Rows.Count; i++)
                    {
                        for (int j = 1; j <= dataTable.Columns.Count; j++)
                        {
                            string dataRow = dataTable.Rows[i - 1].ItemArray[j - 1].ToString();
                            sheets.Cells[i, j] = dataRow;
                            (sheets.Columns[j] as Excel.Range).AutoFit();
                        }
                    }
                    workbook.SaveAs(Path);
                    workbook.Close(false);
                    MessageBox.Show("Файл успешно сохранён!", "Результат");
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        public async void Write(DataTable dataTable, string[] columns, string path)
        {
            try
            {
                await Task.Run(delegate ()
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    CreateNewApp.Wait();
                    workbook = application.Workbooks.Add();
                    sheets = workbook.ActiveSheet;

                    Parallel.Invoke(() =>
                    {
                        Parallel.For(1, dataTable.Columns.Count + 1, (int j) =>
                        {
                            sheets.Cells[1, j] = columns[j - 1];
                            (sheets.Columns[j] as Excel.Range).AutoFit();
                        });
                    },
                    () =>
                    {
                        Parallel.For(2, dataTable.Rows.Count + 2, (int i) =>
                        {
                            Parallel.For(1, dataTable.Columns.Count + 1, (int j) =>
                            {
                                sheets.Cells[i, j] = dataTable.Rows[i - 2].ItemArray[j - 1].ToString();
                                (sheets.Columns[j] as Excel.Range).AutoFit();
                            });
                        });
                    });

                    Excel.Range Range = sheets.UsedRange;
                    Range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    Range.Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

                    Excel.ChartObjects chartObjs = (Excel.ChartObjects)sheets.ChartObjects();
                    Excel.ChartObject chartObj = chartObjs.Add(1500, 10, 300, 200);
                    Excel.Chart xlChart = chartObj.Chart;
                    //Range = sheets.Range[$"M1:N{dataTable.Rows.Count + 1}"];

                    Excel.SeriesCollection seriesCollection = (Excel.SeriesCollection)xlChart.SeriesCollection(Type.Missing);
                    Excel.Series series = seriesCollection.NewSeries();
                    series.XValues = sheets.get_Range("B2", $"B{dataTable.Columns.Count}");
                    series.Values = sheets.get_Range("N2", $"N{dataTable.Rows.Count + 1}");

                    xlChart.ChartType = Excel.XlChartType.xlLine;
                    series.Name = "Зарплата";
                    //xlChart.SetSourceData(Range);
                    xlChart.HasLegend = true;
                    
                    stopwatch.Stop();

                    workbook.SaveAs(path);

                    workbook.Close(false);
                    CloseExcelFile();

                    MessageBox.Show($"Файл успешно сохранён!\nВремя затраченное на выполнение алгоритма - {stopwatch.Elapsed.TotalSeconds}", "Результат");
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }
    }
}
