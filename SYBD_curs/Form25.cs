using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form25 : Form
    {
        private NpgsqlConnection conn;
        private int idObject;
        private int idAdres;
        public Form25(int id, string name, int street, string type)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            textBox1.Text = name;
            textBox2.Text = type;
            idAdres = street;
            idObject = id;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
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
                comboBox1.SelectedValue = idAdres;
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

        private void Form25_Load(object sender, EventArgs e)
        {
            boxAdres();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int addressId = Convert.ToInt32(comboBox1.SelectedValue);
            try
            {
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand(
                 "UPDATE curse.\"Object\" SET " +
                 "\"Name\" = @name, " +
                 "\"Address\" = @address, " +
                 "\"Object_type\" = @object_type " +
                 "WHERE \"ID\" = @id",
                  conn);
                command.Parameters.AddWithValue("id", idObject);
                command.Parameters.AddWithValue("name", textBox1.Text);
                command.Parameters.AddWithValue("address", addressId);
                command.Parameters.AddWithValue("object_type", textBox2.Text);
                command.ExecuteNonQuery();

                MessageBox.Show(
                    "Объект успешно обновлен",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                this.Close(); // закрываем форму после успешного добавления

            }
            catch (PostgresException ex)
            {
                // Нарушение CHECK
                if (ex.SqlState == "23514") // CHECK
                {
                    if (ex.ConstraintName == "object_name_no_whitespace" ||
                      ex.ConstraintName == "object_object_type_no_whitespace")
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
                // Превышение длины строки
                if (ex.SqlState == "22001")
                {
                    MessageBox.Show(
                        "Превышено допустимое количество символов в строке",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }
            }
            finally
            {
                conn.Close();
            }
        }
    }
}


