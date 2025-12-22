using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form17 : Form
    {
        private Form parentForm;
        private int contractID;
        private NpgsqlConnection conn;
        private bool skipFormClosingDelete = false;
        public Form17(Form parent, int id)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            parentForm = parent;
            contractID = id;
            this.FormClosing += Form17_FormClosing;
        }
        private void Form17_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (skipFormClosingDelete)
                return; // пропускаем удаление контракта

            DialogResult res = MessageBox.Show(
                "Вы уверены, что хотите отменить создание договора?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (res != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            try
            {
                conn.Open();
                var cmd = new NpgsqlCommand(
                    "DELETE FROM curse.\"Contract\" WHERE \"ID\" = @id",
                    conn
                );
                cmd.Parameters.AddWithValue("@id", contractID);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            conn.Open( );  
            NpgsqlCommand cmd = new NpgsqlCommand(
                   "DELETE FROM curse.\"Contract\" WHERE \"ID\" = @id",
                  conn
              );

            cmd.Parameters.AddWithValue("id", contractID);

            cmd.ExecuteNonQuery();
            conn.Close( );
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();

                // Проверка смен
                var cmdSmen = new NpgsqlCommand(
                    "SELECT COUNT(*) FROM curse.\"Contract_graphik\" WHERE \"ID_Contract\" = @id",
                    conn
                );
                cmdSmen.Parameters.AddWithValue("@id", contractID);
                int smenCount = Convert.ToInt32(cmdSmen.ExecuteScalar());

                // Проверка услуг
                var cmdServices = new NpgsqlCommand(
                    "SELECT COUNT(*) FROM curse.\"Services_Contract\" WHERE \"ID_Contract\" = @id",
                    conn
                );
                cmdServices.Parameters.AddWithValue("@id", contractID);
                int servicesCount = Convert.ToInt32(cmdServices.ExecuteScalar());

                // Если нет хотя бы одной смены или услуги
                if (smenCount == 0 || servicesCount == 0)
                {
                    MessageBox.Show(
                        "Нельзя закрыть форму.\n" +
                        "В договоре должна быть добавлена хотя бы одна смена и одна услуга.",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
            }
            finally
            {
                conn.Close();
            }
            skipFormClosingDelete = true;
            // Если всё ок — закрываем формы
            this.Close();
            parentForm.Close();
            
        }


        private void boxServis()
        {
            

            try
            {
                conn.Open();
                var cmd = new NpgsqlCommand(
                 "SELECT * FROM curse.get_available_services_for_contract(@contract_id, @spec)",
                 conn
                 );

                cmd.Parameters.AddWithValue("contract_id", contractID);
                cmd.Parameters.AddWithValue("spec", "Отсутствует");


                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "full_servis";
                comboBox1.ValueMember = "ID";
            }
            finally
            {
                conn.Close();
            }
        }
        private void boxSmen() {
            conn.Open();

            try
            {
                var cmd = new NpgsqlCommand(
                    "SELECT * FROM curse.get_available_shifts_for_contract(@id_contract)",
                    conn
                );

                cmd.Parameters.AddWithValue("@id_contract", contractID);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                comboBox2.DataSource = dt;
                comboBox2.DisplayMember = "Name_smena";
                comboBox2.ValueMember = "ID";
            }
            finally
            {
                conn.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue == null || comboBox1.SelectedValue == DBNull.Value)
            {
                MessageBox.Show(
                    "Ошибка: нет доступных услуг",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return; // выходим из метода, чтобы не выполнять вставку
            }

            int servisId = Convert.ToInt32(comboBox1.SelectedValue);

            try
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "INSERT INTO curse.\"Services_Contract\" (\"ID_Contract\", \"ID_Services\") " +
                    "VALUES (@id_contract, @id_services)",
                    conn
                );

                cmd.Parameters.AddWithValue("id_contract", contractID);
                cmd.Parameters.AddWithValue("id_services", servisId);

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
            finally { 
                conn.Close(); 
                boxServis();
                servise_contract();
            }
        }
        private void smen_contract()
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

            command.Parameters.AddWithValue("@id_contract", contractID);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            // Заполняем набор данных данными, которые вернул запрос
            da.Fill(datasetmain, "\"Contract_graphik\"");

            // Связываем элемент DataGridView1 с набором данных
            dataGridView2.DataSource = datasetmain;

            dataGridView2.DataMember = "\"Contract_graphik\"";
            dataGridView2.Columns["ID"].Visible = false;

            // Закрываем подключение
            conn.Close();
        }
        private void servise_contract()
        {
            // Создадим новый набор данных
            DataSet datasetmain = new DataSet();

            // Открываем подключение
            conn.Open();

            datasetmain.Clear();

            NpgsqlCommand command = new NpgsqlCommand(
                "SELECT " +
                "sc.\"ID\", " +
                "sc.\"ID_Contract\" AS \"Номер контракта\", " +
                "s.\"Name\" AS \"Название услуги\", " +
                "s.\"Price\" AS \"Цена\" " +
                "FROM curse.\"Services_Contract\" sc " +
                "JOIN curse.\"Services\" s ON s.\"ID\" = sc.\"ID_Services\" " +
                "WHERE sc.\"ID_Contract\" = @id_contract " +
                "ORDER BY sc.\"ID_Contract\"",
                conn
            );

            // Параметр контракта
            command.Parameters.AddWithValue("@id_contract", contractID);

            // Адаптер
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);

            // Заполняем DataSet
            da.Fill(datasetmain, "\"Services_Contract\"");

            // Привязываем к DataGridView
            dataGridView1.DataSource = datasetmain;
            dataGridView1.DataMember = "\"Services_Contract\"";

            // Скрываем технический ID
            dataGridView1.Columns["ID"].Visible = false;

            // Закрываем подключение
            conn.Close();

        }
        private void Form17_Load(object sender, EventArgs e)
        {
            smen_contract();
            servise_contract();
            boxServis();
            boxSmen();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
          dataGridView1.CurrentRow.Cells["ID"].Value == null ||
          dataGridView1.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите услугу для удаления",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            DataGridViewRow row = dataGridView1.CurrentRow;


            // Получаем ID выбранной услуги
            int servisId = Convert.ToInt32(
                dataGridView1.SelectedRows[0].Cells["ID"].Value
            );

            try
            {
                conn.Open();

                NpgsqlCommand cmd = new NpgsqlCommand(
                "DELETE FROM curse.\"Services_Contract\" WHERE \"ID\" = @id",
                   conn
                 );
                cmd.Parameters.AddWithValue("@id", servisId);
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
                boxServis();
                servise_contract();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow == null ||
           dataGridView2.CurrentRow.Cells["ID"].Value == null ||
           dataGridView2.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите смену для удаления",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            DataGridViewRow row = dataGridView2.SelectedRows[0];
            int id = Convert.ToInt32(row.Cells["ID"].Value);

            try
            {
                conn.Open();

                NpgsqlCommand cmd = new NpgsqlCommand(
                     "DELETE FROM curse.\"Contract_graphik\" WHERE \"ID\" = @id",
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
            finally { 
                conn.Close();
                boxSmen();
                smen_contract();
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
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
                    "INSERT INTO curse.\"Contract_graphik\" (\"ID_Contract\", \"ID_graphik\") " +
                    "VALUES (@id_contract, @id_graphic)",
                    conn
                );

                cmd.Parameters.AddWithValue("id_contract", contractID);
                cmd.Parameters.AddWithValue("id_graphic", nameId);

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
            finally {
                conn.Close();
                boxSmen();
                smen_contract();
            }
        }
    }
}
