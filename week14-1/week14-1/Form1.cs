using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace week14_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (MySqlConnection sqlConnection = new MySqlConnection("Server=localhost;Database=gamesdb;Uid=gamesdba;Pwd=gamesdba123;"))
            {
                try
                {
                    sqlConnection.Open();

                    string query = "SELECT id, title, category, publishDate from `game`";
                    MySqlCommand sqlCommand = new MySqlCommand(query, sqlConnection);

                    MySqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                    //listBox1.DataSource = sqlDataReader; //not allowed

                    List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
                    while (sqlDataReader.Read())
                    {
                        list.Add(new KeyValuePair<int, string>(sqlDataReader.GetInt32(0), sqlDataReader["title"].ToString()));
                    }
                    sqlDataReader.Close();

                    listBox1.DataSource = list;
                    listBox1.DisplayMember = "Value";
                    listBox1.ValueMember = "Key";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                return;

            var keyvaluepair = (KeyValuePair<int, string>)listBox1.SelectedItem;

            using (MySqlConnection sqlConnection = new MySqlConnection("Server=localhost;Database=gamesdb;Uid=gamesdba;Pwd=gamesdba123;"))
            {
                try
                {
                    sqlConnection.Open();

                    string query = "SELECT id, title, category, publishDate from `game` WHERE id = @id";
                    MySqlCommand sqlCommand = new MySqlCommand(query, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@id", keyvaluepair.Key);
                    MySqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                    //listBox1.DataSource = sqlDataReader; //not allowed

                    if (sqlDataReader.Read())
                    {
                        textBox1.Text = sqlDataReader["id"].ToString();
                        textBox2.Text = sqlDataReader["title"].ToString();
                        textBox3.Text = sqlDataReader["category"].ToString();
                        dateTimePicker1.Value = DateTime.Parse(sqlDataReader["publishDate"].ToString());
                    }
                    sqlDataReader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
                return;

            using (MySqlConnection sqlConnection = new MySqlConnection("Server=localhost;Database=gamesdb;Uid=gamesdba;Pwd=gamesdba123;"))
            {
                try
                {
                    sqlConnection.Open();

                    string query = "UPDATE `game` set title=@title, category=@category, publishDate=@publishDate WHERE id = @id";
                    MySqlCommand sqlCommand = new MySqlCommand(query, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@id", Convert.ToInt32(textBox1.Text));
                    sqlCommand.Parameters.AddWithValue("@title", textBox2.Text);
                    sqlCommand.Parameters.AddWithValue("@category", textBox3.Text);
                    sqlCommand.Parameters.AddWithValue("@publishDate", dateTimePicker1.Value);

                    var affectedRowCount = sqlCommand.ExecuteNonQuery();
                    if (affectedRowCount > 0)
                    {
                        MessageBox.Show($"{affectedRowCount} rows updated. ");
                        button1_Click(sender, e);
                    }                        
                    else
                        MessageBox.Show("No rows updated. ");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
    }
}
