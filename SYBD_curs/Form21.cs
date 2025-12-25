using Npgsql;
using System;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form21 : Form
    {
        private NpgsqlConnection conn;
        public Form21()
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string Home = textBox1.Text.Trim();
            string Street = textBox2.Text.Trim();
            string City = textBox3.Text.Trim();
            string Region = textBox4.Text.Trim();
            string Country = textBox5.Text.Trim();


            conn.Open();

            using (var command = new NpgsqlCommand(
                "INSERT INTO curse.\"Addres\" (\"Home\", \"Street\", \"City\", \"Region\", \"Country\") " +
                "VALUES (@home, @street, @city, @region, @country) RETURNING \"ID\"", conn))
            {
                command.Parameters.AddWithValue("home", Home);
                command.Parameters.AddWithValue("street", Street);
                command.Parameters.AddWithValue("city", City);
                command.Parameters.AddWithValue("region", Region);
                command.Parameters.AddWithValue("country", Country);

                try
                {
                    int newAddressId = (int)command.ExecuteScalar();
                    MessageBox.Show(
                        "Адрес успешно добавлен",
                        "Успех",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    this.Tag = newAddressId;
                    this.DialogResult = DialogResult.OK;
                    conn.Close();
                    this.Close(); // закрываем форму после успешного добавления
                }
                catch (PostgresException ex)
                {
                    // Нарушение CHECK
                    if (ex.SqlState == "23514")
                    {
                        if (ex.ConstraintName == "addres_home_no_whitespace")
                        {
                            MessageBox.Show(
                                "Не все поля заполнены",
                                "Ошибка",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            conn.Close();
                            return;
                        }

                        if (ex.ConstraintName == "addres_street_no_whitespace")
                        {
                            MessageBox.Show(
                                "Не все поля заполнены",
                                "Ошибка",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            conn.Close();
                            return;
                        }
                        if (ex.ConstraintName == "addres_city_no_whitespace_no_digits")
                        {
                            MessageBox.Show(
                                "Поле \"Город\" введено некорректно, оно не може быть пустым или содержать цифры",
                                "Ошибка",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            conn.Close();
                            return;
                        }
                        if (ex.ConstraintName == "addres_region_no_whitespace_no_digits")
                        {
                            MessageBox.Show(
                                "Поле \"Регион\" введено некорректно, оно не може быть пустым или содержать цифры",
                                "Ошибка",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            conn.Close();
                            return;
                        }
                        if (ex.ConstraintName == "addres_country_no_whitespace_no_digits")
                        {
                            MessageBox.Show(
                                "Поле \"Страна\" введено некорректно, оно не може быть пустым или содержать цифры",
                                "Ошибка",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            conn.Close();
                            return;
                        }
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
            }
        }
    }
}
