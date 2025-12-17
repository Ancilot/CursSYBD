using Npgsql;
using System;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form1 : Form
    {
        private NpgsqlConnection conn;

        public Form1()
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
   
        private void label1_Click(object sender, EventArgs e)
        {

        }

        // Обработчик события для кнопки входа
        private void btnLogin_Click_1(object sender, EventArgs e)
        {

            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();

              // Открываем соединение
                conn.Open();

                // Запрос для проверки логина и пароля
                // Используем crypt для безопасной проверки хэша
                string query = @"
                    SELECT ""ID_emloyee""
                    FROM curse.""Login_parol""
                    WHERE ""Login"" = @login
                    AND ""Password"" = crypt(@password, ""Password"") ";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    // Добавляем параметры для предотвращения SQL-инъекций
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@password", password);

                    // Выполняем запрос и проверяем результат
                    object result = cmd.ExecuteScalar();  // Возвращает ID_employee, если найдено

                    if (result != null)
                    {

                        string queryJobTitle = @"
                           SELECT jt.""Name""
                           FROM curse.""Employee"" e
                           JOIN curse.""Job_title"" jt ON e.""Job_title"" = jt.""ID""
                           WHERE e.""ID"" = @employeeId  ";

                        string jobTitleName = null;
                        using (NpgsqlCommand cmdJobTitle = new NpgsqlCommand(queryJobTitle, conn))
                        {
                            cmdJobTitle.Parameters.AddWithValue("@employeeId", result);
                            object resultJobTitle = cmdJobTitle.ExecuteScalar();
                            if (resultJobTitle != null)
                            {
                                jobTitleName = resultJobTitle.ToString();
                            }
                        }

                        // Определяем форму на основе должности и переходим
                        if (!string.IsNullOrEmpty(jobTitleName))
                        {
                            if (jobTitleName == "Менеджер")
                            {
                            // Переход на Form2 для менеджера
                            Form2 editForm = new Form2();
                            this.Hide();
                            editForm.ShowDialog();
                              // Скрываем текущую форму 
                            }
                            if (jobTitleName == "Начальник")
                            {
                            // Переход на Form3 для начальника
                            Form3 editForm = new Form3();
                            this.Hide();
                            editForm.ShowDialog();
                              // Скрываем текущую форму
                            }
                        }
                        this.Show();
                        txtLogin.Clear();
                        txtPassword.Clear();
                }
                    else
                    {
                        // Неверные данные
                        lblStatus.Text = "Неверный логин или пароль.";
                    }
                }     
                // закрываем соединение
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            

        }
    }
}
