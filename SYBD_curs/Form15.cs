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
    public partial class Form15 : Form
    {
        private NpgsqlConnection conn;
        public Form15()
        {
            InitializeComponent();
            string connString = "Host=localhost; Database=Ancilot; User Id=postgres; Password=1235;";
            conn = new NpgsqlConnection(connString);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (Form27 editForm = new Form27())
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    string newINNId = (string)editForm.Tag;

                    boxClient(); // обновляем список

                    comboBox1.SelectedValue = newINNId;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form17 editForm = new Form17();
            editForm.ShowDialog();
        }
        private void boxClient()
        {
            conn.Open();

            try
            {
                var cmd = new NpgsqlCommand(
                    "SELECT " +
                    "\"INN\", " +
                    "concat(\"Surname\", ', ', \"Name\", ', ', \"Patronymic\") AS full_name " +
                    "FROM curse.\"Client\" " +
                    "ORDER BY \"INN\"",
                    conn
                );

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "full_name";
                comboBox1.ValueMember = "INN";
            }
            finally
            {
                conn.Close();
            }
        }
        private void Form15_Load(object sender, EventArgs e)
        {
            boxClient();
        }
    }
}
