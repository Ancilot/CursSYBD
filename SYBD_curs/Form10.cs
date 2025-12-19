using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SYBD_curs
{
    
    public partial class Form10 : Form
    {
        private NpgsqlConnection conn;
        private int serviceId;
        public Form10(int id, string name, decimal price)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            serviceId = id;

            textBox1.Text = name;
            textBox2.Text = price.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (!decimal.TryParse(textBox2.Text, out decimal price))
            {
                MessageBox.Show("Некорректная цена");
                return;
            }
            try
            {
                conn.Open();

                NpgsqlCommand cmd = new NpgsqlCommand(
                 "CALL curse.update_service(@id, @name, @price)",
                  conn
                  );

                cmd.Parameters.AddWithValue("@id", serviceId);
                cmd.Parameters.AddWithValue("@name", textBox1.Text);
                cmd.Parameters.AddWithValue("@price", price);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Услуга обновлена");
                this.Close();
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
            finally
            {
                conn.Close();
            }
        }
    }
}
