using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form27 : Form
    {
        private NpgsqlConnection conn;
        public Form27()
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
        }

        private void button1_Click(object sender, EventArgs e)
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

        private void Form27_Load(object sender, EventArgs e)
        {
            boxAdres();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int addressId = Convert.ToInt32(comboBox1.SelectedValue);

            try
            {

                conn.Open();
                using (var checkCmd = new NpgsqlCommand(
                  "SELECT 1 FROM curse.\"Client\" WHERE \"INN\" = @inn",
                   conn))
                {
                    checkCmd.Parameters.AddWithValue("inn", textBox1.Text.Trim());

                    if (checkCmd.ExecuteScalar() != null)
                    {
                        MessageBox.Show(
                            "Клиент с таким ИНН уже существует",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                }
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "INSERT INTO curse.\"Client\" (\"INN\", \"Name_organization\", \"Surname\", " +
                    "\"Name\", \"Patronymic\", \"Address\", " +
                    " \"Number\", \"Account_number\",\"Email\") " +
                    "VALUES (@inn, @name_organization, @surname, @name, @patronymic, @address, @number, @account_number, @email) RETURNING \"INN\"",
                    conn
                );

                cmd.Parameters.AddWithValue("inn", textBox1.Text.Trim());
                if (string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    cmd.Parameters.AddWithValue("name_organization", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("name_organization", textBox2.Text.Trim());
                }
                cmd.Parameters.AddWithValue("surname", textBox3.Text.Trim());
                cmd.Parameters.AddWithValue("name", textBox4.Text.Trim());
                cmd.Parameters.AddWithValue("patronymic", textBox5.Text.Trim());
                cmd.Parameters.AddWithValue("address", addressId);
                cmd.Parameters.AddWithValue("number", textBox6.Text.Trim());
                cmd.Parameters.AddWithValue("account_number", textBox7.Text.Trim());
                cmd.Parameters.AddWithValue("email", textBox8.Text.Trim());


                string newINNId = (string)cmd.ExecuteScalar();


                MessageBox.Show(
                    "Клиент успешно добавлен",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                this.Tag = newINNId;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "23514") // CHECK
                {
                    if (ex.ConstraintName == "client_inn_type_logic")
                    {
                        MessageBox.Show(
                            "Неверно введены данные клиента.\n\n" +
                            "---ПРИМЕР ВВОДА---\n" +
                            "Физическое лицо:\n" +
                             " ИНН — 12 цифр\n" +
                             " поле «Организация» пустое\n" +
                             "счёт начинается с 408\n\n" +
                             "Юридическое лицо:\n" +
                             " ИНН — 10 цифр\n" +
                             " поле «Организация» обязательно\n" +
                             " счёт начинается с 405 по 407",
                             "Ошибка ввода",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "inn_digits_only")
                    {
                        MessageBox.Show(
                            "ИНН может содержать только цифры",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "inn_length_check")
                    {
                        MessageBox.Show(
                            "Длина ИНН должна быть 10 или 12 цифр",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "client_name_organization_no_whitespace")
                    {
                        MessageBox.Show(
                            "Поле организации не может содержать пробелы",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "client_surname_no_whitespace_no_digits")
                    {
                        MessageBox.Show(
                            "Поле с фамилией не может быть пустым или содержать цифры",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "client_name_no_whitespace_no_digits")
                    {
                        MessageBox.Show(
                            "Поле с именем не может быть пустым или содержать цифры",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "client_patronymic_no_whitespace_no_digits")
                    {
                        MessageBox.Show(
                            "Поле с отчеством не может быть пустым или содержать цифры",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "number_check")
                    {
                        MessageBox.Show(
                            "Некорретно введен номер. Номер должен быть длины 10 цифр, не содержать других символов и начинаться с 8",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "client_number_no_whitespace")
                    {
                        MessageBox.Show(
                            "Не зполнено поле с номером телефона",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "account_number_check")
                    {
                        MessageBox.Show(
                            "Поле с лицевым счетом не может быть пусты и должно начинаться:\n Для юридического лица: 405, 406, 407\n " +
                            "Для физического лица: 408",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "client_email_no_whitespace")
                    {
                        MessageBox.Show(
                            "Поле почты не может содержать пробелы",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }

                }
            }
            finally { conn.Close(); }
        }
    }
}
