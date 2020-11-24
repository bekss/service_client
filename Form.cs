using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using SocialExplorer.IO.FastDBF;
using System.Linq;
using System.Threading;

namespace service_client
{
    public partial class Form : System.Windows.Forms.Form
    {
        private static Dictionary<string, Dictionary<string, int>> regions_districts = new Dictionary<string, Dictionary<string, int>>
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
        private string Service { get; set; } = "http://report.stat.kg/api/report/download/T_Month";
        private string current_dir { get; set; } = "";

        private string Filename { get; set; } = "417";

        public Form()
        {
            InitializeComponent();

            current_dir = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"Emgek/directory.txt"));
            // Initial values of datetime pickers
            DateTime month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            input_start.Value = month.AddMonths(-1);
            input_end.Value = month.AddDays(-1);

            // region, district data initializing
            input_region.DataSource = regions_districts.ToList();
            input_region.DisplayMember = "Key";
            input_region.ValueMember = "Value";
            input_region.DropDownStyle = ComboBoxStyle.DropDownList;

            input_district.DataSource = regions_districts.First().Value.ToArray();
            input_district.DisplayMember = "Key";
            input_district.ValueMember = "Value";
            input_district.DropDownStyle = ComboBoxStyle.DropDownList;

            // set progreass bar minimum to 0
            progress.Minimum = 0;
        }


        private void btn_save_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            if (Directory.Exists(current_dir))
            {
                fbd.SelectedPath = current_dir;
            }
            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                current_dir = fbd.SelectedPath;
                File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"Emgek/directory.txt"),
                    current_dir);
                string date_start = input_start.Value.ToString("dd-MM-yy");
                string date_end = input_end.Value.ToString("dd-MM-yy");
                string uri_text = $"{Service}?startDate={date_start}&&expirationDate={date_end}";

                Filename = get_filename(current_dir, Filename);

                Thread thread = new Thread(() => {
                    WebClient client = new WebClient();
                        Invoke((MethodInvoker)delegate () {
                            progress.Style = ProgressBarStyle.Marquee;
                            progress.MarqueeAnimationSpeed = 1;
                            btn_save.Enabled = false;
                            btn_exit.Enabled = false;
                        });
                 
                        client.DownloadFileAsync(new Uri(uri_text), Path.Combine(current_dir, Filename));
                        client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
                        client.DownloadFileCompleted += new AsyncCompletedEventHandler(Client_DownloadFileCompleted);
                });
                thread.Start();
            }
        }
        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    btn_save.Enabled = true;
                    btn_exit.Enabled = true;
                });
                if (e.Error.GetType().Equals(typeof(WebException)))
                {
                    if (e.Error.InnerException != null)
                    {
                        if (e.Error.InnerException.GetType().Equals(typeof(IOException)))
                        {
                            MessageBox.Show($"Ката: {Filename} файлы башка програмда колдонулуп атат аны жаап кайра аракет кылыңыз.");
                            return;
                        }
                        if (e.Error.GetType().Equals(typeof(UnauthorizedAccessException)))
                        {
                            MessageBox.Show($"{current_dir} директориасына жеткиңиз жок. Башка директорианы тандап көрүңүз");
                            return;
                        }
                    }
                    MessageBox.Show($"Ката: интернет жок болушу мүмкүн же report.stat.kg иштебей атат", "Ката");
                    return;
                }
                MessageBox.Show("Күтүлбөгөн ката кетти!");
                return;
            }
            if (e.Cancelled)
            {
                File.Delete(Path.Combine(current_dir, Filename));
                MessageBox.Show("Файлды жүктөө жокко чыгарылды", "Жокко чыгарылды");
                Invoke((MethodInvoker)delegate
                {
                    btn_save.Enabled = true;
                    btn_exit.Enabled = true;
                });
                return;
            }
            else
            {

                string current_region = "";
                Invoke((MethodInvoker)delegate { current_region = ((KeyValuePair<string, Dictionary<string, int>>)input_region.SelectedItem).Key; });

                if (current_region == "Кыргызстан")
                {
                    Invoke((MethodInvoker)delegate { lbl_status.Text = $"{Filename} иштелип aтат..."; });
                    Thread.Sleep(2000);
                    Invoke((MethodInvoker)delegate { 
                        lbl_status.Text = $"{Filename} иштелип бүттү";
                        btn_exit.Enabled = true;
                    });

                    return;
                }

                DbfFile global_db = new DbfFile(Encoding.GetEncoding(1251));
                DbfFile inner_db = new DbfFile(Encoding.GetEncoding(1251));

                File.SetAttributes(Path.Combine(current_dir, Filename), FileAttributes.Hidden);
                global_db.Open(Path.Combine(current_dir, Filename), FileMode.Open);

                Invoke((MethodInvoker)delegate
                {
                    progress.Value = 0;
                    progress.Maximum = (int)global_db.Header.RecordCount;
                    lbl_rec_count.Text = "1";
                });
                int territory = 0;
                Invoke((MethodInvoker)delegate { territory = ((KeyValuePair<string, int>)input_district.SelectedItem).Value; });

                string inner_filename = get_filename(current_dir, territory.ToString());
                Invoke((MethodInvoker)delegate { lbl_status.Text = $"{inner_filename} иштелип атат..."; });
                inner_db.Open(Path.Combine(current_dir, inner_filename), FileMode.Create);
                
                for (int i = 0; i < global_db.Header.ColumnCount; i++)
                {
                    inner_db.Header.AddColumn(global_db.Header[i]);
                }

                HashSet<string> okpo_list = new HashSet<string>();

                DbfRecord record = new DbfRecord(global_db.Header);
                record.AllowDecimalTruncate = true;
                DbfRecord new_record = new DbfRecord(inner_db.Header);
                new_record.AllowDecimalTruncate = true;
                new_record.AllowIntegerTruncate = true;
                long record_count = 0;
                while (global_db.ReadNext(record))
                {
                    if (!record.IsDeleted &&
                        record[record.FindColumn("AIL")].Contains(territory.ToString()) &&
                        record[record.FindColumn("NMES")].Replace(" ", "") == input_start.Value.Month.ToString() &&
                        !okpo_list.Contains(record[record.FindColumn("RN")]))  // main grouping by region condition
                    {
                        for (int j = 0; j < inner_db.Header.ColumnCount; j++)
                        {
                            new_record[j] = record[j];
                        }
                        Invoke((MethodInvoker)delegate {
                            lbl_rec_count.Text = record_count.ToString();
                        });
                        record_count++;
                        inner_db.Write(new_record, true);
                        okpo_list.Add(record[record.FindColumn("RN")]);
                    }
                    Invoke((MethodInvoker)delegate { if (progress.Value < progress.Maximum) progress.Value++; });
                }
                Invoke((MethodInvoker)delegate
                {
                    lbl_rec_count.Text = record_count.ToString();
                    lbl_status.Text = $"{inner_filename} иштелип бүттү";
                    btn_exit.Enabled = true;
                });
                inner_db.Close();
                global_db.Close();
                File.Delete(Path.Combine(current_dir, Filename));
            }

        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                lbl_status.Text = "Файл жүктөлүп атат...";
                progress.Style = ProgressBarStyle.Blocks;
                lbl_percent.Text = $"{e.ProgressPercentage} / 100";
                progress.Value = e.ProgressPercentage;
                progress.Maximum = 100;
            });
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void input_start_ValueChanged(object sender, EventArgs e)
        {
            lbl_rec_count.Text = "";
            lbl_percent.Text = "";
            lbl_status.Text = "";
            progress.Value = 0;
            input_end.Value = new DateTime(input_start.Value.Year, input_start.Value.Month, DateTime.DaysInMonth(input_start.Value.Year, input_start.Value.Month));
            btn_save.Enabled = true;
        }

        private void input_region_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbl_rec_count.Text = "";
            lbl_percent.Text = "";
            lbl_status.Text = "";
            progress.Value = 0;
            input_district.DataSource = ((KeyValuePair<string, Dictionary<string, int>>)input_region.SelectedItem).Value.ToArray();
            btn_save.Enabled = true;
        }

        private void input_district_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbl_rec_count.Text = "";
            lbl_percent.Text = "";
            lbl_status.Text = "";
            progress.Value = 0;
            btn_save.Enabled = true;
        }
        private string get_filename(string dir, string arg)
        {
            int counter = 1;
            string temp_name = $"{arg}.dbf";
            while (File.Exists(Path.Combine(dir, temp_name)))
            {
                temp_name = $"{arg} ({counter}).dbf";
                counter++;
            }
            return temp_name;
        }
    }
}
