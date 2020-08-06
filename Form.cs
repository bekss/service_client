using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using SocialExplorer.IO.FastDBF;
using System.Security.Permissions;
using System.Linq;

namespace service_client
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public partial class Form : System.Windows.Forms.Form
    {
        public static Dictionary<string, Dictionary<string, int>> regions_districts = new Dictionary<string, Dictionary<string, int>>
        {
            { "Кыргызстан",
                new Dictionary<string, int>
                {
                    { "Баары", 417 }
                }
            },
            { "Бишкек ш.",
                new Dictionary<string, int>{
                    { "Баары", 41711 }
                }
            },
            { "Ош ш.",
                new Dictionary<string, int>{
                    {"Баары", 41721 }
                }
            },
            { "Ысык-Көл",
                new Dictionary<string, int>{
                    { "Баары", 41702  },
                    { "Каракол", 41702410 },
                    { "Балыкчы", 41702420 },
                    { "Ак-Суу", 41702205 },
                    { "Жети-Өгүз", 41702210 },
                    { "Ысык-Көл", 41702215 },
                    { "Тоң", 41702220 },
                    { "Түп", 41702225 },
                }
            },
            { "Жалал-Абад",
                new Dictionary<string, int>{
                    { "Баары", 41703 },
                    { "Ала-Бука", 41703204 },
                    { "Базар-Коргон", 41703207 },
                    { "Аксы", 41703211 },
                    { "Ноокен", 41703215 },
                    { "Сузак", 41703220 },
                    { "Тогуз-Торо", 41703223 },
                    { "Токтогул", 41703225 },
                    { "Чаткал", 41703230 },
                    { "Жалал-Абад ш.", 41703410 },
                    { "Таш-Көмүр ш.", 41703420 },
                    { "Майлуу-Суу ш.", 41703430 },
                    { "Кара-Көл ш.", 41703440 },
                }
            },
            { "Нарын",
                new Dictionary<string, int>{
                    { "Баары", 41704 },
                    { "Ак-Талаа", 41704210 },
                    { "Ат-Башы", 41704220 },
                    { "Жумгал", 41704230 },
                    { "Кочкор", 41704235 },
                    { "Нарын", 41704245 },
                    { "Нарын ш.", 41704400 },
                }
            },
            { "Баткен",
                new Dictionary<string, int>{
                    { "Баары", 41705 },
                    { "Баткен", 41705214 },
                    { "Лейлек", 41705236 },
                    { "Кадамжай", 41705258 },
                    { "Баткен ш.", 41705410 },
                    { "Сүлүктү ш.", 41705420 },
                    { "Кызыл-Кыя ш.", 41705430 },
                }
            },
            { "Ош",
                new Dictionary<string, int>{
                    { "Баары", 41706 },
                    { "Алай", 41706207},
                    { "Араван", 41706211 },
                    { "Кара-Суу", 41706226 },
                    { "Ноокат", 41706242 },
                    { "Кара-Кулжа", 41706246 },
                    { "Өзгөн", 41706255 },
                    { "Чоң-Алай", 41706259 }
                }
            },
            { "Талас",
                new Dictionary<string, int>{
                    { "Баары",  41707 },
                    { "Кара-Буура", 41707215 },
                    { "Бакай-Ата", 41707220 },
                    { "Манас", 41707225 },
                    { "Талас", 41707232 },
                    { "Талас ш.", 41707400 }
                }
            },
            { "Чүй",
                new Dictionary<string, int>{
                    { "Баары", 41708},
                    { "Аламүдүн", 41708203 },
                    { "Ысык-ата", 41708206 },
                    { "Жайыл", 41708209 },
                    { "Кемин", 41708213 },
                    { "Москва", 41708217 },
                    { "Панфилов", 41708219 },
                    { "Сокулук", 41708222 },
                    { "Чүй", 41708223 },
                    { "Токмок ш.", 41708400 },
                }
            }
        };
        public int Percent { get; set; } = 0;
        public string Service { get; set; } = "http://report.stat.kg/api/report/download/T_Month";
        public string Directory { get; set; } = "";

        public string Filename { get; set; } = "417.dbf";

        public Form()

        {
            InitializeComponent();
            input_region.DataSource = regions_districts.ToList();
            input_region.DisplayMember = "Key";
            input_region.ValueMember = "Value";
            input_region.DropDownStyle = ComboBoxStyle.DropDownList;

            input_district.DataSource = regions_districts.First().Value.ToArray();
            input_district.DisplayMember = "Key";
            input_district.ValueMember = "Value";
            input_district.DropDownStyle = ComboBoxStyle.DropDownList;
        }


        private void btn_save_Click(object sender, EventArgs e)
        {
            btn_save.Enabled = false;
            var fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                Directory = fbd.SelectedPath;
                string date_start = input_start.Value.ToString("dd-MM-yy");
                string date_end = input_end.Value.ToString("dd-MM-yy");
                string uri_text = $"{Service}?startDate={date_start}&&expirationDate={date_end}";
                int counter = 1;
                while (File.Exists(Path.Combine(Directory, Filename)))
                {
                    Filename = Filename.Substring(0, 3) + $" ({counter}).dbf";
                    counter++;
                }

                WebClient client = new WebClient();

                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(Client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri(uri_text), Path.Combine(Directory, Filename));
            }
            btn_save.Enabled = true;
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

                if (((KeyValuePair<string, Dictionary<string, int>>)input_region.SelectedItem).Key == "Кыргызстан")
                {
                    MessageBox.Show($" {Filename} файлы жазылып бүттү! Программдан чыга берсеңиз болот.\n" +
                        "Можете выйти из приложения", "Бүттү");
                    return;
                }

                File.SetAttributes(Path.Combine(Directory, Filename), FileAttributes.Hidden);
                global_db.Open(Path.Combine(Directory, Filename), FileMode.Open);

                int territory = ((KeyValuePair<string, int>)input_district.SelectedItem).Value;
                string inner_filename = territory.ToString() + ".dbf";
                string base_inner_filename = inner_filename;
                
                int counter = 1;
                while (File.Exists(Path.Combine(Directory, inner_filename)))
                {
                    inner_filename = $"{base_inner_filename} ({counter}).dbf";
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
                HashSet<string> okpo_list = new HashSet<string>();
                while (global_db.ReadNext(record))
                {
                    DbfRecord new_record = new DbfRecord(inner_db.Header);
                    if (!record.IsDeleted &&
                        record[record.FindColumn("AIL")].Contains(territory.ToString()) &&
                        record[record.FindColumn("NMES")].Replace(" ", "") == input_start.Value.Month.ToString() &&
                        !okpo_list.Contains(record[record.FindColumn("RN")]))  // main grouping by region condition
                    {
                        for (int j = 0; j < inner_db.Header.ColumnCount; j++)
                        {
                            bytes = Encoding.GetEncoding(1251).GetBytes(record[j]);
                            temp = Encoding.GetEncoding(1251).GetString(bytes);
                            if (temp.Replace(" ", "") == "?")
                            {                                                    //   Imitating null values with '' for both chararter columns and numeric columns.
                                new_record[j] = "";                              //   But be aware if you have character columns that may contain '?' symbol
                            }                                                    //   in that case you will simply loose records that contain '?' symbol.
                            else
                                new_record[j] = temp;
                        }
                        inner_db.Write(new_record, true);
                        okpo_list.Add(record[record.FindColumn("RN")]);
                    }
                }
                inner_db.Close();
                global_db.Close();
                File.Delete(Path.Combine(Directory, Filename));
                MessageBox.Show($" {Filename} жазылып бүттү! Программдан чыга берсеңиз болот.\n" +
                    "Можете выйти из приложения");
                btn_save.Text = "Кайрадан сактоо";
                btn_save.Enabled = true;
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

        private void input_start_ValueChanged(object sender, EventArgs e)
        {
            input_end.Value = new DateTime(input_start.Value.Year, input_start.Value.Month, DateTime.DaysInMonth(input_start.Value.Year, input_start.Value.Month));
        }

        private void input_region_SelectedIndexChanged(object sender, EventArgs e)
        {
            input_district.DataSource = ((KeyValuePair<string, Dictionary<string, int>>)input_region.SelectedItem).Value.ToArray();
        }
    }
}
