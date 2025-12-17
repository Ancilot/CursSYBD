using Npgsql;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form3 : Form
    {


        private NpgsqlConnection conn;
        public Form3()
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
           
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // Создадим новый набор данных
            DataSet datasetmain = new DataSet();
            
            // Открываем подключение
            conn.Open();
            // Очищаем набор данных
            datasetmain.Clear();
           
            NpgsqlCommand command = new NpgsqlCommand("SELECT " +
             "e.\"ID\","+
             "e.\"Surname\", " +
             "e.\"Name\", " +
             "e.\"Patronymic\", " +
             "e.\"Date_receipt\", " +
             "jt.\"Name\" AS job_title, " +
             "e.\"Wages\", " +
             "e.\"Education\" " +
             "FROM curse.\"Employee\" e " +
             "JOIN curse.\"Job_title\" jt ON jt.\"ID\" = e.\"Job_title\"", conn);
           
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            // Заполняем набор данных данными, которые вернул запрос
            da.Fill(datasetmain, "\"Employee\"");

            // Связываем элемент DataGridView1 с набором данных
            dataGridView1.DataSource = datasetmain;       

            dataGridView1.DataMember = "\"Employee\"";
            // Закрываем подключение
            dataGridView1.Columns["ID"].Visible = false;
            conn.Close();
            dataGridView1.CellClick += dataGridView1_CellClick;

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int id = (int)dataGridView1.Rows[e.RowIndex].Cells["ID"].Value;

            conn.Open();

            using (var cmd = new NpgsqlCommand(
                "SELECT \"Foto_employee\" FROM curse.\"Employee\" WHERE \"ID\"=@id", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);

                var data = cmd.ExecuteScalar();

                if (data != DBNull.Value)
                {
                    using (var ms = new System.IO.MemoryStream((byte[])data))
                        pictureBox1.Image = Image.FromStream(ms);
                }
                else
                {
                    pictureBox1.Image = null;
                }
            }

            conn.Close();
        }


        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            string searchText = textBox1.Text.Trim();

            DataSet datasetSearch = new DataSet();

            try
            {
                conn.Open();
                datasetSearch.Clear();

                NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM curse.search_employee_by_fio(@search_text)", conn);
                command.Parameters.AddWithValue("@search_text", string.IsNullOrEmpty(searchText) ? (object)DBNull.Value : searchText);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
                da.Fill(datasetSearch, "SearchResults");

                dataGridView1.DataSource = datasetSearch.Tables["SearchResults"];
                dataGridView1.Columns["ID"].Visible = false;

                // Очищаем фотографию при поиске
                pictureBox1.Image = null;
            }
            catch (Exception)
            {
                MessageBox.Show("Произошла ошибка при поиске");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }
    }
}
