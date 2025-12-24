using Npgsql;
using System;
using System.Globalization;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SYBD_curs
{
    public partial class Form35 : Form
    {
        private NpgsqlConnection conn;
        public Form35()
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            DateTime startDate = dateTimePicker1.Value;
            DateTime finishDate = dateTimePicker2.Value;
            try
            {

                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "INSERT INTO curse.\"graphik_smen\" (\"Date_time_start\", \"Date_time_finish\", \"Name_smena\") " +
                    "VALUES (@date_time_start, @date_time_finish, @name_smena)",
                    conn
                );

                cmd.Parameters.AddWithValue("date_time_start", startDate);
                cmd.Parameters.AddWithValue("date_time_finish", finishDate);
                cmd.Parameters.AddWithValue("name_smena", textBox1.Text);
                cmd.ExecuteNonQuery();

                MessageBox.Show(
                    "Смена успешно добавлена",
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
                    if (ex.ConstraintName == "graphik_smen_dates_check")
                    {
                        MessageBox.Show(
                            "Дата начала не может быть больше даты конца",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "graphik_smen_status_date_logic")
                    {
                        MessageBox.Show(
                            "Статус смены не соответствует датам",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "graphik_smen_name_smena_no_whitespace")
                    {
                        MessageBox.Show(
                            "не заполнено поле названия смены",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
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

        private void Form35_Load(object sender, EventArgs e)
        {

        }
    }
    
}
