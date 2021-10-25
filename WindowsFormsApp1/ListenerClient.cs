using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsFormsApp1.Entity;

namespace WindowsFormsApp1
{
    public class ListenerClient
    {
        public ListenerClient()
        {
            StartListen();
        }
        HubConnection connection = null;
        IHubProxy rhub = null;
        public void StartListen()
        {
            var serverUri = ConfigurationManager.AppSettings["ServerUri"].ToString();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("userName", "client1");
            connection = new HubConnection(serverUri, dic);
            //类名必须与服务端一致
            //myHub = connection.CreateHubProxy("BroadcastHub");                  
            rhub = connection.CreateHubProxy("ServerHub");
            connection.Start();//连接服务器  
            //注册客户端方法名称"addMessage"与服务器端Send方法对应，对应的 callback方法 ReceiveMsg
            rhub.On<List<TimeCard>>("sendtimecardlist", ReceiveCardList);
            //rhub.On<string>("addMessage", GetMsg);
        }
        private void ReceiveCardList(List<TimeCard> list)
        {
            List<string> messageLst = list.Select(x => x.UserName).ToList();
            //new PopupForm(messageLst);
        }
    }
}
