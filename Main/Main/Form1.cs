using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Diagnostics;

namespace Main
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        //Dummy Line
        //127.0.0.1 - - [14/Jan/2017:15:23:28 +0100] "GET /img/steam.png HTTP/1.1" 200 8687 "http://boehmer.pro/index.php" "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:50.0) Gecko/20100101 Firefox/50.0"

        public Form1()
        {
            InitializeComponent();
        }
        private HttpStatusCode INT2HTTP(int code)
        {
            try
            {
                return (HttpStatusCode) code;
            }catch
            {
                return HttpStatusCode.Unused;
            }
        }
            
        private Dictionary<string, List<LogEntry>> entries = new Dictionary<string, List<LogEntry>>();
        private string[] logEntries = new string[] { "" };
        private readonly string QUOTE = @"""";
        
        private void ParseEntries()
        {

            label2.Text = "Unique IPs: " + entries.Count.ToString() + " | Total requests: " + LogEntry.entryCount;

            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();

            foreach (string key in entries.Keys)
            {
                
                TreeNode node = treeView1.Nodes.Add(key);
                foreach (LogEntry req in entries[key])
                {
                    TreeNode childNode = node.Nodes.Add(req.time);
                    childNode.Nodes.Add("Host: " + req.host);
                    childNode.Nodes.Add("Request: " + req.request);
                    childNode.Nodes.Add("Response Code: " + INT2HTTP(int.Parse(req.response.Split(' ')[1])).ToString());
                    childNode.Nodes.Add("UserAgent: " + req.useragent);
                }
            }

            treeView1.EndUpdate();
        }
        private void UpdateDic()
        {
            foreach (string entry in logEntries)
            {
                try
                {
                    string[] entrySplit = entry.Split(new string[] { " - - " }, StringSplitOptions.None);
                    string ip = entrySplit[0];
                    string body = entrySplit[1];
                    string[] bodyParts = body.Split(new string[] { QUOTE }, StringSplitOptions.None);
                    if (!entries.ContainsKey(ip))
                    {
                        entries.Add(ip, new List<Main.LogEntry>() { new LogEntry(bodyParts[0], bodyParts[1], bodyParts[2], bodyParts[3], bodyParts[5]) });
                    }
                    else
                    {
                        entries[ip].Add(new LogEntry(bodyParts[0], bodyParts[1], bodyParts[2], bodyParts[3], bodyParts[5]));
                    }
                }catch { continue; }
            }

            ParseEntries();
        }
        private void metroButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opnDlg = new OpenFileDialog();
            opnDlg.FileName = "";
            opnDlg.Title = "Access Log";
            opnDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            opnDlg.Filter = "text files (*.txt;*.log)|*.txt;*.log";

            if (opnDlg.ShowDialog() == DialogResult.OK)
            {
                accessLogPath.Text = opnDlg.FileName;
            }

            logEntries = File.ReadAllLines(accessLogPath.Text);
            UpdateDic();
        }

        Stopwatch sw = new Stopwatch();
        private void metroTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void Search(string term)
        {
            treeView2.BeginUpdate();
            treeView2.Nodes.Clear();

            foreach (string key in entries.Keys)
            {
                if (key.ToLower().Contains(term.ToLower()))
                {
                    TreeNode node = treeView2.Nodes.Add(key);
                    foreach (LogEntry req in entries[key])
                    {
                        TreeNode childNode = node.Nodes.Add(req.time);
                        childNode.Nodes.Add("Host: " + req.host);
                        childNode.Nodes.Add("Request: " + req.request);
                        childNode.Nodes.Add("Response Code: " + INT2HTTP(int.Parse(req.response.Split(' ')[1])).ToString());
                        childNode.Nodes.Add("UserAgent: " + req.useragent);
                    }
                }else
                {
                    foreach (LogEntry req in entries[key])
                    {
                        if (req.HasTerm(term))
                        {
                            TreeNode node = null;
                            TreeNode check = TreeAlreadyHas(key, treeView2);
                            if (check != null)
                            {
                                node = check;
                            }
                            else
                            {
                                node = treeView2.Nodes.Add(key);
                            }
                            
                            TreeNode childNode = node.Nodes.Add(req.time);
                            childNode.Nodes.Add("Host: " + req.host);
                            childNode.Nodes.Add("Request: " + req.request);
                            childNode.Nodes.Add("Response Code: " + INT2HTTP(int.Parse(req.response.Split(' ')[1])).ToString());
                            childNode.Nodes.Add("UserAgent: " + req.useragent);
                        }

                    }
                }
                
            }

            treeView2.EndUpdate();
        }
        private TreeNode TreeAlreadyHas(string s, TreeView trView)
        {
            foreach (TreeNode node in trView.Nodes)
            {
                if (node.Text == s)
                {
                    return node;
                }
            }
            return null;
        }
        private void metroButton2_Click(object sender, EventArgs e)
        {
            Search(metroTextBox1.Text);
        }
    }
}
