using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form26 : Form
    {
        private NpgsqlConnection conn;
        public Form26()
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
        }
        private void Client()
        {
            // Создадим новый набор данных

            DataSet datasetmain = new DataSet();

            // Открываем подключение
            conn.Open();

            datasetmain.Clear();

            NpgsqlCommand command = new NpgsqlCommand(
             "SELECT " +
             "c.\"INN\" AS \"ИНН\", " +
             "ad.\"ID\" AS \"AddressID\"," +
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
             "WHERE c.\"Arhive\" IS NULL OR c.\"Arhive\" <> 'В архиве'" +
             "ORDER BY c.\"INN\"",
             conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            // Заполняем набор данных данными, которые вернул запрос
            da.Fill(datasetmain, "\"Client\"");

            // Связываем элемент DataGridView1 с набором данных
            dataGridView1.DataSource = datasetmain;

            dataGridView1.DataMember = "\"Client\"";
            dataGridView1.Columns["AddressID"].Visible = false;

            // Закрываем подключение
            conn.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form26_Load(object sender, EventArgs e)
        {
            Client();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            this.Hide();

            using (Form27 editForm = new Form27())
            {
                editForm.ShowDialog();
            }
            Client();
            this.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
          dataGridView1.CurrentRow.Cells["ИНН"].Value == null ||
          dataGridView1.CurrentRow.Cells["ИНН"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите клиента для редактирования",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            this.Hide();
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            int adres = Convert.ToInt32(row.Cells["AddressID"].Value);
            string inn = row.Cells["ИНН"].Value.ToString();
            string organization = row.Cells["Организация"].Value.ToString();
            string sername = row.Cells["Фамилия"].Value.ToString();
            string name = row.Cells["Имя"].Value.ToString();
            string patronomic = row.Cells["Отчество"].Value.ToString();
            string number = row.Cells["Номер телефона"].Value.ToString();
            string account_number = row.Cells["Номер счета"].Value.ToString();
            string email = row.Cells["Почта"].Value.ToString();


            using (Form28 editForm = new Form28(adres, inn, organization, sername, name, patronomic, number, account_number, email))
            {
                editForm.ShowDialog();
            }
            Client();
            this.Show();
        }

        private void button3_Click(object sender, EventArgs e)
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
            }

            DataGridViewRow row = dataGridView1.CurrentRow;


            // Получаем ID выбранной услуги
            string inn = row.Cells["ИНН"].Value.ToString();

            // Подтверждение удаления
            DialogResult result = MessageBox.Show(
                "Вы уверены, что хотите удалить выбранного клиента?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                conn.Open();

                NpgsqlCommand cmd = new NpgsqlCommand(
                    "DELETE FROM curse.\"Client\" WHERE \"INN\" = @inn",
                   conn
                 );
                cmd.Parameters.AddWithValue("@inn", inn);
                cmd.ExecuteNonQuery();

                MessageBox.Show(
                    "Клиент успешно удален.",
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
            }

            // Обновляем таблицу
            Client();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();

            using (Form29 editForm = new Form29())
            {
                editForm.ShowDialog();
            }
            Client();
            this.Show();
        }
    }
}
