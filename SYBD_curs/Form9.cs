using Npgsql;
using System;
using System.Globalization;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form9 : Form
    {
        private NpgsqlConnection conn;
        public Form9()
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
            // Проверка цены
            if (!decimal.TryParse(textBox2.Text.Replace(',', '.'),
                NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
            {
                MessageBox.Show("Введите корректную цену", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string serviceName = textBox1.Text.Trim();


            conn.Open();

            using (var command = new NpgsqlCommand(
                "INSERT INTO curse.\"Services\" (\"Name\", \"Price\") VALUES (@name, @price)", conn))
            {
                command.Parameters.AddWithValue("name", serviceName);
                command.Parameters.AddWithValue("price", price);

                try
                {
                    command.ExecuteNonQuery();

                    MessageBox.Show(
                        "Услуга успешно добавлена",
                        "Успех",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    this.Close(); // закрываем форму после успешного добавления
                }
                catch (PostgresException ex)
                {
                    // Нарушение CHECK
                    if (ex.SqlState == "23514")
                    {
                        if (ex.ConstraintName == "services_price_positive")
                        {
                            MessageBox.Show(
                                "Цена не может быть меньше 0",
                                "Ошибка",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            return;
                        }

                        if (ex.ConstraintName == "services_name_no_whitespace")
                        {
                            MessageBox.Show(
                                "Название услуги не может быть пустым",
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

        private void Form9_Load(object sender, EventArgs e)
        {

        }
    }
}
