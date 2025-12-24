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
    public partial class Form42 : Form
    {
        private NpgsqlConnection conn;
        private int incId;
        public Form42(int id)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            incId = id;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void Archive()
        {
            // Создадим новый набор данных
            DataSet datasetmain = new DataSet();

            // Открываем подключение
            conn.Open();

            datasetmain.Clear();

            NpgsqlCommand command = new NpgsqlCommand(
              "SELECT " +
               "\"ID\", " +
              "\"Processing_status\" AS \"Процесс выполнения\", " +
              "\"Incedent\" AS \"Происшествие\", " +
              "\"Data_Time\" AS \"Дата\", " +
              "\"Measures_taken\" AS \"Принятые меры\" " +
              "FROM curse.\"Includents\" " +
              "WHERE \"Contract_Smena\" = @id_contract_graphik " +
              "AND \"Arhive\" = 'В архиве' " +
              "ORDER BY \"Data_Time\"",
               conn
                );

            // Передаем параметр
            command.Parameters.AddWithValue("@id_contract_graphik", incId);

            // Адаптер
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);

            // Заполняем DataSet
            da.Fill(datasetmain, "\"Includents\"");

            // Привязываем к DataGridView
            dataGridView1.DataSource = datasetmain;
            dataGridView1.DataMember = "\"Includents\"";

            // Скрываем технический ID
            dataGridView1.Columns["ID"].Visible = false;

            // Закрываем подключение
            conn.Close();
        }
        private void Form42_Load(object sender, EventArgs e)
        {
            Archive();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
             dataGridView1.CurrentRow.Cells["ID"].Value == null ||
             dataGridView1.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите происшествие для возвращения",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            DataGridViewRow row = dataGridView1.CurrentRow;

            // Получаем ID выбранной услуги
            int id = Convert.ToInt32(row.Cells["ID"].Value);
            conn.Open();

            NpgsqlCommand cmd = new NpgsqlCommand(
             "UPDATE curse.\"Includents\" SET " +
             "\"Arhive\" = @arhive " +
             "WHERE \"ID\" = @id",
              conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("arhive", DBNull.Value);
            cmd.ExecuteNonQuery();

            MessageBox.Show(
                "Происшествие успешно возвращено",
                "Успех",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            conn.Close();
            Archive();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
    
}
