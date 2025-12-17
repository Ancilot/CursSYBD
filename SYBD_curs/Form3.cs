using Npgsql;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form3 : Form
    {
        private PictureBox pbTooltip;
        private Timer tooltipTimer;
        private byte[] pendingFotoData;
        private Point pendingCursorPos;


        private NpgsqlConnection conn;
        public Form3()
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);

            pbTooltip = new PictureBox();
            pbTooltip.SizeMode = PictureBoxSizeMode.Zoom;
            pbTooltip.Size = new Size(150, 150); // размер превью
            pbTooltip.Visible = false; // пока скрыт
            pbTooltip.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(pbTooltip);

            // Инициализация таймера
            tooltipTimer = new Timer();
            tooltipTimer.Interval = 400; // задержка в миллисекундах
            tooltipTimer.Tick += TooltipTimer_Tick;


            // Привязываем события к DataGridView
            dataGridView1.CellMouseEnter += DataGridView1_CellMouseEnter;
            dataGridView1.CellMouseLeave += DataGridView1_CellMouseLeave;
        }

        private void DataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                pendingFotoData = row.Cells["Foto_employee"].Value as byte[];

                if (pendingFotoData != null)
                {
                    // Сохраняем позицию курсора на момент наведения
                    pendingCursorPos = dataGridView1.PointToClient(Cursor.Position);

                    // Перезапускаем таймер
                    tooltipTimer.Stop();
                    tooltipTimer.Start();
                }
            }
        }

        private void DataGridView1_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            // Скрываем PictureBox и останавливаем таймер
            tooltipTimer.Stop();
            pbTooltip.Visible = false;

            if (pbTooltip.Image != null)
            {
                pbTooltip.Image.Dispose();
                pbTooltip.Image = null;
            }

            pendingFotoData = null;
        }

        private void TooltipTimer_Tick(object sender, EventArgs e)
        {
            tooltipTimer.Stop(); // один раз показываем фото

            if (pendingFotoData != null)
            {
                using (var ms = new System.IO.MemoryStream(pendingFotoData))
                {
                    pbTooltip.Image = Image.FromStream(ms);
                }

                pbTooltip.Location = new Point(pendingCursorPos.X + 20, pendingCursorPos.Y + 20);
                pbTooltip.Visible = true;
                pbTooltip.BringToFront();
            }
        }



        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // Создадим новый набор данных
            DataSet datasetmain = new DataSet();
            
            // Открываем подключение
            conn.Open();
            // Очищаем набор данных
            datasetmain.Clear();
           
            NpgsqlCommand command = new NpgsqlCommand("SELECT " +
             "e.\"Surname\", " +
             "e.\"Name\", " +
             "e.\"Patronymic\", " +
             "e.\"Date_receipt\", " +
             "jt.\"Name\" AS job_title, " +
             "e.\"Wages\", " +
             "e.\"Education\", " +
             "e.\"Foto_employee\" " +
             "FROM curse.\"Employee\" e " +
             "JOIN curse.\"Job_title\" jt ON jt.\"ID\" = e.\"Job_title\"", conn);
            // Новый адаптер нужен для заполнения набора данных
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            // Заполняем набор данных данными, которые вернул запрос
            da.Fill(datasetmain, "\"Employee\"");

            // Связываем элемент DataGridView1 с набором данных
            dataGridView1.DataSource = datasetmain;       

            dataGridView1.DataMember = "\"Employee\"";
            // Закрываем подключение
            conn.Close();
        }
    }
}
