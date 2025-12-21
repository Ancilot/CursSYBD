using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace SYBD_curs
{
    public partial class Form2 : Form
    {
        private NpgsqlConnection conn;
        public Form2()
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Создадим новый набор данных
           
            DataSet datasetmain = new DataSet();
     
            // Открываем подключение
            conn.Open();
          
            datasetmain.Clear();
            
            NpgsqlCommand command = new NpgsqlCommand("SELECT " +
            "c.\"ID\" AS Number_contract, " +
            "concat(cl.\"Surname\", ' ', cl.\"Name\", ' ', cl.\"Patronymic\") AS client, " +
            "o.\"Name\" AS object, " +
            "c.\"Start_date\", " +
            "c.\"Finish_date\", " +
            "concat(e.\"Surname\", ' ', e.\"Name\", ' ', e.\"Patronymic\") AS manager " +
            "FROM curse.\"Contract\" c " +
            "JOIN curse.\"Client\" cl ON cl.\"INN\" = c.\"Client\" " +
            "JOIN curse.\"Object\" o ON o.\"ID\" = c.\"Object\" " +
            "JOIN curse.\"Employee\" e ON e.\"ID\" = c.\"Manager\"" +
             "ORDER BY c.\"ID\" ASC",
            conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            // Заполняем набор данных данными, которые вернул запрос
            da.Fill(datasetmain, "\"Contract\"");

            // Связываем элемент DataGridView1 с набором данных
            dataGridView.DataSource = datasetmain;
        
            dataGridView.DataMember = "\"Contract\"";
            
            // Закрываем подключение
            conn.Close();
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form5 editForm = new Form5();
            editForm.ShowDialog();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            string searchText = textBox1.Text.Trim();

            try
            {
                DataSet datasetSearch = new DataSet();
                conn.Open();
                datasetSearch.Clear();

                // Используем функцию поиска
                NpgsqlCommand command = new NpgsqlCommand(
                    "SELECT * FROM curse.search_contract_by_id(@search_text)",
                    conn);

                command.Parameters.AddWithValue("@search_text",
                    string.IsNullOrEmpty(searchText) ? (object)DBNull.Value : searchText);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
                da.Fill(datasetSearch, "SearchResults");

                dataGridView.DataSource = datasetSearch.Tables["SearchResults"];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при поиске: " + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form4 editForm = new Form4();
            editForm.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null ||
             dataGridView.CurrentRow.Cells["Number_contract"].Value == null ||
             dataGridView.CurrentRow.Cells["Number_contract"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите договор для просмотра напоминаний",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            DataGridViewRow row = dataGridView.SelectedRows[0];

            int id = Convert.ToInt32(row.Cells["Number_contract"].Value);
            Form12 editForm = new Form12(id);
            editForm.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form15 editForm = new Form15();
            editForm.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null ||
              dataGridView.CurrentRow.Cells["Number_contract"].Value == null ||
              dataGridView.CurrentRow.Cells["Number_contract"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите договор для просмотра процесса выполнения",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            DataGridViewRow row = dataGridView.SelectedRows[0];

            int id = Convert.ToInt32(row.Cells["Number_contract"].Value);
            Form16 editForm = new Form16(id);
            editForm.ShowDialog();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Form23 editForm = new Form23();
            editForm.ShowDialog();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Form20 editForm = new Form20();
            editForm.ShowDialog();
        }
    }
}
