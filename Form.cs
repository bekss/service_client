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
using System.Threading;

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
            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                Directory = fbd.SelectedPath;
                string date_start = input_start.Value.ToString("dd-MM-yy");
                string date_end = input_end.Value.ToString("dd-MM-yy");
                string uri_text = $"{Service}?startDate={date_start}&&expirationDate={date_end}";
                //string uri_text = "https://file-examples-com.github.io/uploads/2017/04/file_example_MP4_1920_18MG.mp4";

                int counter = 1;
                while (File.Exists(Path.Combine(Directory, Filename)))
                {
                    Filename = Filename.Substring(0, 3) + $" ({counter}).dbf";
                    counter++;
                }

                Thread thread = new Thread(() => {
                    WebClient client = new WebClient();
                        Invoke((MethodInvoker)delegate () {
                            progress.Style = ProgressBarStyle.Marquee;
                            progress.MarqueeAnimationSpeed = 1;
                            btn_save.Enabled = false;
                            btn_exit.Enabled = false;
                        });
                 
                        client.DownloadFileAsync(new Uri(uri_text), Path.Combine(Directory, Filename));
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
                        $"Ошибка на сервере.", "Ката");
                    return;
                }
                MessageBox.Show("Күтүлбөгөн ката кетти!");
                return;
            }
            if (e.Cancelled)
            {
                File.Delete(Path.Combine(Directory, Filename));
                MessageBox.Show("Файлды жүктөө жокко чыгарылды\n" +
                    "Файл не выгружен", "Жокко чыгарылды");
                Invoke((MethodInvoker)delegate ()
                {
                    btn_save.Enabled = true;
                    btn_exit.Enabled = true;
                });
                return;
            }
            else
            {
                DbfFile global_db = new DbfFile(Encoding.GetEncoding(1251));
                DbfFile inner_db = new DbfFile(Encoding.GetEncoding(1251));

                string current_region = "";
                Invoke((MethodInvoker)delegate () { current_region = ((KeyValuePair<string, Dictionary<string, int>>)input_region.SelectedItem).Key; });

                if (current_region == "Кыргызстан")
                {
                    Invoke((MethodInvoker)delegate () { lbl_status.Text = "Файл жүктөлүп бүттү"; });
                    MessageBox.Show($"{Filename} файлы жазылып бүттү! Программдан чыга берсеңиз болот.\n" +
                        "Можете выйти из приложения", "Бүттү");

                    Invoke((MethodInvoker)delegate () { btn_exit.Enabled = true; });
                    
                    return;
                }

                File.SetAttributes(Path.Combine(Directory, Filename), FileAttributes.Hidden);
                global_db.Open(Path.Combine(Directory, Filename), FileMode.Open);

                Invoke((MethodInvoker)delegate ()
                {
                    progress.Value = 0;
                    progress.Maximum = (int)global_db.Header.RecordCount;
                    lbl_status.Text = "Файлды иштетип атабыз...";
                    lbl_percent.Text = $"0 / {(int)global_db.Header.RecordCount}";
                });
                
                int territory = 0;
                Invoke((MethodInvoker)delegate () { territory = ((KeyValuePair<string, int>)input_district.SelectedItem).Value; });
               
                string inner_filename = territory.ToString() + ".dbf";
                string base_inner_filename = inner_filename;

                int counter = 1;
                while (File.Exists(Path.Combine(Directory, inner_filename)))
                {
                    inner_filename = $"{base_inner_filename} ({counter}).dbf";
                    counter++;
                }
                DbfRecord record;


                inner_db.Open(Path.Combine(Directory, inner_filename), FileMode.Create);

                for (int i = 0; i < global_db.Header.ColumnCount; i++)
                {
                    inner_db.Header.AddColumn(global_db.Header[i]);
                }

                HashSet<string> okpo_list = new HashSet<string>();
                long read_index = 0;
                long record_index = 0;
                while (read_index < global_db.Header.RecordCount)
                {
                    record = global_db.Read(read_index);
                    record.AllowDecimalTruncate = true;
                    if (!record.IsDeleted &&
                        record[record.FindColumn("AIL")].Contains(territory.ToString()) &&
                        record[record.FindColumn("NMES")].Replace(" ", "") == input_start.Value.Month.ToString() &&
                        !okpo_list.Contains(record[record.FindColumn("RN")]))  // main grouping by region condition
                    {

                        record.RecordIndex = record_index++;
                        inner_db.Write(record);
                        okpo_list.Add(record[record.FindColumn("RN")]);
                        Invoke((MethodInvoker)delegate ()
                        {
                            progress.Value = (int)read_index;
                            lbl_percent.Text = $"{(int)read_index} / {(int)global_db.Header.RecordCount}";
                        });
                    }
                    read_index++;
                }
                
                inner_db.Close();
                
                Invoke((MethodInvoker)delegate ()
                {
                    progress.Value = (int)read_index;
                    lbl_percent.Text = $"{(int)read_index} / {(int)global_db.Header.RecordCount}";
                    lbl_status.Text = "Файл иштелип бүттү";
                    btn_exit.Enabled = true;
                });
                
                global_db.Close();
                
                File.Delete(Path.Combine(Directory, Filename));

            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                lbl_status.Text = "Сиздин Файл жүктөлүп атат...";
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
            input_end.Value = new DateTime(input_start.Value.Year, input_start.Value.Month, DateTime.DaysInMonth(input_start.Value.Year, input_start.Value.Month));
            btn_save.Enabled = true;
        }

        private void input_region_SelectedIndexChanged(object sender, EventArgs e)
        {
            input_district.DataSource = ((KeyValuePair<string, Dictionary<string, int>>)input_region.SelectedItem).Value.ToArray();
            btn_save.Enabled = true;
        }

        private void input_district_SelectedIndexChanged(object sender, EventArgs e)
        {
            btn_save.Enabled = true;
        }
    }
    public static class threadHandling
    {
        internal static void get_region(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
