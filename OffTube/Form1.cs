using System;
using System.Windows.Forms;

namespace OffTube
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Opacity = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form2 fi = new Form2();
            fi.Show();
        }
    }
}
