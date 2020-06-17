﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using SocialExplorer.IO.FastDBF;
using System.Security.Permissions;
using System.Reflection;

namespace service_client
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public partial class Form : System.Windows.Forms.Form
    {
        public static Dictionary<string, int> regions = new Dictionary<string, int>()
        {
            { "Бишкек ш.", 41711 },
            { "Ош ш.", 41721 },
            { "Ысык-Көл", 41702 },
            { "Жалал-Абад", 41703 },
            { "Нарын", 41704 },
            { "Баткен", 41705 },
            { "Ош", 41706 },
            { "Талас", 41707 },
            { "Чүй", 41708 }
        };
        public int Percent { get; set; } = 0;
        public string Service { get; set; } = "http://report.stat.kg/api/report/download/T_Month";
        public string Directory { get; set; } = "";

        public string Filename { get; set; } = "41700.dbf";

        public Form()

        {
            InitializeComponent();
            input_region.Items.AddRange(new string[]{ "Кыргызстан", "Бишкек ш.", "Ош ш.", "Ысык-Көл", "Жалал-Абад", "Нарын", "Баткен", "Ош", "Талас", "Чүй"});
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
                int counter = 1;
                while (File.Exists(Path.Combine(Directory, Filename)))
                {
                    Filename = Filename.Substring(0, 5) + $" ({counter}).dbf";
                    counter++;
                }

                WebClient client = new WebClient();

                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(Client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri(uri_text), Path.Combine(Directory, Filename));
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
                            MessageBox.Show("Ката: Програм колдонгон Кыргызстан.dbf файлы башка програмда колдонулуп атат аны жаап кайра аракет кылыңыз.");
                            return;
                        }
                        if (e.Error.GetType().Equals(typeof(UnauthorizedAccessException)))
                        {
                            MessageBox.Show($"{Directory} директориасына жеткиңиз жок. Башка директорианы тандап көрүңүз");
                            return;
                        }
                    }
                    MessageBox.Show($"Ката: интернет жок болушу мүмкүн же report.stat.kg иштебей атат\n" +
                        $"Ошибка на сервере.");
                    return;
                }
                MessageBox.Show("Күтүлбөгөн ката кетти!");
                return;
            }
            if (e.Cancelled)
            {
                File.Delete(Path.Combine(Directory, Filename));
                MessageBox.Show("Файлды жүктөө жокко чыгарылды\n" +
                    "Файл не выгружен");
                return;
            }
            else
            {

                lbl_status.Text = "Файл жүктөлүп бүттү\n";
                DbfFile global_db = new DbfFile(Encoding.UTF8);
                DbfFile inner_db = new DbfFile(Encoding.UTF8);
                var region_dict = new Dictionary<string, int>();
                if (input_region.SelectedItem.ToString() == "Кыргызстан")
                {
                    MessageBox.Show("ДБФ файлдары жазылып бүттү! Програмдан чыга берсеңиз болот.\n" +
                        "Можете выйти из приложения", "Бүттү");
                    return;
                }

                File.SetAttributes(Path.Combine(Directory, Filename), FileAttributes.Hidden);

                region_dict.Add(input_region.SelectedItem.ToString(), regions[input_region.SelectedItem.ToString()]);
                foreach (KeyValuePair<string, int> region in region_dict)
                {
                    global_db.Open(Path.Combine(Directory, Filename), FileMode.Open);
                    string inner_filename = region.Value.ToString() + ".dbf";
                    int counter = 1;
                    while (File.Exists(Path.Combine(Directory, inner_filename)))
                    {
                        inner_filename = inner_filename.Substring(0, 5) + $" ({counter}).dbf";
                        counter++;
                    }
                    DbfRecord record = new DbfRecord(global_db.Header);
                    
                    inner_db.Open(Path.Combine(Directory, inner_filename), FileMode.Create);

                    for (int i = 0; i < global_db.Header.ColumnCount; i++)
                    {
                        inner_db.Header.AddColumn(global_db.Header[i]);
                    }

                    string temp;
                    byte[] bytes;
                    while (global_db.ReadNext(record))
                    {
                        if (!record.IsDeleted && record[1].ToString().Substring(0, 5) == region.Value.ToString())  // main grouping by region condition
                        {
                            
                            DbfRecord new_record = new DbfRecord(inner_db.Header);
                            for (int j = 0; j < inner_db.Header.ColumnCount; j++)
                            {
                                bytes = Encoding.GetEncoding(1251).GetBytes(record[j]);
                                temp = Encoding.GetEncoding(1251).GetString(bytes);
                                if (temp.Contains("?"))
                                {                                                    //   Imitating null values with '' for both chararter columns and numeric columns.
                                    new_record[j] = "";                              //   But be aware if you have chararter columns that may contain '?' symbol
                                }                                                    //   in that case you will simply loose records that contain '?' symbol.
                                else
                                    new_record[j] = temp;
                            }
                            inner_db.Write(new_record, true);
                        }
                    }
                    inner_db.Close();
                    global_db.Close();
                }
                File.Delete(Path.Combine(Directory, Filename));
                MessageBox.Show("ДБФ файлдары жазылып бүттү! Програмдан чыга берсеңиз болот.\n" +
                    "Можете выйти из приложения");
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
