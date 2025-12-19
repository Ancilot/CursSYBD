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
    public partial class Form5 : Form
    {
        private NpgsqlConnection conn;
        public Form5()
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();

            using (Form8 editForm = new Form8())
            {
                editForm.ShowDialog();
            }

            this.Show();
        }
        private void Services()
        {
            // Создадим новый набор данных

            DataSet datasetmain = new DataSet();

            // Открываем подключение
            conn.Open();

            datasetmain.Clear();

            NpgsqlCommand command = new NpgsqlCommand(
              "SELECT " +
              "s.\"ID\", " +
              "s.\"Name\" AS \"Услуга\", " +
              "s.\"Price\" AS \"Цена\", " +
              "s.specially AS \"Особенность\" " +
              "FROM curse.\"Services\" s " +
              "ORDER BY s.\"Name\" ASC",
             conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            // Заполняем набор данных данными, которые вернул запрос
            da.Fill(datasetmain, "\"Services\"");

            // Связываем элемент DataGridView1 с набором данных
            dataGridView1.DataSource = datasetmain;

            dataGridView1.DataMember = "\"Services\"";
            dataGridView1.Columns["ID"].Visible = false;
            dataGridView1.ClearSelection();
            dataGridView1.CurrentCell = null;
            // Закрываем подключение
            conn.Close();

        }

        private void Form5_Load(object sender, EventArgs e)
        {
            Services();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();

            using (Form9 editForm = new Form9())
            {
                editForm.ShowDialog();
            }
            Services();
            this.Show();
        }

        private void button4_Click(object sender, EventArgs e)
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
            int serviceId = Convert.ToInt32(
                dataGridView1.SelectedRows[0].Cells["ID"].Value
            );

            // Подтверждение удаления
            DialogResult result = MessageBox.Show(
                "Вы уверены, что хотите удалить выбранную услугу?",
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
                  "CALL curse.delete_service(@id)",
                   conn
                 );
                cmd.Parameters.AddWithValue("@id", serviceId);
                cmd.ExecuteNonQuery();

                MessageBox.Show(
                    "Услуга успешно удалена.",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка при удалении услуги:\n" + ex.Message,
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
            Services();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
             dataGridView1.CurrentRow.Cells["ID"].Value == null ||
             dataGridView1.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите услугу для редактирования",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];

            int id = Convert.ToInt32(row.Cells["ID"].Value);
            string name = row.Cells["Услуга"].Value.ToString();
            decimal price = Convert.ToDecimal(row.Cells["Цена"].Value);

            this.Hide();

            using (Form10 editForm = new Form10(id, name, price))
            {
                editForm.ShowDialog();
            }
            Services();
            this.Show();
        }
    }
}
