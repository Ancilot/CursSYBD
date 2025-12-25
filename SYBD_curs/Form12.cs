using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form12 : Form
    {
        private int RemindeId;
        private NpgsqlConnection conn;
        private int selectetReminderId = -1;
        public Form12(int id)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            RemindeId = id;
            dataGridView1.CellClick += dataGridView1_CellClick;
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var cellValue = dataGridView1.Rows[e.RowIndex].Cells["ID"].Value;

            if (cellValue == null || cellValue == DBNull.Value)
                return;

            selectetReminderId = Convert.ToInt32(cellValue);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Remiminde()
        {
            DataSet datasetmain = new DataSet();

            try
            {
                conn.Open();
                datasetmain.Clear();

                NpgsqlCommand command = new NpgsqlCommand(
                    "SELECT " +
                    "r.\"ID\", " +
                    "r.\"Remeinder\" AS \"Напоминание\", " +
                    "r.\"Data\" AS \"Дата создания\"" +
                    "FROM curse.\"Reminder\" r " +
                    "WHERE r.\"Contract_ID\" = @contract_id " +
                    "ORDER BY r.\"Data\" ASC",
                    conn
                );

                command.Parameters.AddWithValue("@contract_id", RemindeId);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
                da.Fill(datasetmain, "Reminder");

                // Проверка: есть ли напоминания
                if (datasetmain.Tables["Reminder"].Rows.Count == 0)
                {
                    MessageBox.Show(
                        "Напоминаний для этого договора ещё нет",
                        "Информация",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    dataGridView1.DataSource = null;
                    return;
                }

                dataGridView1.DataSource = datasetmain;
                dataGridView1.DataMember = "Reminder";
                dataGridView1.Columns["ID"].Visible = false;
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Ошибка загрузки напоминаний ",
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

        private void Form12_Load(object sender, EventArgs e)
        {
            Remiminde();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();

            using (Form13 editForm = new Form13(RemindeId))
            {
                editForm.ShowDialog();
            }
            Remiminde();
            this.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (selectetReminderId == -1)
            {
                MessageBox.Show("Выберите строку в таблице для удаления");
                return;
            }

            // Подтверждение удаления
            DialogResult result = MessageBox.Show(
                "Вы уверены, что хотите удалить выбранное напоминание у договора?",
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
                "DELETE FROM curse.\"Reminder\" WHERE \"ID\" = @id",
                   conn
                 );
                cmd.Parameters.AddWithValue("@id", selectetReminderId);
                cmd.ExecuteNonQuery();

                MessageBox.Show(
                    "Напоминание успешно удалено.",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка при удалении напоминания:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                selectetReminderId = -1;
                conn.Close();
            }

            // Обновляем таблицу
            Remiminde();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectetReminderId == -1)
            {
                MessageBox.Show("Выберите строку в таблице");
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            string reminder = row.Cells["Напоминание"].Value.ToString();
            this.Hide();

            using (Form14 editForm = new Form14(selectetReminderId, reminder))
            {
                editForm.ShowDialog();
            }
            Remiminde();
            selectetReminderId = -1;
            this.Show();
        }
    }
}
