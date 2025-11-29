using System;
using System.Windows.Forms;

namespace SYBD_curs
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            this.FormClosed += Form3_FormClosed;
        }
        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            // При закрытии Form3 открываем форму логина заново
            Form1 loginForm = new Form1();
            loginForm.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
