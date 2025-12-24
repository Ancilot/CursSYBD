using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace SYBD_curs
{
    public partial class Form36 : Form
    {
        private int smenID;
        private NpgsqlConnection conn;
        public Form36( int id)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            smenID = id;
        }
        private void Form36_Load(object sender, System.EventArgs e)
        {
            // Создадим новый набор данных
            DataSet datasetmain = new DataSet();

            // Открываем подключение
            conn.Open();

            datasetmain.Clear();

            NpgsqlCommand command = new NpgsqlCommand(
              "SELECT " +
              "cg.\"ID\"," +
              "c.\"ID\" AS \"Номер контракта\", " +
              "c.\"Start_date\" AS \"Дата начала\", " +
              "c.\"Finish_date\" AS \"Дата окончания\", " +
              "o.\"Name\" AS \"Объект охраны\" " +
              "FROM curse.\"Contract_graphik\" cg " +
              "JOIN curse.\"Contract\" c ON c.\"ID\" = cg.\"ID_Contract\" " +
              "JOIN curse.\"Object\" o ON o.\"ID\" = c.\"Object\" " +
              "WHERE cg.\"ID_graphik\" = @id_graphik " +
              "ORDER BY c.\"Start_date\"",
              conn
              );
            command.Parameters.AddWithValue("@id_graphik", smenID);


            // Адаптер
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);

            // Заполняем DataSet
            da.Fill(datasetmain, "\"Contract_graphik\"");

            // Привязываем к DataGridView
            dataGridView1.DataSource = datasetmain;
            dataGridView1.DataMember = "\"Contract_graphik\"";

            // Скрываем технический ID
            dataGridView1.Columns["ID"].Visible = false;

            // Закрываем подключение
            conn.Close();
        }

        private void button2_Click(object sender, EventArgs e)
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
                    "Выберите контракт для просмотра",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];

            int id = Convert.ToInt32(row.Cells["ID"].Value);
            this.Hide();
            using (Form39 editForm = new Form39(id))
            {
                editForm.ShowDialog();
            }
            this.Show();
        }
    }
}
