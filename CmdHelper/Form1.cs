using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CmdHelper
{
    public partial class Form1 : Form
    {
        public class StartInfo
        {
            public string Name;
            public string Args;
        }

        int mCurrentIndex = 0;
        Process mCurrent;

        public Form1()
        {
            InitializeComponent();
            InitCmdList();

            this.DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }

        public void InitCmdList()
        {
            StartInfo[] cmds = new StartInfo[]
            {
             new StartInfo(){Name="D:/sdk/adt-bundle-windows-x86_64-20130917/adt-bundle-windows-x86_64-20130917/sdk/platform-tools/adb",Args="devices",            },
             new StartInfo(){Name="D:/sdk/adt-bundle-windows-x86_64-20130917/adt-bundle-windows-x86_64-20130917/sdk/platform-tools/adb",Args="tcpip 5555",            },
             new StartInfo(){Name="D:/sdk/adt-bundle-windows-x86_64-20130917/adt-bundle-windows-x86_64-20130917/sdk/platform-tools/adb",Args="connect 192.168.1.102", },
             new StartInfo(){Name="D:/sdk/adt-bundle-windows-x86_64-20130917/adt-bundle-windows-x86_64-20130917/sdk/platform-tools/adb",Args="start-server",      },
             new StartInfo(){Name="D:/sdk/adt-bundle-windows-x86_64-20130917/adt-bundle-windows-x86_64-20130917/sdk/platform-tools/adb",Args="kill-server",       },
             new StartInfo(){Name="D:/sdk/adt-bundle-windows-x86_64-20130917/adt-bundle-windows-x86_64-20130917/sdk/platform-tools/adb",Args="logcat",                },
             new StartInfo(){Name="D:/sdk/adt-bundle-windows-x86_64-20130917/adt-bundle-windows-x86_64-20130917/sdk/platform-tools/adb",Args="logcat -c",             },
             new StartInfo(){Name="D:/sdk/adt-bundle-windows-x86_64-20130917/adt-bundle-windows-x86_64-20130917/sdk/platform-tools/adb",Args="logcat -d > logcat.txt",},
            };
            foreach(var cmd in cmds)
            {
                var node = this.cmdList.Nodes.Add(cmd.Args);
                node.Tag = cmd;
            }
        }
        //process.WaitForExit();
        void Clear()
        {
            if (mCurrent != null)
            {
                mCurrent.ErrorDataReceived -= OnDataReceived;
                mCurrent.OutputDataReceived -= OnDataReceived;
                mCurrent.EnableRaisingEvents = false;
            }
        }
        private void OnDoubleClick(object sender, EventArgs e)
        {
            Clear();

            TreeNode node = this.cmdList.SelectedNode;
        
            StartInfo info = (StartInfo)node.Tag;
            mCurrent = new Process();
            mCurrent.StartInfo = new ProcessStartInfo();
            mCurrent.StartInfo.FileName = info.Name;
            mCurrent.StartInfo.Arguments = info.Args;
            mCurrent.StartInfo.CreateNoWindow = true;
            mCurrent.StartInfo.RedirectStandardError = true;
            mCurrent.StartInfo.RedirectStandardInput = true;
            mCurrent.StartInfo.RedirectStandardOutput = true;
            mCurrent.StartInfo.UseShellExecute = false;
            mCurrent.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            mCurrent.EnableRaisingEvents = true;
            mCurrent.ErrorDataReceived += OnDataReceived;
            mCurrent.OutputDataReceived += OnDataReceived;
            mCurrent.Start();
            SetText(mCurrent.StartInfo.FileName + " " + mCurrent.StartInfo.Arguments);
            mCurrent.BeginOutputReadLine();
        }
        
        delegate void SetTextCallBack(string text);

        public void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                SetText(e.Data);
            }
        }
        public void SetText(string text)
        {
            if (this.console.InvokeRequired)
            {
                SetTextCallBack stcb = new SetTextCallBack(SetText);
                this.Invoke(stcb, new object[] { text });
            }
            else
            {
                if (!string.IsNullOrEmpty(text))
                    this.console.AppendText(text + "\r\n");
                
                bool lockScrollBar = this.checkBox1.Checked;
                
                if (!lockScrollBar)
                {
                    console.SelectionStart = console.Text.Length;
                    console.ScrollToCaret(); 
                }
                else
                {
                    console.SelectionStart = mCurrentIndex;
                    console.ScrollToCaret();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.console.Clear();
        }

        private void mouseup(object sender, MouseEventArgs e)
        {
            int idx = this.console.GetCharIndexFromPosition(new Point(e.X, e.Y));
            mCurrentIndex = idx;
            console.SelectionStart = mCurrentIndex;
            console.ScrollToCaret();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            this.Clear();
        }
    }
}
