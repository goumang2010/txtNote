using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
// Directives
using LocalDatabaseSample.Model;
using LocalDatabaseSample.ViewModel;
using System.IO.IsolatedStorage;

namespace txtnote
{

    public partial class App : Application
    {
        /// <summary>
        ///提供对电话应用程序的根框架的轻松访问。
        /// </summary>
        /// <returns>电话应用程序的根框架。</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Application 对象的构造函数。
        /// </summary>
        /// 
        // The static ViewModel, to be used across the application.
        private static ToDoViewModel viewModel;
        public static ToDoViewModel ViewModel
        {
            get { return viewModel; }
        }

        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // 特定于电话的初始化
            InitializePhoneApplication();

            setting abc = new setting();



            switch (abc.ListBoxSetting)
            {

                case 0:
                    System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("zh-CN");
                    txtnote.Resources.StringLibrary.Culture = ci;
                    break;

                case 1:
                    System.Globalization.CultureInfo en = new System.Globalization.CultureInfo("en-US");
                    txtnote.Resources.StringLibrary.Culture = en;
                    break;
                case 2:
                    System.Globalization.CultureInfo ciJA = new System.Globalization.CultureInfo("ja");
                    txtnote.Resources.StringLibrary.Culture = ciJA;
                    break;
                default:
                    break;

            }

            // 调试时显示图形分析信息。
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 显示当前帧速率计数器。
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // 显示在每个帧中重绘的应用程序区域。
                //Application.Current.Host.Settings.EnableRedrawRegions = true；

                // Enable non-production analysis visualization mode, 
                // 该模式显示递交给 GPU 的包含彩色重叠区的页面区域。
                //Application.Current.Host.Settings.EnableCacheVisualization = true；

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                //  注意: 仅在调试模式下使用此设置。禁用用户空闲检测的应用程序在用户不使用电话时将继续运行
                // 并且消耗电池电量。
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            // Specify the local database connection string.
            string DBConnectionString = "Data Source=isostore:/ToDo.sdf";

            // Create the database if it does not exist.
            using (ToDoDataContext db = new ToDoDataContext(DBConnectionString))
            {
                if (db.DatabaseExists() == false)
                {
                    // Create the local database.
                    db.CreateDatabase();

                    // Prepopulate the categories.
                    db.Categories.InsertOnSubmit(new ToDoCategory { Name = txtnote.Resources.StringLibrary.richang });
                    db.Categories.InsertOnSubmit(new ToDoCategory { Name = txtnote.Resources.StringLibrary.gongzuo });
                    db.Categories.InsertOnSubmit(new ToDoCategory { Name = txtnote.Resources.StringLibrary.xingqu });

                    // Save categories to the database.
                    db.SubmitChanges();
                }

            }

            // Create the ViewModel object.
            viewModel = new ToDoViewModel(DBConnectionString);

            // Query the local database and load observable collections.
            viewModel.LoadCollectionsFromDatabase();






        }

        public   void   cancelNavigate()
        {
            RootFrame.RemoveBackEntry();
                // Refresh the history list since the back stack has been modified.
  
        }


        private void AppBar_Save_Click(object sender, EventArgs e)
        {
            
           /* Picker.Visibility = System.Windows.Visibility.Visible;
            editBox.Visibility = System.Windows.Visibility.Collapsed;

            fileNameInput.Focus();
            ApplicationBar.IsVisible = false;
            * */

        }

        // 应用程序启动(例如，从“开始”菜单启动)时执行的代码
        // 此代码在重新激活应用程序时不执行
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
 

        }

        // 激活应用程序(置于前台)时执行的代码
        // 此代码在首次启动应用程序时不执行
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // 停用应用程序(发送到后台)时执行的代码
        // 此代码在应用程序关闭时不执行
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // 应用程序关闭(例如，用户点击“后退”)时执行的代码
        // 此代码在停用应用程序时不执行
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // 导航失败时执行的代码
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 导航已失败；强行进入调试器
                System.Diagnostics.Debugger.Break();
            }
        }

        // 出现未处理的异常时执行的代码
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 出现未处理的异常；强行进入调试器
                System.Diagnostics.Debugger.Break();
            }
        }

        #region 电话应用程序初始化

        // 避免双重初始化
        private bool phoneApplicationInitialized = false;

        // 请勿向此方法中添加任何其他代码
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // 创建框架但先不将它设置为 RootVisual；这允许初始
            // 屏幕保持活动状态，直到准备呈现应用程序时。
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // 处理导航故障
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // 确保我们未再次初始化
            phoneApplicationInitialized = true;
        }

        // 请勿向此方法中添加任何其他代码
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // 设置根视觉效果以允许应用程序呈现
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // 删除此处理程序，因为不再需要它
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}