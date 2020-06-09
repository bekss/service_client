using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using SocialExplorer.IO.FastDBF;
using System.Security.Permissions;
namespace service_client
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public partial class Form : System.Windows.Forms.Form
    {
        public static Dictionary<string, int> regions = new Dictionary<string, int>()
        {
            { "Баткен", 41705},
            { "Бишкек шаары", 41711 },
            { "Жалалабад", 41703 },
            { "Нарын", 41704 },
            { "Талас", 41707 },
            { "Ош облусу", 41706 },
            { "Ош шаары", 41721 },
            { "Чүй", 41708 },
            { "Ысыккөл", 41702 }
        };
        public int Percent { get; set; } = 0;
        public string Service { get; set; } = "http://report.stat.kg/api/report/download/T_Month";
        public string Directory { get; set; } = "";

        public Form()

        {
            InitializeComponent();
            input_region.Items.AddRange(new string[]{ "Кыргызстан", "Баткен", "Бишкек шаары",
                "Жалалабад", "Нарын", "Талас", "Ош облусу", "Ош шаары", "Чүй", "Ысыккөл"});
            input_region.Text = "Кыргызстан";
            input_region.DropDownStyle = ComboBoxStyle.DropDownList;
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
                try
                {
                    client.DownloadFileAsync(new Uri(uri_text), Path.Combine(Directory, filename));
                } catch
                {
                    MessageBox.Show($"Күтүлбөгөн ката кетти: Програм колдонгон temp.dbf файлы башка програмда колдонулуп атат аны жаап кайра аракет кылыңыз.");
                }

            }
        }
        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (e.Error.GetType().Equals(typeof(WebException)))
                {
                    if (e.Error.InnerException != null)
                    {
                        if (e.Error.InnerException.GetType().Equals(typeof(IOException)))
                        {
                            MessageBox.Show("Ката: Програм колдонгон temp.dbf файлы башка програмда колдонулуп атат аны жаап кайра аракет кылыңыз.");
                            return;
                        }
                        if (e.Error.GetType().Equals(typeof(UnauthorizedAccessException)))
                        {
                            MessageBox.Show($"Жеткиңиз жок: {e.Error.InnerException.Message}, {e.Error.InnerException.GetType()}");
                            return;
                        }
                    }
                    MessageBox.Show($"Ката: интернет жок болушу мүмкүн же report.stat.kg иштебей атат. {e.Error.Message}, {e.Error.GetType()}");
                    return;
                }
                MessageBox.Show("Күтүлбөгөн ката кетти!");
                return;
            }
            if (e.Cancelled)
            {
                MessageBox.Show("Файлды жүктөө жокко чыгарылды.");
                return;
            }
            else
            {

                lbl_status.Text = "Файл жүктөлүп бүттү!";
                DbfFile global_db = new DbfFile(Encoding.UTF8);
                DbfFile inner_db = new DbfFile(Encoding.UTF8);
                var region_dict = new Dictionary<string, int>();
                if (input_region.SelectedItem.ToString() == "Кыргызстан")
                {
                    region_dict = regions;
                }
                else
                {
                    region_dict.Add(input_region.SelectedItem.ToString(), regions[input_region.SelectedItem.ToString()]);
                }
                foreach (KeyValuePair<string, int> region in region_dict)
                {
                    global_db.Open(Path.Combine(Directory, "temp.dbf"), FileMode.Open);
                    inner_db.Open(Path.Combine(Directory, $"{region.Key}.dbf"), FileMode.Create);

                    var new_record = new DbfRecord(global_db.Header);

                    for (int i = 0; i < global_db.Header.ColumnCount; i++)
                    {
                        inner_db.Header.AddColumn(global_db.Header[i]);
                    }

                    int record_index = 0;
                    while (global_db.ReadNext(new_record))
                    {
                        if (new_record[1].ToString().Substring(0, 5) == region.Value.ToString())
                        {
                            new_record.RecordIndex = record_index; // required to force change index.
                            inner_db.Write(new_record);
                            record_index++;
                        }
                    }
                    inner_db.Close();
                    global_db.Close();
                }
                MessageBox.Show("ДБФ файлдары жазылып бүттү! Програмдан чыга берсеңиз болот.");
                btn_save.Text = "Кайрадан сактоо";
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Percent = (int)e.BytesReceived * 100 / (int)e.TotalBytesToReceive;
            lbl_status.Text = "Сиздин Файл жүктөлүп атат";
            lbl_percent.Text = $"{Percent} / 100";
            progress.Maximum = (int)e.TotalBytesToReceive / 1000;
            progress.Value = (int)e.BytesReceived / 1000;
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
