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

        [DllImport("User32.dll", EntryPoint = "LockWorkStation")]   //
        private static extern bool LockWorkStation();

        private const int MOUSEEVENTF_MOVE = 0x0001;

        // 上一位置
        private int last_mouse_x = 0;
        private int last_mouse_y = 0;
        private long last_mouse_timepoint = 0;

        // 锁屏定时器 (分)
        private int screen_lock_time = 30;

        public Form1()
        {
            InitializeComponent();
            this.screen_lock_time = Properties.Settings.Default.screen_lock_time;
            this.设置锁屏时间ToolStripMenuItem.Text = "设置锁屏时间(" + this.screen_lock_time.ToString() + ")";

            Run();
        }
        public bool activeState = true;
        private void Run()
        {
            int time_interval1 = 30;

            System.Timers.Timer timer1 = new System.Timers.Timer(); // 定义定时器
            timer1.Enabled = true; // 启用定时器
            timer1.Interval = 1000 * time_interval1; // 设定时间间隔（毫秒：1秒 = 1000毫秒）
            timer1.Elapsed += new ElapsedEventHandler(MoveMouseHandler); // 等到间隔时间到，执行

            System.Timers.Timer timer2 = new System.Timers.Timer();
            timer2.Enabled = true;
            timer2.Interval = 1000;
            timer2.Elapsed += new ElapsedEventHandler(CheckActive);
        }
        private void MoveMouseHandler(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.activeState)
            {
                // 将鼠标移动一个像素
                mouse_event(MOUSEEVENTF_MOVE, 1, 1, 0, 0);
                mouse_event(MOUSEEVENTF_MOVE, -1, -1, 0, 0);
            }
        }
        private void CheckActive(object sender, System.Timers.ElapsedEventArgs e)
        {
            last_mouse_timepoint++;

            int x = Control.MousePosition.X;
            int y = Control.MousePosition.Y;
            if (x != last_mouse_x || y != last_mouse_y)
            {
                last_mouse_x = x;
                last_mouse_y = y;
                last_mouse_timepoint = 0;
            }
            else if (last_mouse_timepoint > screen_lock_time * 60) 
            {
                last_mouse_timepoint = 0;
                LockWorkStation(); // 锁屏
                //MessageBox.Show("Test:"+ DateTime.Now.ToFileTimeUtc().ToString() + "\n" + last_mouse_timepoint);
            }
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
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

        private void ToolMenuCancel_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void ActiveSwich_Click(object sender, EventArgs e)
        {
            this.activeState = !this.activeState;
            ((ToolStripMenuItem)sender).Text = this.activeState ? "取消激活" : "激活";
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void ToolMenuSetLocktime_Click (object sender, EventArgs e)
        {
            this.numericUpDown1.Value = Properties.Settings.Default.screen_lock_time;
            this.Show();
        }

        private void confirm_button_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.screen_lock_time = (int)numericUpDown1.Value;
            Properties.Settings.Default.Save();
            this.screen_lock_time = Properties.Settings.Default.screen_lock_time;
            this.设置锁屏时间ToolStripMenuItem.Text = "设置锁屏时间(" + this.screen_lock_time.ToString() + ")";

            this.Hide();
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            last_mouse_timepoint = 0;
        }
    }
}
