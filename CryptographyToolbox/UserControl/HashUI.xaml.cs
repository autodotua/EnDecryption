using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static CryptographyToolbox.Tools;
using static CryptographyToolbox.Helper.HashHelper;
using static CryptographyToolbox.Helper.ConvertHelper;
using static CryptographyToolbox.Helper.Settings;
using Windows.UI.Popups;
using System.Text;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace CryptographyToolbox
{
    public sealed partial class HashUI : UserControl, ICryptographyPageBase
    {
        public HashUI()
        {
            this.InitializeComponent();
        }

        public async Task Encrypte()
        {
            if (txtSource.Text == "")
            {
              await  ShowError("未输入任何内容！");
                return;
            }

            byte[] source =await GetBytes(TxtSource.Text,BinaryInputMode);
            if (switchMd5.IsOn)
            {
                txtMd5.Text = await Task.Run(() => BitConverter.ToString(Md5(source)).Replace("-", ""));
            }
            if (switchSha1.IsOn)
            {
                txtSha1.Text = await Task.Run(() => BitConverter.ToString(Sha1(source)).Replace("-", ""));
            }
            if (switchSha256.IsOn)
            {
                txtSha256.Text = await Task.Run(() => BitConverter.ToString(Sha256(source)).Replace("-", ""));
            }
            if (switchSha384.IsOn)
            {
                txtSha384.Text = await Task.Run(() => BitConverter.ToString(Sha384(source)).Replace("-", ""));
            }
            if (switchSha512.IsOn)
            {
                txtSha512.Text = await Task.Run(() => BitConverter.ToString(Sha512(source)).Replace("-", ""));
            }
            if (switchCrc32.IsOn)
            {
                txtCrc32.Text = await Task.Run(() => Crc32(source).ToString("X2"));
            }
        }




        public void RefreshUISettings()
        {
            txtSource.TextWrapping = TextBoxWrapping;
        }

        public TextBox TxtSource => txtSource;
        public TextBox TxtResult => throw new NotImplementedException();

        public Task GenerateKey() => throw new NotImplementedException();

        public Task Decrypte()=> throw new NotImplementedException();

    }
}