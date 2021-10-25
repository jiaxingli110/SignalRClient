using Microsoft.AspNet.SignalR.Client;
using SignalRClient.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Entity;
using ConnectionState = Microsoft.AspNet.SignalR.Client.ConnectionState;

namespace WindowsFormsApp1
{
    public partial class MainForm : Form
    {
        #region SignalR相关变量
        private static HubConnection connection = null;
        private static IHubProxy rhub = null;
        #endregion
        private static System.Security.Principal.WindowsIdentity currentUser = System.Security.Principal.WindowsIdentity.GetCurrent();
        private static string CurrentEid = ConfigurationManager.AppSettings["CurrentUser"];    //eid
        public MainForm()
        {
            InitializeComponent();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + @"\Skins\mp10.ssk";
            Control.CheckForIllegalCrossThreadCalls = false;
            StartListen();

        }
        /// <summary>
        /// 开启SignalR监听
        /// </summary>
        private void StartListen()
        {
            try
            {
                string serverUri = ConfigurationManager.AppSettings["ServerUri"].ToString();
                Dictionary<string, string> dic = new Dictionary<string, string>();
                string eid = currentUser.Name.Substring(currentUser.Name.LastIndexOf("\\") + 1); //ConfigurationManager.AppSettings["CurrentUser"].ToString();
                if (!string.IsNullOrEmpty(CurrentEid))
                {
                    eid = CurrentEid;
                }
                dic.Add("userName", eid);
                connection = new HubConnection(serverUri, dic);
                //类名必须与服务端一致               
                rhub = connection.CreateHubProxy("ServerHub");
                connection.Start();//连接服务器  
                //注册客户端方法名称"addMessage"与服务器端Send方法对应，对应的 callback方法 ReceiveMsg
                rhub.On<List<BusinessBase>>("sendbusinesslist", ReceiveBusinessList);
                rhub.On<string, string>("addMessage", ReceiveMsg);
                rhub.On("focusStop", FocusStop);
                rhub.On<List<BusinessBase>>("sendlistbyid", ReceiveBusinessList);
                Action<StateChange> action = new Action<StateChange>(StateChageWather);
                connection.StateChanged += action;
                //rhub.On("","");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("开启SignalR监听:" + ex.Message.ToString(), ex);
            }
        }
        /// <summary>
        /// 对应的callback方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        private static void ReceiveMsg(string name, string message)
        {
            Thread t1 = new Thread(delegate ()
            {
                Application.Run(new PopupForm(message));
            });
            t1.IsBackground = true; //当主线程退出时，后台线程会被CLR调用Abort()来彻底终止程序
            t1.Start();
        }
        private static void ReceiveBusinessList(List<BusinessBase> list)
        {
            string eid = currentUser.Name.Substring(currentUser.Name.LastIndexOf("\\") + 1);
            LogHelper.WriteLog("当前用户:" + eid);
            if (list!=null && list.Count>0)
            {
                if (!string.IsNullOrEmpty(CurrentEid))
                {
                    eid = CurrentEid;
                    //string[] processIds = new string[2];
                    list = list.Where(x => x.Ops.UserJapId == eid).ToList();
                }
                if (list != null && list.Count > 0)
                {
                    Thread t1 = new Thread(delegate ()
                    {
                        Application.Run(new PopupForm(list));
                    });
                    t1.IsBackground = true; //当主线程退出时，后台线程会被CLR调用Abort()来彻底终止程序
                    t1.Start();
                }
            }
        }
        private void ReceiveMsgByOne(string message)
        {

        }
        private static void FocusStop()
        {
            //Application.Run();
            PopupForm.Instance().CloseForm();
        }
        //private void SetStatusTxt(string str)
        //{
        //    if (statusLabel.InvokeRequired)
        //    {
        //        Action<string> listbox = (x) => { this.listBox1.Items.Add(message); };
        //        listBox1.Invoke(listbox, message);
        //    }
        //}
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendBtn_Click(object sender, EventArgs e)
        {
            string m = textBox1.Text;
            string id = connection.ConnectionId;
            //调用 hub中的方法 Send
            rhub.Invoke("Send", id, m).Wait();
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startBtn_Click(object sender, EventArgs e)
        {
            string m = textBox1.Text;
            string id = connection.ConnectionId;
            //调用 hub中的方法 Send
            rhub.Invoke("Send", id, m).Wait();
        }
        /// <summary>
        /// 还原窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //判断是否已经最小化于托盘 
            if (WindowState == FormWindowState.Minimized)
            {
                //还原窗体显示 
                WindowState = FormWindowState.Normal;
                //激活窗体并给予它焦点 
                this.Activate();
            }
        }      
        /// <summary>
        /// 状态监控
        /// </summary>
        /// <param name="state"></param>
        private void StateChageWather(StateChange state)
        {
            if (state.NewState== ConnectionState.Connected)
            {
                this.statusLabel.Text = "客户端已连接";
            }
            else
            {
                this.statusLabel.Text = "客户端未连接";
            }
        }
        private void btn_ReConnect_Click(object sender, EventArgs e)
        {
            if (connection.State != ConnectionState.Connected)
            {
                StartListen();
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}
