using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NetworkRepair.Business;

namespace NetworkRepair
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            //
            //System.Diagnostics.Process.Start("inetcpl.cpl");

            //var userRegisterProvider = new InternetSettingProvider();
            //userRegisterProvider.SetCertificateVerificationChecked(true);
            //userRegisterProvider.SetSSLAndTSLState(false);
            //userRegisterProvider.SetInternetProtectLevelNormal();
        }

        private async void FixIeButton_OnClick(object sender, RoutedEventArgs e)
        {
            var userRegisterProvider = new InternetSettingProvider();
            //还原高级设置
            await userRegisterProvider.RevertHighSettingAsync();
            //重置IE
            await userRegisterProvider.ResetInternetSettingAsync();
            //设置证书吊销验证不勾选
            userRegisterProvider.SetCertificateVerificationChecked(false);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("iexplore.exe", "www.baidu.com");
        }

        private void MoreOptionButton_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentView.Visibility = Visibility.Collapsed;
            MoreOptionsView.Visibility = Visibility.Visible;
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentView.Visibility = Visibility.Visible;
            MoreOptionsView.Visibility = Visibility.Collapsed;
        }

        #region 更多选项

        private async void ResetIe_OnClick(object sender, RoutedEventArgs e)
        {
            var userRegisterProvider = new InternetSettingProvider();
            await userRegisterProvider.ResetInternetSettingAsync();
        }

        private void DeleteRegistry_OnClick(object sender, RoutedEventArgs e)
        {
            var userRegisterProvider = new InternetSettingProvider();
            userRegisterProvider.ResetIeByRegistry();
            MessageBox.Show(Window.GetWindow(this), "设置成功", "提示");
        }

        private async void RevertHighSetting_OnClick(object sender, RoutedEventArgs e)
        {
            var userRegisterProvider = new InternetSettingProvider();
            await userRegisterProvider.RevertHighSettingAsync();
        }

        private void SetIeSecurity_OnClick(object sender, RoutedEventArgs e)
        {
            var userRegisterProvider = new InternetSettingProvider();
            userRegisterProvider.SetInternetProtectLevelNormal();
        }

        private void FixCertificateButton_OnClick(object sender, RoutedEventArgs e)
        {
            var userRegisterProvider = new InternetSettingProvider();
            userRegisterProvider.SetCertificateVerificationChecked(false);
            MessageBox.Show(Window.GetWindow(this), "设置成功", "提示");
        }

        private void ResetNetworkButton_OnClick(object sender, RoutedEventArgs e)
        {
            var result = CmdExecuteHelper.Execute("netsh winsock reset");
            MessageBox.Show(Window.GetWindow(this), result, "提示");
        }

        private async void UpdateSystemTime_OnClick(object sender, RoutedEventArgs e)
        {
            IsSearching = true;
            string result = string.Empty;
            await Task.Run(() => { SystemTimeHelper.UpdateSystemTime(out result); });
            Application.Current.Dispatcher?.Invoke(() =>
            {
                MessageBox.Show(Window.GetWindow(this), result, "提示");
            });

            IsSearching = false;
        }

        #endregion

        #region 属性


        public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching",
            typeof(bool), typeof(MainView), new PropertyMetadata(default(bool)));

        public bool IsSearching
        {
            get { return (bool)GetValue(IsSearchingProperty); }
            set { SetValue(IsSearchingProperty, value); }
        }

        #endregion
    }

}
