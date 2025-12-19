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
    public partial class Form8 : Form
    {
        private NpgsqlConnection conn;
        public Form8()
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
            
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            // Создадим новый набор данных

            DataSet datasetmain = new DataSet();

            // Открываем подключение
            conn.Open();

            datasetmain.Clear();

            NpgsqlCommand command = new NpgsqlCommand(
              "SELECT " +
              "sc.\"ID\"," +
              "sc.\"ID_Contract\" AS \"Номер контракта\", " +
              "o.\"Name\" AS \"Название объекта\", " +
              "s.\"Name\" AS \"Название услуги\" " +
              "FROM curse.\"Services_Contract\" sc " +
              "JOIN curse.\"Contract\" c ON c.\"ID\" = sc.\"ID_Contract\" " +
              "JOIN curse.\"Object\" o ON o.\"ID\" = c.\"Object\" " +
              "JOIN curse.\"Services\" s ON s.\"ID\" = sc.\"ID_Services\" " +
              "ORDER BY sc.\"ID_Contract\", s.\"Name\"",
              conn
             );

            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            // Заполняем набор данных данными, которые вернул запрос
            da.Fill(datasetmain, "\"Services_Contract\"");

            // Связываем элемент DataGridView1 с набором данных
            dataGridView1.DataSource = datasetmain;

            dataGridView1.DataMember = "\"Services_Contract\"";
            dataGridView1.Columns["ID"].Visible = false;
            dataGridView1.ClearSelection();
            dataGridView1.CurrentCell = null;
            // Закрываем подключение
            conn.Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
