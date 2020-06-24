using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;

namespace minecraft_server_backup_windows
{
    public partial class Form1 : Form
    {
        static String cfg;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 获取运行目录
            String td = System.IO.Directory.GetCurrentDirectory();
            if (td.EndsWith("\\"))
                td.Remove(td.Length - 1);

            // 配置文件目录
            cfg = td + "\\config.json";

            textBox1.Text = getCfg();
            if (textBox1.Text.Equals(""))
            {
                textBox1.Text = td + "\\world\\";
            }

            textBox2.Text = td + "\\world_"
                + DateTime.Today.Year + "-" + DateTime.Today.Month + "-" + DateTime.Today.Day + "-"
                + DateTime.Now.Hour + "-" + DateTime.Now.Minute + ".zip";
        }

        String getCfg()
        {
            if (!File.Exists(cfg))
                return "";

            try
            {
                StreamReader file = File.OpenText(cfg);
                JsonTextReader reader = new JsonTextReader(file);
                JObject jsonObject = (JObject)JToken.ReadFrom(reader);

                if (!jsonObject.ContainsKey("world-folder"))
                {
                    return "";
                }

                String a = (String)jsonObject["world-folder"];

                reader.Close();
                file.Close();

                return a;

            }
            catch (Exception e)
            {
                return "";
            }

        }

        void saveCfg()
        {
            JObject jsonObject = new JObject();
            jsonObject["world-folder"] = textBox1.Text;
            File.WriteAllText(cfg, Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented));

        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveCfg();
try
            {
                packing(textBox1.Text, textBox2.Text, checkBox1.Checked);
            }
            catch (Exception e1)
            {
                if (e1 is UnauthorizedAccessException)
                {
                    print("[ERROR] 目录拒绝储存.");

                }
                else
                {
                    print("[ERROR] 未知错误导致压缩失败.");

                }

                progressBar1.Value = progressBar1.Maximum;
            }
        }

        static ArrayList files;

        void print(String text)
        {
            textBox3.Text += text + "\r\n";
        }

        void packing(string folderName, string zipName, bool over)
        {
            progressBar1.Value = 0;

            if (folderName.EndsWith(@"\"))
                folderName = folderName.Remove(folderName.Length - 1);

            if (!System.IO.Directory.Exists(folderName))
            {
                print("[ERROR] " + folderName + " 不存在!");
                progressBar1.Value = progressBar1.Maximum;
                return;
            }

            if (!over && System.IO.File.Exists(zipName))
            {
                print("[ERROR] " + components + " 已经存在,您没有勾选覆盖!");
                progressBar1.Value = progressBar1.Maximum;
                return;
            }

            files = new ArrayList();
            DirectoryInfo di = new DirectoryInfo(folderName);
            int size = 0;
            foreach (DirectoryInfo item in di.GetDirectories())
            {
                files.Add(item.FullName);
                print("[SCANER] " + item.FullName);
                size++;
            }

            progressBar1.Maximum = size;

            FileStream b = File.Create(zipName);
            ZipOutputStream zipOutputStream = new ZipOutputStream(b);

            foreach (String file in files)
            {
                print("[PACKING] " + file);

                FileStream fs = File.OpenRead(file);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                zipOutputStream.PutNextEntry(new ZipEntry(file));
                zipOutputStream.Write(buffer, 0, buffer.Length);
                fs.Close();
            }

            zipOutputStream.Close();

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("你事有多懒，广告都不能去浏览器自己输一下。");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String td = System.IO.Directory.GetCurrentDirectory();
            if (td.EndsWith("\\"))
                td.Remove(td.Length - 1);

            textBox1.Text = td + "\\world\\";
        }
    }
}
