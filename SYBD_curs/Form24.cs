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
    public partial class Form24 : Form
    {
        private NpgsqlConnection conn;
        public Form24()
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
        }

        private void boxAdres()
        {
            conn.Open();

            try
            {
                var cmd = new NpgsqlCommand(
                    "SELECT " +
                    "\"ID\", " +
                    "concat(\"Home\", ', ', \"Street\", ', ', \"City\", ', ', \"Region\", ', ', \"Country\") AS full_address " +
                    "FROM curse.\"Addres\" " +
                    "ORDER BY \"ID\"",
                    conn
                );

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "full_address";
                comboBox1.ValueMember = "ID";
            }
            finally
            {
                conn.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (Form21 editForm = new Form21())
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    int newAddressId = (int)editForm.Tag;

                    boxAdres(); // обновляем список

                    comboBox1.SelectedValue = newAddressId;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            int addressId = Convert.ToInt32(comboBox1.SelectedValue);

            try
            {

                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "INSERT INTO curse.\"Object\" (\"Name\", \"Address\", \"Object_type\") " +
                    "VALUES (@name, @address, @type)",
                    conn
                );

                cmd.Parameters.AddWithValue("name", textBox1.Text.Trim());
                cmd.Parameters.AddWithValue("address", addressId);
                cmd.Parameters.AddWithValue("type", textBox2.Text.Trim());

                cmd.ExecuteNonQuery();


                MessageBox.Show(
                    "Объект успешно добавлен",
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
                    if (ex.ConstraintName == "object_name_no_whitespace")
                    {
                        MessageBox.Show(
                            "Не все поля заполнены",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                    if (ex.ConstraintName == "object_object_type_no_whitespace")
                    {
                        MessageBox.Show(
                            "Не все поля заполнены",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
                if (ex.SqlState == "P0001")
                {
                    MessageBox.Show(
                        ex.MessageText,
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }
            }
            finally { conn.Close(); }
        }

        private void Form24_Load(object sender, EventArgs e)
        {
            boxAdres();
        }
    }
}