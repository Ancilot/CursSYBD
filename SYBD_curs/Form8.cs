using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form8 : Form
    {
        private NpgsqlConnection conn;
        private int selectedServiceContractId = -1;
        public Form8()
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            dataGridView1.CellClick += dataGridView1_CellClick;
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var cellValue = dataGridView1.Rows[e.RowIndex].Cells["ID"].Value;

            if (cellValue == null || cellValue == DBNull.Value)
                return;

            selectedServiceContractId = Convert.ToInt32(cellValue);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();

        }
        private void Services()
        {
            DataTable dt = new DataTable();

            conn.Open();

            using (var da = new NpgsqlDataAdapter(
                "SELECT \"ID\", \"Name\" FROM curse.\"Services\" " +
                "WHERE specially <> 'Услуга для уже заключенного договора' " +
                "OR specially IS NULL " +
                "ORDER BY \"Name\"",
                conn))
            {
                da.Fill(dt);
            }

            conn.Close();

            comboBox4.DataSource = dt;
            comboBox4.DisplayMember = "Name";
            comboBox4.ValueMember = "ID";
        }
        private void contract_servis()
        {
            // Создадим новый набор данных

            DataSet datasetmain = new DataSet();

            // Открываем подключение
            conn.Open();

            datasetmain.Clear();

            NpgsqlCommand command = new NpgsqlCommand(
              "SELECT " +
              "sc.\"ID\"," +
              "sc.\"ID_Contract\" AS \"Номер договора\", " +
              "o.\"Name\" AS \"Название объекта\", " +
              "s.\"Name\" AS \"Название услуги\" " +
              "FROM curse.\"Services_Contract\" sc " +
              "JOIN curse.\"Contract\" c ON c.\"ID\" = sc.\"ID_Contract\" " +
              "JOIN curse.\"Object\" o ON o.\"ID\" = c.\"Object\" " +
              "JOIN curse.\"Services\" s ON s.\"ID\" = sc.\"ID_Services\" " +
              "ORDER BY sc.\"ID_Contract\", s.\"Name\"",
              conn
             );

            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            // Заполняем набор данных данными, которые вернул запрос
            da.Fill(datasetmain, "\"Services_Contract\"");

            // Связываем элемент DataGridView1 с набором данных
            dataGridView1.DataSource = datasetmain;

            dataGridView1.DataMember = "\"Services_Contract\"";
            dataGridView1.Columns["ID"].Visible = false;
            dataGridView1.ClearSelection();
            dataGridView1.CurrentCell = null;


            // Закрываем подключение
            conn.Close();
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            contract_servis();
            Services();

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }



        private void button3_Click(object sender, EventArgs e)
        {
            if (selectedServiceContractId == -1)
            {
                MessageBox.Show("Выберите строку в таблице");
                return;
            }

            if (comboBox4.SelectedValue == null)
            {
                MessageBox.Show("Выберите новую услугу");
                return;
            }

            try
            {
                int newServiceId = Convert.ToInt32(comboBox4.SelectedValue);

                conn.Open();

                using (var cmd = new NpgsqlCommand(
                    "UPDATE curse.\"Services_Contract\" " +
                    "SET \"ID_Services\" = @service " +
                    "WHERE \"ID\" = @id",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@service", newServiceId);
                    cmd.Parameters.AddWithValue("@id", selectedServiceContractId);
                    cmd.ExecuteNonQuery();
                }



                MessageBox.Show("Услуга успешно обновлена");

                // обновляем таблицу
                contract_servis();

                selectedServiceContractId = -1;
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
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (selectedServiceContractId == -1)
            {
                MessageBox.Show("Выберите строку в таблице для удаления");
                return;
            }

            // Подтверждение удаления
            DialogResult result = MessageBox.Show(
                "Вы уверены, что хотите удалить выбранную услугу у договора?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                conn.Open();
                using (var checkCmd = new NpgsqlCommand(
                "SELECT COUNT(*) FROM curse.\"Services_Contract\" WHERE \"ID_Contract\" = @id_contract",
               conn))
                {
                    DataGridViewRow row = dataGridView1.SelectedRows[0];
                    int id = Convert.ToInt32(row.Cells["Номер договора"].Value);
                    checkCmd.Parameters.AddWithValue("@id_contract", id);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count == 1)
                    {
                        MessageBox.Show(
                            "Созданный контракт нельзя оставить без услуг",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return; // Прерываем удаление
                    }
                }
                NpgsqlCommand cmd = new NpgsqlCommand(
                "DELETE FROM curse.\"Services_Contract\" WHERE \"ID\" = @id",
                   conn
                 );
                cmd.Parameters.AddWithValue("@id", selectedServiceContractId);
                cmd.ExecuteNonQuery();

                MessageBox.Show(
                    "Услуга успешно удалена.",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
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

                // Обновляем таблицу
                contract_servis();
            }

        }
    }
}
