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
        public Form15()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                       "После создания клиента, выберите его из списка",
                       "Информация",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Information
                   );
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form17 editForm = new Form17();
            editForm.ShowDialog();
        }
    }
}
