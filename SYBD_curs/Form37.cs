using Npgsql;
using System;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form37 : Form
    {
        private NpgsqlConnection conn;
        private int graphID;
        public Form37(int id, string name, DateTime start, DateTime finish)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            dateTimePicker1.Value = start;
            dateTimePicker2.Value = finish;
            textBox1.Text = name;
            graphID = id;
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            DateTime startDate = dateTimePicker1.Value;
            DateTime finishDate = dateTimePicker2.Value;
            try
            {

                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(
                     "UPDATE curse.\"graphik_smen\" SET " +
                     "\"Date_time_start\" = @date_time_start, " +
                     "\"Date_time_finish\" = @date_time_finish, " +
                     "\"Name_smena\" = @name_smena " +
                     "WHERE \"ID\" = @id",
                    conn
                );
                cmd.Parameters.AddWithValue("id", graphID);
                cmd.Parameters.AddWithValue("date_time_start", startDate);
                cmd.Parameters.AddWithValue("date_time_finish", finishDate);
                cmd.Parameters.AddWithValue("name_smena", textBox1.Text);
                cmd.ExecuteNonQuery();

                MessageBox.Show(
                    "Смена успешно обновлена",
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form37_Load(object sender, EventArgs e)
        {

        }
    }
    
}
