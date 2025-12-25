using Npgsql;
using System;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form13 : Form
    {
        private NpgsqlConnection conn;
        private int RemindeId;
        public Form13(int id)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            RemindeId = id;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string reminer = textBox1.Text.Trim();


            conn.Open();

            using (var command = new NpgsqlCommand(
                "INSERT INTO curse.\"Reminder\" (\"Contract_ID\",\"Remeinder\") VALUES (@contract_id,@reminder)", conn))
            {
                command.Parameters.AddWithValue("contract_id", RemindeId);
                command.Parameters.AddWithValue("reminder", reminer);

                try
                {
                    command.ExecuteNonQuery();

                    MessageBox.Show(
                        "Напоминание успешно добавлено",
                        "Успех",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    conn.Close();
                    this.Close(); // закрываем форму после успешного добавления
                }
                catch (PostgresException ex)
                {
                    // Нарушение CHECK
                    if (ex.SqlState == "23514")
                    {
                        if (ex.ConstraintName == "reminder_remeinder_no_whitespace")
                        {
                            MessageBox.Show(
                                "Напоминание не может быть пустым",
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
