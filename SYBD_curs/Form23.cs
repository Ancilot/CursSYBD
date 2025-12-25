using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form23 : Form
    {
        private NpgsqlConnection conn;
        public Form23()
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            this.Hide();

            using (Form24 editForm = new Form24())
            {
                editForm.ShowDialog();
            }
            Objects();
            this.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
           dataGridView1.CurrentRow.Cells["ID"].Value == null ||
           dataGridView1.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите объект для редактирования",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            this.Hide();
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            int id = Convert.ToInt32(row.Cells["ID"].Value);
            int street = Convert.ToInt32(row.Cells["AddressID"].Value);
            string name = row.Cells["Название"].Value.ToString();
            string type = row.Cells["Тип объекта"].Value.ToString();

            using (Form25 editForm = new Form25(id, name, street, type))
            {
                editForm.ShowDialog();
            }
            Objects();
            this.Show();
        }

        private void Objects()
        {
            // Создадим новый набор данных

            DataSet datasetmain = new DataSet();

            // Открываем подключение
            conn.Open();

            datasetmain.Clear();

            NpgsqlCommand command = new NpgsqlCommand(
             "SELECT " +
             "ob.\"ID\", " +
             "ad.\"ID\" AS \"AddressID\"," +
             "ob.\"Name\" AS \"Название\", " +
             "concat(ad.\"Home\", ' ', ad.\"Street\", ' ', ad.\"City\", ' ', ad.\"Region\", ' ', ad.\"Country\") AS \"Адрес\", " +
             "ob.\"Object_type\" AS \"Тип объекта\" " +
             "FROM curse.\"Object\" ob " +
             "JOIN curse.\"Addres\" ad ON ad.\"ID\" = ob.\"Address\" " +
             "ORDER BY ob.\"ID\"",
             conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            // Заполняем набор данных данными, которые вернул запрос
            da.Fill(datasetmain, "\"Object\"");

            // Связываем элемент DataGridView1 с набором данных
            dataGridView1.DataSource = datasetmain;

            dataGridView1.DataMember = "\"Object\"";
            dataGridView1.Columns["ID"].Visible = false;
            dataGridView1.Columns["AddressID"].Visible = false;

            // Закрываем подключение
            conn.Close();
        }

        private void Form23_Load(object sender, EventArgs e)
        {
            Objects();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
          dataGridView1.CurrentRow.Cells["ID"].Value == null ||
          dataGridView1.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите объект для удаления",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            DataGridViewRow row = dataGridView1.CurrentRow;


            // Получаем ID выбранной услуги
            int objectId = Convert.ToInt32(
                dataGridView1.SelectedRows[0].Cells["ID"].Value
            );

            // Подтверждение удаления
            DialogResult result = MessageBox.Show(
                "Вы уверены, что хотите удалить выбранный объект?",
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
                   "DELETE FROM curse.\"Object\" WHERE \"ID\" = @id",
                   conn
                 );
                cmd.Parameters.AddWithValue("@id", objectId);
                cmd.ExecuteNonQuery();

                MessageBox.Show(
                    "Объект успешно удален.",
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
            Objects();


        }
    }
}
