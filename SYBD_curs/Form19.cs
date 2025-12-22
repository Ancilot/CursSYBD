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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SYBD_curs
{
    public partial class Form19 : Form
    {
        private NpgsqlConnection conn;
        private int contractID;
        private int menegerID;
        private string innID;
        private int objID;
        public Form19(int id, string inn, int obj, DateTime start, DateTime finish, int meneger)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            dateTimePicker1.Value = start.Date;
            dateTimePicker2.Value = finish.Date;
            contractID = id;
            menegerID = meneger;
            innID = inn;
            objID = obj;
        }

        private void button1_Click(object sender, EventArgs e)
        {
          
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
                comboBox2.SelectedValue = menegerID;
            }
            finally
            {
                conn.Close();
            }
        }
        private void Form19_Load(object sender, EventArgs e)
        {
            boxClient();
            objects();
            meneger();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
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
                comboBox1.SelectedValue = innID;
            }
            finally
            {
                conn.Close();
            }
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
                comboBox3.SelectedValue = objID;
            }
            finally
            {
                conn.Close();
            }
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

        private void button5_Click(object sender, EventArgs e)
        {

            try
            {
                conn.Open();

                NpgsqlCommand cmd = new NpgsqlCommand(
                    "UPDATE curse.\"Contract\" SET " +
                 "\"Client\" = @client, " +
                 "\"Object\" = @object, " +
                 "\"Start_date\" = @start_date, " +
                 "\"Finish_date\" = @finish_date, " +
                 "\"Manager\" = @meneger " +
                 "WHERE \"ID\" = @id",
                  conn); 

                cmd.Parameters.AddWithValue("client", comboBox1.SelectedValue);
                cmd.Parameters.AddWithValue("object", comboBox3.SelectedValue);
                cmd.Parameters.AddWithValue("start_date", dateTimePicker1.Value.Date);
                cmd.Parameters.AddWithValue("finish_date", dateTimePicker2.Value.Date);
                cmd.Parameters.AddWithValue("meneger", comboBox2.SelectedValue);
                cmd.Parameters.AddWithValue("id", contractID);

                cmd.ExecuteNonQuery();


                MessageBox.Show(
                    "Клиент успешно обнавлен",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                this.Close();
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            using (Form30 editForm = new Form30(this, contractID))
            {
                editForm.ShowDialog();
            }
            this.Show();
        }
    }
    
}
