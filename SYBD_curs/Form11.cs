using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form11 : Form
    {
        private NpgsqlConnection conn;
        public Form11()
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

        private void button2_Click(object sender, EventArgs e)
        {
            string Jobs = textBox1.Text.Trim();

            conn.Open();

            using (var command = new NpgsqlCommand(
                "INSERT INTO curse.\"Job_title\" (\"Name\") VALUES (@name)", conn))
            {
                command.Parameters.AddWithValue("name", Jobs);

                try
                {
                    command.ExecuteNonQuery();

                    MessageBox.Show(
                        "Должность успешно добавлена",
                        "Успех",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                catch (PostgresException ex)
                {
                    // Нарушение CHECK
                    if (ex.SqlState == "23514")
                    {
                        if (ex.ConstraintName == "job_title_name_no_whitespace_no_digits")
                        {
                            MessageBox.Show(
                                "Поле должности не может быть пустым или содержать цифры",
                                "Ошибка",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            return;
                        }
                    }
                    if (ex.SqlState == "23505")
                    {
                        if (ex.ConstraintName == "job_title_name_unique_idx")
                        {
                            MessageBox.Show(
                                "Данная долность уже существует",
                                "Ошибка",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            return;
                        }
                    }
                }
                finally
                {
                    conn.Close();
                    Job();
                }
            }
        }


        private void Job()
        {
            // Создадим новый набор данных
            DataSet datasetmain = new DataSet();

            // Открываем подключение
            conn.Open();

            datasetmain.Clear();

            NpgsqlCommand command = new NpgsqlCommand(
                "SELECT " +
                "\"ID\", " +
                "\"Name\" AS \"Должность\" " +
                "FROM curse.\"Job_title\" ",
                conn
            );

            // Адаптер
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);

            // Заполняем DataSet
            da.Fill(datasetmain, "\"Job_title\"");

            // Привязываем к DataGridView
            dataGridView1.DataSource = datasetmain;
            dataGridView1.DataMember = "\"Job_title\"";

            // Скрываем технический ID
            dataGridView1.Columns["ID"].Visible = false;

            // Закрываем подключение
            conn.Close();
        }
        private void Form11_Load(object sender, EventArgs e)
        {
            Job();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
            dataGridView1.CurrentRow.Cells["ID"].Value == null ||
            dataGridView1.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите должность для обновления",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];

            int id = Convert.ToInt32(row.Cells["ID"].Value);
            string jobs = textBox2.Text.Trim();
            try
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(
                    "UPDATE curse.\"Job_title\" " +
                    "SET \"Name\" = @name " +
                    "WHERE \"ID\" = @id",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", jobs);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show(
                    "Должность успешно обновлена",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (PostgresException ex)
            {
                // Нарушение CHECK
                if (ex.SqlState == "23514")
                {
                    if (ex.ConstraintName == "job_title_name_no_whitespace_no_digits")
                    {
                        MessageBox.Show(
                            "Поле должности не может быть пустым или содержать цифры",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                }
                if (ex.SqlState == "23505")
                {
                    if (ex.ConstraintName == "job_title_name_unique_idx")
                    {
                        MessageBox.Show(
                            "Данная долность уже существует",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                }
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
                Job();

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
                dataGridView1.CurrentRow.Cells["ID"].Value == null ||
                dataGridView1.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите должность для удаления",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            // Подтверждение удаления
            DialogResult result = MessageBox.Show(
                "Вы уверены, что хотите удалить выбранную должность?",
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
                     "DELETE FROM curse.\"Job_title\" WHERE \"ID\" = @id",
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
                Job();
            }
        }
    }
}
