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
using System.Data;
using System.Data.SqlClient;

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataTable dt = new DataTable();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void OnPublisherSelect(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            string conn = @"Data Source=DESKTOP-P8RS0PR\SQLEXPRESS;Initial Catalog=Books;Integrated Security=True";
            using (SqlConnection sc = new SqlConnection(conn))
            {
                sc.Open();
                SqlCommand comm = new SqlCommand(
                    "SELECT * FROM izdatel_key", sc);
                SqlDataAdapter da = new SqlDataAdapter(comm);
                da.Fill(dt);
            }
            publishersDataGrid.BeginInit();
            publishersDataGrid.DataContext = dt;
            publishersDataGrid.Items.Refresh();
            publishersDataGrid.EndInit();
        }
        private void OnAuthorsSelect(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            string conn = @"Data Source=DESKTOP-P8RS0PR\SQLEXPRESS;Initial Catalog=Books;Integrated Security=True";
            using (SqlConnection sc = new SqlConnection(conn))
            {
                sc.Open();
                SqlCommand comm = new SqlCommand(
                    "SELECT * FROM keys_name_author", sc);

                SqlDataAdapter da = new SqlDataAdapter(comm);
                da.Fill(dt);
            }
            authorsDataGrid.BeginInit();
            authorsDataGrid.DataContext = dt;
            authorsDataGrid.Items.Refresh();
            authorsDataGrid.EndInit();
        }

        private void dataGrid1_AutoGeneratingColumn(object sender, Microsoft.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            var col = e.Column.Header.ToString();
            if (( col == "Key_book") || (col == "Key_IZD") || (col == "Authors_keys"))
                e.Column.Visibility = Visibility.Collapsed;
        }
        public void UpdateDB()
        {

            string conn = @"Data Source=DESKTOP-P8RS0PR\SQLEXPRESS;Initial Catalog=Books;Integrated Security=True";
            dt = new DataTable();
            using (SqlConnection sc = new SqlConnection(conn))
            {
                SqlCommand comm = new SqlCommand(
                    "SELECT * FROM View_1", sc);
                SqlDataAdapter da = new SqlDataAdapter(comm);
                da.Fill(dt);
            }
            dataGrid1.BeginInit();
            dataGrid1.DataContext = dt;
            dataGrid1.Items.Refresh();
            dataGrid1.EndInit();
        }


        private void change_Click_1(object sender, RoutedEventArgs e)
        {
            var selected = (DataRowView)dataGrid1.SelectedItems[0];
            //MessageBox.Show(selected[0].ToString());
            if (selected != null)
            {
                WpfApplication1.Authors win1 = new WpfApplication1.Authors(selected);
                win1.Show();
            }
           // win1.year = 

        }

        private void insert_Click(object sender, RoutedEventArgs e)
        {

            WpfApplication1.Authors win1 = new WpfApplication1.Authors();
            win1.Show();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {

            var das = dataGrid1.SelectedItems[0] as DataRowView;

            string conn = @"Data Source=DESKTOP-P8RS0PR\SQLEXPRESS;Initial Catalog=Books;Integrated Security=True";
            using (SqlConnection sc = new SqlConnection(conn))
            {
                sc.Open();
                var key = (int)dt.Rows[dataGrid1.SelectedIndex][0];
                SqlCommand command = new SqlCommand("DELETE FROM Author_Books_Keys WHERE Book_Keys=@param", sc);
                SqlParameter parameter = new SqlParameter("@param", SqlDbType.Int);
                parameter.Value = key;
                command.Parameters.Add(parameter);
                command.ExecuteNonQuery();

                command = new SqlCommand("DELETE FROM Main_Table WHERE Key_book=@param", sc);
                parameter = new SqlParameter("@param", SqlDbType.Int);
                parameter.Value = key;
                command.Parameters.Add(parameter);
                command.ExecuteNonQuery();
                command = new SqlCommand(
                    "SELECT * FROM View_1", sc);
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dt);
                UpdateDB();
            }


        }

        private void authorsDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Delete) && (DataRowView)authorsDataGrid.SelectedItems[0] != null)
            {
                try
                {
                    var dt = authorsDataGrid.DataContext as DataTable;
                    string conn = @"Data Source=DESKTOP-P8RS0PR\SQLEXPRESS;Initial Catalog=Books;Integrated Security=True";
                    using (SqlConnection sc = new SqlConnection(conn))
                    {
                        sc.Open();
                        var key = (int)dt.Rows[authorsDataGrid.SelectedIndex][0];
                        SqlCommand command = new SqlCommand("DELETE FROM keys_name_author WHERE keys_author=@param", sc);
                        SqlParameter parameter = new SqlParameter("@param", SqlDbType.Int);
                        parameter.Value = key;
                        command.Parameters.Add(parameter);
                        command.ExecuteNonQuery();
                    }
                }
                catch { MessageBox.Show("Невозможно удалить данную запись."); }
            }
        }
        private void UpdatePublishersDB()
        {
            var dt1 = publishersDataGrid.DataContext as DataTable;
            string conn = @"Data Source=DESKTOP-P8RS0PR\SQLEXPRESS;Initial Catalog=Books;Integrated Security=True";
            if (dt1 != null)
            {
                using (var sc = new SqlConnection(conn))
                {
                    var comm = new SqlCommand("SELECT * FROM izdatel_key", sc);
                    var dataAdapter = new SqlDataAdapter(comm);
                    SqlCommandBuilder sb = new SqlCommandBuilder(dataAdapter);
                    try
                    {
                        dataAdapter.Update(dt1);
                    }
                    catch { MessageBox.Show("error"); }
                }
            }
        }
        private void UpdateAuthorsDB()
        {
            var dt1 = authorsDataGrid.DataContext as DataTable;
            string conn = @"Data Source=DESKTOP-P8RS0PR\SQLEXPRESS;Initial Catalog=Books;Integrated Security=True";
            if (dt1 != null)
            {
                using (var sc = new SqlConnection(conn))
                {
                    var comm = new SqlCommand("SELECT * FROM keys_name_author", sc);
                    var dataAdapter = new SqlDataAdapter(comm);
                    SqlCommandBuilder sb = new SqlCommandBuilder(dataAdapter);
                    try
                    {
                        dataAdapter.Update(dt1);
                    }
                    catch { MessageBox.Show("error"); }
                }
            }
        }
        private void publishersDataGrid_RowEditEnding(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            (e.Row.Item as DataRowView).EndEdit();
            UpdatePublishersDB();
            
        }

        private void authorsDataGrid_RowEditEnding(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            (e.Row.Item as DataRowView).EndEdit();
            UpdateAuthorsDB();
        }
        private void OnBooksSelect(object sender, RoutedEventArgs e)
        {
            UpdateDB();
        }

        private void authorsDataGrid_AutoGeneratingColumn(object sender, Microsoft.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            var col = e.Column.Header.ToString();
            if (col == "keys_author")
                e.Column.Visibility = Visibility.Collapsed;
        }

        private void publishersDataGrid_AutoGeneratingColumn(object sender, Microsoft.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            var col = e.Column.Header.ToString();
            if (col == "Key_IZD")
                e.Column.Visibility = Visibility.Collapsed;
        }

        private void publishersDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Delete) && (DataRowView)publishersDataGrid.SelectedItems[0] != null)
            {
                try
                {
                    var dt = publishersDataGrid.DataContext as DataTable;
                    string conn = @"Data Source=DESKTOP-P8RS0PR\SQLEXPRESS;Initial Catalog=Books;Integrated Security=True";
                    using (SqlConnection sc = new SqlConnection(conn))
                    {
                        sc.Open();
                        var key = (int)dt.Rows[publishersDataGrid.SelectedIndex][0];
                        SqlCommand command = new SqlCommand("DELETE FROM izdatel_key WHERE Key_IZD=@param", sc);
                        SqlParameter parameter = new SqlParameter("@param", SqlDbType.Int);
                        parameter.Value = key;
                        command.Parameters.Add(parameter);
                        command.ExecuteNonQuery();
                    }
                }
                catch { MessageBox.Show("Невозможно удалить данную запись."); }
            }
        }
    }
}

