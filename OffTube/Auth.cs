using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OffTube
{
    public partial class Auth : Form
    {
        public Auth()
        {
            InitializeComponent();
        }
        Uri url = new Uri("https://www.youtube.com/feed/subscriptions");
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if(webBrowser1.Url == url)
            {
               Application.Restart();
            }
            else
            {
                
            }
            textBox1.Text = webBrowser1.Url.ToString();
        }

        private void Auth_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
