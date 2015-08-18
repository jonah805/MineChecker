using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineChecker
{
    public partial class Form1 : Form
    {
        BackgroundWorker worker;

        public Form1()
        {
          worker = new BackgroundWorker();
          worker.WorkerReportsProgress = true;
          worker.WorkerSupportsCancellation = true;
 
          worker.DoWork += new DoWorkEventHandler(worker_DoWork);
          worker.ProgressChanged += 
                      new ProgressChangedEventHandler(worker_ProgressChanged);
          worker.RunWorkerCompleted += 
                     new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            InitializeComponent();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int percentFinished = (int)e.Argument;
            while (!worker.CancellationPending && percentFinished < listBox1.Items.Count)
            {                
                worker.ReportProgress(percentFinished);

                string item = listBox1.Items[percentFinished].ToString();
                string user = item.Split(':')[0];
                string pass = item.Split(':')[1];
                string accesstoken = "";
                string uuid = "";

                Core.LoginResult lr = Core.GetLogin(ref user, pass, ref accesstoken, ref uuid);

                printResult(lr, percentFinished);

                ++percentFinished;
            }
            e.Result = percentFinished;
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripStatusLabel1.Text = e.ProgressPercentage + "/" + listBox1.Items.Count;
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
          button1.Text = "Check";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                worker.CancelAsync();
                button1.Text = "Check";
            }
            else
            {
                listBox2.Items.Clear();
                worker.RunWorkerAsync(0);
                button1.Text = "Stop";
            }
        }

        private void printResult( Core.LoginResult lr, int pos )
        {
            string output = "";

            switch(lr)
            {
                case Core.LoginResult.AccountMigrated:
                    //output = "Account migrated.";
                    break;
                case Core.LoginResult.NotPremium:
                    //output = "Account not premium.";
                    break;
                case Core.LoginResult.OtherError:
                    //output = "Unknown Error.";
                    break;
                case Core.LoginResult.ServiceUnavailable:
                    //output = "Service currently unavailable.";
                    break;
                case Core.LoginResult.SSLError:
                    //output = "SSL Error.";
                    break;
                case Core.LoginResult.Success:
                    //output = "Success.";
                    Action action = () => listBox2.Items.Add(listBox1.Items[pos]);
                    listBox2.Invoke(action); // Or use BeginInvoke
                    break;
                case Core.LoginResult.WrongPassword:
                    //output = "Wrong password.";
                    break;
            }

            toolStripStatusLabel1.Text = output;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {  
            foreach( string line in File.ReadAllLines(openFileDialog1.FileName))
            {
                listBox1.Items.Add(line);
            }
        }

        private void uncheckedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(saveFileDialog1.OpenFile());

                for (int i = 0; i < listBox1.Items.Count; i++)
                {

                    writer.WriteLine(listBox1.Items[i].ToString());

                }

                writer.Dispose();
                writer.Close();
                toolStripStatusLabel1.Text = "Export finished.";
            }
        }

        private void workingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(saveFileDialog1.OpenFile());

                for (int i = 0; i < listBox2.Items.Count; i++)
                {

                    writer.WriteLine(listBox2.Items[i].ToString());

                }

                writer.Dispose();
                writer.Close();
                toolStripStatusLabel1.Text = "Export finished.";
            }
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                foreach( string line in File.ReadAllLines(file))
                {
                    listBox1.Items.Add(line);
                }
            }
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void clearUncheckedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void clearWorkingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
        }
    }
}
