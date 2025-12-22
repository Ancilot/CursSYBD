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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form11_Load(object sender, EventArgs e)
        {
            // Получаем выбранный статус
            string selectedStatus = "Не начат";

            conn.Open();


            // Создаем команду для вызова функции
            using (var command = new NpgsqlCommand("SELECT * FROM curse.get_includents_info_by_status(@p_status)", conn))
            {
                // Добавляем параметр
                command.Parameters.AddWithValue("p_status", selectedStatus);

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
                            "Нет приступивших в исполнение договоров нет, нельзя добавть услугу",
                            "Результат поиска",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        conn.Close();
                        this.Close();
                        return;
                    }

                    // Привязываем к DataGridView
                    dataGridView1.DataSource = dataTable;

                    // Опционально: настраиваем заголовки столбцов
                    if (dataGridView1.Columns.Count > 0)
                    {
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

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
