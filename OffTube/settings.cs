using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OffTube
{
    public partial class settings : Form
    {
        WebBrowser wbc = new WebBrowser();
        private Int32 tmpX;
        private Int32 tmpY;
        private bool flMove = false;
        string[] setting = File.ReadAllLines("settings.ini");
        public settings()
        {
            InitializeComponent();
            wbc.Navigate("https://www.youtube.com/feed/subscriptions");
            _settings();
        }
        void _settings()
        {
            if (File.Exists("settings.ini"))
            {
                string[] set = File.ReadAllLines("settings.ini");
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            string[] setting2 = File.ReadAllLines("settings.ini");
            PictureBox b = sender as PictureBox;
            switch (Convert.ToInt32(b.Tag))
            {

                case 360:
                    pictureBox1.ImageLocation = "settings/360selected.png";
                    pictureBox2.ImageLocation = "settings/no480selected.png";
                    pictureBox3.ImageLocation = "settings/no720selected.png";
                    File.WriteAllText("settings.ini", "360" + Environment.NewLine + setting2[1]);
                    break;
                case 480:
                    pictureBox1.ImageLocation = "settings/360noselected.png";
                    pictureBox2.ImageLocation = "settings/480selected.png";
                    pictureBox3.ImageLocation = "settings/no720selected.png";
                    File.WriteAllText("settings.ini", "480" + Environment.NewLine + setting2[1]);
                    break;
                case 720:
                    pictureBox1.ImageLocation = "settings/360noselected.png";
                    pictureBox2.ImageLocation = "settings/no480selected.png";
                    pictureBox3.ImageLocation = "settings/720selected.png";
                    File.WriteAllText("settings.ini", "720" + Environment.NewLine + setting2[1]);
                    break;
                case 1:
                    if (setting2[1] == "0")
                    {
                        pictureBox4.ImageLocation = "settings/theme/autodownloadon.png";
                        File.WriteAllText("settings.ini", setting2[0] + Environment.NewLine + "1");
                    }
                    else if (setting2[1] == "1")
                    {
                        pictureBox4.ImageLocation = "settings/theme/autodownloadoff.png";
                        File.WriteAllText("settings.ini", setting2[0] + Environment.NewLine + "0");
                    }
                    break;
            }
        }
        private void settings_Load(object sender, EventArgs e)
        {
            switch (setting[0])
            {
                case "360":
                    pictureBox1.ImageLocation = "settings/360selected.png";
                    pictureBox2.ImageLocation = "settings/no480selected.png";
                    pictureBox3.ImageLocation = "settings/no720selected.png";
                    break;
                case "480":
                    pictureBox1.ImageLocation = "settings/360noselected.png";
                    pictureBox2.ImageLocation = "settings/480selected.png";
                    pictureBox3.ImageLocation = "settings/no720selected.png";
                    break;
                case "720":
                    pictureBox1.ImageLocation = "settings/360noselected.png";
                    pictureBox2.ImageLocation = "settings/no480selected.png";
                    pictureBox3.ImageLocation = "settings/720selected.png";
                    break;
            }
            switch (setting[1])
            {
                case "0":
                    pictureBox4.ImageLocation = "settings/theme/autodownloadoff.png";
                    break;
                case "1":
                    pictureBox4.ImageLocation = "settings/theme/autodownloadon.png";
                    break;
            }
        }
        private void lbl_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("CMD.exe", "/C RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 2");
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
