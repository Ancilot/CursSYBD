using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form18 : Form
    {
        private NpgsqlConnection conn;
        private int ContractId;
        public Form18(int id)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            ContractId = id;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (dataGridView1.CurrentRow == null ||
            dataGridView1.CurrentRow.Cells["ID"].Value == null ||
            dataGridView1.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите смену для редактирования",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            int id = Convert.ToInt32(row.Cells["ID"].Value);

            if (comboBox2.SelectedValue == null)
            {
                MessageBox.Show(
                    "Ошибка: нет доступных смен",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return; // выходим из метода, чтобы не выполнять вставку
            }

            int nameId = Convert.ToInt32(comboBox2.SelectedValue);

            try
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(
                     "UPDATE curse.\"Contract_graphik\" SET " +
                     "\"ID_graphik\" = @id_graphic " +
                     "WHERE \"ID\" = @id",
                    conn
                );

                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("id_graphic", nameId);

                cmd.ExecuteNonQuery();
                boxSmena2();
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
            finally { conn.Close(); }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
            dataGridView1.CurrentRow.Cells["ID"].Value == null ||
            dataGridView1.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите смену для удаления",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            int id = Convert.ToInt32(row.Cells["ID"].Value);

            try
            {
                conn.Open();

                using (var checkCmd = new NpgsqlCommand(
                 "SELECT COUNT(*) FROM curse.\"Contract_graphik\" WHERE \"ID_Contract\" = @id_contract",
                conn))
                {
                    checkCmd.Parameters.AddWithValue("@id_contract", ContractId);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count <= 1)
                    {
                        MessageBox.Show(
                            "Созданный контракт нельзя оставить без смен",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return; // Прерываем удаление
                    }
                }

                NpgsqlCommand cmd = new NpgsqlCommand(
                     "DELETE FROM curse.\"Contract_graphik\" WHERE \"ID\" = @id",
                    conn
                );

                cmd.Parameters.AddWithValue("id", id);

                cmd.ExecuteNonQuery();
                boxSmena2();
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
            finally { conn.Close(); }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void smen()
        {
            // Создадим новый набор данных

            DataSet datasetmain = new DataSet();

            // Открываем подключение
            conn.Open();

            datasetmain.Clear();
            NpgsqlCommand command = new NpgsqlCommand(
              "SELECT " +
              "cg.\"ID\", " +
               "cg.\"ID_Contract\" AS \"Номер контракта\", " +
               "gs.\"Name_smena\" AS \"Название смены\" " +
               "FROM curse.\"Contract_graphik\" cg " +
               "JOIN curse.graphik_smen gs ON gs.\"ID\" = cg.\"ID_graphik\" " +
               "WHERE cg.\"ID_Contract\" = @id_contract " +
               "ORDER BY cg.\"ID_Contract\"",
               conn
             );

            command.Parameters.AddWithValue("@id_contract", ContractId);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            // Заполняем набор данных данными, которые вернул запрос
            da.Fill(datasetmain, "\"Contract_graphik\"");

            // Связываем элемент DataGridView1 с набором данных
            dataGridView1.DataSource = datasetmain;

            dataGridView1.DataMember = "\"Contract_graphik\"";
            dataGridView1.Columns["ID"].Visible = false;

            // Закрываем подключение
            conn.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue == null)
            {
                MessageBox.Show(
                    "Ошибка: нет доступных смен",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return; // выходим из метода, чтобы не выполнять вставку
            }

            int nameId = Convert.ToInt32(comboBox1.SelectedValue);

            try
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "INSERT INTO curse.\"Contract_graphik\" (\"ID_Contract\", \"ID_graphik\") " +
                    "VALUES (@id_contract, @id_graphic)",
                    conn
                );

                cmd.Parameters.AddWithValue("id_contract", ContractId);
                cmd.Parameters.AddWithValue("id_graphic", nameId);

                cmd.ExecuteNonQuery();
                boxSmena1();
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
            finally { conn.Close(); }
        }
        private void boxSmena1()
        {
            conn.Open();

            try
            {
                var cmd = new NpgsqlCommand(
                    "SELECT * FROM curse.get_available_shifts_for_contract(@id_contract)",
                    conn
                );

                cmd.Parameters.AddWithValue("@id_contract", ContractId);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "Name_smena";
                comboBox1.ValueMember = "ID";
            }
            finally
            {
                conn.Close();
            }
        }
        private void boxSmena2()
        {
            conn.Open();

            try
            {
                var cmd = new NpgsqlCommand(
                    "SELECT * FROM curse.get_available_shifts_for_contract(@id_contract)",
                    conn
                );

                cmd.Parameters.AddWithValue("@id_contract", ContractId);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                comboBox2.DataSource = dt;
                comboBox2.DisplayMember = "Name_smena";
                comboBox2.ValueMember = "ID";
                comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            finally
            {
                conn.Close();
            }
        }


        private void Form18_Load(object sender, EventArgs e)
        {
            smen();
            boxSmena1();
            boxSmena2();
        }
    }
}
