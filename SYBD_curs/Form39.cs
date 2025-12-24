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
using System.Xml.Linq;

namespace SYBD_curs
{
    public partial class Form39 : Form
    {
        private NpgsqlConnection conn;
        private int Con_smenId;
        public Form39(int id)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            Con_smenId = id;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void incident()
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
              "AND (\"Arhive\" IS NULL OR \"Arhive\" <> 'В архиве') " +
              "ORDER BY \"Data_Time\"",
               conn
                );

            // Передаем параметр
            command.Parameters.AddWithValue("@id_contract_graphik", Con_smenId);



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
        private void Form39_Load(object sender, EventArgs e)
        {
            incident();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (Form40 editForm = new Form40(Con_smenId))
            {
                editForm.ShowDialog();
            }
            incident();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (Form42 editForm = new Form42(Con_smenId))
            {
                editForm.ShowDialog();
            }
            incident();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
           dataGridView1.CurrentRow.Cells["ID"].Value == null ||
           dataGridView1.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите смену для обновления",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];

            int id = Convert.ToInt32(row.Cells["ID"].Value);
            string inc = row.Cells["Происшествие"].Value.ToString();
            string mera = row.Cells["Принятые меры"].Value.ToString();
            DateTime data = Convert.ToDateTime(row.Cells["Дата"].Value);

            using (Form41 editForm = new Form41(id, inc, mera, data))
            {
                editForm.ShowDialog();
            }
            incident();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
          dataGridView1.CurrentRow.Cells["ID"].Value == null ||
          dataGridView1.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите происшествие для удаления",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            // Подтверждение удаления
            DialogResult result = MessageBox.Show(
                "Вы уверены, что хотите удалить выбранное происшествие?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            int id = Convert.ToInt32(row.Cells["ID"].Value);

            try
            {
                conn.Open();

                NpgsqlCommand cmd = new NpgsqlCommand(
                     "DELETE FROM curse.\"Includents\" WHERE \"ID\" = @id",
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
            finally
            {
                conn.Close();
                incident();
            }
        }
    }
}
