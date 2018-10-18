using CryptographyToolbox.Helper;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static CryptographyToolbox.Helper.ConvertHelper;
using static CryptographyToolbox.Helper.Settings;
using static CryptographyToolbox.Tools;
using C = CoreCodes.Cryptography;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace CryptographyToolbox
{
    public sealed partial class AesUI : UserControl, ICryptographyPageBase
    {
        public AesUI()
        {
            this.InitializeComponent();
        }


        public async Task Encrypte()
        {
            //if (txtKey.Text == "" && txtIv.Text == "")
            //{
            //    await GenerateKey();
            //}
            (byte[] value, byte[] key, byte[] iv)? info = await Check(txtSource.Text,true);
            if (info.HasValue)
            {
                byte[] result = null;
                var cipherMode = CipherMode;
                var paddingMode = PaddingMode;
                await Task.Run(() =>
                {
                    C.Aes aes = new C.Aes();
                    aes.Manager.Key = info.Value.key;
                    aes.Manager.IV = info.Value.iv;
                    aes.Manager.Mode = cipherMode;
                    aes.Manager.Padding = paddingMode;
                    result = aes.Encrypt(info.Value.value);
                });
                txtResult.Text =await GetString(result, BinaryOutputMode);
            }
        }


        public async Task Decrypte()
        {
            (byte[] value, byte[] key, byte[] iv)? info = await Check(txtResult.Text,false);

            if (info.HasValue)
            {
                byte[] result = null;
                var cipherMode = CipherMode;
                var paddingMode = PaddingMode;
                await Task.Run(() =>
                {
                    C.Aes aes = new C.Aes();
                    aes.Manager.Key = info.Value.key;
                    aes.Manager.IV = info.Value.iv;
                    aes.Manager.Mode = cipherMode;
                    aes.Manager.Padding = paddingMode;
                    result = aes.Decrypt(info.Value.value);
                });
                txtSource.Text = Encoding.GetString(result);
            }
        }
        public async Task GenerateKey()
        {
            if (BinaryKeyMode == BinaryMode.String)
            {
                await ShowError("在密钥格式为字符串时，无法生成。");
                return;
            }
            Aes aes = Aes.Create();
            txtKey.Text =await GetString(aes.Key, BinaryKeyMode);
            txtIv.Text =await GetString(aes.IV, BinaryKeyMode);
            aes.Dispose();


        }
        private async Task<(byte[] value, byte[] key, byte[] iv)?> Check(string text,bool encrypte)
        {
            byte[] value = null;
            byte[] key = null;
            byte[] iv = null;
            if (string.IsNullOrEmpty(text))
            {
                await ShowError("内容不能为空");
                return null;
            }

            value =await GetBytes(text,encrypte? BinaryInputMode:BinaryOutputMode);
            if (value==null)
            {
                await ShowError("内容不合法");
                return null;
            }


            int length = AesLength / 8;

            key =await GetBytes(txtKey.Text, BinaryKeyMode);
            if (key == null)
            {
                await ShowError("密钥输入格式不正确");
                return null;
            }
            key = EnsureLengthCorrect(key, length);

            if (CipherMode == CipherMode.ECB)
            {
                iv = new byte[16];
            }
            else
            {
                iv =await GetBytes(txtIv.Text, BinaryKeyMode);
                if (iv == null)
                {
                    await ShowError("偏移输入格式不正确");
                    return null;
                }
                iv = EnsureLengthCorrect(iv, 16);
            }
            
            return (value, key, iv);
        }



        public byte[] EnsureLengthCorrect(byte[] bytes, int length)
        {
            byte padding = byte.Parse(txtKeyPadding.Text);
            if (bytes.Length == length)
            {
                return bytes;
            }
            else if (bytes.Length < length)
            {
                List<byte> keyList = new List<byte>(bytes);
                keyList.Capacity = length;
                while (keyList.Count < length)
                {
                    keyList.Add(padding);
                }
                return keyList.ToArray();
            }
            else
            {
                byte[] result = new byte[length];
                Array.Copy(bytes, result, length);
                return result;
            }
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

        private void cbbAesLength_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbbAesLength.SelectedIndex)
            {
                case 0:
                    AesLength = 128;
                    break;
                case 1:

                    AesLength = 192;
                    break;
                case 2:
                    AesLength = 256;
                    break;
            }
        }

        public void RefreshUISettings()
        {
            txtSource.TextWrapping = txtResult.TextWrapping = txtKey.TextWrapping = txtIv.TextWrapping = TextBoxWrapping;
        }

        public TextBox TxtSource => txtSource;
        public TextBox TxtResult => txtResult;

        private void txtKeyPadding_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void txtKeyPadding_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {

                KeyPadding = byte.Parse(txtKeyPadding.Text);
            }
            catch
            {
                await ShowError("输入的密钥补码不正确！应为0-255的数字");
                txtKeyPadding.Text = KeyPadding.ToString();
            }
        }
    }
}
