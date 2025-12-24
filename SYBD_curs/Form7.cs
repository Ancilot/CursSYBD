using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form7 : Form
    {
        private NpgsqlConnection conn;
        public Form7()
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void boxStutys()
        {
            // Добавляем статусы в ComboBox
            comboBox1.Items.Add("Не начата");
            comboBox1.Items.Add("В процессе");
            comboBox1.Items.Add("Закончена");

            // Устанавливаем первый элемент по умолчанию 
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }
        private void boxSmen()
        {
            try
            {
                conn.Open();
                var cmd = new NpgsqlCommand(
                    "SELECT " +
                    "\"ID\", " +
                     "\"Name_smena\" " +
                    "FROM curse.graphik_smen",
                    conn
                );

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                comboBox2.DataSource = dt;
                comboBox2.DisplayMember = "Name_smena";
                comboBox2.ValueMember = "ID";
            }
            finally
            {
                conn.Close();
            }
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            boxStutys();
            boxSmen();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Получаем выбранный статус
            string Statuss = comboBox1.SelectedItem.ToString();

            try
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM curse.get_graphik_info_by_status(@p_status)", conn);

                cmd.Parameters.AddWithValue("p_status", Statuss);


                DataTable dt = new DataTable();
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    // Если данных нет
                    dataGridView1.DataSource = null;
                    MessageBox.Show(
                        "Смен с данным статусом нет",
                        "Результат поиска",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    conn.Close();
                    return;
                }
                dataGridView1.DataSource = dt;


            }
            finally
            {
                conn.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedValue == null)
            {
                MessageBox.Show(
                    "Ошибка: нет смен",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return; // выходим из метода, чтобы не выполнять вставку
            }
            int grapfId = Convert.ToInt32(comboBox2.SelectedValue);
            try
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM curse.get_incidents_by_smena(@p_graphik_id)", conn);

                cmd.Parameters.AddWithValue("p_graphik_id", grapfId);


                DataTable dt = new DataTable();
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    // Если данных нет
                    dataGridView1.DataSource = null;
                    MessageBox.Show(
                        "Происшествий нет",
                        "Результат поиска",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    conn.Close();
                    return;
                }
                dataGridView1.DataSource = dt;


            }
            finally
            {
                conn.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM curse.get_employee_statistics()", conn);


                DataTable dt = new DataTable();
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    // Если данных нет
                    dataGridView1.DataSource = null;
                    MessageBox.Show(
                        "Статистика отутствует",
                        "Результат поиска",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    conn.Close();
                    return;
                }
                dataGridView1.DataSource = dt;


            }
            finally
            {
                conn.Close();
            }
        }
    }
}
