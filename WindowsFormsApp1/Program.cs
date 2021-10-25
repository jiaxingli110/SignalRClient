using Microsoft.AspNet.SignalR.Client;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Entity;

namespace WindowsFormsApp1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {    
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AutoStart();
            //Application.Run();
            // StartListen();
            Application.Run(new MainForm());
        }
        /// <summary>
        /// 修改程序在注册表中的键值
        /// </summary>
        private static void AutoStart()
        {
            try
            {
                bool isAuto = Convert.ToBoolean(ConfigurationManager.AppSettings["iSStartAuto"]);
                if (isAuto == true)
                {
                    RegistryKey R_local = Registry.CurrentUser;//RegistryKey R_local = Registry.CurrentUser;
                    RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    R_run.SetValue("SignalRClient", Application.ExecutablePath);
                    R_run.Close();
                    R_local.Close();
                }
                else
                {
                    RegistryKey R_local = Registry.CurrentUser;//RegistryKey R_local = Registry.CurrentUser;
                    RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    R_run.DeleteValue("SignalRClient", false);
                    R_run.Close();
                    R_local.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("开机启动注册异常:",ex);
            }
        }
        ///// <summary>
        ///// 开启SignalR监听
        ///// </summary>
        //private static void StartListen()
        //{
        //    try
        //    {
        //        string serverUri = ConfigurationManager.AppSettings["ServerUri"].ToString();
        //        Dictionary<string, string> dic = new Dictionary<string, string>();
        //        string currentUser = ConfigurationManager.AppSettings["CurrentUser"].ToString();
        //        dic.Add("userName", currentUser);
        //        connection = new HubConnection(serverUri, dic);
        //        //类名必须与服务端一致               
        //        rhub = connection.CreateHubProxy("ServerHub");
        //        connection.Start();//连接服务器  
        //        //注册客户端方法名称"addMessage"与服务器端Send方法对应，对应的 callback方法 ReceiveMsg
        //        // rhub.On<List<TimeCard>>("sendtimecardlist", ReceiveCardList);
        //        rhub.On<string, string>("addMessage", ReceiveMsg);
        //        rhub.On("focusStop", FocusStop);
        //        //rhub.On("","");
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.WriteLog("开启SignalR监听:"+ ex.Message.ToString(), ex);
        //    }
        //}
        ///// <summary>
        ///// 对应的callback方法
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="message"></param>
        //private static void ReceiveMsg(string name, string message)
        //{
        //    Thread t1 = new Thread(delegate ()
        //    {
        //        Application.Run(new PopupForm(message)); 
        //    });
        //    t1.IsBackground = true; //当主线程退出时，后台线程会被CLR调用Abort()来彻底终止程序
        //    t1.Start();
        //}
        //private static void ReceiveCardList(List<TimeCard> list)
        //{
        //    Thread t1 = new Thread(delegate ()
        //    {
        //        Application.Run(new PopupForm(list));
        //    });
        //    t1.IsBackground = true; //当主线程退出时，后台线程会被CLR调用Abort()来彻底终止程序
        //    t1.Start();
        //}
        //private static void FocusStop()
        //{
        //    //Application.Run();
        //    PopupForm.Instance().CloseForm();
        //}
    }
}
