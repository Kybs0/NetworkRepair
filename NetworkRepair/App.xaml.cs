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
        private static readonly string FixSystemTimeByDefinedCommand = "FixSystemTimeByDefined";
        private void App_Startup(object sender, StartupEventArgs e)
        {
            var eArgs = e.Args;
            if (eArgs.Length == 1)
            {
                var eArg = eArgs[0];
                if (eArg == FixIeCommand)
                {
                    FixIe();
                    return;
                }
                else if (eArg == FixSystemTimeCommand)
                {
                    FixSystemTime();
                    return;
                }
            }
            else if (eArgs.Length == 2)
            {
                var args1 = eArgs[0];
                var args2 = eArgs[1];

                if (args1 == FixSystemTimeByDefinedCommand)
                {
                    //UTC时间
                    if (!string.IsNullOrEmpty(args2) &&
                        long.TryParse(args2, out long dateTimeValue)
                        && dateTimeValue > 0)
                    {
                        //TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(milliseconds);
                        var utcTime = new DateTime(1970, 1, 1).AddMilliseconds(dateTimeValue);
                        //处置北京时间 +8时   
                        var newTime = utcTime.AddHours(8);
                        FixSystemTime(newTime);
                    }
                    else
                    {
                        //如外界提供时间为空，则取国际服务器获取时间修复
                        FixSystemTime();
                    }
                    return;
                }
            }
            ShowMainWindow();
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
        private void FixSystemTime(DateTime time)
        {
            try
            {
                SystemTimeHelper.SetLocalTime(time);
                LogHelper.Log($@"{true}|{string.Empty}");
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
