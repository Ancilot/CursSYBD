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
    public partial class Form40 : Form
    {
        private NpgsqlConnection conn;
        private int smenID;
        public Form40(int id)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            smenID = id;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime startDate = dateTimePicker1.Value;
            try
            {

                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "INSERT INTO curse.\"Includents\" (\"Contract_Smena\", \"Processing_status\", \"Incedent\"," +
                    " \"Data_Time\", \"Measures_taken\") " +
                    "VALUES (@contract_Smena, @processing_status, @incedent, @data_time, @measures_taken)",
                    conn
                );
                cmd.Parameters.AddWithValue("contract_Smena", smenID);
                cmd.Parameters.AddWithValue("processing_status", "Выполняется");
                cmd.Parameters.AddWithValue("incedent", textBox1.Text);
                cmd.Parameters.AddWithValue("data_time", startDate);
                cmd.Parameters.AddWithValue("measures_taken", textBox2.Text);
                cmd.ExecuteNonQuery();

                MessageBox.Show(
                    "Происшествие успешно добавлено",
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
                    if (ex.ConstraintName == "includents_all_or_none")
                    {
                        MessageBox.Show(
                            "Происшествие должно быть заполнено полностью: описание, дата и принятые меры.",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "includents_incedent_no_whitespace")
                    {
                        MessageBox.Show(
                            "Поле \"происшествие\" должно быть заполненно!",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "includents_measures_taken_no_whitespace")
                    {
                        MessageBox.Show(
                            "Поле \"принятые меры\" должно быть заполненно!",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "includents_processing_status_allowed_values")
                    {
                        MessageBox.Show(
                            "Статус может быть только: Выполняется, Закончен, Не начат",
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

        private void Form40_Load(object sender, EventArgs e)
        {

        }
    }
}
