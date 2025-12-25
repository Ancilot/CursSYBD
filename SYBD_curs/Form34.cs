using Npgsql;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace SYBD_curs
{
    public partial class Form34 : Form
    {
        private Form parentForm;
        private int emploeID;
        private NpgsqlConnection conn;
        public Form34(Form parent, int id)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            parentForm = parent;
            emploeID = id;
            conn.Notice += (senderConn, eNotice) =>
            {
                MessageBox.Show(eNotice.Notice.MessageText, "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

        }
        private void boxSmen()
        {
            try
            {
                conn.Open();
                var cmd = new NpgsqlCommand(
                 "SELECT * FROM curse.get_available_shifts_for_employee(@p_employee_id)",
                 conn
                 );

                cmd.Parameters.AddWithValue("p_employee_id", emploeID);


                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                // Добавляем колонку с объединённым текстом
                dt.Columns.Add("DisplayText", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    DateTime start = (DateTime)row["Date_time_start"];
                    DateTime end = (DateTime)row["Date_time_finish"];
                    row["DisplayText"] = $"{row["Name_smena"]} ({start:dd.MM.yyyy HH:mm} - {end:dd.MM.yyyy HH:mm})";
                }

                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "DisplayText";
                comboBox1.ValueMember = "ID";
            }
            finally
            {
                conn.Close();
            }
        }
        private void license()
        {
            // Создадим новый набор данных
            DataSet datasetmain = new DataSet();

            // Открываем подключение
            conn.Open();

            datasetmain.Clear();

            NpgsqlCommand command = new NpgsqlCommand(
            "SELECT " +
            "el.\"ID\", " +
            "el.\"ID_License\", " +
            "l.\"Name\" AS \"Название лицензии\", " +
            "l.\"Number\" AS \"Номер Лицензии\", " +
            "l.\"Start_date\" AS \"Начало\", " +
            "l.\"Finish_date\" AS \"Окончание\", " +
            "l.\"Status\" AS \"Статус\" " +
            "FROM curse.\"Employee_License\" el " +
            "JOIN curse.\"License\" l ON l.\"ID\" = el.\"ID_License\" " +
            "WHERE el.\"ID_Employee\" = @id_employee " +
            "ORDER BY el.\"ID_Employee\"",
            conn
            );



            // Параметр контракта
            command.Parameters.AddWithValue("@id_employee", emploeID);

            // Адаптер
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);

            // Заполняем DataSet
            da.Fill(datasetmain, "\"Employee_License\"");

            // Привязываем к DataGridView
            dataGridView1.DataSource = datasetmain;
            dataGridView1.DataMember = "\"Employee_License\"";

            // Скрываем технический ID
            dataGridView1.Columns["ID"].Visible = false;
            dataGridView1.Columns["ID_License"].Visible = false;

            // Закрываем подключение
            conn.Close();
        }
        private void smen()
        {
            // Создадим новый набор данных
            DataSet datasetmain = new DataSet();

            // Открываем подключение
            conn.Open();

            datasetmain.Clear();

            NpgsqlCommand command = new NpgsqlCommand(
             "SELECT " +
             "eg.\"ID\", " +
             "gs.\"Name_smena\" AS \"Название смены\", " +
             "gs.\"Date_time_start\" AS \"Начало\", " +
             "gs.\"Date_time_finish\" AS \"Окончание\" " +
             "FROM curse.\"Employee_graphik\" eg " +
             "JOIN curse.graphik_smen gs ON gs.\"ID\" = eg.\"ID_graphik\" " +
             "WHERE eg.\"ID_Employee\" = @id_employee " +
             "ORDER BY eg.\"ID_Employee\"",
             conn
             );


            // Параметр контракта
            command.Parameters.AddWithValue("@id_employee", emploeID);

            // Адаптер
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);

            // Заполняем DataSet
            da.Fill(datasetmain, "\"Employee_graphik\"");

            // Привязываем к DataGridView
            dataGridView2.DataSource = datasetmain;
            dataGridView2.DataMember = "\"Employee_graphik\"";

            // Скрываем технический ID
            dataGridView2.Columns["ID"].Visible = false;

            // Закрываем подключение
            conn.Close();

        }
        private void boxStatus()
        {
            // Добавляем статусы в ComboBox
            comboBox2.Items.Add("Истекла");
            comboBox2.Items.Add("Действует");

            // Устанавливаем первый элемент по умолчанию (если нужно)
            if (comboBox2.Items.Count > 0)
            {
                comboBox2.SelectedIndex = 0;
            }
        }
        private void Form34_Load(object sender, System.EventArgs e)
        {
            license();
            smen();
            boxSmen();
            boxStatus();
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            try
            {
                int newId;
                DateTime startDate = dateTimePicker1.Value.Date;
                DateTime finishDate = dateTimePicker2.Value.Date;
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    using (var cmd = new NpgsqlCommand(
                         "INSERT INTO curse.\"License\" (\"Name\", \"Number\",\"Start_date\", \"Finish_date\", \"Status\") " +
                    "VALUES (@name, @number, @start_date, @finish_date, @status) RETURNING \"ID\"",
                    conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("Name", textBox1.Text);
                        cmd.Parameters.AddWithValue("Number", textBox2.Text);
                        cmd.Parameters.AddWithValue("Start_date", startDate);
                        cmd.Parameters.AddWithValue("Finish_date", finishDate);
                        cmd.Parameters.AddWithValue("Status", comboBox2.SelectedItem.ToString());
                        newId = (int)cmd.ExecuteScalar();
                    }

                    using (var cmd = new NpgsqlCommand(
                         "INSERT INTO curse.\"Employee_License\" (\"ID_Employee\", \"ID_License\") " +
                    "VALUES (@id_employee, @id_license)",
                        conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("id_employee", emploeID);
                        cmd.Parameters.AddWithValue("id_license", newId);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "23505")
                {
                    MessageBox.Show(
                        "Лицензия с таким номером уже существует",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
                if (ex.SqlState == "23514") // CHECK
                {
                    if (ex.ConstraintName == "license_dates_check")
                    {
                        MessageBox.Show(
                            "Дата начала не может быть больше даты конца!",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "license_active_date_check")
                    {
                        MessageBox.Show(
                            "Нельзы установить статус \"действует\", так как нынешняя дата больше даты конца!",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "license_name_no_whitespace")
                    {
                        MessageBox.Show(
                            "Поле названия лицензии не может быть пустым",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "license_number_no_whitespace")
                    {
                        MessageBox.Show(
                            "Поле номера лицензии не может быть пустым",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (ex.ConstraintName == "license_status_allowed_values")
                    {
                        MessageBox.Show(
                            "Поле статуса принимает только - 'Истекла', 'Действует','Аннулирована'",
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
            finally
            {
                conn.Close();
                license();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
            dataGridView1.CurrentRow.Cells["ID"].Value == null ||
            dataGridView1.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите лицензию для удаления",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            DataGridViewRow row = dataGridView1.CurrentRow;


            // Получаем ID выбранной услуги
            int licId = Convert.ToInt32(
                dataGridView1.SelectedRows[0].Cells["ID"].Value
            );

            try
            {
                conn.Open();

                NpgsqlCommand cmd = new NpgsqlCommand(
                "DELETE FROM curse.\"Employee_License\" WHERE \"ID\" = @id",
                   conn
                 );
                cmd.Parameters.AddWithValue("@id", licId);
                cmd.ExecuteNonQuery();
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "P0001")
                    MessageBox.Show(
                            ex.MessageText,
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
            }
            finally
            {
                conn.Close();
                license();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue == null)
            {
                MessageBox.Show(
                    "Ошибка: нет доступных смен",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return; // выходим из метода, чтобы не выполнять вставку
            }

            int nameId = Convert.ToInt32(comboBox1.SelectedValue);

            try
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "INSERT INTO curse.\"Employee_graphik\" (\"ID_Employee\", \"ID_graphik\") " +
                    "VALUES (@id_employee, @id_graphic)",
                    conn
                );

                cmd.Parameters.AddWithValue("id_employee", emploeID);
                cmd.Parameters.AddWithValue("id_graphic", nameId);

                cmd.ExecuteNonQuery();
            }
            catch (PostgresException ex)
            {

                if (ex.SqlState == "P0001")
                    MessageBox.Show(
                            ex.MessageText,
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
            }
            finally
            {
                conn.Close();
                boxSmen();
                smen();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow == null ||
            dataGridView2.CurrentRow.Cells["ID"].Value == null ||
            dataGridView2.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите смену для удаления",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            DataGridViewRow row = dataGridView2.CurrentRow;


            // Получаем ID выбранной услуги
            int smenId = Convert.ToInt32(
                dataGridView2.SelectedRows[0].Cells["ID"].Value
            );

            try
            {
                conn.Open();

                NpgsqlCommand cmd = new NpgsqlCommand(
                "DELETE FROM curse.\"Employee_graphik\" WHERE \"ID\" = @id",
                   conn
                 );
                cmd.Parameters.AddWithValue("@id", smenId);
                cmd.ExecuteNonQuery();
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "P0001")
                    MessageBox.Show(
                            ex.MessageText,
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
            }
            finally
            {
                conn.Close();
                smen();
                boxSmen();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
            parentForm.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
           dataGridView1.CurrentRow.Cells["ID"].Value == null ||
           dataGridView1.CurrentRow.Cells["ID"].Value == DBNull.Value)
            {
                MessageBox.Show(
                    "Выберите лицензию аннулирования",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            DataGridViewRow row = dataGridView1.CurrentRow;


            // Получаем ID выбранной услуги
            int licenseId = Convert.ToInt32(
            dataGridView1.SelectedRows[0].Cells["ID_License"].Value
            );


            try
            {
                conn.Open();

                NpgsqlCommand cmd = new NpgsqlCommand(
                 "UPDATE curse.\"License\" SET " +
                 "\"Status\" = @status " +
                 "WHERE \"ID\" = @id",
                   conn
                 );
                cmd.Parameters.AddWithValue("@id", licenseId);
                cmd.Parameters.AddWithValue("@status", "Аннулирована");
                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
                smen();
                license();
            }
        }
    }
    
}
