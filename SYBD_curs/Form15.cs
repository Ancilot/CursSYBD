using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form15 : Form
    {
        private NpgsqlConnection conn;
        public Form15()
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (Form27 editForm = new Form27())
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    string newINNId = (string)editForm.Tag;

                    boxClient(); // обновляем список

                    comboBox1.SelectedValue = newINNId;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue == null || comboBox2.SelectedValue == null || comboBox3.SelectedValue == null)
            {
                MessageBox.Show(
                    "Ошибка: не все данные заполнены",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return; // выходим из метода, чтобы не выполнять вставку
            }

            string clientInn = comboBox1.SelectedValue.ToString();
            int menegerId = Convert.ToInt32(comboBox2.SelectedValue);
            int objectId = Convert.ToInt32(comboBox3.SelectedValue);
            DateTime startDate = dateTimePicker1.Value.Date;
            DateTime finishDate = dateTimePicker2.Value.Date;

            try
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "INSERT INTO curse.\"Contract\" (\"Client\", \"Object\", \"Start_date\", \"Finish_date\", \"Manager\" ) " +
                    "VALUES (@client, @object, @start_date, @finish_date, @maneger) RETURNING \"ID\"",
                    conn
                );

                cmd.Parameters.AddWithValue("client", clientInn);
                cmd.Parameters.AddWithValue("object", objectId);
                cmd.Parameters.AddWithValue("start_date", startDate);
                cmd.Parameters.AddWithValue("finish_date", finishDate);
                cmd.Parameters.AddWithValue("maneger", menegerId);

                int newId = (int)cmd.ExecuteScalar();
                Form17 editForm = new Form17(this, newId);
                editForm.ShowDialog();

            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "23514") // CHECK
                {
                    if (ex.ConstraintName == "contract_dates_check")
                    {
                        MessageBox.Show(
                            "Дата начала не может быть больше даты конца договора!",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                    return;
                }

                if (ex.SqlState == "P0001")
                    MessageBox.Show(
                            ex.MessageText,
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
            }
            finally { conn.Close(); }

        }
        private void boxClient()
        {
            conn.Open();

            try
            {
                var cmd = new NpgsqlCommand(
                    "SELECT " +
                    "\"INN\", " +
                    "concat(\"Surname\", ', ', \"Name\", ', ', \"Patronymic\") AS full_name " +
                    "FROM curse.\"Client\" " +
                    "ORDER BY \"INN\"",
                    conn
                );

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "full_name";
                comboBox1.ValueMember = "INN";
            }
            finally
            {
                conn.Close();
            }
        }
        private void objects()
        {
            conn.Open();

            try
            {
                var cmd = new NpgsqlCommand(
                    "SELECT " +
                    "\"ID\", " +
                    "\"Name\" " +
                    "FROM curse.\"Object\" " +
                    "ORDER BY \"ID\"",
                    conn
                );

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                comboBox3.DataSource = dt;
                comboBox3.DisplayMember = "Name";
                comboBox3.ValueMember = "ID";
            }
            finally
            {
                conn.Close();
            }
        }
        private void meneger()
        {
            conn.Open();

            try
            {
                var cmd = new NpgsqlCommand(
                "SELECT e.\"ID\", " +
                "concat(e.\"Surname\", ', ', e.\"Name\", ', ', e.\"Patronymic\") AS full_name " +
                "FROM curse.\"Employee\" e " +
                "JOIN curse.\"Job_title\" j ON e.\"Job_title\" = j.\"ID\" " +
                "WHERE j.\"Name\" = @job_name " +
                "ORDER BY e.\"ID\"",
                 conn
                );

                cmd.Parameters.AddWithValue("job_name", "Менеджер");


                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                comboBox2.DataSource = dt;
                comboBox2.DisplayMember = "full_name";
                comboBox2.ValueMember = "ID";
            }
            finally
            {
                conn.Close();
            }
        }
        private void Form15_Load(object sender, EventArgs e)
        {
            boxClient();
            meneger();
            objects();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (Form24 editForm = new Form24())
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    int objectId = (int)editForm.Tag;

                    objects(); // обновляем список

                    comboBox3.SelectedValue = objectId;
                }
            }
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }
    }
}
