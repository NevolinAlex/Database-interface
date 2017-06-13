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
    /// Interaction logic for Authors.xaml
    /// </summary>
    public partial class Authors : Window
    {
        public List<int> authorKeys = new List<int>();
        public bool isUpdate = false;
        public int globalKey;
        private string conn = @"Data Source=DESKTOP-P8RS0PR\SQLEXPRESS;Initial Catalog=Books;Integrated Security=True";

        public Authors()
        {
            InitializeComponent();

        }
        public Authors(DataRowView dr){
            InitializeComponent();
            globalKey = Int32.Parse(dr[0].ToString());
            nameBook.Text = dr[1].ToString();
            price.Text = dr[3].ToString();
            year.Text = dr[2].ToString();
            publishersComboBox.SelectedValue = Int32.Parse(dr[6].ToString());
            authorsTextBox.Text = dr[5].ToString();
            foreach(var authorKey in dr[7].ToString().Split(','))
            {
                authorKeys.Add(Int32.Parse(authorKey));
            }
            
            isUpdate = true;
        }

        private void comboBox_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable authorsDataTable = new DataTable();
            using (SqlConnection sc = new SqlConnection(conn))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM dbo.keys_name_author", sc);
                SqlDataAdapter authorsDataAdapter = new SqlDataAdapter(command);
                authorsDataAdapter.Fill(authorsDataTable);
            }
            authorsComboBox.ItemsSource = authorsDataTable.DefaultView;

            DataTable publishersDataTable = new DataTable();
            using (SqlConnection sc = new SqlConnection(conn))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM dbo.izdatel_key", sc);
                SqlDataAdapter publishersDataAdapter = new SqlDataAdapter(command);
                publishersDataAdapter.Fill(publishersDataTable);
            }
            publishersComboBox.ItemsSource = publishersDataTable.DefaultView;
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            authorsTextBox.Text = "";
            MessageBox.Show(publishersComboBox.SelectedItem.ToString());
            authorKeys.Clear();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            var item = ((DataRowView)(authorsComboBox.SelectedItem))["name_author"].ToString();
            if (authorsTextBox.Text.Equals(string.Empty))
            {
                authorsTextBox.Text += item;
                authorKeys.Add((int)authorsComboBox.SelectedValue);
            }
            else
            {
                authorsTextBox.Text += "," + item;
                authorKeys.Add((int)authorsComboBox.SelectedValue);
            }
        }

        private void buttonCansel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private bool IsCorrectlyEnteredData()
        {
            int value;
            if (!Int32.TryParse(price.Text, out value))
            {
                price.Text = "";
                return false;
            }
            if (!Int32.TryParse(year.Text, out value))
            {
                year.Text = "";
                return false;
            }
            if (string.IsNullOrWhiteSpace(nameBook.Text)) //str.TrimStart();
            {
                nameBook.Text = "";
                return false;
            }
            nameBook.Text.TrimStart();
            return true;
        }
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if (IsCorrectlyEnteredData())
            {
                if (!isUpdate)
                {
                    using (SqlConnection sc = new SqlConnection(conn))
                    {
                        sc.Open();
                        SqlCommand comm = new SqlCommand("INSERT INTO Main_Table" +
                            "(Name_book, Key_izd, Year, Price) VALUES (@name,@keyPub,@year,@price);" +
                            " SELECT CAST(SCOPE_IDENTITY() AS INT)", sc);
                        SqlParameter parameter = new SqlParameter("@name", SqlDbType.NVarChar);
                        parameter.Value = nameBook.Text;
                        comm.Parameters.Add(parameter);

                        parameter = new SqlParameter("@keyPub", SqlDbType.Int);
                        parameter.Value = (int)publishersComboBox.SelectedValue;
                        comm.Parameters.Add(parameter);

                        parameter = new SqlParameter("@year", SqlDbType.Int);
                        parameter.Value = Int32.Parse(year.Text);
                        comm.Parameters.Add(parameter);

                        parameter = new SqlParameter("@price", SqlDbType.Int);
                        parameter.Value = Int32.Parse(price.Text);
                        comm.Parameters.Add(parameter);
                        int key = (Int32)comm.ExecuteScalar();

                        foreach (var authorKey in authorKeys)
                        {
                            comm = new SqlCommand("INSERT INTO Author_Books_Keys" +
                                "(Book_Keys, Author_Keys)VALUES (@param1,@param2)", sc);
                            parameter = new SqlParameter("@param1", SqlDbType.Int);
                            parameter.Value = key;
                            comm.Parameters.Add(parameter);

                            parameter = new SqlParameter("@param2", SqlDbType.Int);
                            parameter.Value = authorKey;
                            comm.Parameters.Add(parameter);
                            comm.ExecuteNonQuery();
                        }
                        this.Close();
                    }
                }
                else
                {
                    using (SqlConnection sc = new SqlConnection(conn))
                    {
                        sc.Open();
                        SqlCommand comm = new SqlCommand("UPDATE Main_Table SET Name_book=@name,Key_izd=@keyPub,Year=@year,Price=@price WHERE Key_book=@key", sc);
                        SqlParameter parameter = new SqlParameter("@name", SqlDbType.NVarChar);
                        parameter.Value = nameBook.Text;
                        comm.Parameters.Add(parameter);

                        parameter = new SqlParameter("@keyPub", SqlDbType.Int);
                        parameter.Value = (int)publishersComboBox.SelectedValue;
                        comm.Parameters.Add(parameter);

                        parameter = new SqlParameter("@year", SqlDbType.Int);
                        parameter.Value = Int32.Parse(year.Text);
                        comm.Parameters.Add(parameter);

                        parameter = new SqlParameter("@price", SqlDbType.Int);
                        parameter.Value = Int32.Parse(price.Text);
                        comm.Parameters.Add(parameter);

                        parameter = new SqlParameter("@key", SqlDbType.Int);
                        parameter.Value = globalKey;
                        comm.Parameters.Add(parameter);

                        comm.ExecuteNonQuery();

                        comm = new SqlCommand("DELETE FROM Author_Books_Keys WHERE Book_Keys=@param", sc);
                        parameter = new SqlParameter("@param", SqlDbType.Int);
                        parameter.Value = globalKey;
                        comm.Parameters.Add(parameter);
                        comm.ExecuteNonQuery();

                        foreach (var authorKey in authorKeys)
                        {
                            comm = new SqlCommand("INSERT INTO Author_Books_Keys" +
                                "(Book_Keys, Author_Keys)VALUES (@param1,@param2)", sc);
                            parameter = new SqlParameter("@param1", SqlDbType.Int);
                            parameter.Value = globalKey;
                            comm.Parameters.Add(parameter);

                            parameter = new SqlParameter("@param2", SqlDbType.Int);
                            parameter.Value = authorKey;
                            comm.Parameters.Add(parameter);
                            comm.ExecuteNonQuery();
                        }
                        this.Close();
                    }

                }
            }
            else MessageBox.Show("Некорректно введены данные.");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            WpfApplication1.MainWindow mw = new MainWindow();
        }
    }
}
