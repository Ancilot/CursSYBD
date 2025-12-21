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
    public partial class Form22 : Form
    {
        private NpgsqlConnection conn;
        private int addresID;
        public Form22(int id, string Home, string Street, string City, string Region, string Country)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            addresID = id;
            textBox1.Text = Home;
            textBox2.Text = Street;
            textBox3.Text = City;
            textBox4.Text = Region;
            textBox5.Text = Country;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            try
            {
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand(
                 "UPDATE curse.\"Addres\" SET " +
                 "\"Home\" = @home, " +
                 "\"Street\" = @street, " +
                 "\"City\" = @city, " +
                 "\"Region\" = @region, " +
                 "\"Country\" = @country " +
                 "WHERE \"ID\" = @id",
                  conn);
                
                    command.Parameters.AddWithValue("id", addresID);
                    command.Parameters.AddWithValue("home", textBox1.Text);
                    command.Parameters.AddWithValue("street", textBox2.Text);
                    command.Parameters.AddWithValue("city", textBox3.Text);
                    command.Parameters.AddWithValue("region", textBox4.Text);
                    command.Parameters.AddWithValue("country", textBox5.Text);
                    command.ExecuteNonQuery();

                    MessageBox.Show(
                        "Адрес успешно обновлен",
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
                    if (ex.ConstraintName == "addres_home_no_whitespace")
                    {
                        MessageBox.Show(
                            "Не все поля заполнены",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
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
                        
                        return;
                    }
                }
                if (ex.SqlState == "P0001")
                {
                    MessageBox.Show(
                        ex.MessageText,
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }
            }
            finally {
                conn.Close();
            }
            }
        }
    }

