using System;
using System.Windows.Forms;

namespace OffTube
{
    public partial class Form2 : Form
    {
        WebBrowser wbc = new WebBrowser();
        int check = 0;
        private Int32 tmpX;
        private Int32 tmpY;
        private bool flMove = false;
        public Form2()
        {
            InitializeComponent();
            wbc.Navigate("https://www.youtube.com/feed/subscriptions");
            wbc.DocumentCompleted += Wbc_DocumentCompleted;
        }

        private void Wbc_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            ++check;
            if (check == 1)
            {
                if (wbc.Url.ToString() == "https://www.youtube.com/feed/subscriptions")
                {
                    OffTube off = new OffTube();
                    off.Show();
                    this.Close();
                }
                else
                {
                    Auth auth = new Auth();
                    auth.Show();
                }
                
            }
            
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //перетаскивание формы по нажатии мышки кнопки
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (flMove)
            {
                this.Left = this.Left + (Cursor.Position.X - tmpX);
                this.Top = this.Top + (Cursor.Position.Y - tmpY);

                tmpX = Cursor.Position.X;
                tmpY = Cursor.Position.Y;
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            tmpX = Cursor.Position.X;
            tmpY = Cursor.Position.Y;
            flMove = true;
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            flMove = false;
        }
    }
}
