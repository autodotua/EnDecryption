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
using static EnDecryption.Tools;
using static EnDecryption.MessageDigestHelper;
using Windows.UI.Popups;
using System.Text;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace EnDecryption
{
    public sealed partial class MessageDigestUI : UserControl, ICryptography
    {
        public MessageDigestUI()
        {
            this.InitializeComponent();
        }

        public async Task Encrypte()
        {
            if (txtSource.Text == "")
            {
                ShowError("未输入任何内容！");
                return;
            }

            byte[] source = GetBytes(TxtSource.Text);
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


        public async Task EncrypteFile()
        {
            if (switchMd5.IsOn)
            {
                txtMd5.Text = await Task.Run(() => BitConverter.ToString(Md5(currentFileContent)).Replace("-", ""));
            }
            if (switchSha1.IsOn)
            {
                txtSha1.Text = await Task.Run(() => BitConverter.ToString(Sha1(currentFileContent)).Replace("-", ""));
            }
            if (switchSha256.IsOn)
            {
                txtSha256.Text = await Task.Run(() => BitConverter.ToString(Sha256(currentFileContent)).Replace("-", ""));
            }
            if (switchSha384.IsOn)
            {
                txtSha384.Text = await Task.Run(() => BitConverter.ToString(Sha384(currentFileContent)).Replace("-", ""));
            }
            if (switchSha512.IsOn)
            {
                txtSha512.Text = await Task.Run(() => BitConverter.ToString(Sha512(currentFileContent)).Replace("-", ""));
            }
            if (switchCrc32.IsOn)
            {
                txtCrc32.Text = await Task.Run(() => Crc32(currentFileContent).ToString("X2"));
            }
            currentFileContent = null;
        }



        public async void SaveResultAsFile() 
        {
            StringBuilder str = new StringBuilder(1024);
            if (txtMd5.Text != "")
            {
                str.AppendLine("MD5:" + txtMd5.Text);
            }
            if (txtSha1.Text != "")
            {
                str.AppendLine("SHA1:" + txtSha1.Text);
            }
            if (txtSha256.Text != "")
            {
                str.AppendLine("SHA256:" + txtSha256.Text);
            }
            if (txtSha384.Text != "")
            {
                str.AppendLine("SHA384:" + txtCrc32.Text);
            }
            if (txtSha512.Text != "")
            {
                str.AppendLine("SHA512:" + txtSha512.Text);
            }
            if (txtCrc32.Text != "")
            {
                str.AppendLine("CRC32:" + txtCrc32.Text);
            }

            await PickAndSaveFile(CurrentEncoding.GetBytes(str.ToString()), new Dictionary<string, string>() { { "文本文件", ".txt" } }, true);
        }
        public void RefreshUISettings()
        {
            txtSource.TextWrapping  = (TextWrapping)resource["TextWrapping"];
        }

        public TextBox TxtSource => txtSource;
        public TextBox TxtResult => throw new NotImplementedException();

        public string Separator => throw new NotImplementedException();
        public string Encoding => GetComboBoxSelectedItemString(cbbEncoding);
        public string DisplayMode => throw new NotImplementedException();

        public Task Decrypte() => throw new NotImplementedException();
        public Task GenerateKey() => throw new NotImplementedException();
        public Task DecrypteFile() => throw new NotImplementedException();
        public void SaveSourceAsFile() => throw new NotImplementedException();
    }
}