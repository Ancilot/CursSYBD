using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form16 : Form
    {
        private NpgsqlConnection conn;
        private int ContractId;
        public Form16(int id)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            ContractId = id;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form16_Load(object sender, EventArgs e)
        {
            // Создадим новый набор данных

            DataSet datasetmain = new DataSet();

            // Открываем подключение
            conn.Open();

            datasetmain.Clear();
            NpgsqlCommand command = new NpgsqlCommand(
              "SELECT " +
              "gs.\"Name_smena\" AS \"Смена\", " +
              "i.\"Processing_status\"  AS \"Процесс выполнения\", " +
              "i.\"Incedent\"  AS \"Происшествия\", " +
              "i.\"Data_Time\"  AS \"Дата\", " +
              "i.\"Measures_taken\"  AS \"Принятые меры\" " +
             "FROM curse.\"Includents\" i " +
             "JOIN curse.\"Contract_graphik\" cg ON cg.\"ID\" = i.\"Contract_Smena\" " +
             "JOIN curse.graphik_smen gs ON gs.\"ID\" = cg.\"ID_graphik\" " +
             "WHERE cg.\"ID_Contract\" = @contract_id " +
             "AND (i.\"Arhive\" IS NULL OR i.\"Arhive\" <> 'В архиве') " +
             "ORDER BY gs.\"Name_smena\"",
              conn
                );
            command.Parameters.AddWithValue("@contract_id", ContractId);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            // Заполняем набор данных данными, которые вернул запрос
            da.Fill(datasetmain, "\"Contract\"");

            // Связываем элемент DataGridView1 с набором данных
            dataGridView1.DataSource = datasetmain;

            dataGridView1.DataMember = "\"Contract\"";

            // Закрываем подключение
            conn.Close();
        }
    }
}
