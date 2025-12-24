using Npgsql;
using System;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form14 : Form
    {
        private NpgsqlConnection conn;
        private int RemindeId;
        public Form14(int id, string remind)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            RemindeId = id;
            textBox1.Text = remind;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string reminer = textBox1.Text.Trim();
            try
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(
                    "UPDATE curse.\"Reminder\" " +
                    "SET \"Remeinder\" = @reminder " +
                    "WHERE \"ID\" = @id",
                    conn))
                {
                    cmd.Parameters.AddWithValue("reminder", reminer);
                    cmd.Parameters.AddWithValue("@id", RemindeId);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show(
                    "Напоминание успешно обновлено",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                this.Close();
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
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
