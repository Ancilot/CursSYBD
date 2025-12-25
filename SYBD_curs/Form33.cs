using Npgsql;
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form33 : Form
    {
        private NpgsqlConnection conn;
        private byte[] photoBytes;
        private int emploeeID;
        private int Jobs;
        public Form33(int id, string sername, string name, string patro, DateTime data_start, int job, decimal wages, string educ, byte[] photoBytes)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            this.photoBytes = photoBytes;
            conn = new NpgsqlConnection(connString);
            emploeeID = id;
            textBox1.Text = sername;
            textBox2.Text = name;
            textBox3.Text = patro;
            dateTimePicker1.Value = data_start.Date;
            Jobs = job;
            textBox5.Text = wages.ToString();
            textBox6.Text = educ.ToString();
            if (photoBytes != null && photoBytes.Length > 0)
            {
                using (var ms = new MemoryStream(photoBytes))
                {
                    pictureBox1.Image = Image.FromStream(ms);
                }
            }
            else
            {
                pictureBox1.Image = null; // или картинка по умолчанию
            }
        }

        private void Form33_Load(object sender, System.EventArgs e)
        {
            Job();
        }
        private void Job()
        {
            try
            {
                conn.Open();
                var cmd = new NpgsqlCommand(
                 "SELECT " +
                 "\"ID\"," +
                 "\"Name\" " +
                 "FROM curse.\"Job_title\"",
                 conn
                 );


                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "Name";
                comboBox1.ValueMember = "ID";
                comboBox1.SelectedValue = Jobs;
            }
            finally
            {
                conn.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue == null)
            {
                MessageBox.Show(
                    "Ошибка: не все данные заполнены",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return; // выходим из метода, чтобы не выполнять вставку
            }
            int JobId = Convert.ToInt32(comboBox1.SelectedValue);
            DateTime startDate = dateTimePicker1.Value.Date;

            try
            {

                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "UPDATE curse.\"Employee\" SET " +
                 "\"Surname\" = @surname, " +
                 "\"Name\" = @name, " +
                 "\"Patronymic\" = @patronymic, " +
                 "\"Date_receipt\" = @date_receipt, " +
                 "\"Job_title\" = @job_title, " +
                 "\"Wages\" = @wages, " +
                 "\"Education\" = @education, " +
                 "\"Foto_employee\" = @foto_employee " +
                 "WHERE \"ID\" = @id", 
                    conn
                );
                cmd.Parameters.AddWithValue("id", emploeeID);
                cmd.Parameters.AddWithValue("surname", textBox1.Text);
                cmd.Parameters.AddWithValue("name", textBox2.Text);
                cmd.Parameters.AddWithValue("patronymic", textBox3.Text);
                cmd.Parameters.AddWithValue("date_receipt", startDate);
                cmd.Parameters.AddWithValue("job_title", JobId);

                string input = textBox5.Text.Trim().Replace(',', '.');

                if (!decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal wages))
                {
                    MessageBox.Show(
                        "Введите корректную зарплату",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }


                cmd.Parameters.AddWithValue("wages", wages);
                cmd.Parameters.AddWithValue("education", textBox6.Text);

                if (this.photoBytes == null)
                {
                    cmd.Parameters.AddWithValue("foto_employee", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue(
                        "foto_employee",
                        NpgsqlTypes.NpgsqlDbType.Bytea,
                        this.photoBytes
                    );
                }
                cmd.ExecuteNonQuery();

                MessageBox.Show(
                    "Сотрудник успешно обновлен",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                ); ;
            }
            catch (PostgresException ex)
            {
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

                // Превышение допустимого диапазона числа
                if (ex.SqlState == "22003")
                {
                    MessageBox.Show(
                        "Значение числа вне допустимого диапазона",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }
                if (ex.SqlState == "23514") // CHECK
                {
                    if (ex.ConstraintName == "employee_surname_no_whitespace_no_digits")
                    {
                        MessageBox.Show(
                            "Фамилия не может быть пустой и не должна содержать цифры",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "employee_name_no_whitespace_no_digits")
                    {
                        MessageBox.Show(
                            "Имя не может быть пустым и не должно содержать цифры",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "employee_patronymic_no_whitespace_no_digits")
                    {
                        MessageBox.Show(
                            "Отчество не может быть пустым и не должно содержать цифры",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "employee_education_no_whitespace_no_digits")
                    {
                        MessageBox.Show(
                            "поле 'Образование' не может быть пустым и не должно содержать цифры",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "employee_wages_positive")
                    {
                        MessageBox.Show(
                            "Зарплата не может уходить в минус",
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

                // Превышение допустимого диапазона числа
                if (ex.SqlState == "22003")
                {
                    MessageBox.Show(
                        "Значение числа вне допустимого диапазона",
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
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Изображения (*.jpg;*.png)|*.jpg;*.png";
                    ofd.Title = "Выберите фотографию сотрудника";

                    if (ofd.ShowDialog() != DialogResult.OK)
                        return;

                    byte[] imageBytes = File.ReadAllBytes(ofd.FileName);

                    photoBytes = File.ReadAllBytes(ofd.FileName);
                    // Освобождаем старое изображение
                    pictureBox1.Image?.Dispose();
                    pictureBox1.Image = null;

                    using (var ms = new MemoryStream(imageBytes))
                    {
                        Image img = Image.FromStream(ms);
                        pictureBox1.Image = new Bitmap(img);
                    }

                    // imageBytes можно сохранить в БД
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка при загрузке фото фото");
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form34 editForm = new Form34(this, emploeeID);
            editForm.ShowDialog();
        }
    }
    
}

