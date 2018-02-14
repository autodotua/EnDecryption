using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static EnDecryption.Tools;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using Windows.UI.Popups;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace EnDecryption
{
    public sealed partial class AesUI : UserControl, ICryptography
    {
        public AesUI()
        {
            this.InitializeComponent();
            cbbDisplayMode.SelectionChanged += (p1, p2) => DisplayOrInputModeChanged();
            cbbInputMode.SelectionChanged += (p1, p2) => DisplayOrInputModeChanged();
        }

        private void DisplayOrInputModeChanged()
        {
            if (cbbDisplayMode.SelectedIndex == 1 || cbbDisplayMode.SelectedIndex == 2 || cbbInputMode.SelectedIndex == 2 || cbbInputMode.SelectedIndex == 3)
            {
                cbbSeparator.Visibility = Visibility.Visible;
            }
            else
            {
                cbbSeparator.Visibility = Visibility.Collapsed;
            }
        }

        public async Task Encrypte()
        {
            if (txtKey.Text == "" && txtIv.Text == "")
            {
                await GenerateKey();
            }

            //if (txtSource.Text == "（已选择文件）" && file != null)
            //{
            //    if (CheckLegal(out byte[] key, out byte[] iv))
            //    {
            //        byte[] result = null;
            //        FileSavePicker picker = new FileSavePicker();
            //        picker.FileTypeChoices.Add("AES加密文件", new List<string>() { ".aes" });
            //        picker.FileTypeChoices.Add("二进制文件", new List<string>() { ".bin" });
            //        picker.FileTypeChoices.Add("所有文件", new List<string>() { "." });
            //        picker.SuggestedFileName = file.Name.Replace(file.FileType, "");

            //        StorageFile saveFile = await picker.PickSaveFileAsync();
            //        if (saveFile != null)
            //        {
            //            try
            //            {

            //                await Task.Run(async () =>
            //            {


            //                using (Stream stream = await file.OpenStreamForReadAsync())
            //                {
            //                    using (var memoryStream = new MemoryStream())
            //                    {
            //                        stream.CopyTo(memoryStream);
            //                        result = memoryStream.ToArray();
            //                    }
            //                }
            //            });

            //                using (MemoryStream memory = AesClass.EncrypteToStream(result, key, iv, OperationMode, PaddingMode))
            //                {

            //                    int bufferLength = 1024 * 32;
            //                    int length;
            //                    byte[] buffer = new byte[bufferLength];

            //                    using (Stream stream = await saveFile.OpenStreamForWriteAsync())
            //                    {
            //                        memory.Position = 0;
            //                        while ((length = memory.Read(buffer, 0, bufferLength)) > 0)
            //                        {
            //                            await stream.WriteAsync(buffer, 0, length);
            //                        }
            //                        await stream.FlushAsync();
            //                    }
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                ShowError("加密失败：" + Environment.NewLine + ex.ToString());
            //            }
            //        }
            //        MessageDialog dialog = new MessageDialog("加密成功.", "成功");
            //        await dialog.ShowAsync();
            //    }
            //}
            //else
            //{
            if (Check(txtSource.Text, out byte[] text, out byte[] key, out byte[] iv))
            {
                byte[] result = null;
                var cipherMode = CipherMode;
                var paddingMode = PaddingMode;
                await Task.Run(() =>
               result = AesHelper.Encrypte(
                text,
                key,
                iv,
                cipherMode,
                paddingMode));
                txtResult.Text = ToText(result, cbbDisplayMode);
            }
            //}
        }

        public async Task EncrypteFile()
        {
            if (currentFileContent == null)
            {
                throw new ArgumentNullException("文件为Null");
            }
            if (txtKey.Text == "" && txtIv.Text == "")
            {
                await GenerateKey();
            }
            if (Check(out byte[] key, out byte[] iv))
            {
                FileSavePicker picker = new FileSavePicker();
                picker.FileTypeChoices.Add("AES加密文件", new List<string>() { ".aes" });
                picker.FileTypeChoices.Add("二进制文件", new List<string>() { ".bin" });
                picker.FileTypeChoices.Add("所有文件", new List<string>() { "." });
                picker.SuggestedFileName = currentFile.Name.Replace(currentFile.FileType, "");

                var file = await picker.PickSaveFileAsync();


                byte[] result = null;
                var cipherMode = CipherMode;
                var paddingMode = PaddingMode;
                await Task.Run(() =>
               result = AesHelper.Encrypte(
               currentFileContent,
                key,
                iv,
                cipherMode,
                paddingMode));

                if (result.Length > 0)
                {
                    await Task.Run(() => FileIO.WriteBufferAsync(file, CryptographicBuffer.CreateFromByteArray(result)));
                    currentFile = null;
                    currentFileContent = null;
                }
            }
        }
        public async Task DecrypteFile()
        {
            if (currentFileContent == null)
            {
                throw new ArgumentNullException("文件为Null");
            }
            if (txtKey.Text == "")
            {
                ShowError("请输入密钥!");
            }
            if (Check(out byte[] key, out byte[] iv))
            {
                FileSavePicker picker = new FileSavePicker();
                picker.FileTypeChoices.Add("请指定文件类型！", new List<string>() { "." });
                picker.SuggestedFileName = currentFile.Name.Replace(currentFile.FileType, "");

                var file = await picker.PickSaveFileAsync();


                byte[] result = null;
                var cipherMode = CipherMode;
                var paddingMode = PaddingMode;
                await Task.Run(() =>
               result = AesHelper.Decrypte(
               currentFileContent,
                key,
                iv,
                cipherMode,
                paddingMode));

                if (result.Length > 0)
                {
                    await Task.Run(() => FileIO.WriteBufferAsync(file, CryptographicBuffer.CreateFromByteArray(result)));
                    currentFile = null;
                    currentFileContent = null;
                }
            }
        }


        public async Task Decrypte()
        {
            if (txtKey.Text == "")
            {
                ShowError("请输入密钥!");
            }

            if (Check(txtResult.Text, out byte[] text, out byte[] key, out byte[] iv))
            {
                byte[] result = null;
                var cipherMode = CipherMode;
                var paddingMode = PaddingMode;
                await Task.Run(() =>
                    result = AesHelper.Decrypte(
                    text,
                    key,
                    iv,
                    cipherMode,
                    paddingMode));
                txtSource.Text = CurrentEncoding.GetString(result);
            }
        }
        public async Task GenerateKey()
        {
            Aes aes = null;
            await Task.Run(() =>
            {
                aes = Aes.Create();
            });
            byte[] key = aes.Key;
            byte[] iv = aes.IV;

            if (cbbInputMode.SelectedIndex == 0)
            {
                cbbInputMode.SelectedIndex = 1;
            }
            switch (cbbInputMode.SelectedIndex)
            {
                //case 0:
                //    ShowError("无法在输入模式为“字符串”的情况下生成随机密钥和初始向量");
                //    return;
                case 1:
                    txtKey.Text = Convert.ToBase64String(key);
                    txtIv.Text = Convert.ToBase64String(iv);
                    break;
                case 2:
                    txtKey.Text = ToDec(key);
                    txtIv.Text = ToDec(iv);
                    break;
                case 3:
                    txtKey.Text = ToHex(key);
                    txtIv.Text = ToHex(iv);
                    break;
                default:
                    return;
            }
        }
        private bool Check(string text, out byte[] value, out byte[] key, out byte[] iv)
        {
            key = iv = null;
            value = null;
            if (text == "")
            {
                ShowError("待加密内容不合法。");
                return false;
            }

            value = GetBytes(text);

            key = GetBytes(txtKey.Text, cbbInputMode);
            if (key == null || (key.Length != 16 && key.Length != 24 && key.Length != 32))
            {
                ShowError("密钥不合法：转换后应为16、24或32个8位数字");
                return false;
            }
            iv = GetBytes(txtIv.Text, cbbInputMode);
            if ((iv == null || iv.Length != 16) && CipherMode != CipherMode.ECB)
            {
                ShowError("初始向量不合法：转换后应为16个8位数字");
                return false;
            }
            return true;
        }
        private bool Check(out byte[] key, out byte[] iv)
        {
            key = iv = null;

            key = GetBytes(txtKey.Text, cbbInputMode);
            if (key == null || (key.Length != 16 && key.Length != 24 && key.Length != 32))
            {
                ShowError("密钥不合法：转换后应为16、24或32个8位数字");
                return false;
            }
            iv = GetBytes(txtIv.Text, cbbInputMode);
            if ((iv == null || iv.Length != 16) && CipherMode != CipherMode.ECB)
            {
                ShowError("初始向量不合法：转换后应为16个8位数字");
                return false;
            }
            return true;
        }
        private CipherMode CipherMode
        {
            get
            {
                switch (cbbMode.SelectedIndex)
                {
                    case 0:
                        return CipherMode.ECB;
                    case 1:
                        return CipherMode.CBC;
                    default:
                        return CipherMode.ECB;
                }
            }
        }

        private PaddingMode PaddingMode
        {
            get
            {
                switch (cbbPadding.SelectedIndex)
                {
                    case 0:
                        return PaddingMode.PKCS7;
                    case 1:
                        return PaddingMode.Zeros;
                    default:
                        return PaddingMode.PKCS7;
                }
            }
        }

        private void CbbModeSelectionChangedEventHandler(object sender, SelectionChangedEventArgs e)
        {
            if (CipherMode == CipherMode.ECB)
            {
                txtIv.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtIv.Visibility = Visibility.Visible;

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
            await PickAndSaveFile(GetBytes(txtResult.Text, cbbDisplayMode), new Dictionary<string, string>() { { "AES加密文件", ".aes" }, { "二进制文件", ".bin" } }, true)));
            dialog.Commands.Add(new UICommand("显示的文本", async (p1) =>
            await PickAndSaveFile(CurrentEncoding.GetBytes(txtResult.Text), new Dictionary<string, string>() { { "文本文件", ".txt" } }, true)));
            await dialog.ShowAsync();
        }

        public int SeparatorIndex => cbbSeparator.SelectedIndex;

        public int DisplayModeIndex => cbbDisplayMode.SelectedIndex;
        public int InputModeIndex => cbbInputMode.SelectedIndex;

        public int EncodingIndex => cbbEncoding.SelectedIndex;

        public TextBox TxtSource => txtSource;
        public TextBox TxtResult => txtResult;

    }
}
