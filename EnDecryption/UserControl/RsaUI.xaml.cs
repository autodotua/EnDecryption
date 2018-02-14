using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static EnDecryption.Tools;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace EnDecryption
{
    public sealed partial class RsaUI : UserControl, ICryptography
    {
        public RsaUI()
        {
            this.InitializeComponent();
            cbbDisplayMode.SelectionChanged += (p1, p2) => DisplayOrInputModeChanged();
        }

        private void DisplayOrInputModeChanged()
        {
            if (cbbDisplayMode.SelectedIndex == 1 || cbbDisplayMode.SelectedIndex == 2)
            {
                cbbSeparator.Visibility = Visibility.Visible;
            }
            else
            {
                cbbSeparator.Visibility = Visibility.Collapsed;
            }
        }
        public async Task GenerateKey()
        {
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(KeyLength))
            {
                await Task.Run(() =>
                {
                    var p = RSA.ExportParameters(true);
                });

                txtPublicKey.Text = RSA.ToPublicXmlString();
                txtPrivateKey.Text = RSA.ToPriavateXmlString();

            }
        }
        public async Task Encrypte()
        {
            if (txtPublicKey.Text == "")
            {
                await GenerateKey();
            }
            if (CheckEncrypte(txtSource.Text, out byte[] text))
            {
                byte[] result = null;
                string key = txtPublicKey.Text;
                bool oaep = cbbPadding.SelectedIndex == 1;
                int keyLength = KeyLength;
                await Task.Run(() =>
              result = RsaHelper.Encrypte(
                    text,
                    key,
                    keyLength,
                    oaep));
                txtResult.Text = ToText(result, cbbDisplayMode);
            }
        }



        public async Task Decrypte()
        {

            if (txtPrivateKey.Text == "")
            {
                ShowError("请输入私钥！");
                return;
            }
            if (CheckDecrypte(txtResult.Text, out byte[] text))
            {
                byte[] result = null;
                string key = txtPrivateKey.Text;
                bool oaep = cbbPadding.SelectedIndex == 1;
                int keyLength = KeyLength;
                await Task.Run(() =>
                result = RsaHelper.Decrypte(
                    text,
                    key,
                    keyLength,
                    oaep));
                txtSource.Text = CurrentEncoding.GetString(result);
            }
        }

        private int KeyLength => 1024 * (int)Math.Pow(2, cbbKeyLength.SelectedIndex);

        private bool CheckDecrypte(string text, out byte[] value)
        {
            value = null;
            if (text == "")
            {
                ShowError("待加密内容不合法。");
                return false;
            }

            value = GetBytes(text, cbbDisplayMode);

            if (value == null)
            {
                ShowError("密文解析错误，请检查密文格式");
                return false;
            }

            return true;
        }
        private bool CheckEncrypte(string text, out byte[] value)
        {
            value = null;
            if (text == "")
            {
                ShowError("待加密内容不合法。");
                return false;
            }

            value = GetBytes(txtSource.Text);

            return true;
        }
        public async Task EncrypteFile()
        {
            if (currentFileContent == null)
            {
                throw new ArgumentNullException("文件为Null");
            }
            if (txtPublicKey.Text == "")
            {
                await GenerateKey();
            }



            FileSavePicker picker = new FileSavePicker();
            picker.FileTypeChoices.Add("RSA加密文件", new List<string>() { ".rsa" });
            picker.FileTypeChoices.Add("二进制文件", new List<string>() { ".bin" });
            picker.FileTypeChoices.Add("所有文件", new List<string>() { "." });
            picker.SuggestedFileName = currentFile.Name.Replace(currentFile.FileType, "");

            var file = await picker.PickSaveFileAsync();

            byte[] result = null;
            string key = txtPublicKey.Text;
            bool oaep = cbbPadding.SelectedIndex == 1;
            int keyLength = KeyLength;
            await Task.Run(() =>
          result = RsaHelper.Encrypte(
                currentFileContent,
                key,
                keyLength,
                oaep));

            if (result.Length > 0)
            {
                await Task.Run(() => FileIO.WriteBufferAsync(file, CryptographicBuffer.CreateFromByteArray(result)));
                currentFile = null;
                currentFileContent = null;
            }

        }
        public async Task DecrypteFile()
        {
            if (currentFileContent == null)
            {
                throw new ArgumentNullException("文件为Null");
            }
            if (txtPrivateKey.Text == "")
            {
                ShowError("请输入私钥！");
                return;
            }

            FileSavePicker picker = new FileSavePicker();
            picker.FileTypeChoices.Add("请指定文件类型！", new List<string>() { "." });
            picker.SuggestedFileName = currentFile.Name.Replace(currentFile.FileType, "");

            var file = await picker.PickSaveFileAsync();


            byte[] result = null;
            string key = txtPrivateKey.Text;
            bool oaep = cbbPadding.SelectedIndex == 1;
            int keyLength = KeyLength;
            await Task.Run(() =>
            result = RsaHelper.Decrypte(
                currentFileContent,
                key,
                keyLength,
                oaep));

            if (result.Length > 0)
            {
                await Task.Run(() => FileIO.WriteBufferAsync(file, CryptographicBuffer.CreateFromByteArray(result)));
                currentFile = null;
                currentFileContent = null;
            }

        }
        public async void SaveSourceAsFile()
        {
            await PickAndSaveFile(CurrentEncoding.GetBytes(TxtSource.Text), new Dictionary<string, string>() { { "文本文件", ".txt" } }, true);
        }

        public async void SaveResultAsFile()
        {
            MessageDialog dialog = new MessageDialog("保存", "请选择要保存的格式");
            dialog.Commands.Add(new UICommand("二进制", async (p1) =>
            await PickAndSaveFile(GetBytes(txtResult.Text, cbbDisplayMode), new Dictionary<string, string>() { { "RSA加密文件", ".rsa" }, { "二进制文件", ".bin" } }, true)));
            dialog.Commands.Add(new UICommand("显示的文本", async (p1) =>
            await PickAndSaveFile(CurrentEncoding.GetBytes(txtResult.Text), new Dictionary<string, string>() { { "文本文件", ".txt" } }, true)));
            await dialog.ShowAsync();
        }

        public int SeparatorIndex => cbbSeparator.SelectedIndex;

        public int DisplayModeIndex => cbbDisplayMode.SelectedIndex;
        public int InputModeIndex => -1;

        public int EncodingIndex => cbbEncoding.SelectedIndex;

        public TextBox TxtSource => txtSource;
        public TextBox TxtResult => txtResult;


    }
}
