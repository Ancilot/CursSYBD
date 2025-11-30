using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form3 : Form
    {
        private NpgsqlConnection conn;
        public Form3()
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

        private void Form3_Load(object sender, EventArgs e)
        {
            // Создадим новый набор данных
            DataSet datasetmain1 = new DataSet();
            DataSet datasetmain2 = new DataSet();
            DataSet datasetmain3 = new DataSet();
            DataSet datasetmain4 = new DataSet();
            DataSet datasetmain5 = new DataSet();
            DataSet datasetmain6 = new DataSet();
            DataSet datasetmain7 = new DataSet();
            // Открываем подключение
            conn.Open();
            // Очищаем набор данных
            datasetmain1.Clear();
            datasetmain2.Clear();
            datasetmain3.Clear();
            datasetmain4.Clear();
            datasetmain5.Clear();
            datasetmain6.Clear();
            datasetmain7.Clear();
            NpgsqlCommand command1 = new NpgsqlCommand("SELECT * FROM curse.\"Employee\"", conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da1 = new NpgsqlDataAdapter(command1);
            // Заполняем набор данных данными, которые вернул запрос
            da1.Fill(datasetmain1, "\"Employee\"");

            NpgsqlCommand command2 = new NpgsqlCommand("SELECT * FROM curse.\"Includents\"", conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da2 = new NpgsqlDataAdapter(command2);
            // Заполняем набор данных данными, которые вернул запрос
            da2.Fill(datasetmain2, "\"Includents\"");

            NpgsqlCommand command3 = new NpgsqlCommand("SELECT * FROM curse.\"graphik_smen\"", conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da3 = new NpgsqlDataAdapter(command3);
            // Заполняем набор данных данными, которые вернул запрос
            da3.Fill(datasetmain3, "\"graphik_smen\"");

            NpgsqlCommand command4 = new NpgsqlCommand(@"SELECT 
             el.""ID"",
             e.""Surname"" || ' ' || e.""Name"" || ' ' || e.""Patronymic"" AS ""Employee"",
             l.""Name"" AS ""License""
             FROM curse.""Employee_License"" el
             JOIN curse.""Employee"" e ON el.""ID_Employee"" = e.""ID""
             JOIN curse.""License"" l ON el.""ID_License"" = l.""ID"";", conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da4 = new NpgsqlDataAdapter(command4);
            // Заполняем набор данных данными, которые вернул запрос
            da4.Fill(datasetmain4, "\"Employee_License\"");

            NpgsqlCommand command5 = new NpgsqlCommand(@"SELECT 
             eg.""ID"",
             e.""Surname"" || ' ' || e.""Name"" || ' ' || e.""Patronymic"" AS ""Employee"",
             g.""ID"" AS ""Graphik"",
             g.""Type_smena"" AS ""Type_smena""
             FROM curse.""Employee_graphik"" eg
             JOIN curse.""Employee"" e ON eg.""ID_Employee"" = e.""ID""
             JOIN curse.""graphik_smen"" g ON eg.""ID_graphik"" = g.""ID"";", conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da5 = new NpgsqlDataAdapter(command5);
            // Заполняем набор данных данными, которые вернул запрос
            da5.Fill(datasetmain5, "\"Employee_graphik\"");

            NpgsqlCommand command6 = new NpgsqlCommand(@"SELECT 
             cg.""ID"",
             c.""ID"" AS ""Contract"",
             c.""Start_date"" || '_' || c.""Finish_date"" AS ""Date_Contract"",
             g.""ID"" AS ""Graphik"",
             g.""Date_time_start"" || '_' || g.""Date_time_finish"" AS ""Date_graphik""
             FROM curse.""Contract_graphik"" cg
             JOIN curse.""Contract"" c ON cg.""ID_Contract"" = c.""ID""
             JOIN curse.""graphik_smen"" g ON cg.""ID_graphik"" = g.""ID"";", conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da6 = new NpgsqlDataAdapter(command6);
            // Заполняем набор данных данными, которые вернул запрос
            da6.Fill(datasetmain6, "\"Contract_graphik\"");

            NpgsqlCommand command7 = new NpgsqlCommand("SELECT * FROM curse.\"License\"", conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da7 = new NpgsqlDataAdapter(command7);
            // Заполняем набор данных данными, которые вернул запрос
            da7.Fill(datasetmain7, "\"License\"");



            // Связываем элемент DataGridView1 с набором данных
            dataGridView1.DataSource = datasetmain1;
            dataGridView2.DataSource = datasetmain2;
            dataGridView3.DataSource = datasetmain3;
            dataGridView4.DataSource = datasetmain4;
            dataGridView5.DataSource = datasetmain5;
            dataGridView6.DataSource = datasetmain6;
            dataGridView7.DataSource = datasetmain7;

            dataGridView1.DataMember = "\"Employee\"";
            dataGridView2.DataMember = "\"Includents\"";
            dataGridView3.DataMember = "\"graphik_smen\"";
            dataGridView4.DataMember = "\"Employee_License\"";
            dataGridView5.DataMember = "\"Employee_graphik\"";
            dataGridView6.DataMember = "\"Contract_graphik\"";
            dataGridView7.DataMember = "\"License\"";
            // Закрываем подключение
            conn.Close();
        }
    }
}
