using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NetworkRepair.Business;

namespace NetworkRepair
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            KillProcess(System.Windows.Forms.Application.ProductName);

            Startup += App_Startup;
        }
        private static readonly string FixIeCommand = "FixIe";
        private static readonly string FixSystemTimeCommand = "FixSystemTime";
        private void App_Startup(object sender, StartupEventArgs e)
        {
            var eArgs = e.Args;
            if (eArgs.Length == 1)
            {
                var eArg = eArgs[0];
                if (eArg == FixIeCommand)
                {
                    FixIe();
                }
                else if (eArg == FixSystemTimeCommand)
                {
                    FixSystemTime();
                }
                else
                {
                    ShowMainWindow();
                }
            }
            else
            {
                ShowMainWindow();
            }
        }

        private const string DataSepartor = " | ";
        private void FixSystemTime()
        {
            try
            {
                var result = SystemTimeHelper.UpdateSystemTime(out var errorMsg);
                LogHelper.Log($@"{result}{DataSepartor}{errorMsg}");
            }
            catch (Exception e)
            {
                LogHelper.Log($@"{false}{DataSepartor}{e.Message}");
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        private void FixIe()
        {
            try
            {
                var userRegisterProvider = new InternetSettingProvider();
                userRegisterProvider.SetCertificateVerificationChecked(true);
                userRegisterProvider.SetSSLAndTSLState(false);
                userRegisterProvider.SetInternetProtectLevelNormal();
                LogHelper.Log($@"{true}{DataSepartor}{string.Empty}");
            }
            catch (Exception e)
            {
                LogHelper.Log($@"{false}{DataSepartor}{e.Message}");
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        private void ShowMainWindow()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        #region 删除原有进程

        /// <summary>
        /// 删除原有进程
        /// </summary>
        /// <param name="processName"></param>
        private void KillProcess(string processName)
        {
            try
            {
                //删除所有同名进程
                Process currentProcess = Process.GetCurrentProcess();
                var processes = Process.GetProcessesByName(processName).Where(process => process.Id != currentProcess.Id);
                foreach (Process thisproc in processes)
                {
                    thisproc.Kill();
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion
    }
}
