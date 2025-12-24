using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form29 : Form
    {
        private NpgsqlConnection conn;
        public Form29()
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form29_Load(object sender, EventArgs e)
        {
            arhiveClient();
        }
        private void arhiveClient()
        {
            // Создадим новый набор данных

            DataSet datasetmain = new DataSet();

            // Открываем подключение
            conn.Open();

            datasetmain.Clear();

            NpgsqlCommand command = new NpgsqlCommand(
             "SELECT " +
             "c.\"INN\" AS \"ИНН\", " +
             "c.\"Name_organization\" AS \"Организация\", " +
             "c.\"Surname\" AS \"Фамилия\", " +
             "c.\"Name\" AS \"Имя\", " +
             "c.\"Patronymic\" AS \"Отчество\", " +
             "concat(ad.\"Home\", ' ', ad.\"Street\", ' ', ad.\"City\", ' ', ad.\"Region\", ' ', ad.\"Country\") AS \"Адрес\", " +
             "c.\"Number\" AS \"Номер телефона\", " +
             "c.\"Account_number\" AS \"Номер счета\", " +
             "c.\"Email\" AS \"Почта\" " +
             "FROM curse.\"Client\" c " +
             "JOIN curse.\"Addres\" ad ON ad.\"ID\" = c.\"Address\" " +
             "WHERE c.\"Arhive\" = 'В архиве'" +
             "ORDER BY c.\"INN\"",
             conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            // Заполняем набор данных данными, которые вернул запрос
            da.Fill(datasetmain, "\"Client\"");

            // Связываем элемент DataGridView1 с набором данных
            dataGridView1.DataSource = datasetmain;

            dataGridView1.DataMember = "\"Client\"";

            // Закрываем подключение
            conn.Close();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
         dataGridView1.CurrentRow.Cells["ИНН"].Value == null ||
         dataGridView1.CurrentRow.Cells["ИНН"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите клиента для удаления",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            DataGridViewRow row = dataGridView1.CurrentRow;

            // Получаем ID выбранной услуги
            string inn = row.Cells["ИНН"].Value.ToString();
            conn.Open();

            NpgsqlCommand cmd = new NpgsqlCommand(
                "UPDATE curse.\"Client\" SET " +
             "\"Arhive\" = @arhive " +
             "WHERE \"INN\" = @inn",
              conn);
            cmd.Parameters.AddWithValue("inn", inn);
            cmd.Parameters.AddWithValue("arhive", DBNull.Value);
            cmd.ExecuteNonQuery();

            MessageBox.Show(
                "Клиент успешно возвращен",
                "Успех",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            conn.Close();
            arhiveClient();
        }
    }
}
