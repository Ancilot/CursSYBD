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
    public partial class Form20 : Form
    {
        private NpgsqlConnection conn;
        public Form20()
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
        }

        private void adress()
        {
            // Создадим новый набор данных

            DataSet datasetmain = new DataSet();

            // Открываем подключение
            conn.Open();

            datasetmain.Clear();

            NpgsqlCommand command = new NpgsqlCommand("SELECT " +
            "\"ID\", " +
            "\"Home\" AS \"Дом\", " +
            "\"Street\" AS \"Улица\", " +
            "\"City\" AS \"Город\", " +
            "\"Region\" AS \"Регион\", " +
            "\"Country\" AS \"Страна\" " +
            "FROM curse.\"Addres\"",
            conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            // Заполняем набор данных данными, которые вернул запрос
            da.Fill(datasetmain, "\"Addres\"");

            // Связываем элемент DataGridView1 с набором данных
            dataGridView1.DataSource = datasetmain;

            dataGridView1.DataMember = "\"Addres\"";
            dataGridView1.Columns["ID"].Visible = false;

            // Закрываем подключение
            conn.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();

            using (Form21 editForm = new Form21())
            {
                editForm.ShowDialog();
            }
            adress();
            this.Show();
        }

        private void Form20_Load(object sender, EventArgs e)
        {
            adress();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
            dataGridView1.CurrentRow.Cells["ID"].Value == null ||
            dataGridView1.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите адрес для редактирования",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            this.Hide();
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            int id = Convert.ToInt32(row.Cells["ID"].Value);
            string Home = row.Cells["Дом"].Value.ToString();
            string Street = row.Cells["Улица"].Value.ToString();
            string City = row.Cells["Город"].Value.ToString();
            string Region = row.Cells["Регион"].Value.ToString();
            string Country = row.Cells["Страна"].Value.ToString();
            using (Form22 editForm = new Form22(id,Home, Street, City, Region, Country))
            {
                editForm.ShowDialog();
            }
            adress();
            this.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
           dataGridView1.CurrentRow.Cells["ID"].Value == null ||
           dataGridView1.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите адрес для удаления",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            DataGridViewRow row = dataGridView1.CurrentRow;


            // Получаем ID выбранной услуги
            int addresId = Convert.ToInt32(
                dataGridView1.SelectedRows[0].Cells["ID"].Value
            );

            // Подтверждение удаления
            DialogResult result = MessageBox.Show(
                "Вы уверены, что хотите удалить выбранный адрес?",
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
                   "DELETE FROM curse.\"Addres\" WHERE \"ID\" = @id",
                   conn
                 );
                cmd.Parameters.AddWithValue("@id", addresId);
                cmd.ExecuteNonQuery();

                MessageBox.Show(
                    "Адрес успешно удален.",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (PostgresException ex)
            {
                if(ex.SqlState == "P0001")
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
            adress();
            
        }
    }
}
