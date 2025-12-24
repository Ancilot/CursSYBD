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

        private void emploee()
        {
            // Создадим новый набор данных
            DataSet datasetmain = new DataSet();

            // Открываем подключение
            conn.Open();
            // Очищаем набор данных
            datasetmain.Clear();

            NpgsqlCommand command = new NpgsqlCommand("SELECT " +
             "e.\"ID\"," +
             "e.\"Foto_employee\"," +
             "e.\"Job_title\"," +
             "e.\"Surname\" AS \"Фамилия\", " +
             "e.\"Name\" AS \"Имя\", " +
             "e.\"Patronymic\" AS \"Отчество\", " +
             "e.\"Date_receipt\"AS \"Дата поступления\", " +
             "jt.\"Name\" AS \"Должность\", " +
             "e.\"Wages\"AS \"Зарплата\", " +
             "e.\"Education\" AS \"Образование\"" +
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
            dataGridView1.Columns["Foto_employee"].Visible = false;
            dataGridView1.Columns["Job_title"].Visible = false;
            conn.Close();
            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            emploee();

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
                dataGridView1.Columns["Foto_employee"].Visible = false;
                dataGridView1.Columns["Job_title"].Visible = false;

                // Очищаем фотографию при поиске
                pictureBox1.Image = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при поиске:\n" + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
                dataGridView1.CurrentRow.Cells["ID"].Value == null ||
                dataGridView1.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите сотрудника",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Выберите сотрудника");
                return;
            }

            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value);

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Изображения (*.jpg;*.png)|*.jpg;*.png";
                ofd.Title = "Выберите фотографию сотрудника";

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                byte[] imageBytes = System.IO.File.ReadAllBytes(ofd.FileName);

                try
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand(
                        "UPDATE curse.\"Employee\" SET \"Foto_employee\" = @foto WHERE \"ID\" = @id",
                        conn))
                    {
                        cmd.Parameters.AddWithValue("@foto", imageBytes);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                    // Обновляем PictureBox
                    using (var ms = new System.IO.MemoryStream(imageBytes))
                        pictureBox1.Image = Image.FromStream(ms);
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка при обновлении фото");
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.Hide();

            using (Form11 editForm = new Form11())
            {
                editForm.ShowDialog();
            }
            emploee();
            this.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form7 editForm = new Form7();
            editForm.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (Form31 editForm = new Form31())
            {
                editForm.ShowDialog();
            }
            emploee();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            using (Form6 editForm = new Form6())
            {
                editForm.ShowDialog(); 
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show(
                    "Выберите сотрудника",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            DataGridViewRow row = dataGridView1.CurrentRow;

            int id = Convert.ToInt32(row.Cells["ID"].Value);
            string surname = row.Cells["Фамилия"].Value.ToString();
            string name = row.Cells["Имя"].Value.ToString();
            string patronymic = row.Cells["Отчество"].Value.ToString();
            DateTime dateStart = Convert.ToDateTime(row.Cells["Дата поступления"].Value);
            int jobId = Convert.ToInt32(row.Cells["Job_title"].Value); 
            decimal wages = Convert.ToDecimal(row.Cells["Зарплата"].Value);
            string education = row.Cells["Образование"].Value.ToString();
            byte[] photoBytes = null;

            if (row.Cells["Foto_employee"].Value != DBNull.Value)
            {
                photoBytes = (byte[])row.Cells["Foto_employee"].Value;
            }


            using (Form33 editForm = new Form33(
                id, surname, name, patronymic, dateStart, jobId, wages, education, photoBytes))
            {
                editForm.ShowDialog();
            }

            emploee();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
           dataGridView1.CurrentRow.Cells["ID"].Value == null ||
           dataGridView1.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите сотрудника для удаления",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            // Подтверждение удаления
            DialogResult result = MessageBox.Show(
                "Вы уверены, что хотите удалить выбранного сотрудника?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            int id = Convert.ToInt32(row.Cells["ID"].Value);

            try
            {
                conn.Open();

                NpgsqlCommand cmd = new NpgsqlCommand(
                     "DELETE FROM curse.\"Employee\" WHERE \"ID\" = @id",
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
                emploee();
            }
        }
    }
}

