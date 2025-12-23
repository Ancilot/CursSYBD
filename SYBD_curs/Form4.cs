using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form4 : Form
    {
        private NpgsqlConnection conn;
        public Form4()
        {

            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235; Include Error Detail=true;";
            conn = new NpgsqlConnection(connString);
            conn.Notice += Conn_Notice;
        }
        private void Conn_Notice(object sender, NpgsqlNoticeEventArgs e)
        {
            // Можно выводить в Label
            labelResult.Invoke(new Action(() =>
            {
                labelResult.Text += e.Notice.MessageText + Environment.NewLine;
                labelResult.Visible = true;
            }));
        }
        

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form4_Load(object sender, EventArgs e)
        {
            // Добавляем статусы в ComboBox
            comboBox1.Items.Add("Не начат");
            comboBox1.Items.Add("Выполняется");
            comboBox1.Items.Add("Закончен");

            // Устанавливаем первый элемент по умолчанию (если нужно)
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            LoadServicesToComboBox();
        }
        private void LoadServicesToComboBox()
        {
            conn.Open();

            comboBox2.Items.Clear();

            // Первый пункт
            comboBox2.Items.Add("Все услуги");

            using (var command = new NpgsqlCommand(
                "SELECT DISTINCT \"Name\" FROM curse.\"Services\" ORDER BY \"Name\"", conn))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        comboBox2.Items.Add(reader.GetString(0));
                    }
                }
            }

            comboBox2.SelectedIndex = 0;

            conn.Close();
        }



        private void button2_Click(object sender, EventArgs e)
        {

            // Получаем выбранный статус
            string selectedStatus = comboBox1.SelectedItem.ToString();

            conn.Open();


            // Создаем команду для вызова функции
            using (var command = new NpgsqlCommand("SELECT * FROM curse.get_includents_info_by_status(@p_processing_status)", conn))
            {
                // Добавляем параметр
                command.Parameters.AddWithValue("p_processing_status", selectedStatus);

                // Создаем адаптер и DataTable
                using (var adapter = new NpgsqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();

                    // Заполняем DataTable
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count == 0)
                    {
                        // Если данных нет
                        dataGridView1.DataSource = null;
                        MessageBox.Show(
                            "Договоров с данным статусом нет",
                            "Результат поиска",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        conn.Close();
                        return;
                    }

                    // Привязываем к DataGridView
                    dataGridView1.DataSource = dataTable;

                    // Опционально: настраиваем заголовки столбцов
                    if (dataGridView1.Columns.Count > 0)
                    {
                        dataGridView1.Columns["processing_status"].HeaderText = "Статус обработки";
                        dataGridView1.Columns["object_name"].HeaderText = "Объект";
                        dataGridView1.Columns["full_address"].HeaderText = "Адрес";
                        dataGridView1.Columns["contract_id"].HeaderText = "ID договора";
                        dataGridView1.Columns["contract_start_date"].HeaderText = "Дата начала";
                        dataGridView1.Columns["contract_finish_date"].HeaderText = "Дата окончания";
                        dataGridView1.Columns["manager_fio"].HeaderText = "Менеджер (ФИО)";
                    }
                    MessageBox.Show($"Количество договоров: {dataTable.Rows.Count}", "Результат",
                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            conn.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            labelResult.Text = "";
            labelResult.Visible = false;
            // Получаем даты из DateTimePicker
            DateTime startDate = dateTimePicker1.Value.Date;
            DateTime endDate = dateTimePicker2.Value.Date;

            conn.Open();

            // Создаем команду для вызова хранимой процедуры
            using (var command = new NpgsqlCommand("curse.calculate_revenue", conn))
            {
                try
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Добавляем входные параметры
                    //command.Parameters.AddWithValue("start_date", startDate);
                    //command.Parameters.AddWithValue("end_date", endDate);

                    command.Parameters.Add(new NpgsqlParameter("start_date", NpgsqlTypes.NpgsqlDbType.Date)
                    {
                        Value = startDate
                    });

                    command.Parameters.Add(new NpgsqlParameter("end_date", NpgsqlTypes.NpgsqlDbType.Date)
                    {
                        Value = endDate
                    });
                    // Добавляем выходной параметр
                    var outputParam = new NpgsqlParameter("total_revenue", NpgsqlTypes.NpgsqlDbType.Numeric)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    // Выполняем процедуру
                    command.ExecuteNonQuery();

                    // Получаем результат из выходного параметра
                    decimal totalRevenue = Convert.ToDecimal(outputParam.Value);
                    conn.Close();
                }
                catch (Exception ex)
                {
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        conn.Close();
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            conn.Open();

            string selectedService = comboBox2.SelectedItem.ToString();

            using (var command = new NpgsqlCommand(
                "SELECT * FROM curse.get_services_revenue(@p_service_name)", conn))
            {
                if (selectedService == "Все услуги")
                {
                    command.Parameters.AddWithValue("p_service_name", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("p_service_name", selectedService);
                }

                using (var adapter = new NpgsqlDataAdapter(command))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    if (table.Rows.Count == 0)
                    {
                        dataGridView1.DataSource = null;
                        MessageBox.Show(
                            "Данные отсутствуют",
                            "Результат",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else
                    {
                        dataGridView1.DataSource = table;
                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dataGridView1.Columns["service_name"].HeaderText = "Услуга";
                        dataGridView1.Columns["price"].HeaderText = "Цена";
                        dataGridView1.Columns["contract_count"].HeaderText = "Кол-во договоров";
                        dataGridView1.Columns["revenue"].HeaderText = "Выручка";
                    }
                }
            }
            conn.Close();
        }

    }
}
            
