using Npgsql;
using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace SYBD_curs
{
    public partial class Form36 : Form
    {
        private Form parentForm;
        private int smenID;
        private NpgsqlConnection conn;
        private bool skipFormClosingDelete = false;
        public Form36(Form parent, int id)
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
            parentForm = parent;
            smenID = id;
            this.FormClosing += Form36_FormClosing;
        }
        private void Form36_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (skipFormClosingDelete)
                return; // пропускаем 

            DialogResult res = MessageBox.Show(
                "Вы уверены, что хотите отменить добавление сотрудников в смену?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (res != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
            skipFormClosingDelete = true;

            // Закрываем родительскую форму
            parentForm?.Close();

        }
        private void boxemploee()
        {

        }

        private void Form36_Load(object sender, System.EventArgs e)
        {

        }
    }
}
