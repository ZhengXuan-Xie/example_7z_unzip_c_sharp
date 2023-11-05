using System.Threading;

using System.Diagnostics;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace _7z_unzip
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread unzip = new Thread(Thread_unzip);
            unzip.Start();
        }

        public Int32 get_package_files_count()
        {
            string package_name = this.textBox1.Text;
            Process exe = new Process();

            exe.StartInfo.FileName = "7z.exe";
            exe.StartInfo.Arguments = "l " + package_name;

            exe.StartInfo.UseShellExecute = false;
            exe.StartInfo.RedirectStandardOutput = true;
            exe.StartInfo.RedirectStandardError = true;
            exe.StartInfo.CreateNoWindow = true;
            exe.Start();

            string output = exe.StandardOutput.ReadToEnd();
            string error = exe.StandardError.ReadToEnd();

            exe.WaitForExit();

            //通过这种方式得到7z.exe的stdout输出的最后一行关键内容
            output = output.Substring(output.LastIndexOf("--") + 3);

            //this.textBox1.Text = output;
            //[文件修改日期-不准确的值]        [原大小]      [压缩后大小] [文件数量] [文件夹数量]     
            //2023 - 08 - 04 23:47:50          287466909     38572164  110 files, 39 folders

            string[] temp = output.Split(' ');
            List<string> result = new List<string>();
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] != "")
                {
                    result.Add(temp[i]);
                    //this.textBox1.AppendText(temp[i] + "\r\n");
                }
            }
            /*
            for (int i = 0; i < result.Count; i++)
            {
                this.textBox1.AppendText(result[i] + "\r\n");
            }*/


            Int32 sum = 0;
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i].Contains("files"))
                {
                    sum += Convert.ToInt32(result[i - 1]);
                }
                if (result[i].Contains("folders"))
                {
                    sum += Convert.ToInt32(result[i - 1]);
                }
            }

            //this.textBox1.AppendText(Convert.ToString(sum));
            return sum;
        }
        public void Thread_unzip()
        {
            Int32 sum = get_package_files_count();
            string package_name = this.textBox1.Text;

            Process exe = new Process();
            exe.StartInfo.FileName = "7z.exe";
            exe.StartInfo.Arguments = "x -y -bb3 -o./funny " + package_name;

            exe.StartInfo.UseShellExecute = false;
            exe.StartInfo.RedirectStandardOutput = true;
            exe.StartInfo.RedirectStandardError = true;
            exe.StartInfo.CreateNoWindow = true;
            exe.Start();

            Int32 count = 0;
            while (!exe.StandardOutput.EndOfStream)
            {
                string standard_output = exe.StandardOutput.ReadLine();
                if (standard_output.Contains("- "))
                {
                    count++;
                    this.progressBar1.Value = (Int32)((double)count * 100 / (double)(sum));
                }
                Thread.Sleep(10);
            }
            exe.WaitForExit();
        }
    }
}