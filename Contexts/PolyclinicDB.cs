using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Polyclinic
{

    class PolyclinicDB
    {
        SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.MyPolyclinic);
        public DataTable View(string inform, DataGrid dataGrid)
        {
            try
            {
                DataTable dataTable = new DataTable();
                sqlConnection.Open();
                if (sqlConnection.State == ConnectionState.Open)
                {
                    SqlCommand sqlCommand = new SqlCommand(inform, sqlConnection);
                    sqlCommand.ExecuteNonQuery();
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    sqlDataAdapter.Fill(dataTable);
                    sqlConnection.Close();
                }
                else
                    return null;
                dataGrid.ItemsSource = dataTable.DefaultView;
                return dataTable;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
                sqlConnection.Close();
                return null;
            }
        }
        public void EditData(string edit)
        {
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State == ConnectionState.Open)
                {
                    SqlCommand add = new SqlCommand(edit, sqlConnection);
                    add.ExecuteNonQuery();

                    sqlConnection.Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }
    }
}