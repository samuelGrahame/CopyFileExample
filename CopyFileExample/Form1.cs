using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CopyFileExample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            progressBar1.Visible = comboBox1.SelectedIndex > 1;            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = System.IO.File.Exists(textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex == 0)
            {
                CopyWindows();
            }else if(comboBox1.SelectedIndex == 1)
            {
                CopyFile();
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                CopyFileEx();
            }
            else if (comboBox1.SelectedIndex == 3)
            {
                CopyFileExThread();
            }
        }

        public void CopyFileEx()
        {
            var sw = Stopwatch.StartNew();
            progressBar1.Value = 0;

            CopyFileCallbackAction myCallback(FileInfo source, FileInfo destination, object state, long totalFileSize, long totalBytesTransferred)
            {
                double dProgress = (totalBytesTransferred / (double)totalFileSize) * 100.0;
                   progressBar1.Value = (int)dProgress;
               
                return CopyFileCallbackAction.Continue;
            }

            FileRoutines.CopyFile(textBox1.Text, textBox2.Text, myCallback);

            sw.Stop();

            if (textBox3.Text.Length > 0)
                textBox3.AppendText("\r\n");

            textBox3.AppendText($"Copy CopyFileEx No Thread: " + sw.ElapsedMilliseconds);
        }

        public void CopyFileExThread()
        {
            button1.Enabled = false;
            var sw = Stopwatch.StartNew();
            progressBar1.Value = 0;

            var sourceText = textBox1.Text;
            var destinationText = textBox2.Text;

            var thr = new Thread(() =>
            {
                CopyFileCallbackAction myCallback(FileInfo source, FileInfo destination, object state, long totalFileSize, long totalBytesTransferred)
                {
                    double dProgress = (totalBytesTransferred / (double)totalFileSize) * 100.0;

                    this.Invoke((MethodInvoker)delegate { progressBar1.Value = (int)dProgress; });

                    return CopyFileCallbackAction.Continue;
                }

                FileRoutines.CopyFile(sourceText, destinationText, myCallback);
            });
            thr.SetApartmentState(ApartmentState.MTA);
            thr.Priority = ThreadPriority.Highest;
            thr.Start();
            
            while(thr.IsAlive)
            {
                Application.DoEvents();
                Thread.Sleep(1);
            }


            button1.Enabled = true;

            

            sw.Stop();

            if (textBox3.Text.Length > 0)
                textBox3.AppendText("\r\n");

            textBox3.AppendText($"Copy CopyFileEx Thread: " + sw.ElapsedMilliseconds);
        }

        public void CopyFile()
        {
            var sw = Stopwatch.StartNew();

            System.IO.File.Copy(textBox1.Text, textBox2.Text);

            sw.Stop();

            if (textBox3.Text.Length > 0)
                textBox3.AppendText("\r\n");

            textBox3.AppendText($"Copy File.Copy: " + sw.ElapsedMilliseconds);
        }

        private void CopyWindows()
        {
            var sw = Stopwatch.StartNew();
            FileSystem.CopyFile(textBox1.Text, textBox2.Text, UIOption.AllDialogs);
            sw.Stop();

            if (textBox3.Text.Length > 0)
                textBox3.AppendText("\r\n");

            textBox3.AppendText($"Copy Windows: " + sw.ElapsedMilliseconds);
        }
    }
}
