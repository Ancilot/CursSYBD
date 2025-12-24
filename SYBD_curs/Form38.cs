using Npgsql;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form38 : Form
    {
        private NpgsqlConnection conn;
        private int smenID;
        public Form38(int id)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            smenID = id;
        }
        private void emploee_smen()
        {
            // Создадим новый набор данных
            DataSet datasetmain = new DataSet();

            // Открываем подключение
            conn.Open();

            datasetmain.Clear();

            NpgsqlCommand command = new NpgsqlCommand(
              "SELECT " +
              "eg.\"ID\", " +
              "concat(e.\"Surname\", ' ', e.\"Name\", ' ', e.\"Patronymic\") AS \"ФИО\", " +
              "gs.\"Name_smena\" AS \"Смена\" " +
              "FROM curse.\"Employee_graphik\" eg " +
              "JOIN curse.\"Employee\" e ON e.\"ID\" = eg.\"ID_Employee\" " +
              "JOIN curse.graphik_smen gs ON gs.\"ID\" = eg.\"ID_graphik\" " +
              "WHERE eg.\"ID_graphik\" = @id_graphik " +
              "ORDER BY eg.\"ID_graphik\"",
              conn
             );


            // Параметр контракта
            command.Parameters.AddWithValue("@id_graphik", smenID);

            // Адаптер
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);

            // Заполняем DataSet
            da.Fill(datasetmain, "\"Employee_graphik\"");

            // Привязываем к DataGridView
            dataGridView1.DataSource = datasetmain;
            dataGridView1.DataMember = "\"Employee_graphik\"";

            // Скрываем технический ID
            dataGridView1.Columns["ID"].Visible = false;

            // Закрываем подключение
            conn.Close();

        }
        private void boxEmp()
        {
            conn.Open();

            try
            {
                var cmd = new NpgsqlCommand(
                    "SELECT " +
                    "\"ID\", " +
                    "concat(\"Surname\", ', ', \"Name\", ', ', \"Patronymic\") AS full_name " +
                    "FROM curse.\"Employee\" " +
                    "ORDER BY \"ID\"",
                    conn
                );

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "full_name";
                comboBox1.ValueMember = "ID";
            }
            finally
            {
                conn.Close();
            }
        }
        private void Form38_Load(object sender, System.EventArgs e)
        {
            emploee_smen();
            boxEmp();
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            if (comboBox1.SelectedValue == null)
            {
                MessageBox.Show(
                    "Ошибка: нет доступных сотрудников",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return; // выходим из метода, чтобы не выполнять вставку
            }

            int emploeID = Convert.ToInt32(comboBox1.SelectedValue);

            try
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "INSERT INTO curse.\"Employee_graphik\" (\"ID_Employee\", \"ID_graphik\") " +
                    "VALUES (@id_employee, @id_graphic)",
                    conn
                );

                cmd.Parameters.AddWithValue("id_employee", emploeID);
                cmd.Parameters.AddWithValue("id_graphic", smenID);

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
                boxEmp();
                emploee_smen();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
          dataGridView1.CurrentRow.Cells["ID"].Value == null ||
          dataGridView1.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите Сотрудника для удаления",
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
                     "DELETE FROM curse.\"Employee_graphik\" WHERE \"ID\" = @id",
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
                emploee_smen();
            }
        }
    }
}
