using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace SYBD_curs
{
    public partial class Form2 : Form
    {
        private NpgsqlConnection conn;
        public Form2()
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Создадим новый набор данных
           
            DataSet datasetmain = new DataSet();
     
            // Открываем подключение
            conn.Open();
          
            datasetmain.Clear();
            
            NpgsqlCommand command = new NpgsqlCommand("SELECT " +
            "c.\"ID\" AS Number_contract, " +
            "concat(cl.\"Surname\", ' ', cl.\"Name\", ' ', cl.\"Patronymic\") AS client, " +
            "o.\"Name\" AS object, " +
            "c.\"Start_date\", " +
            "c.\"Finish_date\", " +
            "concat(e.\"Surname\", ' ', e.\"Name\", ' ', e.\"Patronymic\") AS manager " +
            "FROM curse.\"Contract\" c " +
            "JOIN curse.\"Client\" cl ON cl.\"INN\" = c.\"Client\" " +
            "JOIN curse.\"Object\" o ON o.\"ID\" = c.\"Object\" " +
            "JOIN curse.\"Employee\" e ON e.\"ID\" = c.\"Manager\"" +
             "ORDER BY c.\"ID\" ASC",
            conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            // Заполняем набор данных данными, которые вернул запрос
            da.Fill(datasetmain, "\"Contract\"");

            // Связываем элемент DataGridView1 с набором данных
            dataGridView.DataSource = datasetmain;
        
            dataGridView.DataMember = "\"Contract\"";
            
            // Закрываем подключение
            conn.Close();
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }
    }
}
