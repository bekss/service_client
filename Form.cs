using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using SocialExplorer.IO.FastDBF;

namespace service_client
{
    public partial class Form : System.Windows.Forms.Form
    {
        private string service = "http://report.stat.kg/api/report/download/T_Month";
        private string directory = "";
        private int percent = 0;
        public int Percent { get { return percent; } set { percent = value; } }
        public string Service { get { return service; } set { service = value; } }
        public string Directory { get { return directory; } set { directory = value; } }

        public Form()

        {
            InitializeComponent();
            input_region.Items.Add("Баары");
            input_region.Items.Add("Бишкек шаары");
            input_region.Items.Add("Ош шаары");
            input_region.Items.Add("Чүй");
            input_region.Items.Add("Ош облусу");
            input_region.Items.Add("Ысыккөл");
            input_region.Items.Add("Жалалабад");
            input_region.Items.Add("");
            input_region.Items.Add("Бишкек");

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter_1(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                Directory = fbd.SelectedPath;
                string date_start = input_start.Value.ToShortDateString();
                string date_end = input_end.Value.ToShortDateString();

                string uri_text = $"{Service}?startDate={date_start}&&expirationDate={date_end}";
                string filename = "temp.dbf";

                WebClient client = new WebClient();


                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(Client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri(uri_text), filename); ;

            }
        }
        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            lbl_status.Text = "Файл жүктөлүп бүттү!";
            DbfFile globalFile = new DbfFile(Encoding.UTF8);
            globalFile.Open("temp.dbf", FileMode.Open);
            DbfFile writen = new DbfFile(Encoding.UTF8);
            writen.Open(Path.Combine(Directory, "testout.dbf"), FileMode.Create);

            var outrecord = new DbfRecord(globalFile.Header);

            for (int i=0; i<globalFile.Header.ColumnCount; i++)
            {
                writen.Header.AddColumn(globalFile.Header[i]);
            }


            while (globalFile.ReadNext(outrecord))
            {
                writen.Write(outrecord);
            }

            writen.Close();
            MessageBox.Show("ДБФ файлдары жазылып бүттү!");

        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Percent = (int)e.BytesReceived * 100 / (int)e.TotalBytesToReceive;
            lbl_status.Text = "Сиздин Файл жүктөлүп атат";
            lbl_percent.Text = $"{Percent.ToString()} / 100";
            progress.Maximum = (int)e.TotalBytesToReceive / 1000;
            progress.Value = (int)e.BytesReceived / 1000;
        }
    }
}
