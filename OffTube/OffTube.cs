using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using YoutubeExtractor;
using System.Threading;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace OffTube
{
    public partial class OffTube : Form
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)+@"\OffTube\";
        WebBrowser webBrowser1 = new WebBrowser();


        private void LoadVideo_DoWork(object sender, DoWorkEventArgs e)
        {
            pictureBox1.ImageLocation = string.Format(@"http://i3.ytimg.com/vi/{0}/hqdefault.jpg", GetVideoIDFromUrl("https://youtube.com" + videohref[1]));
            pictureBox2.ImageLocation = string.Format(@"http://i3.ytimg.com/vi/{0}/hqdefault.jpg", GetVideoIDFromUrl("https://youtube.com" + videohref[2]));           
            pictureBox3.ImageLocation = string.Format(@"http://i3.ytimg.com/vi/{0}/hqdefault.jpg", GetVideoIDFromUrl("https://youtube.com" + videohref[3]));           
            pictureBox4.ImageLocation = string.Format(@"http://i3.ytimg.com/vi/{0}/hqdefault.jpg", GetVideoIDFromUrl("https://youtube.com" + videohref[4]));
            pictureBox5.ImageLocation = string.Format(@"http://i3.ytimg.com/vi/{0}/hqdefault.jpg", GetVideoIDFromUrl("https://youtube.com" + videohref[5]));
            Invoke((MethodInvoker)delegate
            {
                label1.Text = videotitle[1];
                label2.Text = videotitle[2];
                label3.Text = videotitle[3];
                label4.Text = videotitle[4];
                label5.Text = videotitle[5];
            });
            LoadVideo.CancelAsync();
        }

        public OffTube()
        {
            InitializeComponent();
            settings();
            webBrowser1.Navigate("https://www.youtube.com/feed/subscriptions");
            webBrowser1.DocumentCompleted += Youtube_Subscribtion;
            LoadVideo.RunWorkerCompleted += LoadVideo_RunWorkerCompleted;
            
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        void settings()
        {
            if (File.Exists("settings.ini"))
            {
                string[] set = File.ReadAllLines("settings.ini");
                switch (set[0])
                {
                    case "360":
                        resolution = 360;
                        break;
                    case "480":
                        resolution = 480;
                        break;
                    case "720":
                        resolution = 720;
                        break;
                }
                switch (set[1])
                {
                    case "0":
                        autodownload = "0";
                        break;
                    case "1":
                        autodownload = "1";
                        break;
                }
            }
            else
            {
                File.WriteAllText("settings.ini", "360" + Environment.NewLine + "0");
            }
        }

        private void LoadVideo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string[] str = File.ReadAllLines("settings.ini");
            if (str[1] == "1")
            get_video_urls.RunWorkerAsync();
        }



        public string str = "";     //переменная для фреймов
        public List<string> videohref = new List<string>();      //Ссылки на видеоролики
        public List<string> videotitle = new List<string>();     //Названия видеороликов
        //
        public List<string> videodownloadurl = new List<string>();      //Прямые ссылки на видеоролики
        public List<string> videodownloadtitle = new List<string>();     //Названия видеороликов
        //
        public int resolution = 144;        //Разрешение видеороликов
        public bool flag = true;
        public string autodownload = "0";
        private Int32 tmpX;
        private Int32 tmpY;
        private bool flMove = false;
        public int id = 0;
        //

        private void Youtube_Subscribtion(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            str += "a";
            if (str.Length > 2)
            {
                webBrowser1.Stop();
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(webBrowser1.DocumentText);
                HtmlNodeCollection links = doc.DocumentNode.SelectNodes(".//div/h3/a");
                foreach (HtmlNode link in links)
                {
                    videohref.Add(link.GetAttributeValue("href", ""));
                    videotitle.Add(link.InnerText);
                }
                webBrowser1.Dispose();
                LoadVideo.RunWorkerAsync();
            }
        }
        
        public static string GetVideoIDFromUrl(string url)  //Получение ID видеоролика, используется для вывода картинки видеоролика в pictureBox'ы
        {
            url = url.Substring(url.IndexOf("?") + 1);
            string[] props = url.Split('&');

            string videoid = "";
            foreach (string prop in props)
            {
                if (prop.StartsWith("v="))
                    videoid = prop.Substring(prop.IndexOf("v=") + 2);
            }

            return videoid;
        }

        public string GetVideoUrl(string title)       //Получение названия видеоролика
        {
            string videotitle = null;
            try
            {
                Thread.Sleep(3000);
                IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls("https://youtube.com" + title);
                VideoInfo video = videoInfos
                        .First(info => info.VideoType == VideoType.Mp4 && info.Resolution == resolution);
                videotitle = video.Title.Replace('|', '_').Replace('[', '_').Replace(']', '_').Replace(':', '_').Replace('+', '_') + video.VideoExtension + "|" + video.DownloadUrl;
            }
            catch (YoutubeParseException) { }
            return videotitle;
        }

        private void get_video_urls_DoWork(object sender, DoWorkEventArgs e)
        {
            for(int i = 1; i<10; i++)
            {
                string half = GetVideoUrl(videohref[i]);
                if (half != null)
                {
                    string[] full = half.Split('|');
                    videodownloadurl.Add(full[1]);
                    videodownloadtitle.Add(full[0]);
                }
                if (videodownloadtitle.Count == 5)
                {
                    if (autodownload == "1")
                    {
                        Thread one = new Thread(doDownload);
                        one.IsBackground = true;
                        one.Start();
                    }
                }
            }
        }


        private void doDownload()
        {
            if (!File.Exists(path + videodownloadtitle[0]))
            {
                id = 1;
                _downloader(videodownloadurl[0] + '|' + videodownloadtitle[0]);
                flag = false;
            }
            else
            {
                Invoke((MethodInvoker)delegate
                {
                    label1.ForeColor = Color.Aquamarine;
                    label1.Cursor = Cursors.Hand;
                    label1.Click += ChangeLabelsDoExists;
                    
                });flag = true;
            }
            while (!flag)
            {
                Thread.Sleep(2000);
            }
            if (!File.Exists(path + videodownloadtitle[1]))
            {
                id = 2;
                _downloader(videodownloadurl[1] + '|' + videodownloadtitle[1]);
                flag = false;
            }
            else
            {
                Invoke((MethodInvoker)delegate
                {
                    label2.ForeColor = Color.Aquamarine;
                    label2.Cursor = Cursors.Hand;
                    label2.Click += ChangeLabelsDoExists;
                }); flag = true;
            }
            while (!flag)
            {
                Thread.Sleep(2000);
            }
            if (!File.Exists(path + videodownloadtitle[2]))
            {
                id = 3;
                _downloader(videodownloadurl[2] + '|' + videodownloadtitle[2]);
                flag = false;
            }
            else
            {
                Invoke((MethodInvoker)delegate
                {
                    label3.ForeColor = Color.Aquamarine;
                    label3.Cursor = Cursors.Hand;
                    label3.Click += ChangeLabelsDoExists;
                }); flag = true;
            }
            while (!flag)
            {
                Thread.Sleep(2000);
            }
            if (!File.Exists(path + videodownloadtitle[3]))
            {
                id = 4;
                _downloader(videodownloadurl[3] + '|' + videodownloadtitle[3]);
                flag = false;
            }
            else
            {
                Invoke((MethodInvoker)delegate
                {
                    label4.ForeColor = Color.Aquamarine;
                    label4.Cursor = Cursors.Hand;
                    label4.Click += ChangeLabelsDoExists;
                }); flag = true;
            }
            while (!flag)
            {
                Thread.Sleep(2000);
            }
            if (!File.Exists(path + videodownloadtitle[4]))
            {
                id = 5;
                _downloader(videodownloadurl[4] + '|' + videodownloadtitle[4]);
                flag = false;
            }
            else
            {
                Invoke((MethodInvoker)delegate
                {
                    label5.ForeColor = Color.Aquamarine;
                    label5.Cursor = Cursors.Hand;
                    label5.Click += ChangeLabelsDoExists;
                }); flag = true;
            }
            while (!flag)
            {
                Thread.Sleep(2000);
            }
        }

        private void ChangeLabelsDoExists(object sender, EventArgs e)
        {
            Label b = sender as Label;
            switch (Convert.ToInt32(b.Tag))
            {
                case 1:
                    Process.Start(path+videodownloadtitle[0]);
                    break;
                case 2:
                    Process.Start(path + videodownloadtitle[1]);
                    break;
                case 3:
                    Process.Start(path + videodownloadtitle[2]);
                    break;
                case 4:
                    Process.Start(path + videodownloadtitle[3]);
                    break;
                case 5:
                    Process.Start(path + videodownloadtitle[4]);
                    break;
            }
        }
    

        public void _downloader (string url)
        {
            WebClient wbc = new WebClient();
            string[] str = url.Split('|');
            wbc.DownloadFileAsync(new Uri(str[0]), path+str[1]);
            wbc.DownloadFileCompleted += downloader_completed;
            wbc.DownloadProgressChanged += downloader_progress;
        }

        private void downloader_progress(object sender, DownloadProgressChangedEventArgs e)
        {
            if (id == 1)
            {
                Invoke((MethodInvoker)delegate
                {
                progressBar1.Visible = true;
                progressBar1.Value = e.ProgressPercentage;
                });
            }
            if (id == 2)
            {
                Invoke((MethodInvoker)delegate
                {
                    progressBar2.Visible = true;
                    progressBar2.Value = e.ProgressPercentage;
                });
                }
            if (id == 3)
            {
                Invoke((MethodInvoker)delegate
                {
                    progressBar3.Visible = true;
                    progressBar3.Value = e.ProgressPercentage;
                });
            }
            if (id == 4)
            {
                Invoke((MethodInvoker)delegate
                {
                    progressBar4.Visible = true;
                    progressBar4.Value = e.ProgressPercentage;
                });
            }
            if (id == 5)
            {
                Invoke((MethodInvoker)delegate
                {
                    progressBar5.Visible = true;
                    progressBar5.Value = e.ProgressPercentage;
                });
            }
        }

        private void downloader_completed(object sender, AsyncCompletedEventArgs e)
        {
            flag = true;
        }


        private void lbl_settings_Click(object sender, EventArgs e)
        {
            settings set = new settings();
            set.Show();
        }

        private void lbl_exit_Click(object sender, EventArgs e)
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
