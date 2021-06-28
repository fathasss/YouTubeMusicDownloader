using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoLibrary;
using MediaToolkit;
using System.IO;
using System.Net;

namespace YouTubeMusic
{
    public partial class HomeController : Form
    {
        Boolean format = true;
        public HomeController()
        {
            InitializeComponent();
        }
        int startpoint = 0;
        private void Form1_Load(object sender, EventArgs e)
        {
            bunifuProgressBar1.Value = 0;
        }
        private async void Btn_Baslat_Click(object sender, EventArgs e)
        {
            
            if(comboBox1.Text == "mp3")
            {
                VideoTitle();
                using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
                {
                    Description = "Lütfen Klasör belirleyin."
                })
                {
                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        label4.Text = "İndirme Başlatıldı.";
                        timer1.Start();
                        var youTube = YouTube.Default;
                        var video = await youTube.GetVideoAsync(bunifuMaterialTextbox1.Text);
                        File.WriteAllBytes(folderBrowserDialog.SelectedPath + @"\" + video.FullName, await video.GetBytesAsync());

                        var inputFile = new MediaToolkit.Model.MediaFile { Filename = folderBrowserDialog.SelectedPath + @"\" + video.FullName };
                        var outputFile = new MediaToolkit.Model.MediaFile { Filename = $"{folderBrowserDialog.SelectedPath + @"\" + video.FullName}.mp3" };

                        using (var engineTool = new Engine())
                        {
                            engineTool.GetMetadata(inputFile);
                            engineTool.Convert(inputFile, outputFile);
                        }
                        if (format == true)
                        {
                            File.Delete(folderBrowserDialog.SelectedPath + @"\" + video.FullName);
                        }
                        else
                        {
                            File.Delete($"{folderBrowserDialog.SelectedPath + @"\" + video.FullName}.mp3");
                        }
                        bunifuProgressBar1.Value = 100;
                    }
                    else
                    {                     
                        MessageBox.Show("Dosya yolu seçilmedi.","UYARI",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        label4.Text = "Dosya yolunu seçip yeniden deneyin";
                    }                 
                }         
            }
        }
        void VideoTitle()
        {
            WebRequest istek = HttpWebRequest.Create(bunifuMaterialTextbox1.Text);
            WebResponse cevap;
            cevap = istek.GetResponse();
            StreamReader streamRd = new StreamReader(cevap.GetResponseStream());
            string gelenYanit = streamRd.ReadToEnd();
            int baslangic = gelenYanit.IndexOf("<title>") + 7;
            int finish = gelenYanit.Substring(baslangic).IndexOf("</title>");
            string gelenBilgi = gelenYanit.Substring(baslangic, finish);
            listBox1.Items.Add(gelenBilgi);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            startpoint += 1;
            bunifuProgressBar1.Value = startpoint;
            if(bunifuProgressBar1.Value == 100)
            {
                timer1.Stop();
                label4.Text = "İndirme işlemi Tamamlandı.";
                bunifuProgressBar1.Value = 0;
                startpoint = 0;
            }           
        }
        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void bunifuImageButton3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
