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
            DataSet datasetmain1 = new DataSet();
            DataSet datasetmain2 = new DataSet();
            DataSet datasetmain3 = new DataSet();
            DataSet datasetmain4 = new DataSet();
            DataSet datasetmain5 = new DataSet();
            DataSet datasetmain6 = new DataSet();
            DataSet datasetmain7= new DataSet();
            // Открываем подключение
            conn.Open();
            // Очищаем набор данных
            datasetmain1.Clear();
            datasetmain2.Clear();
            datasetmain3.Clear();
            datasetmain4.Clear();
            datasetmain5.Clear();
            datasetmain7.Clear();
            
            datasetmain7.Clear();
            NpgsqlCommand command1 = new NpgsqlCommand("SELECT * FROM curse.\"Addres\"", conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da1 = new NpgsqlDataAdapter(command1);
            // Заполняем набор данных данными, которые вернул запрос
            da1.Fill(datasetmain1, "\"Addres\"");

            NpgsqlCommand command2 = new NpgsqlCommand("SELECT * FROM curse.\"Client\"", conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da2 = new NpgsqlDataAdapter(command2);
            // Заполняем набор данных данными, которые вернул запрос
            da2.Fill(datasetmain2, "\"Client\"");

            NpgsqlCommand command3 = new NpgsqlCommand("SELECT * FROM curse.\"Contract\"", conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da3 = new NpgsqlDataAdapter(command3);
            // Заполняем набор данных данными, которые вернул запрос
            da3.Fill(datasetmain3, "\"Contract\"");

            NpgsqlCommand command4 = new NpgsqlCommand("SELECT * FROM curse.\"Object\"", conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da4 = new NpgsqlDataAdapter(command4);
            // Заполняем набор данных данными, которые вернул запрос
            da4.Fill(datasetmain4, "\"Object\"");

            NpgsqlCommand command5 = new NpgsqlCommand("SELECT * FROM curse.\"Reminder\"", conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da5 = new NpgsqlDataAdapter(command5);
            // Заполняем набор данных данными, которые вернул запрос
            da5.Fill(datasetmain5, "\"Reminder\"");

            NpgsqlCommand command6 = new NpgsqlCommand("SELECT * FROM curse.\"Services\"", conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da6 = new NpgsqlDataAdapter(command6);
            // Заполняем набор данных данными, которые вернул запрос
            da6.Fill(datasetmain6, "\"Services\"");

            NpgsqlCommand command7 = new NpgsqlCommand(@"SELECT 
             sc.""ID"",
             c.""ID"" AS ""Contract"",
             obj.""Name"" AS ""Object"",
             s.""Name"" AS ""Type_services""
             FROM curse.""Services_Contract"" sc
             JOIN curse.""Contract"" c ON sc.""ID_Contract"" = c.""ID""
             JOIN curse.""Object"" obj ON c.""Object"" = obj.""ID""
             JOIN curse.""Services"" s ON sc.""ID_Services"" = s.""ID"";", conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da7 = new NpgsqlDataAdapter(command7);
            // Заполняем набор данных данными, которые вернул запрос
            da7.Fill(datasetmain7, "\"Services_Contract\"");


            // Связываем элемент DataGridView1 с набором данных
            dataGridView1.DataSource = datasetmain1;
            dataGridView2.DataSource = datasetmain2;
            dataGridView3.DataSource = datasetmain3;
            dataGridView4.DataSource = datasetmain4;
            dataGridView5.DataSource = datasetmain5;
            dataGridView6.DataSource = datasetmain6;
            dataGridView7.DataSource = datasetmain7;

            dataGridView1.DataMember = "\"Addres\"";
            dataGridView2.DataMember = "\"Client\"";
            dataGridView3.DataMember = "\"Contract\"";
            dataGridView4.DataMember = "\"Object\"";
            dataGridView5.DataMember = "\"Reminder\"";
            dataGridView6.DataMember = "\"Services\"";
            dataGridView7.DataMember = "\"Services_Contract\"";
            // Закрываем подключение
            conn.Close();


        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
