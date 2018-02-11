using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Timers;

namespace KeepScreen
{
    public partial class Form1 : Form
    {
        [DllImport("user32")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]   //找子窗体   
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]   //用于发送信息给窗体   
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport("User32.dll", EntryPoint = "ShowWindow")]   //
        private static extern bool ShowWindow(IntPtr hWnd, int type);

        private const int MOUSEEVENTF_MOVE = 0x0001;


        public Form1()
        {
            InitializeComponent();
            Run();
        }
        public bool activeState = true;
        private void Run()
        {
            int TimerInterval = 30;

            System.Timers.Timer timer = new System.Timers.Timer(); // 定义定时器
            timer.Enabled = true; // 启用定时器
            timer.Interval = 1000 * TimerInterval; // 设定时间间隔（毫秒：1秒 = 1000毫秒）
            //timer.Interval = TimerInterval; // Test

            timer.Elapsed += new ElapsedEventHandler(moveMouseHandler); // 等到间隔时间到，执行

        }
        private void moveMouseHandler(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.activeState)
            {
                // 将鼠标移动一个像素
                mouse_event(MOUSEEVENTF_MOVE, 1, 1, 0, 0);
                mouse_event(MOUSEEVENTF_MOVE, -1, -1, 0, 0);
            }
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button) {
                case MouseButtons.Left:
                case MouseButtons.Right:
                    notifyMenu.Show();
                    break;
                default:
                    break;
            }
        }

        private void toolMenuCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void activeSwich_Click(object sender, EventArgs e)
        {
            this.activeState = !this.activeState;
            ((ToolStripMenuItem)sender).Text = this.activeState ? "取消激活" : "激活";
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
