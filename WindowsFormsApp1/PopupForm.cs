using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using SignalRClient.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Entity;

namespace WindowsFormsApp1
{
    public partial class PopupForm : Form
    {
        #region 声明的变量
        private System.Drawing.Rectangle Rect;//定义一个存储矩形框的数组
        private FormState InfoStyle = FormState.Hide;//定义变量为隐藏
        static private PopupForm dropDownForm = new PopupForm("");//实例化当前窗体
        private static int AW_HIDE = 0x00010000; //该变量表示动画隐藏窗体
        private static int AW_SLIDE = 0x00040000;//该变量表示出现滑行效果的窗体
        private static int AW_VER_NEGATIVE = 0x00000008;//该变量表示从下向上开屏
        private static int AW_VER_POSITIVE = 0x00000004;//该变量表示从上向下开屏
        #endregion
        private static int duration =  Convert.ToInt32(ConfigurationManager.AppSettings["CloseDuration"]) ;    //弹出窗口关闭时长
        private static string processIds = ConfigurationManager.AppSettings["ProcessesId"];
        public PopupForm()
        {
            InitializeComponent();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + @"\Skins\PageColor2.ssk";//
            //初始化工作区大小
            System.Drawing.Rectangle rect = System.Windows.Forms.Screen.GetWorkingArea(this);//实例化一个当前窗口的对象
            this.Rect = new System.Drawing.Rectangle(rect.Right - this.Width - 1, rect.Bottom - this.Height - 1, this.Width, this.Height);//为实例化的对象创建工作区域        
            //this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            ShowForm();
        }
        public PopupForm(string message):this()
        {
            noticeLabel.Text = message;
        }
        
        public PopupForm(List<BusinessBase> businessLst) : this()
        {
            if (businessLst!=null && businessLst.Count>0)
            {
                //REMAIN TO BE DONE
                //目前StepName只涉及了Entry、QC、NC, 实际流程中还会有
                //Ariba拒否、日本QC、Ariba登录、请求入力承认、大连QC
                int ncCount1 = businessLst.Where(x => x.StepName == "NC" && x.ProcessId == "164").Count();
                int ncCount2 = businessLst.Where(x => x.StepName == "NC" && x.ProcessId == "187").Count();
                int ncCount3 = businessLst.Where(x => x.StepName == "NC" && x.ProcessId == "188").Count();
                int ncCount4 = businessLst.Where(x => x.StepName == "NC" && x.ProcessId == "190").Count();
                int qcCount1 = businessLst.Where(x => x.StepName == "QC" && x.ProcessId == "164").Count();
                int qcCount2 = businessLst.Where(x => x.StepName == "QC" && x.ProcessId == "187").Count();
                int qcCount3 = businessLst.Where(x => x.StepName == "QC" && x.ProcessId == "188").Count();
                int qcCount4 = businessLst.Where(x => x.StepName == "QC" && x.ProcessId == "190").Count();
                int entryCount1 = businessLst.Where(x => x.StepName == "Entry" && x.ProcessId == "164").Count();
                int entryCount2 = businessLst.Where(x => x.StepName == "Entry" && x.ProcessId == "187").Count();
                int entryCount3 = businessLst.Where(x => x.StepName == "Entry" && x.ProcessId == "188").Count();
                int entryCount4 = businessLst.Where(x => x.StepName == "Entry" && x.ProcessId == "190").Count();
                nclabel1.Text = ncCount1.ToString();
                nclabel2.Text = ncCount2.ToString();
                nclabel3.Text = ncCount3.ToString();
                nclabel4.Text = ncCount4.ToString();
                qclabel1.Text = qcCount1.ToString();
                qclabel2.Text = qcCount2.ToString();
                qclabel3.Text = qcCount3.ToString();
                qclabel4.Text = qcCount4.ToString();
                entryLabel1.Text = entryCount1.ToString();
                entryLabel2.Text = entryCount2.ToString();
                entryLabel3.Text = entryCount3.ToString();
                entryLabel4.Text = entryCount4.ToString();
            }
        }
        #region 调用API函数显示窗体
        [DllImportAttribute("user32.dll")]
        private static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);
        #endregion
        #region 定义标识窗体移动状态的枚举值
        protected enum FormState
        {
            //隐藏窗体
            Hide = 0,
            //显示窗体
            Display = 1,
            //显示窗体中
            Displaying = 2,
            //隐藏窗体中
            Hiding = 3
        }
        #endregion
        #region 用属性标识当前状态
        protected FormState FormNowState
        {
            get { return this.InfoStyle; }   //返回窗体的当前状态
            set { this.InfoStyle = value; }  //设定窗体当前状态的值
        }
        #endregion
        #region 显示窗体
        public void ShowForm()
        {
            switch (this.FormNowState)
            {
                case FormState.Hide:
                    if (this.Height <= this.Rect.Height - 192)//当窗体没有完全显示时
                        this.SetBounds(Rect.X, this.Top - 192, Rect.Width, this.Height + 192);//使窗体不断上移
                    else
                    {
                        this.SetBounds(Rect.X-9, Rect.Y-35, Rect.Width, Rect.Height);//设置当前窗体的边界
                    }         
                    AnimateWindow(this.Handle, 800, AW_SLIDE + AW_VER_NEGATIVE);//动态显示本窗体
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                    //SetWindowPos(Handle, new IntPtr(-1), Rect.X - 9, Rect.Y - 35, Rect.Width, Rect.Height, 0);
                    this.TopMost = true;
                    break;
            }
            Thread t1 = new Thread(delegate ()
            {
                Thread.Sleep(duration);
                this.Close();
            });
            t1.IsBackground = true;
            t1.Start();
        }
        #endregion
        #region 关闭窗体
        public void CloseForm()
        {
            AnimateWindow(this.Handle, 800, AW_SLIDE + AW_VER_POSITIVE + AW_HIDE); //动画隐藏窗体
            this.FormNowState = FormState.Hide;//设定当前窗体的状态为隐藏
        }
        #endregion

        #region 返回当前窗体的实例化对象
        static public PopupForm Instance()
        {
            return dropDownForm; //返回当前窗体的实例化对象
        }
        #endregion
        ///// <summary>
        ///// 根据数据动态加载窗体组件
        ///// </summary>
        ///// <param name="businessLst"></param>
        //private void AddWidget(List<BusinessBase> businessLst)
        //{
        //    //获取流程个数
        //    List<BusinessBase> proTypeList = businessLst.Where((p,i)=> businessLst.FindIndex(z=>z.ProcessId==p.ProcessId)==i).ToList();
        //    int processCount = proTypeList == null ? 0 : proTypeList.Count;
        //    //获取步骤个数
        //    List<string> processIdLst = processIds.Split(',').ToList();
        //    int stepCount = 0;
        //    foreach (var item in processIdLst)
        //    {
        //        int? currentCount = businessLst.Select(s => s.StepName)?.Distinct()?.Count();
        //        if (currentCount > stepCount)
        //        {
        //            stepCount = Convert.ToInt32(currentCount);           
        //        }
        //    }
        //    //设置窗体大小
        //    int width = 1;
        //    int height = 39 + 93 * processCount;
        //    this.Size = new Size(width , height);

           
        //    var x = businessLst.GroupBy(p=>p.ProcessId);

        //    List<BusinessBase> lst = new List<BusinessBase>();

        //    GroupBox groupBox1 = new System.Windows.Forms.GroupBox();
        //    groupBox1.SuspendLayout();
        //    // 
        //    // entryLabel1
        //    // 
        //    Label entryLabel1 = new Label();
        //    entryLabel1.AutoSize = true;
        //    entryLabel1.BackColor = System.Drawing.SystemColors.Window;
        //    entryLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        //    entryLabel1.Location = new System.Drawing.Point(76, 23);
        //    entryLabel1.Name = "entryLabel1";
        //    entryLabel1.TabIndex = 4;
        //    entryLabel1.Text = "0";
        //    // 
        //    // nclabel1
        //    // 
        //    Label nclabel1 = new Label();
        //    nclabel1.AutoSize = true;
        //    nclabel1.BackColor = System.Drawing.SystemColors.Window;
        //    nclabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        //    nclabel1.Location = new System.Drawing.Point(162, 23);
        //    nclabel1.Name = "nclabel1";
        //    nclabel1.TabIndex = 3;
        //    nclabel1.Text = "0";
        //    // 
        //    // qclabel1
        //    // 
        //    Label qclabel1 = new Label();
        //    qclabel1.AutoSize = true;
        //    qclabel1.BackColor = System.Drawing.SystemColors.Window;
        //    qclabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        //    qclabel1.Location = new System.Drawing.Point(253, 23);
        //    qclabel1.Name = "qclabel1";
        //    qclabel1.TabIndex = 5;
        //    qclabel1.Text = "0";
        //    // 
        //    // label3
        //    //
        //    Label label3 = new Label();
        //    label3.AutoSize = true;
        //    label3.BackColor = System.Drawing.SystemColors.Window;
        //    label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        //    label3.Location = new System.Drawing.Point(205, 23);
        //    label3.Name = "label3";
        //    label3.TabIndex = 2;
        //    label3.Text = "QC：";
        //    // 
        //    // label2
        //    // 
        //    Label label2 = new Label();
        //    label2.AutoSize = true;
        //    label2.BackColor = System.Drawing.SystemColors.Window;
        //    label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        //    label2.Location = new System.Drawing.Point(115, 23);
        //    label2.Name = "label2";
        //    label2.TabIndex = 0;
        //    label2.Text = "NC：";
        //    // 
        //    // lable1
        //    // 
        //    Label lable1 = new Label();
        //    lable1.AutoSize = true;
        //    lable1.BackColor = System.Drawing.SystemColors.Window;
        //    lable1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        //    lable1.Location = new System.Drawing.Point(18, 23);
        //    lable1.Name = "lable1";
        //    lable1.TabIndex = 1;
        //    lable1.Text = "Entry：";
        //    groupBox1.Controls.Add(lable1);
        //    groupBox1.Controls.Add(label2);
        //    groupBox1.Controls.Add(label3);
        //    groupBox1.Controls.Add(qclabel1);
        //    groupBox1.Controls.Add(nclabel1);
        //    groupBox1.Controls.Add(entryLabel1);
        //    groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        //    groupBox1.Location = new System.Drawing.Point(18, 13);
        //    groupBox1.Name = "groupBox1";
        //    groupBox1.Size = new System.Drawing.Size(283, 55);
        //    groupBox1.TabIndex = 22;
        //    groupBox1.TabStop = false;
        //    groupBox1.Text = "饮料";
        //    this.Controls.Add(groupBox1);
        //    groupBox1.ResumeLayout(false);
        //    groupBox1.PerformLayout();
        //    foreach (var item in proTypeList)
        //    {

        //    }
        //    businessLst.Select(p => p.ProcessId);
        //}


    }
}
