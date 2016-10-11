using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using IService;
using System.Threading.Tasks;
using log4net;


namespace ServiceManageWin
{
    public partial class MainForm : Form, IService.IForm
    {
        public MainForm()
        {
            InitializeComponent();
            string iconFile = Util.GetAppSetting("Icon");
            if (!string.IsNullOrWhiteSpace(iconFile))
            {
                this.Icon = new Icon(iconFile);
                this.notifyIcon1.Icon = new Icon(iconFile);
            }

            this.Text = Util.GetAppSetting("Title");
            notifyIcon1.BalloonTipText = this.Text;
            notifyIcon1.Text = this.Text;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            WriteToConsole("发生了未处理的异常：", "error");
            WriteToConsole(sender.ToString(), "error");
            WriteToConsole(((Exception)e.ExceptionObject).ToString(), "error");
            if (e.IsTerminating)
            {
                //重启程序
                //System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
                System.Windows.Forms.Application.Restart(); //重启当前程序
            }
        }
        #region
        private List<ServiceProp> ServiceList = new List<ServiceProp>();
        private List<ServiceManager> ServiceManagerList = new List<ServiceManager>();
        #endregion
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                Task task = new Task(() => { LoadSerivce(); });
                task.Start();
            }
            catch (Exception ex)
            {
                WriteToConsole("启动失败！" + ex.Message, "error");
            }
        }

        #region　初始化
        private List<ServiceProp> GetServiceList()
        {
            try
            {
                var list = (List<ServiceProp>)XMLSerializer.LoadFromXml("service.config", typeof(List<ServiceProp>));
                return list;
            }
            catch (Exception ex)
            {
                MessageBox.Show("服务配置不正确！");
                throw ex;
            }
        }

        /// <summary>
        /// 加载项到界面
        /// </summary>
        /// <param name="list"></param>
        private void LoadServiceList(List<ServiceProp> list)
        {
            try
            {
                foreach (var s in list)
                {
                    AddServiceItem(s);
                }
            }
            catch (Exception ex)
            {
                WriteToConsole("加载服务列表发生错误", "error");
                WriteToConsole(ex.ToString(), "error");
                MessageBox.Show("加载服务列表发生错误");
            }
        }


        private void LoadSerivce()
        {
            try
            {
                ServiceList = GetServiceList();

                LoadServiceList(ServiceList);
                foreach (var serviceProp in ServiceList)
                {
                    if (serviceProp.Start)
                    {
                        CallStartService(serviceProp.Code);
                        //StartServiceDomain(serviceProp);
                    }
                }
            }
            catch { }
        }

        public void RefreshServiceList()
        {
            try
            {
                var list = GetServiceList();

                var newList = list.Where(sp => !ServiceList.Any(os => os.Code == sp.Code)).ToList();
                if (newList.Count > 0)
                {
                    ServiceList = list;
                    foreach (var serviceProp in newList)
                    {
                        AddServiceItem(serviceProp);

                        if (serviceProp.Start)
                        {
                            CallStartService(serviceProp.Code);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("没有发现新的服务！");
                }

            }
            catch (Exception ex)
            {
                WriteToConsole("刷新服务时发生错误", "error");
                WriteToConsole(ex.ToString(), "error");
            }
        }

        #endregion

        #region 公共方法
        /// <summary>
        /// 异步call
        /// </summary>
        /// <param name="serviceCode"></param>
        public void CallStartService(string serviceCode)
        {
            Task t = new Task(delegate() { _StartService(serviceCode); });
            t.Start();
        }
        /// <summary>
        /// 异步call
        /// </summary>
        /// <param name="serviceCode"></param>
        public void CallStopService(string serviceCode)
        {
            Task t = new Task(delegate() { _StopService(serviceCode); });
            t.Start();
        }

        public void CallRestartService(string serviceCode)
        {
            Task t = new Task(delegate() { _StopService(serviceCode); _StartService(serviceCode); });
            t.Start();
        }

        /// <summary>
        /// 输出信息到tab
        /// </summary>
        /// <param name="serviceCode"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public void WriteLog(string serviceCode, string message, string type, DateTime time)
        {
            try
            {
                //写日志文件
                var manager = ServiceManagerList.FirstOrDefault(m => m.ServiceProp.Code == serviceCode);
                if (manager != null)
                {
                    if (type == "test")
                    {
                        manager.Logger.Debug(message);
                    }
                    else if (type == "error")
                    {
                        manager.Logger.Fatal(message);
                    }
                    else
                    {
                        manager.Logger.Info(message);
                    }
                }
                TextBox serviceInfo = (TextBox)this.tab.Controls["tab" + serviceCode].Controls["txt" + serviceCode];

                WriteToTextBox(serviceInfo, message, type, time);

                if (type == "error")
                {
                    WriteToConsole("服务" + serviceCode + "发生错误", "error");
                    WriteToConsole(message, "error");
                }
            }
            catch (Exception ex) { }
        }
        public static ILog Log = Util.GetLogger("console");
        public void WriteToConsole(string message, string type)
        {
            if (type == "test")
            {
                Log.Debug(message);
            }
            else if (type == "error")
            {
                Log.Fatal(message);
            }
            else
            {
                Log.Info(message);
            }
            WriteToTextBox(consoleInfo, message, type, DateTime.Now);
        }


        #endregion




        #region 基础方法
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="serviceCode"></param>
        private void _StopService(string serviceCode)
        {
            var manager = ServiceManagerList.FirstOrDefault(m => m.ServiceProp.Code == serviceCode);

            try
            {
                if (manager == null)
                {
                    return;
                }
                SetServiceStatus(serviceCode, "正在停止服务。。");
                WriteToConsole("正在停止服务" + manager.ServiceProp.Name, "info");
                try
                {
                    manager.Service.StopService();
                }
                catch (Exception ex)
                {
                    WriteToConsole("服务" + manager.ServiceProp.Name + "调用StopService时发生错误", "info");
                    WriteToConsole(ex.ToString(), "error");
                }
                WriteToConsole("正在卸载程序集 " + manager.ServiceProp.Name, "info");
                manager.ServiceLoader.Unload();
                WriteToConsole("卸载程序集完成 " + manager.ServiceProp.Name, "info");

                SetServiceStatus(serviceCode, "停止");
                WriteToConsole("服务" + manager.ServiceProp.Name + "已停止", "info");
                ServiceManagerList.Remove(manager);
                manager = null;
                System.GC.Collect();
            }
            catch (Exception ex)
            {
                WriteToConsole("停止服务时发生错误:" + manager.ServiceProp.Name, "error");
                WriteToConsole(ex.ToString(), "error");
                SetServiceStatus(serviceCode, "停止服务时发生错误");
            }
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="serviceCode"></param>
        private void _StartService(string serviceCode)
        {
            try
            {
                var manager = ServiceManagerList.FirstOrDefault(m => m.ServiceProp.Code == serviceCode);
                if (manager != null)
                {
                    return;
                }
                var serviceProp = ServiceList.First(s => s.Code == serviceCode);
                _StartServiceDomain(serviceProp);
            }
            catch (Exception ex)
            {
                WriteToConsole("启动服务时发生错误:" + serviceCode, "error");
                WriteToConsole(ex.ToString(), "error");
                SetServiceStatus(serviceCode, "启动服务时发生错误");
            }
        }
        /// <summary>
        /// 启动服务域
        /// </summary>
        /// <param name="serviceProp"></param>
        private void _StartServiceDomain(ServiceProp serviceProp)
        {
            try
            {
                SetServiceStatus(serviceProp.Code, "正在启动。。");
                WriteToConsole("正在启动服务！" + serviceProp.Name, "info");

                IService.Host serviceHost = new Host(this, serviceProp);

                //创建AppDomain 隔离 服务
                ServiceManager manager = new ServiceManager();
                manager.Logger = Util.GetLogger(serviceProp);
                manager.ServiceProp = serviceProp;
                manager.ServiceLoader = new ServiceLoader.ServiceLoader(serviceHost);
                manager.ServiceLoader.LoadAssembly(serviceProp);
                manager.Service = (IService.IService)manager.ServiceLoader.GetInstance(serviceProp.ClassName);

                //创建新的标签页
                CreateTabPage(serviceProp);

                try
                {
                    manager.Service.StartService(serviceHost);
                    SetServiceStatus(serviceProp.Code, "启动");
                    WriteToConsole("服务" + serviceProp.Name + "已启动", "info");
                }
                catch (Exception ex0)
                {
                    WriteToConsole("启动服务时发生错误:" + serviceProp.Name, "error");
                    WriteToConsole(ex0.ToString(), "error");
                    SetServiceStatus(serviceProp.Code, "启动时发生错误");
                }

                ServiceManagerList.Add(manager);
            }
            catch (Exception ex)
            {
                WriteToConsole("启动服务时发生错误:" + serviceProp.Name, "error");
                WriteToConsole(ex.ToString(), "error");
                SetServiceStatus(serviceProp.Code, "启动时发生错误");
            }
        }



        #endregion

        #region 界面异步操作
        /// <summary>
        /// 向服务列表中添加一项
        /// </summary>
        /// <param name="s"></param>
        private void AddServiceItem(ServiceProp s)
        {
            if (listView1.InvokeRequired)
            {
                Action actionDelegate = () =>
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.SubItems[0].Text = s.Code;
                    lvi.SubItems.Add(s.Name);
                    lvi.SubItems.Add(" ");
                    lvi.SubItems.Add(" ");
                    listView1.Items.Add(lvi);
                };
                listView1.BeginInvoke(actionDelegate);
            }
            else
            {
                ListViewItem lvi = new ListViewItem();
                lvi.SubItems[0].Text = s.Code;
                lvi.SubItems.Add(s.Name);
                lvi.SubItems.Add(" ");
                lvi.SubItems.Add(" ");
                listView1.Items.Add(lvi);
            }
        }
        /// <summary>
        /// 创建标签页
        /// </summary>
        /// <param name="serviceProp"></param>
        private void CreateTabPage(ServiceProp serviceProp)
        {
            try
            {
                //如果已存在就不再重新创建
                if (tab.TabPages["tab" + serviceProp.Code] != null)
                {
                    return;
                }
                //创建新的标签页
                if (tab.InvokeRequired)
                {
                    Action actionDelegate = () =>
                    {
                        //创建textbox
                        System.Windows.Forms.TextBox serviceInfo = new TextBox();
                        serviceInfo.BackColor = System.Drawing.SystemColors.ControlLightLight;
                        serviceInfo.Dock = System.Windows.Forms.DockStyle.Fill;
                        serviceInfo.Location = new System.Drawing.Point(3, 3);
                        serviceInfo.Multiline = true;
                        serviceInfo.Name = "txt" + serviceProp.Code;
                        serviceInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;

                        serviceInfo.ReadOnly = true;
                        serviceInfo.Size = new System.Drawing.Size(624, 455);
                        //创建tablepage
                        TabPage tabService = new TabPage();

                        tabService.Controls.Add(serviceInfo);
                        tabService.Location = new System.Drawing.Point(4, 22);
                        tabService.Name = "tab" + serviceProp.Code;
                        tabService.Padding = new System.Windows.Forms.Padding(3);
                        tabService.Size = new System.Drawing.Size(630, 461);
                        tabService.Text = serviceProp.Name;
                        tabService.UseVisualStyleBackColor = true;

                        this.tab.Controls.Add(tabService);
                    };
                    tab.BeginInvoke(actionDelegate);
                }
                else
                {
                    //创建textbox
                    System.Windows.Forms.TextBox serviceInfo = new TextBox();
                    serviceInfo.BackColor = System.Drawing.SystemColors.ControlLightLight;
                    serviceInfo.Dock = System.Windows.Forms.DockStyle.Fill;
                    serviceInfo.Location = new System.Drawing.Point(3, 3);
                    serviceInfo.Multiline = true;
                    serviceInfo.Name = "txt" + serviceProp.Code;
                    serviceInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;

                    serviceInfo.ReadOnly = true;
                    serviceInfo.Size = new System.Drawing.Size(624, 455);
                    //创建tablepage
                    TabPage tabService = new TabPage();

                    tabService.Controls.Add(serviceInfo);
                    tabService.Location = new System.Drawing.Point(4, 22);
                    tabService.Name = "tab" + serviceProp.Code;
                    tabService.Padding = new System.Windows.Forms.Padding(3);
                    tabService.Size = new System.Drawing.Size(630, 461);
                    tabService.Text = serviceProp.Name;
                    tabService.UseVisualStyleBackColor = true;

                    this.tab.Controls.Add(tabService);
                }
            }
            catch (Exception ex)
            {
                WriteToConsole("创建服务标签页时发生错误：", "error");
                WriteToConsole(ex.ToString(), "error");
            }
        }

        /// <summary>
        /// 输出信息
        /// </summary>
        /// <param name="txtbox"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        private void WriteToTextBox(System.Windows.Forms.TextBox txtbox, string message, string type, DateTime time)
        {
            try
            {
                if (txtbox.InvokeRequired)
                {
                    Action actionDelegate = () =>
                    {
                        string info = time + "\t" + type + "\t" + message;
                        txtbox.AppendText(info);
                        txtbox.AppendText(Environment.NewLine);
                        txtbox.AppendText(Environment.NewLine);
                        txtbox.ScrollToCaret();
                    };
                    txtbox.BeginInvoke(actionDelegate);
                }
                else
                {
                    string info = time + "\t" + type + "\t" + message;
                    txtbox.AppendText(info);
                    txtbox.AppendText(Environment.NewLine);
                    txtbox.AppendText(Environment.NewLine);
                    txtbox.ScrollToCaret();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("写入内容时发生错误:" + txtbox.Name + ":" + message);
            }
        }
        /// <summary>
        /// 设置项的状态
        /// </summary>
        /// <param name="serviceCode"></param>
        /// <param name="status"></param>
        public void SetServiceStatus(string serviceCode, string status)
        {
            try
            {
                if (listView1.InvokeRequired)
                {
                    Action actionDelegate = () =>
                    {
                        //找到该行记录，修改状态
                        for (int i = 0; i < listView1.Items.Count; i++)
                        {
                            if (listView1.Items[i].SubItems[0].Text == serviceCode)
                            {
                                listView1.Items[i].SubItems[2].Text = status;
                                listView1.Items[i].SubItems[3].Text = DateTime.Now.ToString();
                            }
                        }
                    };
                    listView1.BeginInvoke(actionDelegate);
                }
                else
                {
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        if (listView1.Items[i].SubItems[0].Text == serviceCode)
                        {
                            listView1.Items[i].SubItems[2].Text = status;
                            listView1.Items[i].SubItems[3].Text = DateTime.Now.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToConsole("设置服务状态时发生错误", "error");
                WriteToConsole(ex.ToString(), "error");
            }
        }
        #endregion

        #region 菜单

        private void ExitSystem()
        {
            try
            {
                Environment.Exit(0);
            }
            catch { }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void toolStripLabel2_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("否要退出本系统", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
            {
                Task task = new Task(delegate { ExitSystem(); });
                task.Start();
            }
        }
        /// <summary>
        /// 刷新服务列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 刷新服务ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task t = new Task(() => { RefreshServiceList(); });
            t.Start();
        }
        private void 启动服务ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }
            string serviceCode = listView1.SelectedItems[0].SubItems[0].Text;
            CallStartService(serviceCode);

        }

        private void 重启服务ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }
            string serviceCode = listView1.SelectedItems[0].SubItems[0].Text;
            CallRestartService(serviceCode);
        }

        private void 停止服务ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }
            string serviceCode = listView1.SelectedItems[0].SubItems[0].Text;
            CallStopService(serviceCode);
        }
        /// <summary>
        /// 打开程序目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 打开目录ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            try
            {
                if (listView1.SelectedItems.Count != 0)
                {
                    string serviceCode = listView1.SelectedItems[0].SubItems[0].Text;
                    dir = Path.Combine(dir, ServiceList.FirstOrDefault(s => s.Code == serviceCode).Directory);
                }
            }
            catch (Exception ex)
            {
                WriteToConsole("打开程序目录时发生错误：", "error");
                WriteToConsole(ex.ToString(), "error");
            }

            System.Diagnostics.Process.Start("explorer.exe", new System.IO.DirectoryInfo(dir).FullName);
        }
        private void 查看日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tab.SelectedTab != tabPage1)
            {
                return;
            }
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }
            string serviceCode = listView1.SelectedItems[0].SubItems[0].Text;
            var tabpage = tab.TabPages["tab" + serviceCode];
            if (tabpage != null)
            {
                tab.SelectedTab = tabpage;
            }
            else
            {
                MessageBox.Show("该服务的控制台未启动");
            }
        }
        private void 清空日志toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (tab.SelectedTab == tabPage1)
            {
                return;
            }
            if (tab.SelectedTab == tabPage2)
            {
                consoleInfo.Text = "";
                return;
            }

            var tabpage = tab.SelectedTab;
            if (tabpage != null)
            {
                var serviceInfo = tabpage.Controls["txt" + tabpage.Name.Substring(3)];
                serviceInfo.Text = "";
            }

        }
        /// <summary>
        /// 打开日志目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 打开目录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dir = Util.LogDir;
            try
            {
                if (listView1.SelectedItems.Count != 0)
                {
                    string serviceCode = listView1.SelectedItems[0].SubItems[0].Text;
                    dir = Path.Combine(dir, ServiceList.FirstOrDefault(s => s.Code == serviceCode).Directory);

                }
            }
            catch (Exception ex)
            {
                WriteToConsole("打开日志目录时发生错误：", "error");
                WriteToConsole(ex.ToString(), "error");
            }

            System.Diagnostics.Process.Start("explorer.exe", new System.IO.DirectoryInfo(dir).FullName);
        }
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("否要退出本系统", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
            {
                Task task = new Task(delegate { ExitSystem(); });
                task.Start();
            }
        }
        #endregion

        #region 通知栏按钮
        /// <summary>
        /// 双击显示界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }
        /// <summary>
        /// 显示界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Show();
        }
        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("否要退出本系统", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
            {
                Task task = new Task(delegate { ExitSystem(); });
                task.Start();
            }
        }
        #endregion


        #region
        /// <summary>
        /// 服务异常处理
        /// </summary>
        /// <param name="serviceProp"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleServiceException(IService.ServiceProp serviceProp, object sender, UnhandledExceptionEventArgs e)
        {
            SetServiceStatus(serviceProp.Code, "服务异常终止");
            Exception ex = (Exception)e.ExceptionObject;
            //输出信息
            WriteLog(serviceProp.Code, "服务 " + serviceProp.Name + " 发生了未处理的异常：", "error",DateTime.Now);
            WriteLog(serviceProp.Code, ex.ToString(), "error",DateTime.Now);

            //重启服务
            CallRestartService(serviceProp.Code);

        }
        #endregion

        public void ExecuteCmd(string cmd)
        {
            throw new NotImplementedException();
        }

    }
}
