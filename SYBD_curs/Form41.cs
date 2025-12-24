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
    public partial class Form41 : Form
    {
        private NpgsqlConnection conn;
        private int incID;
        public Form41(int id, string inc, string mera, DateTime data)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            incID = id;
            textBox1.Text = inc;
            textBox2.Text = mera;
            dateTimePicker1.Value = data;
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
                    "UPDATE curse.\"Includents\" SET " +
                    "\"Incedent\" = @incedent, " +
                    "\"Data_Time\" = @data_time, " +
                    "\"Measures_taken\" = @measures_taken, " +
                    "WHERE \"ID\" = @id",
                    conn
                );
                cmd.Parameters.AddWithValue("ID", incID);
                cmd.Parameters.AddWithValue("incedent", textBox1.Text);
                cmd.Parameters.AddWithValue("data_time", startDate);
                cmd.Parameters.AddWithValue("measures_taken", textBox2.Text);
                cmd.ExecuteNonQuery();

                MessageBox.Show(
                    "Происшествие успешно обновлено",
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
    }
}
