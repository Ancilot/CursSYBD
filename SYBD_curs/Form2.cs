using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

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

        private void contract()
        {
            // Создадим новый набор данных

            DataSet datasetmain = new DataSet();

            // Открываем подключение
            conn.Open();

            datasetmain.Clear();

            NpgsqlCommand command = new NpgsqlCommand("SELECT " +
            "c.\"ID\" AS \"Номер договора\", " +
            "c.\"Client\", " +
            "o.\"ID\" AS objID, " +
            "c.\"Manager\", " +
            "concat(cl.\"Surname\", ' ', cl.\"Name\", ' ', cl.\"Patronymic\") AS \"Клиент\", " +
            "o.\"Name\" AS  \"Объект\", " +
            "c.\"Start_date\" AS \"Дата начала\", " +
            "c.\"Finish_date\" AS \"Дата конца\", " +
            "concat(e.\"Surname\", ' ', e.\"Name\", ' ', e.\"Patronymic\") AS \"Менеджер\" " +
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
            dataGridView.Columns["Client"].Visible = false;
            dataGridView.Columns["Manager"].Visible = false;
            dataGridView.Columns["objID"].Visible = false;

            // Закрываем подключение
            conn.Close();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            contract();
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
                dataGridView.Columns["Client"].Visible = false;
                dataGridView.Columns["Manager"].Visible = false;
                dataGridView.Columns["objID"].Visible = false;
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
             dataGridView.CurrentRow.Cells["Номер договора"].Value == null ||
             dataGridView.CurrentRow.Cells["Номер договора"].Value == DBNull.Value)
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

            int id = Convert.ToInt32(row.Cells["Номер договора"].Value);
            Form12 editForm = new Form12(id);
            editForm.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            using (Form15 editForm = new Form15())
            {
                editForm.ShowDialog();
            }
            contract();
        }


        private void button9_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null ||
              dataGridView.CurrentRow.Cells["Номер договора"].Value == null ||
              dataGridView.CurrentRow.Cells["Номер договора"].Value == DBNull.Value)
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

            int id = Convert.ToInt32(row.Cells["Номер договора"].Value);
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

        private void button10_Click(object sender, EventArgs e)
        {
            Form26 editForm = new Form26();
            editForm.ShowDialog();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null ||
             dataGridView.CurrentRow.Cells["Номер договора"].Value == null ||
             dataGridView.CurrentRow.Cells["Номер договора"].Value == DBNull.Value)
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

            int id = Convert.ToInt32(row.Cells["Номер договора"].Value);
            Form18 editForm = new Form18(id);
            editForm.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null ||
            dataGridView.CurrentRow.Cells["Номер договора"].Value == null ||
            dataGridView.CurrentRow.Cells["Номер договора"].Value == DBNull.Value)
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

            int id = Convert.ToInt32(row.Cells["Номер договора"].Value);
            string client = row.Cells["Client"].Value.ToString();
            int obj = Convert.ToInt32(row.Cells["objID"].Value);
            DateTime data_start = Convert.ToDateTime(row.Cells["Дата начала"].Value);
            DateTime data_finish = Convert.ToDateTime(row.Cells["Дата конца"].Value);
            int meneger = Convert.ToInt32(row.Cells["Manager"].Value);

            using (Form19 editForm = new Form19(id, client, obj, data_start, data_finish, meneger))
            {
                editForm.ShowDialog();
            }
            contract();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null ||
           dataGridView.CurrentRow.Cells["Номер договора"].Value == null ||
           dataGridView.CurrentRow.Cells["Номер договора"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите договор для удаления",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            DataGridViewRow row = dataGridView.SelectedRows[0];
            int id = Convert.ToInt32(row.Cells["Номер договора"].Value);

            try
            {
                conn.Open();

                NpgsqlCommand cmd = new NpgsqlCommand(
                     "DELETE FROM curse.\"Contract\" WHERE \"ID\" = @id",
                    conn
                );

                cmd.Parameters.AddWithValue("id", id);

                cmd.ExecuteNonQuery();
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "P0001")
                    MessageBox.Show(
                            ex.MessageText,
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
            }
            finally
            {
                conn.Close();
                contract();
            }

        }
    }
}
